#!/usr/bin/env pwsh

<#
.SYNOPSIS
TypeScriptParser NuGet包构建脚本

.DESCRIPTION  
自动化构建TypeScriptParser和TypeScriptParser.Native NuGet包

.PARAMETER Configuration
构建配置 (Debug/Release)，默认为Release

.PARAMETER SkipTests  
跳过测试运行

.PARAMETER PackageOnly
只打包，不构建

.EXAMPLE
./build.ps1
./build.ps1 -Configuration Debug
./build.ps1 -SkipTests
#>

param(
    [string]$Configuration = "Release",
    [switch]$SkipTests = $false,
    [switch]$PackageOnly = $false,
    [switch]$Clean = $false,
    [string]$OutputPath = "./artifacts"
)

# 错误时停止
$ErrorActionPreference = "Stop"

Write-Host "🚀 TypeScriptParser NuGet包构建脚本" -ForegroundColor Cyan
Write-Host "配置: $Configuration" -ForegroundColor Yellow

# 清理输出目录
if ($Clean) {
    Write-Host "🧹 清理构建输出..." -ForegroundColor Yellow
    if (Test-Path $OutputPath) {
        Remove-Item $OutputPath -Recurse -Force
    }
    dotnet clean --verbosity minimal
}

# 创建输出目录
New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null

# 恢复依赖包
if (-not $PackageOnly) {
    Write-Host "📦 恢复NuGet包..." -ForegroundColor Yellow
    dotnet restore --verbosity minimal
    if ($LASTEXITCODE -ne 0) {
        throw "包恢复失败"
    }
}

# 构建项目
if (-not $PackageOnly) {
    Write-Host "🔨 构建项目..." -ForegroundColor Yellow
    
    # 构建Native包
    Write-Host "  构建 TypeScriptParser.Native..." -ForegroundColor Gray
    dotnet build TypeScriptParser.Native/TypeScriptParser.Native.csproj -c $Configuration --no-restore --verbosity minimal
    if ($LASTEXITCODE -ne 0) {
        throw "TypeScriptParser.Native构建失败"
    }
    
    # 构建主项目
    Write-Host "  构建 TypeScriptParser..." -ForegroundColor Gray  
    dotnet build TypeScriptParser/TypeScriptParser.csproj -c $Configuration --no-restore --verbosity minimal
    if ($LASTEXITCODE -ne 0) {
        throw "TypeScriptParser构建失败"
    }
}

# 运行测试
if (-not $SkipTests -and (Test-Path "TypeScriptParser.Tests")) {
    Write-Host "🧪 运行测试..." -ForegroundColor Yellow
    dotnet test TypeScriptParser.Tests --configuration $Configuration --no-build --verbosity minimal --logger trx --results-directory "$OutputPath/TestResults"
    if ($LASTEXITCODE -ne 0) {
        throw "测试失败"
    }
}

# 打包NuGet包
Write-Host "📦 打包NuGet包..." -ForegroundColor Yellow

# 打包Native库
Write-Host "  打包 TypeScriptParser.Native..." -ForegroundColor Gray
dotnet pack TypeScriptParser.Native/TypeScriptParser.Native.csproj -c $Configuration --no-build -o $OutputPath --verbosity minimal
if ($LASTEXITCODE -ne 0) {
    throw "TypeScriptParser.Native打包失败"
}

# 打包主项目  
Write-Host "  打包 TypeScriptParser..." -ForegroundColor Gray
dotnet pack TypeScriptParser/TypeScriptParser.csproj -c $Configuration --no-build -o $OutputPath --verbosity minimal
if ($LASTEXITCODE -ne 0) {
    throw "TypeScriptParser打包失败"
}

# 显示结果
Write-Host "✅ 构建完成!" -ForegroundColor Green
Write-Host "生成的包:" -ForegroundColor Cyan
Get-ChildItem "$OutputPath/*.nupkg" | ForEach-Object {
    $size = [math]::Round($_.Length / 1KB, 1)
    Write-Host "  📦 $($_.Name) ($size KB)" -ForegroundColor White
}

Write-Host ""
Write-Host "💡 下一步:" -ForegroundColor Yellow
Write-Host "  验证包: dotnet nuget verify $OutputPath/*.nupkg" -ForegroundColor Gray
Write-Host "  推送到NuGet.org: dotnet nuget push $OutputPath/*.nupkg --source https://api.nuget.org/v3/index.json --api-key YOUR_API_KEY" -ForegroundColor Gray