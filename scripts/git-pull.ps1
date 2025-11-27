#!pwsh
# Git を SSH で安全に pull / clone するための PowerShell スクリプト
# 実行: pwsh .\scripts\git-pull.ps1

$ErrorActionPreference = 'Stop'

function Write-Info([string]$msg){ Write-Host $msg -ForegroundColor Cyan }
function Write-Warn([string]$msg){ Write-Host $msg -ForegroundColor Yellow }
function Write-Err([string]$msg){ Write-Host $msg -ForegroundColor Red }

Write-Info "作業ディレクトリ: $(Get-Location)"

# Git インストール確認
try {
    & git --version > $null
} catch {
    Write-Err "git が見つかりません。Git をインストールしてから実行してください。"
    exit 1
}

# Git 管理下か確認
$insideRepo = $false
try {
    $inside = (& git rev-parse --is-inside-work-tree 2>$null).Trim()
    if ($inside -eq 'true') { $insideRepo = $true }
} catch {
    $insideRepo = $false
}

if ($insideRepo) {
    $root = (& git rev-parse --show-toplevel).Trim()
    Write-Info "既存の Git リポジトリを検出: $root"

    # 未コミット変更チェック
    $status = (& git status --porcelain)
    if ($status) {
        Write-Warn "未コミットの変更が検出されました。続行する前に commit または stash してください。"
        Write-Host $status
        exit 1
    }

    # SSH 接続確認（出力は GitHub 側メッセージ）
    Write-Info "SSH 接続確認: ssh -T git@github.com"
    try {
        & ssh -T git@github.com 2>&1 | ForEach-Object { Write-Host $_ }
    } catch {
        Write-Warn "SSH 接続でエラーが発生しました。SSH キー設定を確認してください。"
    }

    # origin の URL を確認し、HTTPS なら SSH に切替を提案
    $remoteUrl = ''
    try { $remoteUrl = (& git remote get-url origin) } catch {}
    if (-not $remoteUrl) {
        Write-Warn "origin リモートが設定されていません。必要なら手動で追加してください。"
    } elseif ($remoteUrl.StartsWith('https://')) {
        Write-Warn "origin が HTTPS です。SSH に切り替えます。"
        $sshUrl = 'git@github.com:scottlz0310/PhotoGeoPreviewHandler.git'
        & git remote set-url origin $sshUrl
        Write-Info "origin を $sshUrl に変更しました。"
    } else {
        Write-Info "origin: $remoteUrl"
    }

    # fetch + checkout + pull --rebase
    Write-Info "リモートを取得しています..."
    & git fetch origin

    try {
        & git checkout main
    } catch {
        Write-Warn "ローカルに 'main' ブランチが存在しないため作成/チェックアウトします。"
        & git checkout -b main
        & git branch --set-upstream-to=origin/main main 2>$null || Write-Warn "origin/main がないかもしれません。"
    }

    Write-Info "origin/main を rebase で取得します..."
    & git pull --rebase origin main

    # ファイル確認
    Write-Info "確認: .gitignore と LICENSE の存在"
    if (Test-Path .gitignore) { Write-Info " .gitignore: 存在します" } else { Write-Warn " .gitignore: 見つかりません" }
    if (Test-Path LICENSE) { Write-Info " LICENSE: 存在します" } else { Write-Warn " LICENSE: 見つかりません" }

    Write-Info "完了: リモートの main を取得しました。"
    exit 0
}

# Git 管理下でない場合 → クローンするか確認
Write-Warn "このディレクトリは Git 管理下ではありません。現在のディレクトリにクローンしますか？"
$answer = Read-Host "クローンしてよい場合は 'y' を入力してください (y/n)"
if ($answer -ne 'y') {
    Write-Info "中止しました。既存ディレクトリに残したい場合はバックアップ後に手動で以下を実行してください:"
    Write-Host "git clone --depth 1 git@github.com:scottlz0310/PhotoGeoPreviewHandler.git ."
    exit 0
}

# クローン実行
$sshRepo = 'git@github.com:scottlz0310/PhotoGeoPreviewHandler.git'
Write-Info "クローンを開始します: $sshRepo"
& git clone --depth 1 $sshRepo .

Write-Info "クローン完了。'main' ブランチをチェックアウトします。"
try { & git checkout main } catch {}

Write-Info "完了。"