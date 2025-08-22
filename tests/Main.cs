using System;
using System.IO;
using TreeSitter.TypeScript;

var file = args[0];
var filetext = File.ReadAllText(file);

var analyzer = new TypeScriptAnalyzer();
var exportedFunctions = analyzer.AnalyzeFile(filetext);

foreach (var func in exportedFunctions)
{
    Console.WriteLine($"  函数名: {func.Name}");
    Console.WriteLine("  参数:");
    
    if (func.Parameters.Count == 0)
    {
        Console.WriteLine("    (无参数)");
    }
    else
    {
        for (int i = 0; i < func.Parameters.Count; i++)
        {
            var param = func.Parameters[i];
            var paramInfo = $"    参数{i + 1}: {param.Name}";
            
            if (!string.IsNullOrEmpty(param.Type))
                paramInfo += $": {param.Type}";
                
            if (!string.IsNullOrEmpty(param.DefaultValue))
                paramInfo += $" = {param.DefaultValue}";
                
            if (param.IsOptional)
                paramInfo += " (可选)";
                
            if (param.IsRestParameter)
                paramInfo += " (剩余参数)";
                
            Console.WriteLine(paramInfo);
        }
    }
    
    Console.WriteLine($"  返回类型: {func.ReturnType}");
    Console.WriteLine($"  行号: {func.LineNumber}");
    Console.WriteLine();
}