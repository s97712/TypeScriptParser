# TypeScriptParser

基于Tree-sitter的TypeScript解析器.NET绑定库，提供高性能的TypeScript代码解析功能。

## 安装

```bash
dotnet add package TypeScriptParser
```

## 快速开始

```csharp
using System;
using TreeSitter.TypeScript;
using GitHub.TreeSitter;

// 创建解析器实例
using var parser = new TypeScriptParser();

// 解析TypeScript代码
string code = @"
function greet(name: string): string {
    return `Hello, ${name}!`;
}
";

using var tree = parser.ParseString(code);
var rootNode = tree.root_node();

// 遍历语法树
Console.WriteLine($"语法树类型: {rootNode.type()}");
Console.WriteLine($"子节点数量: {rootNode.child_count()}");

// 获取源码文本
Console.WriteLine($"源码内容: {rootNode.text(code)}");
```

## 主要功能

- ✅ 完整的TypeScript语法支持
- ✅ 高性能的增量解析
- ✅ 跨平台支持 (Windows, Linux, macOS)
- ✅ 详细的语法错误信息
- ✅ 语法树遍历和查询

## API 参考

### TypeScriptParser 类

位于 `TreeSitter.TypeScript` 命名空间。

#### 构造函数
```csharp
var parser = new TypeScriptParser();
```

#### 主要方法

**ParseString(string sourceCode)**
- 解析TypeScript源代码字符串
- 返回: `TSTree` - 解析后的语法树

```csharp
var tree = parser.ParseString("const x = 42;");
```

**CreateCursor(TSTree tree)**
- 创建语法树游标用于高效遍历
- 返回: `TSCursor` - 语法树游标对象

```csharp
using var cursor = parser.CreateCursor(tree);
```

#### 属性

**Language**
- 获取TypeScript语言对象
- 类型: `TSLanguage`

**IsAvailable**
- 检查解析器是否可用
- 类型: `bool`

### TSTree 类 (来自 GitHub.TreeSitter)

**主要方法:**
- `root_node()` - 获取根节点
- `copy()` - 复制语法树
- `edit(TSInputEdit edit)` - 编辑语法树

### TSNode 结构 (来自 GitHub.TreeSitter)

**主要方法:**
- `type()` - 获取节点类型名称
- `child_count()` - 获取子节点数量
- `child(uint index)` - 获取指定索引的子节点
- `start_point()` / `end_point()` - 获取节点位置
- `text(string sourceCode)` - 获取节点对应的源码文本

## 语法树遍历示例

```csharp
using var parser = new TypeScriptParser();
var tree = parser.ParseString(@"
class Calculator {
    add(a: number, b: number): number {
        return a + b;
    }
}
");

var root = tree.root_node();

// 递归遍历所有节点
void TraverseNode(TSNode node, string source, int depth = 0)
{
    var indent = new string(' ', depth * 2);
    Console.WriteLine($"{indent}{node.type()}: {node.text(source)}");
    
    for (uint i = 0; i < node.child_count(); i++)
    {
        TraverseNode(node.child(i), source, depth + 1);
    }
}

TraverseNode(root, code);
```

## 错误处理

```csharp
using var parser = new TypeScriptParser();
var tree = parser.ParseString("const x = ;"); // 语法错误

var root = tree.root_node();
if (root.has_error())
{
    Console.WriteLine("语法树包含错误");
}
```

## 系统要求

- .NET 9.0 或更高版本
- 支持的平台：Windows (x64), Linux (x64), macOS (ARM64/x64)

## 相关包

- `TypeScriptParser.Native` - 包含跨平台原生库的运行时包

## 许可证

本项目基于MIT许可证开源。