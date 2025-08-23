# TypeScript Parser

基于Tree-sitter的TypeScript解析器 - .NET绑定

## 构建步骤

```bash
# 1. 初始化子模块
git submodule update --init --recursive

# 2. 构建Native库
(cd tree-sitter && make clean && make all)

# 3. 恢复依赖和构建
dotnet restore
dotnet build -c Release

# 4. 运行测试
dotnet test --configuration Release --no-build

# 5. 打包NuGet包
dotnet pack -c Release --no-build -o ./artifacts
```

## 🔄 自动化发布

- **推送main分支** → 自动构建和测试
- **创建tag** → 自动发布到NuGet

```bash
git tag v1.2.0
git push origin v1.2.0
```

## 项目结构

- `TypeScriptParser/` - 主要的.NET绑定库
- `TypeScriptParser.Native/` - 跨平台Native库包
- `TypeScriptParser.Tests/` - 单元测试

## 支持平台

- Linux x64
- macOS ARM64  
- Windows x64
