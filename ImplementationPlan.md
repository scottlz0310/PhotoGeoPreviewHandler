# 実装計画書（Windows Preview Handler with Geolocation Map）

目的:
- エクスプローラーのプレビュー領域で画像とそのEXIFジオロケーションを同時に表示するプレビュー ハンドラ（COM Shell Extension）を実装する。
- 画像上部、地図下部（Leaflet via WebView2）、GridSplitterで可変分割するUIを提供する。

スコープ（Phase 1 - MVP）:
- COM PreviewHandler の骨組み（IPreviewHandler / IInitializeWithStream 実装）
- WPF UserControl の XAML レイアウト（Image + GridSplitter + WebView2）
- EXIF GPS 抽出（MetadataExtractor ベース）
- Leaflet マップ HTML 生成と WebView2 経由での注入（Null Islandフォールバック）
- レジストリ登録スクリプト（x86/x64 対応）

非スコープ（Phase 1で行わない）:
- スプリット比の永続化（Phase 2）
- 逆ジオコーディング（Phase 2）
- 大規模なパフォーマンス最適化（必要時に対応）

実装ステップ（高レベル）:
1. プロジェクトセットアップ（Class Library、WPF UserControl を含む）
2. COM インターフェイスの実装（PreviewHandler.cs）
3. WPF レイアウト実装（PreviewHandlerControl.xaml / .xaml.cs）
4. EXIF 抽出ユーティリティ（ExifDataExtractor.cs）
5. Map HTML 生成（MapHtmlGenerator.cs + template）
6. WebView2 統合とデバッグ
7. レジストリ登録インストーラ作成（PowerShell / reg files）
8. 動作検証（多フォーマット、HEIC 含む）
9. ドキュメント作成とリリース手順

成功基準:
- エクスプローラーで画像を選択すると、画像上部 + 地図下部が表示される
- GridSplitter で分割を調整可能
- GPS があれば該当座標にマーカーを表示、無ければ Null Island を表示

注意点:
- HEIC サポートは環境依存（Windows の HEIF コーデックや外部ライブラリを利用）
- WebView2 ランタイムは事前要件（インストーラまたは検出ロジックを用意）