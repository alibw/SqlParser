using System.Text;
using NUnit.Framework;

namespace SQLParser;

[TestFixture]
public class Test
{
    public static string script = @"USE [College]
    CREATE TABLE [dbo].[Lesson](
    [Id] [bigint] IDENTITY(1,1) NOT NULL,
    [Title] [nvarchar](max) NOT NULL,
    [UnitNum] [bigint] NOT NULL)";

    [Test]
    public void SplitTest()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var item in Parser.Split(script))
        {
            sb.Append("|" + item);
        }

        Console.WriteLine(sb.ToString());
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
        var parsed = Parser.Parse(script);
        StringBuilder sb = new StringBuilder();
        foreach (var item in parsed.Properties)
        {
            sb.Append("|" + item.Name);
        }
    }
}