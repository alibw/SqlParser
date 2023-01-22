namespace SQLParser;

public class ParsedModel
{
    public string DbName { get; set; }
    
    public string TableName { get; set; }

    public List<ParsedProperty> Properties { get; set; }
}