using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using TypeScriptParser;
using TypeScriptParser.TreeSitter;

namespace TypeScriptParser.Tests
{
    public class ExportExtractorTests
    {
        [Fact]
        public void ExtractExportFunctions_SimpleExport_ShouldReturnFunctionNames()
        {
            // Arrange
            string code = @"
export function add(a: number, b: number): number {
    return a + b;
}

export function multiply(x: number, y: number): number {
    return x * y;
}";
            
            // Act
            var exportedFunctions = ExtractExportFunctions(code);
            
            // Assert
            Assert.Equal(2, exportedFunctions.Count);
            Assert.Contains("add", exportedFunctions);
            Assert.Contains("multiply", exportedFunctions);
        }

        [Fact]
        public void ExtractExportFunctions_MixedExports_ShouldOnlyReturnFunctions()
        {
            // Arrange
            string code = @"
export function add(a: number, b: number): number {
    return a + b;
}

function helper() {
    return 'not exported';
}

export const PI = 3.14;

export function multiply(x: number, y: number): number {
    return x * y;
}";
            
            // Act
            var exportedFunctions = ExtractExportFunctions(code);
            
            // Assert
            Assert.Equal(2, exportedFunctions.Count);
            Assert.Contains("add", exportedFunctions);
            Assert.Contains("multiply", exportedFunctions);
            Assert.DoesNotContain("helper", exportedFunctions);
            Assert.DoesNotContain("PI", exportedFunctions);
        }

        [Fact]
        public void ExtractExportFunctions_NoExports_ShouldReturnEmpty()
        {
            // Arrange
            string code = @"
function helper() {
    return 'not exported';
}

const PI = 3.14;";
            
            // Act
            var exportedFunctions = ExtractExportFunctions(code);
            
            // Assert
            Assert.Empty(exportedFunctions);
        }

        [Fact]
        public void ExtractExportFunctions_EmptyCode_ShouldReturnEmpty()
        {
            // Arrange
            string code = "";
            
            // Act
            var exportedFunctions = ExtractExportFunctions(code);
            
            // Assert
            Assert.Empty(exportedFunctions);
        }

        [Fact]
        public void ExtractExportFunctions_ComplexExports_ShouldExtractAllFunctions()
        {
            // Arrange
            string code = @"
export function calculate(x: number): number {
    return x * 2;
}

export function processData(data: any[]): any[] {
    return data.filter(item => item != null);
}

export function validateInput(input: string): boolean {
    return input && input.length > 0;
}

// 非函数导出
export const config = { api: 'https://api.example.com' };
export let counter = 0;

// 非导出函数
function internalHelper() {
    return 'internal';
}";
            
            // Act
            var exportedFunctions = ExtractExportFunctions(code);
            
            // Assert
            Assert.Equal(3, exportedFunctions.Count);
            Assert.Contains("calculate", exportedFunctions);
            Assert.Contains("processData", exportedFunctions);
            Assert.Contains("validateInput", exportedFunctions);
            Assert.DoesNotContain("internalHelper", exportedFunctions);
        }

        [Fact]
        public void ExtractExportFunctions_AsyncFunctions_ShouldExtractAsyncFunctions()
        {
            // Arrange
            string code = @"
export async function fetchData(url: string): Promise<any> {
    const response = await fetch(url);
    return response.json();
}

export function syncFunction(): string {
    return 'sync';
}";
            
            // Act
            var exportedFunctions = ExtractExportFunctions(code);
            
            // Assert
            Assert.Equal(2, exportedFunctions.Count);
            Assert.Contains("fetchData", exportedFunctions);
            Assert.Contains("syncFunction", exportedFunctions);
        }

        /// <summary>
        /// 提取TypeScript代码中所有导出的函数名称
        /// </summary>
        /// <param name="code">TypeScript源代码</param>
        /// <returns>导出函数名称列表</returns>
        private List<string> ExtractExportFunctions(string code)
        {
            var exportedFunctions = new List<string>();
            
            using var parser = new Parser();
            using var tree = parser.ParseString(code);
            
            // 创建查询匹配 export 函数
            var query = tree.language().query_new(@"
                (export_statement 
                    declaration: (function_declaration 
                        name: (identifier) @func_name))
            ", out var offset, out var error);
            
            if (query != null && error == TSQueryError.TSQueryErrorNone)
            {
                using var cursor = new TSQueryCursor();
                cursor.exec(query, tree.root_node());
                
                while (cursor.next_match(out var match, out var captures))
                {
                    foreach (var capture in captures)
                    {
                        var funcName = capture.node.text(code);
                        if (!string.IsNullOrWhiteSpace(funcName))
                        {
                            exportedFunctions.Add(funcName);
                        }
                    }
                }
                
                query.Dispose();
            }
            
            return exportedFunctions;
        }
    }
}