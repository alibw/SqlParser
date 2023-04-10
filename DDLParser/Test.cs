using System.Text;
using NUnit.Framework;

namespace SQLParser;

[TestFixture]
public class Test
{
    public static string script = @"
CREATE TABLE [dbo].[Session]
(
[Id] [bigint] NOT NULL IDENTITY(1, 1),
[MasterId] [bigint] NOT NULL,
[WeekDay] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Time] [time] NOT NULL,
[ClassNum] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[LessonId] [bigint] NOT NULL
)
GO
ALTER TABLE [dbo].[Session] ADD CONSTRAINT [PK_Session] PRIMARY KEY CLUSTERED ([Id])
GO
ALTER TABLE [dbo].[Session] ADD CONSTRAINT [FK_Session_Lesson] FOREIGN KEY ([LessonId]) REFERENCES [dbo].[Lesson] ([Id])
GO
ALTER TABLE [dbo].[Session] ADD CONSTRAINT [FK_Session_Master] FOREIGN KEY ([MasterId]) REFERENCES [dbo].[Master] ([Id])
GO
";

    [Test]
    public void SplitTest()
    {
        List<string> tokens = new()
        {
            "CREATE","TABLE","dbo","Lesson","(","Id","bigint",
            "NOT","NULL,","IDENTITY(1,1)","Title","nvarchar","(max)","COLLATE","SQL_Latin1_General_CP1_CI_AS","NOT","NULL,","UnitNum","bigint","NOT","NULL",")"
        };
        var f  =Parser.Split(script);
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