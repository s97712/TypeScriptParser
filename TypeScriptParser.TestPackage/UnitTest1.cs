using Xunit;

namespace TypeScriptParser.TestPackage;

public class UnitTest1
{
    [Fact]
    public void Can_Create_Parser()
    {
        using var parser = new TreeSitter.TypeScript.TypeScriptParser();
        Assert.True(parser.IsAvailable);
    }

    [Fact]
    public void Can_Parse_Code()
    {
        using var parser = new TreeSitter.TypeScript.TypeScriptParser();
        var tree = parser.ParseString("const x = 1;");
        Assert.Equal("program", tree.root_node().type());
    }
}
