using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using UniversiteDomain.Entities;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Dtos;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteCsvProvider.Services;

public class GenerateCsvService(IEtudiantRepository _etudiantRepository, IUeRepository _ueRepository)
{
    
    public MemoryStream GenerateTemplate(long ueId)
    {
        var ue = _ueRepository.FindAsync(ueId).Result;
        if (ue is null) throw new UeNotFoundException($"UE {ueId} introuvable");

        var etudiants = _etudiantRepository.GetByUEAsync(ueId).Result;

        var memoryStream = new MemoryStream();
        var writer = new StreamWriter(memoryStream);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            HasHeaderRecord = true
        };

        // Supprimez le bloc 'using' pour éviter la fermeture prématurée
        var csv = new CsvWriter(writer, config);

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

        // Forcez l'écriture des données et réinitialisez le flux
        csv.Flush();
        writer.Flush();
        memoryStream.Position = 0;

        return memoryStream;
    }
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Scolarite);
    }
}