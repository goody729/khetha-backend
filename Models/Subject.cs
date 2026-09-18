namespace TertiaryInstitutions.Models;

public class Subject
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsDesignated { get; set; }
    public bool IsCompulsory { get; set; }
    public string Notes { get; set; } = string.Empty;
}
