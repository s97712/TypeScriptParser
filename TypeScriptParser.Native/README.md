# TypeScriptParser.Native

This package contains the native libraries required for TypeScriptParser to function across different platforms.

## Supported Platforms

- Windows (x64, x86)
- Linux (x64) 
- macOS (x64, ARM64)

## Usage

This package is automatically referenced by the main TypeScriptParser package. You typically don't need to reference this package directly.

## Native Libraries Included

- `tree-sitter` - Core Tree-sitter parsing library
- `tree-sitter-typescript` - TypeScript language grammar for Tree-sitter

## Version Information

This package is built against:
- Tree-sitter core version: 0.20.8
- TypeScript grammar version: 0.20.3