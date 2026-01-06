
# filename: import-cert-admin.ps1
# 目的: 非管理者で起動されても UAC 昇格して、ローカルコンピュータの Root と TrustedPeople に .cer をインポート

# --- 管理者権限かチェック ---
$IsAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()
           ).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

# スクリプト自身のフルパス（相対パスで起動された場合も解決する）
$scriptPath = $MyInvocation.MyCommand.Path
if (-not $scriptPath) {
    Write-Error "このスクリプトは .ps1 ファイルとして実行してください（対話型では不可）。"
    exit 1
}

# --- 非管理者なら管理者権限の pwsh/powershell で再起動 ---
if (-not $IsAdmin) {
    Write-Host "管理者権限で再起動します (UAC の許可が必要です)…"

    # 引数を引き継ぐ（今回は未使用でも安全に引き継げるようにしておく）
    $argsJoined = ($args | ForEach-Object { '"{0}"' -f ($_ -replace '"','""') }) -join ' '

    # pwsh 優先／powershell フォールバック
    $pwshPath = (Get-Command pwsh -ErrorAction SilentlyContinue)?.Source
    if ($pwshPath) {
        $exe   = $pwshPath
        $alist = "-NoProfile -ExecutionPolicy Bypass -File `"$scriptPath`" $argsJoined"
    } else {
        $exe   = (Get-Command powershell -ErrorAction SilentlyContinue)?.Source
        if (-not $exe) {
            Write-Error "pwsh も powershell も見つかりません。PowerShell のインストールを確認してください。"
            exit 1
        }
        $alist = "-NoProfile -ExecutionPolicy Bypass -File `"$scriptPath`" $argsJoined"
    }

    Start-Process -FilePath $exe -ArgumentList $alist -Verb RunAs
    exit
}

# --- ここから下が管理者権限での本処理 ---
Write-Host "管理者権限で実行中: $([Environment]::UserName)"

# 証明書ファイル（相対パスを安全に絶対パスへ解決）
$relativeCer = ".\wack\certs\PhotoGeoExplorer_test.cer"
try {
    if (-not (Test-Path -Path $relativeCer -PathType Leaf)) {
        throw "証明書ファイルが見つかりません: $relativeCer"
    }
    $cerPath = (Resolve-Path -Path $relativeCer).Path
} catch {
    Write-Error $_.Exception.Message
    exit 1
}

Write-Host "インポート対象: $cerPath"

# インポート処理（Root と TrustedPeople）
$targets = @(
    "Cert:\LocalMachine\Root",
    "Cert:\LocalMachine\TrustedPeople"
)

foreach ($store in $targets) {
    try {
        Write-Host "インポート中 -> $store"
        $result = Import-Certificate -FilePath $cerPath -CertStoreLocation $store -ErrorAction Stop
        # Import-Certificate は X509Certificate2 を返す。重複の場合は既存にマージされることが多い
        if ($result) {
            Write-Host "成功: $($result.Thumbprint) を $store に追加/更新"
        } else {
            Write-Warning "結果オブジェクトがありませんが続行します（環境により挙動が異なる場合あり）。"
        }
    } catch {
        Write-Error "失敗: $store へのインポート中にエラー -> $($_.Exception.Message)"
        # 片方失敗しても他方は続けたい場合は 'continue' に変更
        exit 1
    }
}

Write-Host "すべてのインポートが完了しました。"
