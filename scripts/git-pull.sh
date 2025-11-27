#!/usr/bin/env sh
set -e

# 1. リポジトリルートにいることを確認
pwd
git rev-parse --show-toplevel

# 2. 現在の状態確認（未コミットがある場合は stash か commit）
git status --porcelain
echo "未コミットの変更がある場合は commit または git stash を行ってください。"

# 3. SSH接続確認
echo "SSH 接続確認: ssh -T git@github.com"
ssh -T git@github.com || true

# 4. リモート確認・必要なら SSH に切替
git remote -v
# 例: HTTPS になっている場合は以下で SSH に変更
# git remote set-url origin git@github.com:scottlz0310/PhotoGeoPreviewHandler.git

# 5. 最新を取得して main を更新（安全のため fetch -> checkout -> pull）
git fetch origin
git checkout main || git checkout -b main
git pull --rebase origin main

# 6. pull 後に .gitignore と LICENSE の存在確認
echo "確認: .gitignore と LICENSE の存在"
ls -la .gitignore LICENSE || echo "いずれかのファイルが見つかりませんでした。"

echo "完了: リモートの main を取得しました。"