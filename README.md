# TypeScriptParser

## Introduction

TypeScriptParser provides C# bindings for the [Tree-sitter](https://github.com/tree-sitter/tree-sitter) parsing library, specifically optimized for TypeScript language parsing. Through P/Invoke calls, C# developers can directly use the powerful Tree-sitter parsing functionality in the .NET environment.

## Project Structure

- **TypeScriptParser**: Main library project containing complete Tree-sitter C# bindings and TypeScript parser
- **TypeScriptParser.Tests**: xUnit test project ensuring parser correctness

## Features

- Complete Tree-sitter API C# bindings
- Dedicated TypeScript language support
- Syntax tree traversal and querying
- Memory-safe resource management
- Support for incremental parsing and syntax tree editing

## Requirements

- .NET 9.0 or higher
- Windows/Linux/macOS (cross-platform support)
- x64 architecture

## Setup

Pull the required submodules:

```bash
git submodule update --init --recursive
```

## Building

1. Build native dependency libraries:
   ```bash
   cd tree-sitter
   make  # Linux/macOS
   # or use nmake on Windows
   cd ..
   ```

2. Build the .NET project:
   ```bash
   dotnet build
   ```

## Usage

### Basic Usage Example

```csharp
using TreeSitter.TypeScript;

// Create parser instance
using var parser = new TypeScriptParser();

// Parse TypeScript code
string sourceCode = "const message: string = 'Hello, World!';";
using var tree = parser.ParseString(sourceCode);

// Get root node
var rootNode = tree.root_node();

// Create cursor for tree traversal
using var cursor = parser.CreateCursor(tree);
```

### Syntax Tree Traversal

```csharp
using var parser = new TypeScriptParser();
string code = @"
function greet(name: string): string {
    return `Hello, ${name}!`;
}
";

using var tree = parser.ParseString(code);
using var cursor = parser.CreateCursor(tree);

// Traverse syntax tree
while (cursor.goto_first_child())
{
    var node = cursor.current_node();
    Console.WriteLine($"Node: {node.type()}, Text: {node.text(code)}");
}
```

## API Documentation

### TypeScriptParser Class

- `ParseString(string sourceCode)`: Parse string and return syntax tree
- `CreateCursor(TSTree tree)`: Create syntax tree cursor
- `Language`: Get TypeScript language object
- `IsAvailable`: Check if parser is available

### Core Types

- `TSTree`: Syntax tree object
- `TSNode`: Syntax tree node
- `TSCursor`: Syntax tree cursor
- `TSLanguage`: Language definition
- `TSParser`: Underlying parser

## Testing

Run unit tests:

```bash
dotnet test
```

Tests cover core parser functionality:
- Basic parsing functionality
- Error handling
- Resource management
- Syntax tree traversal

## Performance Features

- Incremental parsing: Support for incremental updates to existing syntax trees
- Memory efficiency: Uses reference counting and automatic garbage collection
- High-speed parsing: Based on LR parsing algorithm
- Error recovery: Able to recover from syntax errors and continue parsing

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contributing

Issues and Pull Requests are welcome! Before contributing, please ensure:

1. All tests run and pass
2. Follow existing code style
3. Add appropriate test cases for new features

## Technology Stack

- **Language**: C# (.NET 9.0)
- **Test Framework**: xUnit
- **Native Interop**: P/Invoke
- **Package Management**: NuGet
- **Build System**: MSBuild

---

For more information, visit the [Tree-sitter official documentation](https://tree-sitter.github.io/tree-sitter/)
