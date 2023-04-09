using System.Text;
using NUnit.Framework;

namespace SQLParser;

[TestFixture]
public class Test
{
    public static string script = @"
    CREATE TABLE [dbo].[Lesson]
    (
    [Id] [bigint] NOT NULL IDENTITY(1,1),
    [Title] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [UnitNum] [bigint] NOT NULL
    )";

    [Test]
    public void SplitTest()
    {
        List<string> tokens = new()
        {
            "CREATE","TABLE","dbo","Lesson","(","Id","bigint",
            "NOT","NULL,","IDENTITY(1,1)","Title","nvarchar","(max)","COLLATE","SQL_Latin1_General_CP1_CI_AS","NOT","NULL,","UnitNum","bigint","NOT","NULL",")"
        };
        Assert.AreEqual(tokens, Parser.Split(script));
    }

    [Test]
    public void GetDbNameTest()
    {
        Assert.AreEqual("College", Parser.Parse(script).DbName);
    }

    [Test]
    public void GetTableNameTest()
    {
        Assert.AreEqual("Lesson", Parser.Parse(script).TableName);
    }
    
    [Test]
    public void GetPropertiesNamesTest()
    {
        List<Column> columns = new()
        {
            new()
            {
                Name = "Id",
                Type = "bigint",
                Nullable = false
            },
            new()
            {
                Name = "Title",
                Type = "nvarchar",
                Nullable = false
            },
            new()
            {
                Name = "UnitNum",
                Type = "bigint",
                Nullable = false
            }

        };
        var parsed = Parser.Parse(script);
        Assert.AreEqual(columns,parsed.Properties);
    }
}