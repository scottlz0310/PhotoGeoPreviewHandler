# 技術スタック計画書（Modern スタイル）

基本プラットフォーム:
- .NET 10 / C# 14.0（プロジェクト標準に合わせる）
- ターゲット: Windows 10/11 の Explorer Preview Pane（x86/x64）

UI:
- WPF（UserControl + XAML）
- レイアウト: Grid + GridSplitter、Image、WebView2 コンテナ

ブラウザ / 地図:
- WebView2（Microsoft Edge WebView2 Runtime）
- Map: Leaflet.js（CDN） + OpenStreetMap タイル
  - Map HTML はローカルで生成し WebView2 に注入

EXIF / 画像処理:
- NuGet: `MetadataExtractor`（EXIF / GPS 抽出の第一候補）
- 必要なら: `Magick.NET` または WIC（Windows Imaging Component）を補助で使用（HEIC 対応で検討）
- 画像変換はメモリ内ストリームで行い、UI スレッドをブロックしない（Task + async）

COM / レジストリ:
- COM インターフェイスを C# で宣言（GUID とインターフェイス実装）
- レジストリ登録スクリプト: PowerShell または .reg（x86/x64 対応）
- 開発時は __CommandName__ や __Setting__ を明記して Visual Studio での登録/デバッグ手順を提供

CI / 開発ツール:
- Visual Studio 2026 （開発/デバッグ）
- Optional: GitHub Actions（ビルドとアーティファクト作成、x86/x64 ビルド）
- テスト: 手動 UI テスト + ユニットテスト（Exif 抽出など）

その他:
- ロギング: 軽量（EventSource / Trace）— 必要時のみ有効化
- ライセンスは依存ライブラリに合わせる（OSS ライセンス確認）