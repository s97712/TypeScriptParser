# TypeScript Parser API 参考

基于 Tree-sitter 的 TypeScript 解析器 .NET 绑定。

## 快速开始

```csharp
using TypeScriptParser;

using var parser = new Parser();
using var tree = parser.ParseString("const x: number = 42;");
var root = tree.root_node();
Console.WriteLine(root.type()); // "program"
```

## 高级 API

### Parser

```csharp
public class Parser : IDisposable
{
    public Parser()
    public TSTree ParseString(string sourceCode)
    public void Dispose()
}
```

**使用：**
```csharp
using var parser = new Parser();
using var tree = parser.ParseString(code);
```

## 底层 API

### TSTree - 语法树

```csharp
public sealed class TSTree : IDisposable
{
    public TSNode root_node()
    public TSTree copy()                    // 多线程使用
    public TSLanguage language()
    public void edit(TSInputEdit edit)      // 增量解析
}
```

### TSNode - 节点

```csharp
public struct TSNode
{
    // 基本信息
    public string type()
    public bool is_null()
    public bool is_named()
    public bool has_error()
    
    // 位置
    public uint start_offset()
    public uint end_offset()
    public TSPoint start_point()
    public TSPoint end_point()
    
    // 导航
    public TSNode parent()
    public TSNode child(uint index)
    public uint child_count()
    public TSNode next_sibling()
    public TSNode prev_sibling()
    
    // 字段访问
    public TSNode child_by_field_name(string field_name)
    
    // 文本
    public string text(string sourceCode)
}
```

### TSCursor - 遍历

```csharp
public sealed class TSCursor : IDisposable
{
    public TSCursor(TSNode node, TSLanguage lang)
    
    public TSNode current_node()
    public string current_symbol()
    public bool goto_first_child()
    public bool goto_next_sibling()
    public bool goto_parent()
}
```

**遍历示例：**
```csharp
using var cursor = new TSCursor(tree.root_node(), tree.language());
do {
    var node = cursor.current_node();
    Console.WriteLine($"{cursor.current_symbol()}: {node.type()}");
} while (cursor.goto_next_sibling() || cursor.goto_first_child());
```

### TSQuery - 查询

```csharp
public sealed class TSQuery : IDisposable
{
    public uint pattern_count()
    public uint capture_count()
    public void disable_pattern(uint patternIndex)
}

public sealed class TSQueryCursor : IDisposable
{
    public void exec(TSQuery query, TSNode node)
    public bool next_match(out TSQueryMatch match, out TSQueryCapture[] captures)
    public void set_match_limit(uint limit)
}
```

**查询示例：**
```csharp
var query = language.query_new("(function_declaration name: (identifier) @name)", 
                               out var offset, out var error);
using var queryCursor = new TSQueryCursor();
queryCursor.exec(query, tree.root_node());

while (queryCursor.next_match(out var match, out var captures)) {
    foreach (var capture in captures) {
        Console.WriteLine($"函数名: {capture.node.text(sourceCode)}");
    }
}
```

### TSLanguage - 语言定义

```csharp
public sealed class TSLanguage : IDisposable
{
    public string[] symbols
    public string[] fields
    
    public string symbol_name(ushort symbol)
    public string field_name_for_id(ushort fieldId)
    public TSQuery query_new(string source, out uint error_offset, out TSQueryError error_type)
}
```

## 数据结构

### TSPoint
```csharp
public struct TSPoint
{
    public uint row;        // 行号（从0开始）
    public uint column;     // 列号（从0开始）
}
```

### TSRange
```csharp
public struct TSRange
{
    public TSPoint start_point;
    public TSPoint end_point;
    public uint start_byte;
    public uint end_byte;
}
```

### TSQueryCapture
```csharp
public struct TSQueryCapture
{
    public TSNode node;
    public uint index;
}
```

## 实用示例

### 查找所有函数

```csharp
var query = language.query_new(@"
    (function_declaration 
      name: (identifier) @func_name
      parameters: (formal_parameters) @params) @func
", out var offset, out var error);

using var cursor = new TSQueryCursor();
cursor.exec(query, tree.root_node());

while (cursor.next_match(out var match, out var captures)) {
    var funcNode = captures[0].node;  // @func
    var nameNode = captures[1].node;  // @func_name
    
    Console.WriteLine($"函数: {nameNode.text(sourceCode)}");
    Console.WriteLine($"位置: 行{funcNode.start_point().row + 1}");
}
```

### 遍历所有变量声明

```csharp
using var cursor = new TSCursor(tree.root_node(), tree.language());

void TraverseNode() {
    var node = cursor.current_node();
    
    if (node.type() == "variable_declarator") {
        var nameNode = node.child_by_field_name("name");
        if (!nameNode.is_null()) {
            Console.WriteLine($"变量: {nameNode.text(sourceCode)}");
        }
    }
    
    if (cursor.goto_first_child()) {
        do {
            TraverseNode();
        } while (cursor.goto_next_sibling());
        cursor.goto_parent();
    }
}

TraverseNode();
```

### 增量解析

```csharp
// 初始解析
using var tree = parser.ParseString("const x = 1;");

// 编辑：在末尾添加内容
var edit = new TSInputEdit {
    start_byte = 12,  // "const x = 1;" 长度
    old_end_byte = 12,
    new_end_byte = 25,  // 新增 "const y = 2;" 长度
    start_point = new TSPoint(0, 12),
    old_end_point = new TSPoint(0, 12),
    new_end_point = new TSPoint(0, 25)
};

tree.edit(edit);
var newTree = parser.ParseString(tree, "const x = 1;const y = 2;");
```

## 注意事项

- 所有 `IDisposable` 对象使用 `using` 语句
- 多线程使用 `tree.copy()` 创建副本
- 设置超时：`parser.set_timeout_micros(5000000)`
- 检查错误：`root.has_error()`

## 平台支持

- Linux x64
- macOS ARM64  
- Windows x64