# TypeScript Parser

基于Tree-sitter的TypeScript解析器 - .NET绑定

## 安装

```bash
dotnet add package TypeScriptParser
```

## 文档

- [使用指南](docs/USAGE_GUIDE.md) - 快速开始和常用示例
- [API参考](docs/API_REFERENCE.md) - 完整的API文档

## 构建项目

```bash
# 1. 构建native库
(cd tree-sitter/ && make clean && make)

# 2. 复制运行时文件
RID=$(dotnet --info | grep "RID:" | awk '{print $2}')
mkdir -p TypeScriptParser.Native/runtimes/$RID/native
cp -r tree-sitter/dist/* TypeScriptParser.Native/runtimes/$RID/native/

# 3. 构建和测试
dotnet restore
dotnet build -c Release
dotnet test --configuration Release 

# 4. 打包NuGet包
dotnet pack -c Release  -o ./artifacts

# 5. 测试打包
dotnet package add  TypeScriptParser --project TypeScriptParser.TestPackage/ --version 0.0.1-dev
dotnet test TypeScriptParser.TestPackage
```

## 项目结构

- [`TypeScriptParser/`](TypeScriptParser/) - 主要的.NET绑定库
- [`TypeScriptParser.Native/`](TypeScriptParser.Native/) - 跨平台Native库包
- [`TypeScriptParser.Tests/`](TypeScriptParser.Tests/) - 单元测试

## 支持平台
- Linux x64
- macOS ARM64  
- Windows x64

## CI/CD流程

### 开发流程
1. 创建功能分支
2. 提交Pull Request → 自动构建测试
3. 合并到main分支

### 发布流程
1. 创建版本标签：
```bash
(VERSION=v0.0.1 && git tag $VERSION && git push origin $VERSION)
```
2. 自动构建测试发布到NuGet.org
3. 版本号格式：`1.2.0.{构建号}`
