using System.Diagnostics;
using System.Runtime.CompilerServices;

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

    public static Table Parse(string input)
    {
        var splitted = Split(input);
        var model = new Table();
        List<Column> columns = new List<Column>();
        var column = new Column();
        bool columnTokens = false;

        var properties = new List<Column>();
        for (int i = 0; i < splitted.Count; i++)
        {

            if (splitted[i - 1] == "dbo")
            {
                model.TableName = splitted[i];
            }

            if (splitted[i] == "(")
            {
                columnTokens = true;
            }

            if (columnTokens)
            {
                if (!splitted[i].Contains("IDENTITY"))
                {
                    if (!splitted[i].Contains("NOT"))
                    {
                        if (!splitted[i].Contains("NULL") || !splitted[i].Contains("NULL,"))
                        {
                            if (splitted[i].All(Char.IsLetter))
                            {
                                if (column.Name != null)
                                {
                                    column.Type = splitted[i];
                                }
                                else
                                {
                                    column.Name = splitted[i];
                                }
                            }
                        }
                        else
                        {
                            if (splitted[i].ToCharArray().Last() == ',')
                                column = new Column();
                            column.Nullable = false;
                        }
                    }
                    else
                    {
                        column.Nullable = true;
                    }
                    if (splitted[i].ToCharArray().Last() == ')' && !splitted[i].Any(Char.IsDigit) && splitted[i] != "(max)")
                    {
                        columnTokens = false;
                    }
                }

                if (column.Name != null && column.Type != null)
                {
                    columns.Add(column);
                    column = new Column();
                }
            }
        }

        model.Properties = columns;

        return model;
    }
}