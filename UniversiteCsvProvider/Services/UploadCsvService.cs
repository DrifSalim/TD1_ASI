// UniversiteCsvProvider/Services/CsvUploadService.cs
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using UniversiteDomain.Entities;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Dtos;
using UniversiteDomain.Exceptions;

namespace UniversiteCsvProvider.Services;

public class UploadCsvService(IEtudiantRepository _etudiantRepository, IUeRepository _ueRepository, INoteRepository _noteRepository){

    public async Task<(bool Success, List<string> Errors)> ProcessUploadAsync(Stream csvStream, Ue ueCible)
    {
        var errors = new List<string>();
        var notesToAdd = new List<NoteCsvDto>();

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            HasHeaderRecord = true,
            TrimOptions = TrimOptions.Trim
        };

        using (var reader = new StreamReader(csvStream))
        using (var csv = new CsvReader(reader, config))
        {
            var records = csv.GetRecords<NoteCsvDto>().ToList();

            foreach (var record in records)
            {
                var lineErrors = new List<string>();

                // 1. Vérification UE
                if (record.NumeroUe != ueCible.NumeroUe || record.Intitule != ueCible.Intitule)
                {
                    lineErrors.Add($"UE invalide : Trouvé : {record.NumeroUe} {record.Intitule} ≠ {ueCible.NumeroUe} {ueCible.Intitule}");
                }

                // 2. Validation Étudiant
                var etudiant = await _etudiantRepository.FindAsync(record.NumEtud);
                if (etudiant == null)
                {
                    lineErrors.Add($"Étudiant : '{record.NumEtud}' n'existe pas");
                }
                else
                {
                    // 3. Vérification Nom/Prénom
                    var nomValide = etudiant.Nom.Equals(record.Nom.Trim(), StringComparison.OrdinalIgnoreCase);
                    var prenomValide = etudiant.Prenom.Equals(record.Prenom.Trim(), StringComparison.OrdinalIgnoreCase);
                    
                    if (!nomValide || !prenomValide)
                    {
                        lineErrors.Add($"Nom ou Prénom d'étudiant invalide. Attendu : {etudiant.Nom} {etudiant.Prenom}, Trouvé : {record.Nom} {record.Prenom}");
                    }

                    // 4. Inscription à l'UE
                    if (etudiant.ParcoursSuivi?.UesEnseignees?.Any(u => u.Id == ueCible.Id) != true)
                    {
                        lineErrors.Add($"L'Étudiant {etudiant} n'est pas inscrit à cette UE {ueCible.NumeroUe} ");
                    }
                }

                // 5. Validation Note
                
                if (record.Note.HasValue && (record.Note < 0 || record.Note > 20))
                {
                    lineErrors.Add($"Note invalide : {record.Note}/20 (doit être entre 0 et 20)");
                }
                

                // Collecte des erreurs
                if (lineErrors.Any())
                {
                    errors.Add($"{string.Join(", ", lineErrors)}");
                }
                else
                {
                    notesToAdd.Add(record);
                }
            }
        }

        if (errors.Any())
        {
            return (false, errors);
        }

        // Enregistrement des notes
        foreach (var noteDto in notesToAdd.Where(n => n.Note.HasValue))
        {
            var etudiant = await _etudiantRepository.FindAsync(noteDto.NumEtud);
            await _noteRepository.AddNoteAsync(etudiant!, ueCible, noteDto.Note!.Value);
        }

        return (true, new List<string>());
    }
}