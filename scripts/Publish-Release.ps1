# ============================================================================
#  Публикация MedClinic: сборка Release, подготовка папки и ZIP-архива.
#
#  Как запустить (из папки проекта):
#      powershell -ExecutionPolicy Bypass -File scripts\Publish-Release.ps1
#
#  Результат:
#      dist\MedClinic-1.0.0\        — готовая к работе папка
#      dist\MedClinic-1.0.0.zip     — архив для передачи пользователю
#
#  База данных и журналы в пакет НЕ входят: при первом запуске программа
#  создаст их сама и заполнит справочники начальными данными.
# ============================================================================

param(
    [string]$Version = "1.0.0",
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

# Корень проекта — папка на уровень выше scripts
$root = Split-Path -Parent $PSScriptRoot
$solution = Join-Path $root "MedClinic.sln"
$projectDir = Join-Path $root "MedClinic"
$binDir = Join-Path $projectDir "bin\$Configuration"

$distDir = Join-Path $root "dist"
$packageName = "MedClinic-$Version"
$packageDir = Join-Path $distDir $packageName
$zipPath = Join-Path $distDir "$packageName.zip"

Write-Host "== Сборка $Configuration ==" -ForegroundColor Cyan

# Ищем MSBuild: сначала в стандартном месте Visual Studio, потом через vswhere
$msbuild = $null
$candidates = @(
    "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
)

foreach ($candidate in $candidates)
{
    if (Test-Path $candidate) { $msbuild = $candidate; break }
}

if ($null -eq $msbuild)
{
    $vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
    if (Test-Path $vswhere)
    {
        $found = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild `
                            -find "MSBuild\**\Bin\MSBuild.exe" | Select-Object -First 1
        if ($found) { $msbuild = $found }
    }
}

if ($null -eq $msbuild)
{
    throw "Не найден MSBuild. Установите Visual Studio или укажите путь к MSBuild.exe вручную."
}

& $msbuild $solution /t:Build /p:Configuration=$Configuration /v:m /nologo /clp:Summary
if ($LASTEXITCODE -ne 0) { throw "Сборка завершилась с ошибкой." }

if (-not (Test-Path (Join-Path $binDir "MedClinic.exe")))
{
    throw "Не найден файл MedClinic.exe в $binDir"
}

Write-Host "== Подготовка папки $packageDir ==" -ForegroundColor Cyan

if (Test-Path $packageDir) { Remove-Item $packageDir -Recurse -Force }
New-Item -ItemType Directory -Path $packageDir | Out-Null

# Копируем всё, кроме рабочих данных, журналов и отладочных файлов
$excludedFolders = @("Data", "Logs")
$excludedExtensions = @(".pdb", ".xml")

Get-ChildItem $binDir | Where-Object {
    $excludedFolders -notcontains $_.Name -and $excludedExtensions -notcontains $_.Extension
} | ForEach-Object {
    Copy-Item $_.FullName -Destination $packageDir -Recurse -Force
}

# Документация рядом с программой
$docsTarget = Join-Path $packageDir "docs"
New-Item -ItemType Directory -Path $docsTarget | Out-Null

$docsSource = Join-Path $root "docs"
if (Test-Path $docsSource)
{
    Copy-Item (Join-Path $docsSource "*") -Destination $docsTarget -Recurse -Force
}

$readme = Join-Path $root "README.md"
if (Test-Path $readme)
{
    Copy-Item $readme -Destination $packageDir -Force
}

# Краткая памятка для получателя пакета
$notice = @"
МЕДИЦИНСКАЯ КЛИНИКА — КАК ЗАПУСТИТЬ

1. Распакуйте архив в удобную папку (например, C:\MedClinic).
2. Запустите MedClinic.exe.
3. Войдите под учётной записью администратора:
       логин:  admin
       пароль: admin123
4. Сразу смените пароль администратора в разделе «Пользователи».

ПЕРВЫЙ ЗАПУСК
Программа сама создаст папки Data (база данных) и Logs (журнал событий)
и заполнит справочники примерами. Интернет нужен только для загрузки
курсов валют в отчётах.

РЕЗЕРВНАЯ КОПИЯ
Перед важными изменениями: меню «Файл» → «Резервная копия базы…» (Ctrl+B).

СПРАВКА
Клавиша F1 открывает краткую инструкцию и горячие клавиши.
Полное руководство лежит в папке docs.

СИСТЕМНЫЕ ТРЕБОВАНИЯ
Windows 7 или новее, .NET Framework 4.7.2.
"@
Set-Content -Path (Join-Path $packageDir "Прочитайте меня.txt") -Value $notice -Encoding UTF8

Write-Host "== Создание архива $zipPath ==" -ForegroundColor Cyan

if (Test-Path $zipPath) { Remove-Item $zipPath -Force }
Compress-Archive -Path (Join-Path $packageDir "*") -DestinationPath $zipPath -Force

$sizeMb = [math]::Round((Get-Item $zipPath).Length / 1MB, 2)
Write-Host ""
Write-Host "Готово!" -ForegroundColor Green
Write-Host "  Папка: $packageDir"
Write-Host "  Архив: $zipPath ($sizeMb МБ)"
