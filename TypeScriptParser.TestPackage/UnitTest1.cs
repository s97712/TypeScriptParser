using Xunit;
using TypeScriptParser;

namespace TypeScriptParser.TestPackage;

public class UnitTest1
{
    [Fact]
    public void Can_Create_Parser()
    {
        using var parser = new Parser();
        // 通过能否成功解析来验证Parser可用性
        var tree = parser.ParseString("const x = 1;");
        Assert.NotNull(tree);
    }

    [Fact]
    public void Can_Parse_Code()
    {
        using var parser = new Parser();
        var tree = parser.ParseString("const x = 1;");
        Assert.Equal("program", tree.root_node().type());
    }
}
