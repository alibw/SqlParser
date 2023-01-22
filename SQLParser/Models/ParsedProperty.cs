namespace SQLParser;

public class ParsedProperty
{
    public string Name { get; set; }
    public string Value { get; set; }
    public string Type { get; set; }
    public bool Nullable { get; set; }
    public string MaxLength { get; set; }
}