<#
    install.ps1  -  Cài add-in "TUAN - Dimension Below" cho Revit 2026
    ------------------------------------------------------------------
    Copy 2 file  TuanDimensionBelow.addin  +  TuanDimensionBelow.dll
    vào:  %AppData%\Autodesk\Revit\Addins\2026\

    Cách chạy (chuột phải > Run with PowerShell, hoặc):
        powershell -ExecutionPolicy Bypass -File .\install.ps1
    Gỡ cài đặt:
        powershell -ExecutionPolicy Bypass -File .\install.ps1 -Uninstall
#>

param(
    [switch]$Uninstall
)

$ErrorActionPreference = 'Stop'

$RevitVersion = '2026'
$AddinName    = 'TuanDimensionBelow'

# Thư mục đích chuẩn của Revit add-in (per-user)
$TargetDir = Join-Path $env:APPDATA "Autodesk\Revit\Addins\$RevitVersion"

$AddinFile = "$AddinName.addin"
$DllFile   = "$AddinName.dll"

# --- GỠ CÀI ĐẶT ---------------------------------------------------------
if ($Uninstall) {
    $removed = 0
    foreach ($f in @($AddinFile, $DllFile)) {
        $p = Join-Path $TargetDir $f
        if (Test-Path $p) { Remove-Item $p -Force; $removed++; Write-Host "Đã xóa: $p" }
    }
    if ($removed -eq 0) { Write-Host "Không tìm thấy file nào để gỡ." }
    else                { Write-Host "Đã gỡ cài đặt xong." -ForegroundColor Green }
    return
}

# --- CÀI ĐẶT ------------------------------------------------------------
# Tìm 2 file nguồn: ưu tiên thư mục 'dist' cạnh script, rồi tới cạnh script.
$searchDirs = @(
    (Join-Path $PSScriptRoot 'dist'),
    $PSScriptRoot
)

$srcAddin = $null
$srcDll   = $null
foreach ($d in $searchDirs) {
    if (-not $srcAddin -and (Test-Path (Join-Path $d $AddinFile))) { $srcAddin = Join-Path $d $AddinFile }
    if (-not $srcDll   -and (Test-Path (Join-Path $d $DllFile)))   { $srcDll   = Join-Path $d $DllFile }
}

if (-not $srcAddin -or -not $srcDll) {
    Write-Host "LỖI: Không tìm thấy $AddinFile và/hoặc $DllFile cạnh script (hoặc trong .\dist)." -ForegroundColor Red
    exit 1
}

# Tạo thư mục đích nếu chưa có
if (-not (Test-Path $TargetDir)) {
    New-Item -ItemType Directory -Path $TargetDir -Force | Out-Null
    Write-Host "Đã tạo thư mục: $TargetDir"
}

Copy-Item $srcAddin (Join-Path $TargetDir $AddinFile) -Force
Copy-Item $srcDll   (Join-Path $TargetDir $DllFile)   -Force

Write-Host "Đã cài đặt add-in vào:" -ForegroundColor Green
Write-Host "  $TargetDir\$AddinFile"
Write-Host "  $TargetDir\$DllFile"
Write-Host ""
Write-Host "Khởi động lại Revit 2026 -> tab 'TUAN' -> button 'Set Below Text'." -ForegroundColor Cyan
