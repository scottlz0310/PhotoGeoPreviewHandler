# タスクリスト（段階的実装）

フェーズ 0 — 準備
- [ ] リポジトリ構造を確定（プロジェクト、Resources、Installers）
- [ ] COPILOT_RULES.md / CONTRIBUTING.md の反映確認
- [ ] WebView2 ランタイム依存をドキュメントに追加

フェーズ 1 — コア実装（MVP）
1. プロジェクト & ビルド
   - [ ] `PhotoGeoView.PreviewHandler` プロジェクト作成（WPF UserControl を含む）
2. COM プレビュー ハンドラ
   - [ ] `PreviewHandler.cs`：`IPreviewHandler`, `IInitializeWithStream` 実装（骨組み）
   - [ ] GUID と COM 登録ポイント定義
3. UI
   - [ ] `PreviewHandlerControl.xaml`：Grid（画像／splitter／map）
   - [ ] `PreviewHandlerControl.xaml.cs`：画像読み込み・WebView2 初期化ロジック
4. EXIF 抽出
   - [ ] `ExifDataExtractor.cs`：`MetadataExtractor` を用いた GPS 抽出 （単体テスト）
5. Map HTML
   - [ ] `map-template.html` と `MapHtmlGenerator.cs` を作成（Leaflet CDN）
6. WebView2 統合
   - [ ] WebView2 初期化、HTML 注入、JS へ座標渡し
7. レジストリ / インストーラ
   - [ ] PowerShell / .reg を用いた登録スクリプト（x86/x64）
8. テスト & 検証
   - [ ] JPEG / PNG / BMP / GIF / HEIC（環境依存）で動作確認
   - [ ] Dark/Light テーマの見た目確認
9. ドキュメント
   - [ ] インストール手順、既知の制約、デバッグ手順を作成

フェーズ 2 — 改善（後続）
- [ ] スプリット比の永続化（オプション）
- [ ] 逆ジオコーディング（表示名のための外部 API 統合）
- [ ] エラーハンドリング UI の整備
- [ ] パフォーマンス最適化（遅延読み込み、キャッシュ）

各タスクの想定所要時間（MVP合計目安）:
- 3〜7 日（開発環境やHEICサポート状況に依存）

備考:
- 問題が出たら都度技術的な対応方針を決定（Modern, 迅速なトライ＆エラー方針）