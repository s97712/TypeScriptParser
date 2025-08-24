# TypeScript Parser 使用指南

基于Tree-sitter的TypeScript解析器.NET绑定库。

## 快速开始

### 安装
```bash
dotnet add package TypeScriptParser
```

### 基本使用
```csharp
using TypeScriptParser;

// 创建解析器
using var parser = new Parser();

// 解析TypeScript代码
using var tree = parser.ParseString(@"
    export function add(a: number, b: number): number {
        return a + b;
    }
");

// 获取根节点
var root = tree.root_node();
Console.WriteLine($"节点类型: {root.type()}");
```

## API概览

### Parser类 (高级API)
```csharp
using var parser = new Parser();

// 解析代码
var tree = parser.ParseString(code);

// 创建游标
var cursor = parser.CreateCursor(tree);

// 获取语言对象
var language = parser.Language;

// 检查状态
bool available = parser.IsAvailable;
```

### Tree-sitter核心类 (底层API)
```csharp
using TypeScriptParser.TreeSitter;

// 直接使用底层API
var parser = new TSParser();
var language = new TSLanguage(ptr);
var tree = parser.parse_string(null, code);
```

## 常用示例

### 遍历语法树
```csharp
using var parser = new Parser();
using var tree = parser.ParseString(code);
using var cursor = parser.CreateCursor(tree);

do {
    var node = cursor.current_node();
    Console.WriteLine($"{cursor.current_symbol()}: {node.text(code)}");
} while (cursor.goto_next_sibling());
```

### 查找特定节点
```csharp
using TypeScriptParser.TreeSitter;

// 使用Query查找函数
var query = language.query_new("(function_declaration name: (identifier) @name)", 
                               out var offset, out var error);
if (query != null) {
    using var queryCursor = new TSQueryCursor();
    queryCursor.exec(query, tree.root_node());
    // 处理匹配结果...
}
```

## 最佳实践

- 使用`using`语句管理资源
- 优先使用高级`Parser`类
- 大文件设置超时：`parser.set_timeout_micros(5000000)`
- 多线程使用`tree.copy()`创建副本

## 故障排除

**Native库加载失败**: 确保安装了`TypeScriptParser.Native`包并且平台支持(Linux x64/macOS ARM64/Windows x64)

**解析失败**: 检查TypeScript代码语法是否正确

**内存泄漏**: 确保所有`IDisposable`对象都使用`using`语句

---

更多详细信息请参考[NuGet包页面](https://www.nuget.org/packages/TypeScriptParser)