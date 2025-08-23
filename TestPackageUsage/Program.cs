fusing System;
using TreeSitter.TypeScript;

Console.WriteLine("=== TypeScript Parser NuGet包测试 ===");

try
{
    // 创建解析器实例
    using var parser = new TypeScriptParser();
    
    Console.WriteLine($"✅ 解析器创建成功: {parser.IsAvailable}");
    
    // 测试解析简单的TypeScript代码
    string testCode = @"
        function greet(name: string): string {
            return `Hello, ${name}!`;
        }
        
        const message = greet('World');
        console.log(message);
    ";
    
    Console.WriteLine("📝 测试解析TypeScript代码...");
    var tree = parser.ParseString(testCode);
    
    Console.WriteLine($"✅ 解析成功! 根节点类型: {tree.root_node().type()}");
    
    // 创建游标遍历语法树
    using var cursor = parser.CreateCursor(tree);
    
    Console.WriteLine("\n🔍 遍历语法树:");
    int nodeCount = 0;
    cursor.goto_first_child();
    
    do
    {
        var node = cursor.current_node();
        if (node.type() != null && !node.type().StartsWith("_"))
        {
            Console.WriteLine($"  - {node.type()} ({node.start_point().row + 1},{node.start_point().column})");
            nodeCount++;
            if (nodeCount > 10) break; // 限制输出数量
        }
    } while (cursor.goto_next_sibling());
    
    Console.WriteLine($"\n✅ 成功遍历 {nodeCount} 个语法节点");
    Console.WriteLine("🎉 NuGet包测试完成 - 所有功能正常！");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ 测试失败: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"   内部错误: {ex.InnerException.Message}");
    }
    Console.WriteLine($"   错误类型: {ex.GetType().Name}");
    Environment.Exit(1);
}
