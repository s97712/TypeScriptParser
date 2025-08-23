# TypeScript Parser

基于Tree-sitter的TypeScript解析器 - .NET绑定

## 构建步骤

```bash
# 1. 复制运行时文件
(cd tree-sitter/ && make clean && make)

RID=$(dotnet --info | grep "RID:" | awk '{print $2}')
mkdir -p TypeScriptParser.Native/runtimes/$RID/native
cp -r tree-sitter/dist/* TypeScriptParser.Native/runtimes/$RID/native/

# 2. 恢复依赖
dotnet restore

# 3. 构建项目
dotnet build -c Release

# 4. 运行测试
dotnet test --configuration Release --no-build

# 5. 打包NuGet包
dotnet pack -c Release --no-build -o ./artifacts
```

## 自动化构建

### 开发流程
1. 创建功能分支
2. 提交Pull Request → 自动构建测试
3. 合并到main分支

### 发布流程
1. 创建版本标签：`git tag v1.2.0 && git push origin v1.2.0`
2. 自动构建测试发布到NuGet.org
3. 版本号格式：`1.2.0.{构建号}`

## 手动构建

```bash
# 1. 构建native库
(cd tree-sitter/ && make clean && make)

# 2. 复制运行时文件
RID=$(dotnet --info | grep "RID:" | awk '{print $2}')
mkdir -p TypeScriptParser.Native/runtimes/$RID/native
cp -r tree-sitter/dist/* TypeScriptParser.Native/runtimes/$RID/native/

# 3. .NET构建
dotnet restore
dotnet build -c Release
dotnet test --configuration Release --no-build
dotnet pack -c Release --no-build -o ./artifacts
```

## 开发构建

```bash
# Debug模式构建和测试
dotnet build -c Debug
dotnet test --configuration Debug --no-build
```

## 项目结构

- `TypeScriptParser/` - 主要的.NET绑定库
- `TypeScriptParser.Native/` - 跨平台Native库包
- `TypeScriptParser.Tests/` - 单元测试

## 支持平台

- Linux x64
- macOS ARM64  
- Windows x64
