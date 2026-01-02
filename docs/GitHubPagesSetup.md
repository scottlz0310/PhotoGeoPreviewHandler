# GitHub Pages セットアップガイド

このドキュメントでは、PhotoGeoExplorer のプライバシーポリシーを GitHub Pages で公開するための手順を説明します。

## 前提条件

- GitHub リポジトリ（scottlz0310/PhotoGeoExplorer）が存在すること
- `docs/privacy-policy.html` ファイルが作成済みであること
- `docs/.nojekyll` ファイルが作成済みであること
- `docs/index.html` ファイル（リダイレクト用）が作成済みであること

## セットアップ手順

### 1. GitHub リポジトリの Settings にアクセス

1. ブラウザで https://github.com/scottlz0310/PhotoGeoExplorer にアクセス
2. リポジトリページの右上にある **Settings** タブをクリック

### 2. Pages 設定にアクセス

1. 左サイドバーの **Code and automation** セクションにある **Pages** をクリック

### 3. ソースの設定

**Build and deployment** セクションで以下のように設定します：

#### Source
- **Deploy from a branch** を選択（デフォルト）

#### Branch
1. ドロップダウンメニューから **main** ブランチを選択
2. フォルダーのドロップダウンメニューから **/docs** を選択
3. **Save** ボタンをクリック

設定は以下のようになります：

```
Source: Deploy from a branch
Branch: main    /docs    [Save]
```

### 4. デプロイの確認

1. 設定保存後、GitHub Actions が自動的にサイトをビルド・デプロイします
2. 数分待つと、Pages セクションの上部に以下のような通知が表示されます：
   ```
   Your site is live at https://scottlz0310.github.io/PhotoGeoExplorer/
   ```

### 5. プライバシーポリシーへのアクセス確認

ブラウザで以下の URL にアクセスして、プライバシーポリシーが表示されることを確認します：

- **プライバシーポリシー直接リンク**: https://scottlz0310.github.io/PhotoGeoExplorer/privacy-policy.html
- **ルート URL**: https://scottlz0310.github.io/PhotoGeoExplorer/ （`privacy-policy.html` にリダイレクト）

## トラブルシューティング

### 404 エラーが表示される

**原因**: デプロイが完了していない、またはファイルパスが間違っている

**対処法**:
1. リポジトリの **Actions** タブで、pages-build-deployment ワークフローが完了しているか確認
2. `docs/privacy-policy.html` が main ブランチにコミット・プッシュされているか確認
3. デプロイ完了まで数分待つ

### スタイルが適用されない

**原因**: Jekyll が HTML を処理している

**対処法**:
- `docs/.nojekyll` ファイルが存在することを確認
- 存在しない場合は作成してコミット・プッシュ

### リダイレクトが動作しない

**原因**: `docs/index.html` が存在しない、またはリダイレクトコードが正しくない

**対処法**:
- `docs/index.html` の内容を確認
- 以下のようなリダイレクトコードが含まれていることを確認：
  ```html
  <!DOCTYPE html>
  <html>
  <head>
    <meta http-equiv="refresh" content="0; url=privacy-policy.html">
    <script>window.location.href = 'privacy-policy.html';</script>
  </head>
  <body>
    <p>リダイレクト中... <a href="privacy-policy.html">こちら</a>をクリックしてください。</p>
  </body>
  </html>
  ```

## Microsoft Partner Center での設定

GitHub Pages でプライバシーポリシーが公開されたら、Partner Center のアプリ情報ページで以下の URL を設定します：

**プライバシーポリシー URL**: `https://scottlz0310.github.io/PhotoGeoExplorer/privacy-policy.html`

## 参考リンク

- [GitHub Pages について](https://docs.github.com/pages/getting-started-with-github-pages/about-github-pages)
- [GitHub Pages でのサイトの作成](https://docs.github.com/pages/getting-started-with-github-pages/creating-a-github-pages-site)
