# 変更履歴

このプロジェクトの主な変更点をここに記録します。

## [未リリース]

### 追加
- WinUI 3 アプリシェル（ファイルブラウザ、画像プレビュー、地図の各ペイン）。
- ローカルの `wwwroot/index.html` マップページを読み込む WebView2 初期化。
- `%LocalAppData%\\PhotoGeoExplorer\\Logs\\app.log` へのアプリログ出力。
- CI/品質/セキュリティのワークフローと、タグベースの未署名 MSIX リリースワークフロー。
- lefthook による pre-commit / pre-push チェック。

### 変更
- 解析の厳格化とフォーマットチェックを CI とフックに導入。
- 主要依存関係の更新（Windows App SDK、WebView2、MetadataExtractor）。

### 修正
- 生成ファイルの出力先を短いパスに変更し、CodeQL のビルド失敗を解消。
- Windows App SDK のブートストラップパスを修正し、アプリ起動を安定化。
- `AppWindow` を安全に扱うようにウィンドウサイズ計算を修正。

### 削除
- まだアプリに接続されていなかった仮置きの Models/Services/ViewModels。
