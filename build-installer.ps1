# Build and Package Script for Tic Tac Fusion PC
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "  Tic Tac Fusion PC - Build & Packaging      " -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan

# Step 0: Read version dynamically from TicTacFusion.csproj
$csprojContent = [xml](Get-Content -Path "TicTacFusion/TicTacFusion.csproj")
$version = $csprojContent.Project.PropertyGroup.Version
if (-not $version) { $version = "1.0.0" }
Write-Host "Target Release Version: v$version" -ForegroundColor Green

# Step 1: Publish standalone self-contained release
Write-Host "`n[1/3] Publishing self-contained win-x64 executable..." -ForegroundColor Yellow
dotnet publish TicTacFusion/TicTacFusion.csproj -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:PublishReadyToRun=true -o ./publish

if ($LASTEXITCODE -ne 0) {
    Write-Host "`n[ERROR] Dotnet publish failed." -ForegroundColor Red
    exit 1
}

# Step 2: Create portable ZIP distribution
$zipFileName = "TicTacFusion-v$version-Standalone-win-x64.zip"
Write-Host "`n[2/3] Creating portable ZIP archive: $zipFileName..." -ForegroundColor Yellow
if (!(Test-Path -Path "./dist")) {
    New-Item -ItemType Directory -Path "./dist" | Out-Null
}

Compress-Archive -Path ./publish/* -DestinationPath "./dist/$zipFileName" -Force
Write-Host "Portable package created at: ./dist/$zipFileName" -ForegroundColor Green

# Step 3: Check for Inno Setup compiler to create setup installer
Write-Host "`n[3/3] Checking for Inno Setup compiler (ISCC.exe)..." -ForegroundColor Yellow
$isccPath = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
if (!(Test-Path $isccPath)) {
    $cmd = Get-Command "ISCC.exe" -ErrorAction SilentlyContinue
    if ($cmd) {
        $isccPath = $cmd.Source
    }
}

if ($isccPath -and (Test-Path $isccPath)) {
    Write-Host "Compiling setup installer with Inno Setup..." -ForegroundColor Cyan
    & $isccPath installer.iss
    Write-Host "`n[SUCCESS] Windows Installer created in ./installer-output/" -ForegroundColor Green
} else {
    Write-Host "`n[NOTE] Inno Setup compiler not found in standard path. You can open 'installer.iss' with Inno Setup GUI to build the installer EXE, or distribute the standalone zip in './dist/'." -ForegroundColor Yellow
}

Write-Host "`n=============================================" -ForegroundColor Green
Write-Host "  Build & Packaging Complete!                " -ForegroundColor Green
Write-Host "=============================================" -ForegroundColor Green
