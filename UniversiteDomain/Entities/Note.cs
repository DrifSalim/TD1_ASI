namespace UniversiteDomain.Entities;

public class Note
{
    public double Valeur { get; set; }
    public long EtudiantId { get; set; }
    public long UeId { get; set; }
    
    public override string ToString()
    {
        return $"Etudiant : {EtudiantId} - a eu {Valeur}/20 en {UeId}";
    }
}