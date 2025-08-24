using System;
using System.Collections.Generic;
using TypeScriptParser.TreeSitter;
using System.Runtime.InteropServices;
using TypeScriptParser;

namespace TypeScriptParser.Tests
{
    // 参数信息结构
    public class Parameter
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string DefaultValue { get; set; }
        public bool IsOptional { get; set; }
        public bool IsRestParameter { get; set; }
    }

    // 导出函数信息结构
    public class ExportedFunction
    {
        public string Name { get; set; }
        public List<Parameter> Parameters { get; set; } = new List<Parameter>();
        public string ReturnType { get; set; }
        public int LineNumber { get; set; }
        public string SourceText { get; set; }
    }

    // TypeScript分析器模块
    public class TypeScriptAnalyzer : IDisposable
    {
        private readonly Parser parser;
        private readonly List<ExportedFunction> exportedFunctions = [];
        private bool disposed = false;

        public TypeScriptAnalyzer()
        {
            parser = new Parser();
        }

        public List<ExportedFunction> AnalyzeFile(string fileContent)
        {
            if (disposed)
                throw new ObjectDisposedException(nameof(TypeScriptAnalyzer));

            exportedFunctions.Clear();
            
            using var tree = parser.ParseString(fileContent);
            if (tree == null) {
                return exportedFunctions;
            }

            using var cursor = new TSCursor(tree.root_node(), tree.language());
            TraverseForExports(cursor, fileContent);
            
            return [.. exportedFunctions];
        }

        private void TraverseForExports(TSCursor cursor, string filetext)
        {
            do {
                var type = cursor.current_symbol();
                int so = (int)cursor.current_node().start_offset();
                int eo = (int)cursor.current_node().end_offset();
                int line = (int)cursor.current_node().start_point().row + 1;

                // 查找 export_statement 节点
                if (type == "export_statement") {
                    ProcessExportStatement(cursor, filetext, line);
                }

                // 递归遍历子节点
                if (cursor.goto_first_child()) {
                    TraverseForExports(cursor, filetext);
                    cursor.goto_parent();
                }
            } while (cursor.goto_next_sibling());
        }

        private void ProcessExportStatement(TSCursor cursor, string filetext, int line)
        {
            int so = (int)cursor.current_node().start_offset();
            int eo = (int)cursor.current_node().end_offset();
            var exportText = filetext.AsSpan(so, eo - so).ToString();

            // 保存当前位置
            var savedCursor = cursor;
            
            if (cursor.goto_first_child()) {
                do {
                    var childType = cursor.current_symbol();
                    
                    // 直接导出函数: export function functionName()
                    if (childType == "function_declaration") {
                        ProcessFunctionDeclaration(cursor, filetext, line, exportText, true);
                    }
                    // 导出列表: export { functionName }
                    else if (childType == "export_clause") {
                        ProcessExportClause(cursor, filetext, line, exportText);
                    }
                } while (cursor.goto_next_sibling());
                
                cursor.goto_parent();
            }
        }

        private void ProcessFunctionDeclaration(TSCursor cursor, string filetext, int line, string exportText, bool isDirectExport)
        {
            string functionName = "";
            var parameters = new List<Parameter>();
            string returnType = "";

            if (cursor.goto_first_child()) {
                do {
                    var childType = cursor.current_symbol();
                    int so = (int)cursor.current_node().start_offset();
                    int eo = (int)cursor.current_node().end_offset();
                    var text = filetext.AsSpan(so, eo - so).ToString();

                    if (childType == "identifier" && string.IsNullOrEmpty(functionName)) {
                        functionName = text;
                    }
                    else if (childType == "formal_parameters") {
                        parameters = ParseParameters(cursor, filetext);
                    }
                    else if (childType == "type_annotation") {
                        returnType = text.Substring(1).Trim(); // 移除前面的 ":"
                    }
                } while (cursor.goto_next_sibling());
                
                cursor.goto_parent();
            }

            if (!string.IsNullOrEmpty(functionName)) {
                exportedFunctions.Add(new ExportedFunction {
                    Name = functionName,
                    Parameters = parameters,
                    ReturnType = returnType,
                    LineNumber = line,
                    SourceText = exportText.Trim()
                });
            }
        }

        private void ProcessExportClause(TSCursor cursor, string filetext, int line, string exportText)
        {
            // 这里处理 export { functionName } 形式
            // 需要查找对应的函数声明
            if (cursor.goto_first_child()) {
                do {
                    var childType = cursor.current_symbol();
                    
                    if (childType == "export_specifier") {
                        if (cursor.goto_first_child()) {
                            var identifierType = cursor.current_symbol();
                            if (identifierType == "identifier") {
                                int so = (int)cursor.current_node().start_offset();
                                int eo = (int)cursor.current_node().end_offset();
                                var functionName = filetext.AsSpan(so, eo - so).ToString();
                                
                                // 添加导出函数（简化版本，不查找具体的函数声明）
                                var placeholderParameters = new List<Parameter>
                                {
                                    new Parameter
                                    {
                                        Name = "// 从导出列表中识别",
                                        Type = "// 需要查找原函数声明",
                                        IsOptional = false,
                                        IsRestParameter = false
                                    }
                                };
                                
                                exportedFunctions.Add(new ExportedFunction {
                                    Name = functionName,
                                    Parameters = placeholderParameters,
                                    ReturnType = "// 需要查找原函数声明",
                                    LineNumber = line,
                                    SourceText = exportText.Trim()
                                });
                            }
                            cursor.goto_parent();
                        }
                    }
                } while (cursor.goto_next_sibling());
                
                cursor.goto_parent();
            }
        }

        private List<Parameter> ParseParameters(TSCursor cursor, string filetext)
        {
            var parameters = new List<Parameter>();
            
            // 保存当前位置
            var originalCursor = cursor;
            
            if (cursor.goto_first_child()) {
                do {
                    var childType = cursor.current_symbol();
                    
                    if (childType == "required_parameter" || childType == "optional_parameter" || childType == "rest_parameter") {
                        var parameter = ParseSingleParameter(cursor, filetext);
                        if (parameter != null) {
                            parameters.Add(parameter);
                        }
                    }
                } while (cursor.goto_next_sibling());
                
                cursor.goto_parent();
            }
            
            return parameters;
        }

        private Parameter ParseSingleParameter(TSCursor cursor, string filetext)
        {
            var parameter = new Parameter();
            bool isRestParameter = cursor.current_symbol() == "rest_parameter";
            bool isOptional = cursor.current_symbol() == "optional_parameter";
            
            parameter.IsRestParameter = isRestParameter;
            parameter.IsOptional = isOptional;
            
            if (cursor.goto_first_child()) {
                do {
                    var childType = cursor.current_symbol();
                    int so = (int)cursor.current_node().start_offset();
                    int eo = (int)cursor.current_node().end_offset();
                    var text = filetext.AsSpan(so, eo - so).ToString();
                    
                    if (childType == "identifier" && string.IsNullOrEmpty(parameter.Name)) {
                        parameter.Name = text;
                    }
                    else if (childType == "type_annotation") {
                        parameter.Type = text.Substring(1).Trim(); // 移除前面的 ":"
                    }
                    else if (childType == "assignment_pattern") {
                        // 处理默认参数值
                        if (cursor.goto_first_child()) {
                            do {
                                var assignChildType = cursor.current_symbol();
                                if (assignChildType == "identifier" && string.IsNullOrEmpty(parameter.Name)) {
                                    int assignSo = (int)cursor.current_node().start_offset();
                                    int assignEo = (int)cursor.current_node().end_offset();
                                    parameter.Name = filetext.AsSpan(assignSo, assignEo - assignSo).ToString();
                                }
                                else if (assignChildType != "identifier" && assignChildType != "=") {
                                    // 默认值
                                    int defaultSo = (int)cursor.current_node().start_offset();
                                    int defaultEo = (int)cursor.current_node().end_offset();
                                    parameter.DefaultValue = filetext.AsSpan(defaultSo, defaultEo - defaultSo).ToString();
                                }
                            } while (cursor.goto_next_sibling());
                            cursor.goto_parent();
                        }
                    }
                    else if (childType == "...") {
                        // Rest parameter spread operator
                        parameter.IsRestParameter = true;
                    }
                } while (cursor.goto_next_sibling());
                
                cursor.goto_parent();
            }
            
            return !string.IsNullOrEmpty(parameter.Name) ? parameter : null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    parser?.Dispose();
                }
                disposed = true;
            }
        }

        ~TypeScriptAnalyzer()
        {
            Dispose(false);
        }
    }
}