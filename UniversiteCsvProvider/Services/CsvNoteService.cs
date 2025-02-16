// UniversiteCsvProvider/Services/CsvNoteService.cs

using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using UniversiteDomain.Entities;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Dtos;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteCsvProvider.Services;

public class CsvNoteService
{
    private readonly IEtudiantRepository _etudiantRepository;
    private readonly IUeRepository _ueRepository;
    private readonly INoteRepository _noteRepository;

    public CsvNoteService(
        IEtudiantRepository etudiantRepository,
        IUeRepository ueRepository,
        INoteRepository noteRepository)
    {
        _etudiantRepository = etudiantRepository;
        _ueRepository = ueRepository;
        _noteRepository = noteRepository;
    }

    // Génère le template CSV
    public MemoryStream GenerateTemplate(long ueId)
    {
        var ue = _ueRepository.FindAsync(ueId).Result;
        if (ue is null)
        {
            throw new UeNotFoundException($"Cette UE {ueId} n'existe pas");
        }
        var etudiants = _etudiantRepository.GetByUEAsync(ueId).Result;
        
        var memoryStream = new MemoryStream(); // Ne pas utiliser "using"
        var writer = new StreamWriter(memoryStream);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";", // Utilisez un point-virgule pour Excel (ou "," selon besoin)
            HasHeaderRecord = true
        };

        using var csv = new CsvWriter(writer, config); // <-- Appliquez la config

        csv.WriteHeader<NoteCsvDto>();
        csv.NextRecord();

       
        foreach (var etudiant in etudiants)
        {
            var noteExistante = etudiant.NotesObtenues.FirstOrDefault(n => n.UeId == ueId);
            csv.WriteRecord(new NoteCsvDto
            {
                NumEtud = etudiant.NumEtud,
                Nom = etudiant.Nom,
                Prenom = etudiant.Prenom,
                NumeroUe = ue.NumeroUe,
                Intitule = ue.Intitule,
                Note = noteExistante?.Valeur
            });
            csv.NextRecord();
        }

        writer.Flush();
        memoryStream.Position = 0;

        // Ne pas fermer le writer ou le memoryStream ici
        return memoryStream;
    }
    // Traite le fichier CSV uploadé
  public async Task<(bool Success, List<string> Errors)> ProcessUploadAsync(Stream csvStream)
{
    var errors = new List<string>();
    var notesToAdd = new List<NoteCsvDto>();
    Ue? ue = null;

    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        Delimiter = ";",
        HasHeaderRecord = true,
        TrimOptions = TrimOptions.Trim // Gère les espaces
    };

    using (var reader = new StreamReader(csvStream))
    using (var csv = new CsvReader(reader, config))
    {
        var records = csv.GetRecords<NoteCsvDto>().ToList();

        // Validation initiale de l'UE (vérifie le premier enregistrement)
        if (records.Any())
        {
            var firstRecord = records.First();
            ue = await _ueRepository.GetByNumeroAsync(firstRecord.NumeroUe);

            // Vérification Numéro UE + Intitulé
            if (ue == null || ue.Intitule != firstRecord.Intitule)
            {
                errors.Add($"UE invalide : {firstRecord.NumeroUe} - {firstRecord.Intitule}");
            }
        }

        foreach (var record in records)
        {
            var lineErrors = new List<string>();

            // 1. Validation UE pour chaque ligne (cohérence globale)
            if (ue == null || record.NumeroUe != ue.NumeroUe || record.Intitule != ue.Intitule)
            {
                lineErrors.Add($"Ligne invalide : UE {record.NumeroUe} - {record.Intitule} ne correspond pas à {ue?.NumeroUe} - {ue?.Intitule}");
            }

            // 2. Validation Étudiant
            var etudiant = await _etudiantRepository.FindAsync(record.NumEtud);
            if (etudiant == null)
            {
                lineErrors.Add($"Étudiant {record.NumEtud} introuvable");
            }
            else
            {
                // 3. Vérification Nom/Prénom
                var nomValide = etudiant.Nom.Equals(record.Nom.Trim(), StringComparison.OrdinalIgnoreCase);
                var prenomValide = etudiant.Prenom.Equals(record.Prenom.Trim(), StringComparison.OrdinalIgnoreCase);
                
                if (!nomValide || !prenomValide)
                {
                    lineErrors.Add($"Nom ou Prénom invalide pour : {record.NumEtud}. Attendu : {etudiant.Nom} {etudiant.Prenom}, Trouvé : {record.Nom} {record.Prenom}");
                }

                // 4. Vérification inscription UE
                if (ue != null && etudiant.ParcoursSuivi?.UesEnseignees?.Any(u => u.Id == ue.Id) != true)
                {
                    lineErrors.Add($"Étudiant non inscrit à l'UE {ue.NumeroUe}");
                }
            }

            // 5. Validation Note
            if (record.Note.HasValue && (record.Note < 0 || record.Note > 20))
            {
                lineErrors.Add($"Note invalide : {record.Note}/20");
            }

            // Collecte des erreurs
            if (lineErrors.Any())
            {
                errors.Add($"  {string.Join(", ", lineErrors)}");
            }
            else
            {
                notesToAdd.Add(record);
            }
        }
    }

    // Si erreurs, on bloque l'enregistrement
    if (errors.Any())
    {
        return (false, errors);
    }

    // Enregistrement des notes
    foreach (var noteDto in notesToAdd.Where(n => n.Note.HasValue))
    {
        var etudiant = await _etudiantRepository.FindAsync(noteDto.NumEtud);
        await _noteRepository.AddNoteAsync(etudiant!, ue!, noteDto.Note!.Value);
    }

    return (true, new List<string>());
}
}