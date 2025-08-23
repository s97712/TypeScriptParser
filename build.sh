#!/bin/bash

# TypeScriptParser NuGet包构建脚本
# 使用方法: ./build.sh [--config Debug] [--skip-tests] [--package-only] [--clean]

set -e  # 出错时停止

# 默认参数
CONFIGURATION="Release"
SKIP_TESTS=false
PACKAGE_ONLY=false
CLEAN=false
OUTPUT_PATH="./artifacts"

# 解析命令行参数
while [[ $# -gt 0 ]]; do
    case $1 in
        --config)
            CONFIGURATION="$2"
            shift 2
            ;;
        --skip-tests)
            SKIP_TESTS=true
            shift
            ;;
        --package-only)
            PACKAGE_ONLY=true
            shift
            ;;
        --clean)
            CLEAN=true
            shift
            ;;
        --output)
            OUTPUT_PATH="$2"
            shift 2
            ;;
        -h|--help)
            echo "使用方法: $0 [选项]"
            echo "选项:"
            echo "  --config CONFIG    构建配置 (Debug/Release，默认Release)"
            echo "  --skip-tests       跳过测试运行"
            echo "  --package-only     只打包，不构建"
            echo "  --clean            清理构建输出"
            echo "  --output PATH      输出路径 (默认 ./artifacts)"
            echo "  -h, --help         显示帮助信息"
            exit 0
            ;;
        *)
            echo "未知参数: $1"
            echo "使用 $0 --help 查看帮助"
            exit 1
            ;;
    esac
done

# 颜色定义
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
GRAY='\033[0;37m'
NC='\033[0m' # No Color

echo -e "${CYAN}🚀 TypeScriptParser NuGet包构建脚本${NC}"
echo -e "${YELLOW}配置: $CONFIGURATION${NC}"

# 清理输出目录
if [ "$CLEAN" = true ]; then
    echo -e "${YELLOW}🧹 清理构建输出...${NC}"
    if [ -d "$OUTPUT_PATH" ]; then
        rm -rf "$OUTPUT_PATH"
    fi
    dotnet clean --verbosity minimal
fi

# 创建输出目录
mkdir -p "$OUTPUT_PATH"

# 恢复依赖包
if [ "$PACKAGE_ONLY" != true ]; then
    echo -e "${YELLOW}📦 恢复NuGet包...${NC}"
    dotnet restore --verbosity minimal
fi

# 构建项目
if [ "$PACKAGE_ONLY" != true ]; then
    echo -e "${YELLOW}🔨 构建项目...${NC}"
    
    # 构建Native包
    echo -e "${GRAY}  构建 TypeScriptParser.Native...${NC}"
    dotnet build TypeScriptParser.Native/TypeScriptParser.Native.csproj -c "$CONFIGURATION" --no-restore --verbosity minimal
    
    # 构建主项目
    echo -e "${GRAY}  构建 TypeScriptParser...${NC}"
    dotnet build TypeScriptParser/TypeScriptParser.csproj -c "$CONFIGURATION" --no-restore --verbosity minimal
fi

# 运行测试
if [ "$SKIP_TESTS" != true ] && [ -d "TypeScriptParser.Tests" ]; then
    echo -e "${YELLOW}🧪 运行测试...${NC}"
    dotnet test TypeScriptParser.Tests --configuration "$CONFIGURATION" --no-build --verbosity minimal --logger trx --results-directory "$OUTPUT_PATH/TestResults"
fi

# 打包NuGet包
echo -e "${YELLOW}📦 打包NuGet包...${NC}"

# 打包Native库
echo -e "${GRAY}  打包 TypeScriptParser.Native...${NC}"
dotnet pack TypeScriptParser.Native/TypeScriptParser.Native.csproj -c "$CONFIGURATION" --no-build -o "$OUTPUT_PATH" --verbosity minimal

# 打包主项目
echo -e "${GRAY}  打包 TypeScriptParser...${NC}"
dotnet pack TypeScriptParser/TypeScriptParser.csproj -c "$CONFIGURATION" --no-build -o "$OUTPUT_PATH" --verbosity minimal

# 显示结果
echo -e "${GREEN}✅ 构建完成!${NC}"
echo -e "${CYAN}生成的包:${NC}"

for file in "$OUTPUT_PATH"/*.nupkg; do
    if [ -f "$file" ]; then
        size=$(du -h "$file" | cut -f1)
        basename=$(basename "$file")
        echo -e "  ${BLUE}📦 $basename ($size)${NC}"
    fi
done

echo ""
echo -e "${YELLOW}💡 下一步:${NC}"
echo -e "${GRAY}  验证包: dotnet nuget verify $OUTPUT_PATH/*.nupkg${NC}"
echo -e "${GRAY}  推送到NuGet.org: dotnet nuget push $OUTPUT_PATH/*.nupkg --source https://api.nuget.org/v3/index.json --api-key YOUR_API_KEY${NC}"