using System;
using Xunit;
using TypeScriptParser;
using TypeScriptParser.TreeSitter;

namespace TypeScriptParser.Tests
{
    public class TypeScriptParserTests
    {
        [Fact]
        public void TypeScriptParser_Constructor_ShouldCreateValidInstance()
        {
            // Arrange & Act
            using var parser = new Parser();
            
            // Assert
            Assert.NotNull(parser);
            // 通过能否成功解析来验证Parser可用性
            var tree = parser.ParseString("const x = 1;");
            Assert.NotNull(tree);
        }

        [Fact]
        public void ParseString_SimpleTypeScriptCode_ShouldReturnValidTree()
        {
            // Arrange
            using var parser = new Parser();
            string sourceCode = "const x: number = 42;";
            
            // Act
            using var tree = parser.ParseString(sourceCode);
            
            // Assert
            Assert.NotNull(tree);
        }

        [Fact]
        public void ParseString_EmptyString_ShouldReturnValidTree()
        {
            // Arrange
            using var parser = new Parser();
            string sourceCode = "";
            
            // Act
            using var tree = parser.ParseString(sourceCode);
            
            // Assert
            Assert.NotNull(tree);
        }

        [Fact]
        public void CreateCursor_WithValidTree_ShouldReturnCursor()
        {
            // Arrange
            using var parser = new Parser();
            string sourceCode = "function hello() { return 'world'; }";
            using var tree = parser.ParseString(sourceCode);
            
            // Act
            using var cursor = new TSCursor(tree.root_node(), tree.language());
            
            // Assert
            Assert.NotNull(cursor);
        }

        [Fact]
        public void CreateCursor_WithValidNode_ShouldWork()
        {
            // Arrange
            using var parser = new Parser();
            var tree = parser.ParseString("const x = 1;");
            
            // Act & Assert - 测试正常创建游标
            using var cursor = new TSCursor(tree.root_node(), tree.language());
            Assert.NotNull(cursor);
            Assert.NotNull(cursor.current_symbol());
        }

        [Fact]
        public void Dispose_AfterDispose_ParseStringShouldThrow()
        {
            // Arrange
            var parser = new Parser();
            
            // Act
            parser.Dispose();
            
            // Assert
            Assert.Throws<ObjectDisposedException>(() => parser.ParseString("const x = 1;"));
        }

        [Fact]
        public void ParseString_AfterDispose_ShouldThrowObjectDisposedException()
        {
            // Arrange
            var parser = new Parser();
            parser.Dispose();
            
            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => parser.ParseString("const x = 1;"));
        }
    }
}
