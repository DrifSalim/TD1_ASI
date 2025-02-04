using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.NotesExceptions;
using UniversiteDomain.Exceptions.ParcoursException;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.EtudiantUseCases.AjouterNote;

public class AddNoteUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task ExecuteAsync(Etudiant etudiant, Ue ue, double valeurNote)
    {
        ArgumentNullException.ThrowIfNull(etudiant);
        ArgumentNullException.ThrowIfNull(ue);
        await ExecuteAsync(etudiant.Id, ue.Id, valeurNote);
    }

    public async Task ExecuteAsync(long etudiantId, long ueId, double valeurNote)
    {
        await CheckBusinessRules(etudiantId, ueId, valeurNote);
        await repositoryFactory.NoteRepository().AddNoteAsync(new Note 
        { 
            EtudiantId = etudiantId, 
            UeId = ueId, 
            Valeur = valeurNote 
        });
    }

    private async Task CheckBusinessRules(long etudiantId, long ueId, double valeurNote)
        {
            // Validation des paramètres
            if (etudiantId <= 0) throw new ArgumentOutOfRangeException(nameof(etudiantId), "L'ID de l'étudiant doit être positif.");
            if (ueId <= 0) throw new ArgumentOutOfRangeException(nameof(ueId), "L'ID de l'UE doit être positif.");
            
            if (valeurNote < 0 || valeurNote > 20)
                throw new NoteInvalideException($"La note doit être comprise entre 0 et 20 (valeur reçue : {valeurNote})");

            // Vérification existence étudiant
            var etudiant = (await repositoryFactory.EtudiantRepository()
                .FindByConditionAsync(e => e.Id == etudiantId))
                .FirstOrDefault() ?? throw new EtudiantNotFoundException(etudiantId.ToString());

            // Vérification existence UE
            var ue = (await repositoryFactory.UeRepository()
                .FindByConditionAsync(u => u.Id == ueId))
                .FirstOrDefault() ?? throw new UeNotFoundException(ueId.ToString());

            // Vérification appartenance UE au parcours
            if (etudiant.ParcoursSuivi == null)
            {
                throw new ParcoursNotFoundException("L'étudiant n'a pas de parcours associé.");
            }

            var parcours = (await repositoryFactory.ParcoursRepository()
                    .FindByConditionAsync(p => p.Id == etudiant.ParcoursSuivi.Id))
                .FirstOrDefault() ?? throw new ParcoursNotFoundException(etudiant.ParcoursSuivi.Id.ToString());

            if (!parcours.UesEnseignees?.Any(u => u.Id == ueId) ?? true)
                throw new UeHorsParcoursException($"L'UE {ueId} n'appartient pas au parcours {parcours.Id}");

            // Vérification note existante
            var noteExistante = await repositoryFactory.NoteRepository()
                .FindByConditionAsync(n => n.EtudiantId == etudiantId && n.UeId == ueId);

            if (noteExistante.Count > 0)
                throw new NoteExistanteException($"L'étudiant {etudiantId} a déjà une note dans l'UE : {ueId}");
        }
}