using System.Diagnostics;

namespace SQLParser;

public static class Parser
{
    private static IEnumerable<char> seperators = new[]
    {
        '\n',
        '\r',
        ' ',
        '.',
        '[',
        ']'
    };

    public static List<string> Split(string input)
    {
        return input.Split((char[]?)seperators, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    public static List<string> SplitLine(string input, char seperator)
    {
        List<string> result = new List<string>();
        var lines = input.Split(input, seperator).ToList();
        foreach (var item in lines)
        {
            result.AddRange(Split(item));
        }

        return result;
    }

    private static List<ParsedProperty> GetProperties(List<string> input)
    {
        string joinedInput = String.Join(" ", input);
        List<ParsedProperty> result = new List<ParsedProperty>();

        foreach (var line in joinedInput.Split(','))
        {
            var lineTokens = Split(line);
            var parsedProperty = new ParsedProperty();
            parsedProperty.Name = lineTokens[0];
            parsedProperty.Type = lineTokens[1];
            if (lineTokens[2].ToCharArray().First() == '(' && lineTokens[2].ToCharArray().Last() == ')')
            {
                parsedProperty.MaxLength = lineTokens[2].Replace('(', ' ').Replace(')', ' ');
            }

            if (lineTokens[3] == "NOT" && lineTokens[4] == "NULL")
            {
                parsedProperty.Nullable = false;
            }
            else
            {
                parsedProperty.Nullable = true;
            }
        }

        return result;
    }

    public static ParsedModel Parse(string input)
    {
        var splitted = Split(input);
        var model = new ParsedModel();
        int propertiesStart = 0;
        int propertiesEnd = 0;

        var properties = new List<ParsedProperty>();
        for (int i = 0; i < splitted.Count; i++)
        {
            if (splitted[i] == "USE")
            {
                model.DbName = splitted[i + 1];
            }

            if (splitted[i] == "dbo")
            {
                model.TableName = splitted[i + 1];
            }

            if (splitted[i].ToCharArray().Last() == '(')
            {
                propertiesStart = splitted.IndexOf(splitted[i]);
            }

            if (splitted[i].ToCharArray().Last() == ')' && !splitted[i - 1].Any(Char.IsDigit))
            {
                propertiesEnd = splitted.IndexOf(splitted[i]);
                GetProperties(splitted.GetRange(propertiesStart, propertiesEnd));
            }

           
        }

        return model;
    }
}