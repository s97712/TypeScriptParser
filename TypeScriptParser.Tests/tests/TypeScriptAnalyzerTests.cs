using System;
using System.IO;
using Xunit;
using TypeScriptParser.Tests;

namespace TypeScriptParser.Tests
{
    public class TypeScriptAnalyzerTests
    {
        [Fact]
        public void AnalyzeFile_SimpleExportFunction_ShouldReturnExportedFunction()
        {
            // Arrange
            var analyzer = new TypeScriptAnalyzer();
            string sourceCode = "export function hello(): string { return 'world'; }";
            
            // Act
            var exportedFunctions = analyzer.AnalyzeFile(sourceCode);
            
            // Assert
            Assert.Single(exportedFunctions);
            Assert.Equal("hello", exportedFunctions[0].Name);
            Assert.Equal("string", exportedFunctions[0].ReturnType);
            Assert.Empty(exportedFunctions[0].Parameters);
        }

        [Fact]
        public void AnalyzeFile_FunctionWithParameters_ShouldParseParametersCorrectly()
        {
            // Arrange
            var analyzer = new TypeScriptAnalyzer();
            string sourceCode = "export function add(a: number, b: number): number { return a + b; }";
            
            // Act
            var exportedFunctions = analyzer.AnalyzeFile(sourceCode);
            
            // Assert
            Assert.Single(exportedFunctions);
            var func = exportedFunctions[0];
            Assert.Equal("add", func.Name);
            Assert.Equal("number", func.ReturnType);
            Assert.Equal(2, func.Parameters.Count);
            Assert.Equal("a", func.Parameters[0].Name);
            Assert.Equal("number", func.Parameters[0].Type);
            Assert.Equal("b", func.Parameters[1].Name);
            Assert.Equal("number", func.Parameters[1].Type);
        }

        [Fact]
        public void AnalyzeFile_EmptyFile_ShouldReturnEmptyList()
        {
            // Arrange
            var analyzer = new TypeScriptAnalyzer();
            string sourceCode = "";
            
            // Act
            var exportedFunctions = analyzer.AnalyzeFile(sourceCode);
            
            // Assert
            Assert.Empty(exportedFunctions);
        }

        [Fact]
        public void AnalyzeFile_NoExportedFunctions_ShouldReturnEmptyList()
        {
            // Arrange
            var analyzer = new TypeScriptAnalyzer();
            string sourceCode = "function internal(): void { console.log('internal'); }";
            
            // Act
            var exportedFunctions = analyzer.AnalyzeFile(sourceCode);
            
            // Assert
            Assert.Empty(exportedFunctions);
        }

        [Fact]
        public void Dispose_AfterDispose_AnalyzeFileShouldThrowObjectDisposedException()
        {
            // Arrange
            var analyzer = new TypeScriptAnalyzer();
            analyzer.Dispose();
            
            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => analyzer.AnalyzeFile("export function test() {}"));
        }
    }
}