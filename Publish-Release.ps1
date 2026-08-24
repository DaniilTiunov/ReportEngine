# Publish-Release.ps1
# Скрипт для публикации релизной сборки ReportEngine на сервер

param(
    [Parameter(Mandatory=$true)]
    [string]$Version,              # Например: 1.0.3
    
    [Parameter(Mandatory=$false)]
    [string]$Changelog = "Очередное обновление",
    
    [Parameter(Mandatory=$false)]
    [bool]$IsStable = $false
)

# ===== КОНФИГУРАЦИЯ =====
$BuildPath = "C:\Work\Prjs\ReportEngine\ReportEngine.App\bin\Release\net8.0-windows"
$ServerRoot = "P:\00 ОКП АСУ\01 Группа разработки ПО\Тиунов\releases"

# ===== ЦВЕТНОЙ ВЫВОД (ИСПРАВЛЕННАЯ ВЕРСИЯ) =====
function Write-ColorOutput {
    param(
        [string]$Color,
        [string]$Message
    )
    
    # Сохраняем текущий цвет
    $currentColor = $host.UI.RawUI.ForegroundColor
    
    try {
        # Преобразуем строку в ConsoleColor
        $consoleColor = [System.ConsoleColor]::$Color
        $host.UI.RawUI.ForegroundColor = $consoleColor
        Write-Output $Message
    }
    catch {
        # Если цвет не распознан, пишем белым
        Write-Output $Message
    }
    finally {
        # Восстанавливаем цвет
        $host.UI.RawUI.ForegroundColor = $currentColor
    }
}

# ===== ПРОВЕРКИ =====
Write-ColorOutput -Color "Green" -Message "========================================"
Write-ColorOutput -Color "Green" -Message "🚀 Публикация ReportEngine v$Version"
Write-ColorOutput -Color "Green" -Message "========================================"
Write-ColorOutput -Color "Cyan" -Message "`n📂 Локальная сборка: $BuildPath"
Write-ColorOutput -Color "Cyan" -Message "📂 Сервер: $ServerRoot"

# 1. Проверяем локальную сборку
if (-not (Test-Path $BuildPath)) {
    Write-ColorOutput -Color "Red" -Message "`n❌ Папка со сборкой не найдена!"
    Write-ColorOutput -Color "Red" -Message "   Путь: $BuildPath"
    Write-ColorOutput -Color "Yellow" -Message "`n💡 Соберите проект в Release:"
    Write-ColorOutput -Color "Yellow" -Message "   dotnet build -c Release"
    Write-ColorOutput -Color "Yellow" -Message "   или через Visual Studio: Сборка -> Собрать решение (Release)"
    exit 1
}

# 2. Проверяем наличие EXE файла
$exeFiles = Get-ChildItem -Path $BuildPath -Filter "*.exe" -File
if ($exeFiles.Count -eq 0) {
    Write-ColorOutput -Color "Red" -Message "`n❌ В папке нет .exe файлов!"
    exit 1
}

$mainExe = $exeFiles[0].Name
$exeSize = [math]::Round($exeFiles[0].Length / 1MB, 2)
Write-ColorOutput -Color "Green" -Message "`n✅ Найден основной файл: $mainExe ($exeSize MB)"

# 3. Проверяем доступность сервера
if (-not (Test-Path $ServerRoot)) {
    Write-ColorOutput -Color "Red" -Message "`n❌ Сервер недоступен!"
    Write-ColorOutput -Color "Red" -Message "   Путь: $ServerRoot"
    Write-ColorOutput -Color "Yellow" -Message "`n💡 Проверьте:"
    Write-ColorOutput -Color "Yellow" -Message "   - Подключен ли диск P:"
    Write-ColorOutput -Color "Yellow" -Message "   - Есть ли доступ к сети"
    exit 1
}
Write-ColorOutput -Color "Green" -Message "✅ Сервер доступен"

# ===== СОЗДАЁМ ПАПКУ ВЕРСИИ =====
$releaseFolder = "$Version"
$releasePath = Join-Path $ServerRoot $releaseFolder

if (Test-Path $releasePath) {
    Write-ColorOutput -Color "Yellow" -Message "`n⚠️  Папка $releaseFolder уже существует на сервере"
    $answer = Read-Host "❓ Перезаписать? (y/n)"
    if ($answer -ne 'y') {
        Write-ColorOutput -Color "Red" -Message "❌ Публикация отменена"
        exit 0
    }
    Write-ColorOutput -Color "Yellow" -Message "🗑️  Удаляем старую версию..."
    Remove-Item -Path $releasePath -Recurse -Force
}

# Создаём новую папку
New-Item -ItemType Directory -Path $releasePath -Force | Out-Null
Write-ColorOutput -Color "Green" -Message "`n✅ Создана папка на сервере: $releaseFolder"

# ===== КОПИРУЕМ ФАЙЛЫ =====
Write-ColorOutput -Color "Cyan" -Message "`n📁 Копирование файлов..."

# Получаем список всех файлов
$files = Get-ChildItem -Path $BuildPath -File -Recurse
$totalFiles = $files.Count
$copied = 0

foreach ($file in $files) {
    $relativePath = $file.FullName.Substring($BuildPath.Length + 1)
    $destFile = Join-Path $releasePath $relativePath
    $destDir = Split-Path $destFile -Parent
    
    # Создаём папку назначения
    if (-not (Test-Path $destDir)) {
        New-Item -ItemType Directory -Path $destDir -Force | Out-Null
    }
    
    # Копируем файл
    Copy-Item -Path $file.FullName -Destination $destFile -Force
    
    $copied++
    $percent = [math]::Round(($copied / $totalFiles) * 100)
    Write-Progress -Activity "Копирование файлов" -Status "$percent% завершено" -PercentComplete $percent
}

Write-Progress -Activity "Копирование файлов" -Completed
Write-ColorOutput -Color "Green" -Message "✅ Скопировано $copied файлов"

# ===== СОЗДАЁМ version.json =====
$versionJson = @{
    Version = $Version
    ReleaseDate = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss")
    IsStable = $IsStable
    Changelog = $Changelog
    MainExe = $mainExe
    TotalFiles = $totalFiles
} | ConvertTo-Json

$versionJsonPath = Join-Path $releasePath "version.json"
$versionJson | Out-File -FilePath $versionJsonPath -Encoding UTF8
Write-ColorOutput -Color "Green" -Message "✅ Создан version.json"

# ===== ОБНОВЛЯЕМ МАНИФЕСТ =====
$manifestPath = Join-Path $ServerRoot "releases.xml"

# Загружаем или создаём манифест
if (Test-Path $manifestPath) {
    [xml]$manifest = Get-Content $manifestPath -Encoding UTF8
    Write-ColorOutput -Color "Green" -Message "✅ Загружен существующий манифест"
} else {
    $manifest = [xml]@'
<?xml version="1.0" encoding="utf-8"?>
<Releases>
</Releases>
'@
    Write-ColorOutput -Color "Yellow" -Message "⚠️  Манифест не найден, создан новый"
}

# Создаём узел новой версии
$releaseNode = $manifest.CreateElement("Release")

# Добавляем все элементы
$versionNode = $manifest.CreateElement("Version")
$versionNode.InnerText = $Version
$releaseNode.AppendChild($versionNode) | Out-Null

$folderNode = $manifest.CreateElement("Folder")
$folderNode.InnerText = $releaseFolder
$releaseNode.AppendChild($folderNode) | Out-Null

$dateNode = $manifest.CreateElement("Date")
$dateNode.InnerText = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss")
$releaseNode.AppendChild($dateNode) | Out-Null

$stableNode = $manifest.CreateElement("IsStable")
$stableNode.InnerText = $IsStable.ToString().ToLower()
$releaseNode.AppendChild($stableNode) | Out-Null

$changelogNode = $manifest.CreateElement("Changelog")
$changelogNode.InnerText = $Changelog
$releaseNode.AppendChild($changelogNode) | Out-Null

# Добавляем в начало (новые версии сверху)
if ($manifest.Releases.Release) {
    $manifest.Releases.InsertBefore($releaseNode, $manifest.Releases.ChildNodes[0]) | Out-Null
} else {
    $manifest.Releases.AppendChild($releaseNode) | Out-Null
}

# Сохраняем
$manifest.Save($manifestPath)
Write-ColorOutput -Color "Green" -Message "✅ Обновлён releases.xml"

# ===== ИТОГОВАЯ ИНФОРМАЦИЯ =====
Write-ColorOutput -Color "Green" -Message "`n========================================"
Write-ColorOutput -Color "Green" -Message "✅ Публикация завершена успешно!"
Write-ColorOutput -Color "Green" -Message "========================================"
Write-ColorOutput -Color "Cyan" -Message "`n📌 Информация о релизе:"
Write-ColorOutput -Color "Cyan" -Message "   Версия:     $Version"
Write-ColorOutput -Color "Cyan" -Message "   Папка:      $releaseFolder"
Write-ColorOutput -Color "Cyan" -Message "   Файлов:     $totalFiles"
Write-ColorOutput -Color "Cyan" -Message "   Основной:   $mainExe"
Write-ColorOutput -Color "Cyan" -Message "   Changelog:  $Changelog"
Write-ColorOutput -Color "Cyan" -Message "   Stable:     $IsStable"
Write-ColorOutput -Color "Cyan" -Message "`n📍 Путь на сервере:"
Write-ColorOutput -Color "Cyan" -Message "   $releasePath"
Write-ColorOutput -Color "Cyan" -Message "`n📋 Манифест:"
Write-ColorOutput -Color "Cyan" -Message "   $manifestPath"

Write-ColorOutput -Color "Yellow" -Message "`n💡 Пользователи увидят новую версию при следующей проверке обновлений"

# ===== ОПЦИОНАЛЬНО: ОТКРЫТЬ ПАПКУ =====
Write-ColorOutput -Color "White" -Message ""
$showFolder = Read-Host "❓ Открыть папку с релизом на сервере? (y/n)"
if ($showFolder -eq 'y') {
    explorer $releasePath
}

Write-ColorOutput -Color "Green" -Message "`n🎉 Готово!"