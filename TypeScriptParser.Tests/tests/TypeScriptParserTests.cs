using System;
using Xunit;
using TreeSitter.TypeScript;

namespace TypeScriptParser.Tests
{
    public class TypeScriptParserTests
    {
        [Fact]
        public void TypeScriptParser_Constructor_ShouldCreateValidInstance()
        {
            // Arrange & Act
            using var parser = new TreeSitter.TypeScript.TypeScriptParser();
            
            // Assert
            Assert.NotNull(parser);
            Assert.NotNull(parser.Language);
            Assert.True(parser.IsAvailable);
        }

        [Fact]
        public void ParseString_SimpleTypeScriptCode_ShouldReturnValidTree()
        {
            // Arrange
            using var parser = new TreeSitter.TypeScript.TypeScriptParser();
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
            using var parser = new TreeSitter.TypeScript.TypeScriptParser();
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
            using var parser = new TreeSitter.TypeScript.TypeScriptParser();
            string sourceCode = "function hello() { return 'world'; }";
            using var tree = parser.ParseString(sourceCode);
            
            // Act
            using var cursor = parser.CreateCursor(tree);
            
            // Assert
            Assert.NotNull(cursor);
        }

        [Fact]
        public void CreateCursor_WithNullTree_ShouldThrowArgumentNullException()
        {
            // Arrange
            using var parser = new TreeSitter.TypeScript.TypeScriptParser();
            
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => parser.CreateCursor(null));
        }

        [Fact]
        public void Dispose_AfterDispose_IsAvailableShouldBeFalse()
        {
            // Arrange
            var parser = new TreeSitter.TypeScript.TypeScriptParser();
            Assert.True(parser.IsAvailable);
            
            // Act
            parser.Dispose();
            
            // Assert
            Assert.False(parser.IsAvailable);
        }

        [Fact]
        public void ParseString_AfterDispose_ShouldThrowObjectDisposedException()
        {
            // Arrange
            var parser = new TreeSitter.TypeScript.TypeScriptParser();
            parser.Dispose();
            
            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => parser.ParseString("const x = 1;"));
        }
    }
}
