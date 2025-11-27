#!/usr/bin/env sh
set -e

# 1) まず現在が Git 管理下か確認
if git rev-parse --is-inside-work-tree >/dev/null 2>&1; then
  echo "既に Git 管理下です（.git が存在）。pull してください。"
  git status --porcelain
  git fetch origin
  git checkout main || git checkout -b main
  git pull --rebase origin main
  exit 0
fi

# 2) Git 管理下でない場合：推奨はクローン（現在ディレクトリへ）
echo "Git 管理下ではありません。リポジトリを SSH でクローンします（現在ディレクトリへ）。"
echo "注意: 既存ファイルがある場合は上書きされないようにバックアップしてください。"
git clone --depth 1 git@github.com:scottlz0310/PhotoGeoPreviewHandler.git .

# 3) （代替）既存ファイルを残して手動で初期化する場合の手順
# git init
# git remote add origin git@github.com:scottlz0310/PhotoGeoPreviewHandler.git
# git fetch origin
# git checkout -t origin/main