# Publish-Release.ps1
# Скрипт публикации ReportEngine.App + ReportEngine.Updater

param(
    [Parameter(Mandatory = $true)]
    [string]$Version,

    [Parameter(Mandatory = $false)]
    [string]$Changelog = "Очередное обновление",

    [Parameter(Mandatory = $false)]
    [bool]$IsStable = $false
)

# ============================================================
# КОНФИГУРАЦИЯ
# ============================================================

$BuildPath = "C:\Work\Prjs\ReportEngine\ReportEngine.App\bin\Release\net8.0-windows"

$UpdaterBuildPath = "C:\Work\Prjs\ReportEngine\ReportEngine.Updater\bin\Release\net8.0-windows"

$ServerRoot = "P:\00 ОКП АСУ\01 Группа разработки ПО\Тиунов\releases"

$LatestReleaseLocal = "C:\Work\Prjs\ReportEngine\latest.json"

$LatestReleaseServerDir = "P:\00 ОКП АСУ\01 Группа разработки ПО\Тиунов\releases"
$LatestReleaseServer = Join-Path $LatestReleaseServerDir "latest.json"

$UpdaterServerPath = Join-Path $ServerRoot "Updater"
$UpdateInfoServerPath = Join-Path $UpdaterServerPath "Config\updateInfo.json"


# ============================================================
# ЦВЕТНОЙ ВЫВОД
# ============================================================

function Write-ColorOutput {
    param(
        [string]$Color,
        [string]$Message
    )

    $currentColor = $host.UI.RawUI.ForegroundColor

    try {
        $consoleColor = [System.ConsoleColor]::$Color
        $host.UI.RawUI.ForegroundColor = $consoleColor

        Write-Output $Message
    }
    catch {
        Write-Output $Message
    }
    finally {
        $host.UI.RawUI.ForegroundColor = $currentColor
    }
}


# ============================================================
# КОПИРОВАНИЕ СБОРКИ
# ============================================================

function Copy-Build {
    param(
        [Parameter(Mandatory = $true)]
        [string]$SourcePath,

        [Parameter(Mandatory = $true)]
        [string]$DestinationPath
    )

    $files = Get-ChildItem `
        -Path $SourcePath `
        -File `
        -Recurse

    $totalFiles = $files.Count
    $copied = 0

    foreach ($file in $files) {

        $relativePath = $file.FullName.Substring(
                $SourcePath.Length + 1
        )

        $destinationFile = Join-Path `
            $DestinationPath `
            $relativePath

        $destinationDirectory = Split-Path `
            $destinationFile `
            -Parent

        if (-not (Test-Path $destinationDirectory)) {
            New-Item `
                -ItemType Directory `
                -Path $destinationDirectory `
                -Force |
                    Out-Null
        }

        Copy-Item `
            -Path $file.FullName `
            -Destination $destinationFile `
            -Force

        $copied++

        if ($totalFiles -gt 0) {

            $percent = [math]::Round(
                    ($copied / $totalFiles) * 100
            )

            Write-Progress `
                -Activity "Копирование файлов" `
                -Status "$percent% завершено" `
                -PercentComplete $percent
        }
    }

    Write-Progress `
        -Activity "Копирование файлов" `
        -Completed

    return $copied
}


# ============================================================
# НАЧАЛО
# ============================================================

Write-ColorOutput `
    -Color "Green" `
    -Message "========================================"

Write-ColorOutput `
    -Color "Green" `
    -Message "🚀 Публикация ReportEngine v$Version"

Write-ColorOutput `
    -Color "Green" `
    -Message "========================================"

Write-ColorOutput `
    -Color "Cyan" `
    -Message "`n📂 App:     $BuildPath"

Write-ColorOutput `
    -Color "Cyan" `
    -Message "📂 Updater: $UpdaterBuildPath"

Write-ColorOutput `
    -Color "Cyan" `
    -Message "📂 Сервер:  $ServerRoot"


# ============================================================
# ПРОВЕРКА APP
# ============================================================

if (-not (Test-Path $BuildPath)) {

    Write-ColorOutput `
        -Color "Red" `
        -Message "`n❌ Папка со сборкой App не найдена!"

    Write-ColorOutput `
        -Color "Red" `
        -Message "   $BuildPath"

    Write-ColorOutput `
        -Color "Yellow" `
        -Message "`n💡 Соберите ReportEngine.App в Release."

    exit 1
}

Write-ColorOutput `
    -Color "Green" `
    -Message "`n✅ Сборка App найдена"


# ============================================================
# ПРОВЕРКА UPDATER
# ============================================================

if (-not (Test-Path $UpdaterBuildPath)) {

    Write-ColorOutput `
        -Color "Red" `
        -Message "`n❌ Папка со сборкой Updater не найдена!"

    Write-ColorOutput `
        -Color "Red" `
        -Message "   $UpdaterBuildPath"

    Write-ColorOutput `
        -Color "Yellow" `
        -Message "`n💡 Соберите ReportEngine.Updater в Release."

    exit 1
}

Write-ColorOutput `
    -Color "Green" `
    -Message "✅ Сборка Updater найдена"


# ============================================================
# ПОИСК EXE APP
# ============================================================

$mainExe = Get-ChildItem `
    -Path $BuildPath `
    -Filter "*.exe" `
    -File |
        Select-Object -First 1

if ($null -eq $mainExe) {

    Write-ColorOutput `
        -Color "Red" `
        -Message "`n❌ В сборке App нет .exe!"

    exit 1
}

$mainExeSize = [math]::Round(
        $mainExe.Length / 1MB,
        2
)

Write-ColorOutput `
    -Color "Green" `
    -Message "✅ App: $($mainExe.Name) ($mainExeSize MB)"


# ============================================================
# ПОИСК EXE UPDATER
# ============================================================

$updaterExe = Get-ChildItem `
    -Path $UpdaterBuildPath `
    -Filter "*.exe" `
    -File |
        Select-Object -First 1

if ($null -eq $updaterExe) {

    Write-ColorOutput `
        -Color "Red" `
        -Message "`n❌ В сборке Updater нет .exe!"

    exit 1
}

$updaterExeSize = [math]::Round(
        $updaterExe.Length / 1MB,
        2
)

Write-ColorOutput `
    -Color "Green" `
    -Message "✅ Updater: $($updaterExe.Name) ($updaterExeSize MB)"


# ============================================================
# ПРОВЕРКА СЕРВЕРА
# ============================================================

if (-not (Test-Path $ServerRoot)) {

    Write-ColorOutput `
        -Color "Red" `
        -Message "`n❌ Сервер недоступен!"

    Write-ColorOutput `
        -Color "Red" `
        -Message "   $ServerRoot"

    Write-ColorOutput `
        -Color "Yellow" `
        -Message "`n💡 Проверьте:"

    Write-ColorOutput `
        -Color "Yellow" `
        -Message "   - Подключен ли диск P:"

    Write-ColorOutput `
        -Color "Yellow" `
        -Message "   - Есть ли доступ к сети"

    exit 1
}

Write-ColorOutput `
    -Color "Green" `
    -Message "✅ Сервер доступен"


# ============================================================
# СОЗДАНИЕ ПАПКИ РЕЛИЗА
# ============================================================

$releaseFolder = $Version

$releasePath = Join-Path `
    $ServerRoot `
    $releaseFolder


if (Test-Path $releasePath) {

    Write-ColorOutput `
        -Color "Yellow" `
        -Message "`n⚠️ Папка $releaseFolder уже существует, перезаписываю..."

    Remove-Item `
        -Path $releasePath `
        -Recurse `
        -Force
}


New-Item `
    -ItemType Directory `
    -Path $releasePath `
    -Force |
        Out-Null

Write-ColorOutput `
    -Color "Green" `
    -Message "`n✅ Создана папка релиза: $releaseFolder"


# ============================================================
# КОПИРОВАНИЕ APP
# ============================================================

Write-ColorOutput `
    -Color "Cyan" `
    -Message "`n📁 Копирование ReportEngine.App..."

$appFiles = Copy-Build `
    -SourcePath $BuildPath `
    -DestinationPath $releasePath

Write-ColorOutput `
    -Color "Green" `
    -Message "✅ Скопировано файлов App: $appFiles"


# ============================================================
# СОЗДАНИЕ UPDATEINFO.JSON
# ============================================================

$updateInfo = @{
    Version = $Version
    Date = (Get-Date).ToString("dd.MM.yyyy")

    Sections = @{
        Added = @()
        Changed = @()
        Fixed = @()
    }
}

$updateInfoPath = Join-Path `
    $releasePath `
    "updateInfo.json"

$updateInfo |
        ConvertTo-Json -Depth 5 |
        Out-File `
        -FilePath $updateInfoPath `
        -Encoding UTF8

Write-ColorOutput `
    -Color "Green" `
    -Message "✅ Создан updateInfo.json"


# ============================================================
# ПУБЛИКАЦИЯ UPDATER
# ============================================================

Write-ColorOutput `
    -Color "Cyan" `
    -Message "`n📦 Публикация ReportEngine.Updater"

if (Test-Path $UpdaterServerPath) {

    Write-ColorOutput `
        -Color "Yellow" `
        -Message "🗑️ Удаляем старую версию Updater..."

    Remove-Item `
        -Path $UpdaterServerPath `
        -Recurse `
        -Force
}

New-Item `
    -ItemType Directory `
    -Path $UpdaterServerPath `
    -Force |
        Out-Null

$updaterFiles = Copy-Build `
    -SourcePath $UpdaterBuildPath `
    -DestinationPath $UpdaterServerPath

Write-ColorOutput `
    -Color "Green" `
    -Message "✅ Updater опубликован"

Write-ColorOutput `
    -Color "Green" `
    -Message "   Файлов: $updaterFiles"

Write-ColorOutput `
    -Color "Green" `
    -Message "   Путь:   $UpdaterServerPath"


# ============================================================
# УДАЛЕНИЕ UPDATEINFO.JSON В ПАПКЕ UPDATER
# ============================================================

Write-ColorOutput `
    -Color "Cyan" `
    -Message "`n🗑️ Проверка updateInfo.json в Updater..."

if (Test-Path $UpdateInfoServerPath) {
    Remove-Item -Path $UpdateInfoServerPath -Force
    Write-ColorOutput `
        -Color "Green" `
        -Message "✅ updateInfo.json удален из Updater"
    Write-ColorOutput `
        -Color "Cyan" `
        -Message "   $UpdateInfoServerPath"
} else {
    Write-ColorOutput `
        -Color "Yellow" `
        -Message "⚠️ updateInfo.json не найден в Updater"
}


# ============================================================
# КОПИРОВАНИЕ LATEST.JSON НА СЕРВЕР
# ============================================================

Write-ColorOutput `
    -Color "Cyan" `
    -Message "`n📋 Копирование latest.json..."

if (Test-Path $LatestReleaseLocal) {
    try {
        Copy-Item -Path $LatestReleaseLocal -Destination $LatestReleaseServer -Force

        Write-ColorOutput `
            -Color "Green" `
            -Message "✅ latest.json скопирован на сервер"
        Write-ColorOutput `
            -Color "Cyan" `
            -Message "   Источник: $LatestReleaseLocal"
        Write-ColorOutput `
            -Color "Cyan" `
            -Message "   Назначение: $LatestReleaseServer"
    }
    catch {
        Write-ColorOutput `
            -Color "Red" `
            -Message "❌ Ошибка копирования latest.json: $_"
    }
} else {
    Write-ColorOutput `
        -Color "Yellow" `
        -Message "⚠️ latest.json не найден локально: $LatestReleaseLocal"
}


# ============================================================
# ИТОГ
# ============================================================

$totalFiles = $appFiles + $updaterFiles

Write-ColorOutput `
    -Color "Green" `
    -Message "`n========================================"

Write-ColorOutput `
    -Color "Green" `
    -Message "✅ Публикация завершена успешно!"

Write-ColorOutput `
    -Color "Green" `
    -Message "========================================"

Write-ColorOutput `
    -Color "Cyan" `
    -Message "`n📌 Информация о релизе:"

Write-ColorOutput `
    -Color "Cyan" `
    -Message "   Версия:     $Version"

Write-ColorOutput `
    -Color "Cyan" `
    -Message "   Папка:      $releaseFolder"

Write-ColorOutput `
    -Color "Cyan" `
    -Message "   App файлов: $appFiles"

Write-ColorOutput `
    -Color "Cyan" `
    -Message "   Updater:    $updaterFiles"

Write-ColorOutput `
    -Color "Cyan" `
    -Message "   Всего:      $totalFiles"

Write-ColorOutput `
    -Color "Cyan" `
    -Message "   App:        $($mainExe.Name)"

Write-ColorOutput `
    -Color "Cyan" `
    -Message "   Updater:    $($updaterExe.Name)"

Write-ColorOutput `
    -Color "Cyan" `
    -Message "   Stable:     $IsStable"

Write-ColorOutput `
    -Color "Cyan" `
    -Message "`n📍 Релиз:"
Write-ColorOutput `
    -Color "Cyan" `
    -Message "   $releasePath"

Write-ColorOutput `
    -Color "Cyan" `
    -Message "`n📍 Updater:"
Write-ColorOutput `
    -Color "Cyan" `
    -Message "   $UpdaterServerPath"

Write-ColorOutput `
    -Color "Cyan" `
    -Message "`n📋 latest.json:"
Write-ColorOutput `
    -Color "Cyan" `
    -Message "   $LatestReleaseServer"

Write-ColorOutput `
    -Color "Yellow" `
    -Message "`n💡 Пользователи увидят новую версию при следующей проверке обновлений"


# ============================================================
# ОТКРЫТИЕ ПАПКИ
# ============================================================

Write-ColorOutput `
    -Color "White" `
    -Message ""

$showFolder = Read-Host `
    "❓ Открыть папку релиза на сервере? (y/n)"

if ($showFolder -eq "y") {
    explorer $releasePath
}

Write-ColorOutput `
    -Color "Green" `
    -Message "`n🎉 Готово!"