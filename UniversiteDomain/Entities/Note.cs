namespace UniversiteDomain.Entities;

public class Note
{
    // Clé primaire composite
    public long EtudiantId { get; set; }
    public long UeId { get; set; }

    // Propriété
    public float Valeur { get; set; }

    // Relations ManyToOne
    public Etudiant Etudiant { get; set; }
    public Ue Ue { get; set; }
    
    public override string ToString()
    {
        return $"Etudiant : {Etudiant} - a eu {Valeur}/20 en {Ue}";
    }
}