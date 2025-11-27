# アーキテクチャ概要

コンポーネント図（概念）:
- PreviewHandler (COM) — IPreviewHandler 準拠
  - 初期化: 受け取った IStream をメモリに読み込む
  - UI ホスト: WPF UserControl を PreviewPane に表示

- PreviewHandlerControl (WPF)
  - ImagePreview（上部）
  - GridSplitter（中央）
  - MapContainer（下部、WebView2）

- ExifDataExtractor
  - 画像ストリームから EXIF を解析し GpsCoordinate を返す

- MapHtmlGenerator
  - Leaflet HTML を生成し、渡された座標（null可）に基づいてマップ表示ロジックを埋め込む

データフロー:
1. Explorer がファイルをプレビュー要求 → COM ハンドラへストリーム供給
2. ハンドラがストリームを非同期で読み込み、画像バイナリと EXIF を解析
3. UI に画像をバインドし、MapHTML を WebView2 に注入
4. 地図は JS 内で座標を受け取り、マーカー・ズームを行う

エラーハンドリング方針:
- UIはなるべく壊さず、画面内に情報ラベルやシンプルなメッセージを表示
- 重大エラーはログに出力（軽量トレース）
- HEIC 等の未対応フォーマットはユーザー向けメッセージを表示

拡張ポイント:
- 逆ジオコーディングは MapHtml へオプションで統合可能
- OSM タイルプロバイダの切替（設定ファイル or コンパイル時）

セキュリティ注意:
- WebView2 に注入する HTML はローカル生成し、外部スクリプトの読み込みは最小限にする
- 必要な場合は Content-Security-Policy を検討