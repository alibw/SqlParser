﻿using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SQLParser;

public static class Parser
{
    private static IEnumerable<char> seperators = new[]
    {
        '\n',
        '\r',
        '\t',
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
        bool notNullable = false;
        bool nameFilled = false;
        bool typeFilled = false;
        bool isForeignKey = false;
        bool isPrimaryKey = false;
        Column? foreignKeyColumn = new Column();

        var properties = new List<Column>();
        for (int i = 0; i < splitted.Count; i++)
        {
            if (i > 0 && splitted[i - 1] == "dbo")
            {
                model.TableName = splitted[i];
            }

            if (splitted[i] == "(" || splitted[i].ToCharArray().Last() == '(') 
            {
                columnTokens = true;
            }

            if (columnTokens)
            {
                if (splitted[i].Contains("IDENTITY"))
                {
                    column.IsIdentity = true;
                }

                if (splitted[i].Contains("NOT"))
                {
                    notNullable = true;
                    column.Nullable = false;
                }

                if ((splitted[i] == "NULL" || splitted[i] == "NULL,") && !notNullable)
                {
                    column.Nullable = true;
                }

                if ((splitted[i-1].ToCharArray().Last() == ',' || splitted[i-1] == "(") && !splitted[i].Contains(')'))
                {
                    column.Name = nameFilled ? column.Name : splitted[i];
                    nameFilled = true;
                    continue;
                }

                if (nameFilled && !typeFilled)
                {
                    column.Type = splitted[i];
                    typeFilled = true;
                    continue;
                }
                
                if ((splitted[i].ToCharArray().Last() == ',' || splitted[i]== ")") && nameFilled && typeFilled)
                {
                    columns.Add(column);
                    nameFilled = false;
                    typeFilled = false;
                    column = new Column();
                }
            }

            if (splitted[i] == "FOREIGN")
            {
                isForeignKey = true;
            }
            
            if (splitted[i] == "PRIMARY")
            {
                isPrimaryKey = true;
            }
            if (isForeignKey)
            {
                if (splitted[i - 1] == "(")
                {
                    foreignKeyColumn = columns.FirstOrDefault(x => x.Name == splitted[i]);
                    foreignKeyColumn!.IsForeignKey = true;
                }

                if ((splitted[i - 1] == "dbo"))
                {
                    foreignKeyColumn!.ForeignKeyTableName = splitted[i];
                    isForeignKey = false;
                }
            }
            if (isPrimaryKey)
            {
                if (splitted[i - 1] == "(")
                {
                    columns.FirstOrDefault(x => x.Name == splitted[i])!.IsPrimaryKey = true;
                    isPrimaryKey = false;
                }
            }
        }

        model.Properties = columns;

        return model;
    }
}