# TypeScript Parser 使用指南

一个简单易用的 TypeScript 解析器，帮你分析和处理 TypeScript 代码。

## 🚀 快速开始

### 安装
```bash
dotnet add package TypeScriptParser
```

### 第一个例子
```csharp
using TypeScriptParser;

// 1. 创建解析器
using var parser = new Parser();

// 2. 解析代码
using var tree = parser.ParseString("const message = 'Hello TypeScript!';");

// 3. 查看结果
var root = tree.root_node();
Console.WriteLine($"解析成功！根节点类型: {root.type()}");
// 输出: 解析成功！根节点类型: program
```

## 📖 基础概念

### 语法树
当你写代码时：
```typescript
const x = 42;
```

解析器会把它变成这样的树结构：
```
program
└── variable_declaration
    └── variable_declarator
        ├── identifier (x)
        └── number (42)
```

### 探索节点
```csharp
using var parser = new Parser();
using var tree = parser.ParseString("const name = 'Alice';");

var root = tree.root_node();
Console.WriteLine($"根节点类型: {root.type()}");
Console.WriteLine($"子节点数量: {root.child_count()}");

// 遍历子节点
for (uint i = 0; i < root.child_count(); i++)
{
    var child = root.child(i);
    Console.WriteLine($"子节点 {i}: {child.type()}");
}
```

## 🎯 实用示例

### 提取所有导出函数
```csharp
using TypeScriptParser;
using TypeScriptParser.TreeSitter;

string code = @"
export function add(a: number, b: number): number {
    return a + b;
}

function helper() {
    return 'not exported';
}

export function multiply(x: number, y: number): number {
    return x * y;
}
";

using var parser = new Parser();
using var tree = parser.ParseString(code);

// 创建查询
var query = tree.language().query_new(@"
    (export_statement 
        declaration: (function_declaration 
            name: (identifier) @func_name))
", out var offset, out var error);

if (query != null && error == TSQueryError.TSQueryErrorNone)
{
    using var cursor = new TSQueryCursor();
    cursor.exec(query, tree.root_node());
    
    Console.WriteLine("导出的函数:");
    while (cursor.next_match(out var match, out var captures))
    {
        foreach (var capture in captures)
        {
            var funcName = capture.node.text(code);
            Console.WriteLine($"  - {funcName}");
        }
    }
    
    query.Dispose();
}

// 输出:
// 导出的函数:
//   - add
//   - multiply
```

### 遍历所有节点
```csharp
using var parser = new Parser();
using var tree = parser.ParseString("const x = 1; let y = 'hello';");
using var cursor = new TSCursor(tree.root_node(), tree.language());

void TraverseTree()
{
    var node = cursor.current_node();
    Console.WriteLine($"{node.type()}: {node.text(code)}");
    
    if (cursor.goto_first_child())
    {
        do
        {
            TraverseTree();
        } while (cursor.goto_next_sibling());
        cursor.goto_parent();
    }
}

TraverseTree();
```

## 💡 最佳实践

### ✅ 推荐做法
```csharp
// 使用 using 语句管理资源
using var parser = new Parser();
using var tree = parser.ParseString(code);

// 检查节点是否为空
var nameNode = node.child_by_field_name("name");
if (!nameNode.is_null())
{
    Console.WriteLine(nameNode.text(code));
}

// 检查解析错误
if (tree.root_node().has_error())
{
    Console.WriteLine("代码有语法错误");
}
```

### ❌ 避免做法
- 忘记释放 `IDisposable` 对象
- 直接访问可能为空的节点
- 在循环中重复创建相同的查询

---

更多详细信息请参考 [API参考文档](API_REFERENCE.md)