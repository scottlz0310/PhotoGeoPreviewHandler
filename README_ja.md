# PhotoGeoPreviewHandler

位置情報（EXIF GPS）が埋め込まれた画像用の Windows エクスプローラー プレビューハンドラー

## 概要

このプロジェクトは、以下を表示する Windows シェル拡張機能（COM プレビューハンドラー）を実装しています：
- **画像プレビュー**（上部ペイン）
- **インタラクティブマップ**（下部ペイン）Leaflet + OpenStreetMap で写真の撮影場所を表示
- **サイズ調整可能な分割ビュー**（GridSplitter 使用）

Windows エクスプローラーで画像ファイルを選択すると、プレビューペインに画像と、写真が撮影された場所を示すインタラクティブマップが表示されます（GPS データが利用可能な場合）。

## 機能

- **マルチフォーマット対応**: JPEG、PNG、BMP、GIF、HEIC（環境依存）
- **EXIF GPS 抽出**: 画像メタデータから位置情報を自動的に読み取り
- **インタラクティブマップ**: WebView2 経由で Leaflet.js を使用
- **フォールバック処理**: GPS データが利用できない場合は「ヌル島」（0°、0°）を表示
- **モダンな UI**: WPF ベースで調整可能な分割レイアウト
- **x86/x64 対応**: 両アーキテクチャで動作

## 前提条件

- **Windows 10/11**（x64 または x86）
- **WebView2 Runtime**（[Microsoft からダウンロード](https://developer.microsoft.com/microsoft-edge/webview2/)）
- **.NET 10 Runtime**（バンドルされていない場合）
- **HEIF Image Extensions**（オプション、Microsoft Store から HEIC サポート用）

## 技術スタック

- **.NET 10** / **C# 14.0**
- **WPF**（Windows Presentation Foundation）
- **WebView2**（Microsoft Edge WebView2）
- **Leaflet.js**（CDN 経由）
- **MetadataExtractor**（EXIF 解析用 NuGet パッケージ）
- **COM Interop**（IPreviewHandler、IInitializeWithStream）

## ビルド手順

1. リポジトリをクローン：
   ```bash
   git clone https://github.com/yourusername/PhotoGeoPreviewHandler.git
   cd PhotoGeoPreviewHandler
   ```

2. Visual Studio 2026 で開く：
   ```bash
   start PhotoGeoPreviewHandler.slnx
   ```

3. NuGet パッケージの復元とビルド：
   ```bash
   dotnet restore
   dotnet build -c Release
   ```

## インストール

### プレビューハンドラーの登録

提供されている登録スクリプトを**管理者権限**で実行：

```powershell
# PowerShell（scripts/ フォルダーから）
.\register-handler.ps1
```

または、手動登録用の `.reg` ファイルを使用（x86/x64）。

### 登録解除

```powershell
.\unregister-handler.ps1
```

### エクスプローラーの再起動

登録後、Windows エクスプローラーを再起動：
```powershell
Stop-Process -Name explorer -Force
```

## 使用方法

1. **Windows エクスプローラー**を開く
2. 画像ファイル（JPEG、PNG など）を選択
3. **プレビューペイン**を有効化（表示 > プレビューペイン、または Alt+P）
4. 画像とマップが並べて表示されます

## プロジェクト構造

```
PhotoGeoPreviewHandler/
├── PhotoGeoPreviewHandler/       # メインプロジェクト
│   ├── PreviewHandler.cs         # COM プレビューハンドラー実装
│   ├── PreviewHandlerControl.xaml # WPF UI レイアウト
│   ├── ExifDataExtractor.cs      # EXIF GPS 抽出
│   ├── MapHtmlGenerator.cs       # Leaflet HTML 生成
│   └── Resources/
│       └── map-template.html     # マップ用 HTML テンプレート
├── scripts/                      # インストールスクリプト
│   ├── register-handler.ps1
│   └── unregister-handler.ps1
├── docs/                         # ドキュメント
│   ├── ARCHITECTURE.md
│   ├── TECHSTACK.md
│   ├── ImplementationPlan.md
│   └── TASKS.md
└── README.md                     # 英語版
```

## 開発

開発ガイドラインについては [CONTRIBUTING.md](CONTRIBUTING.md)、コーディング規約については [COPILOT_RULES.md](COPILOT_RULES.md) を参照してください。

### デバッグ

1. Debug モードでビルド
2. ハンドラーを登録
3. Visual Studio を `explorer.exe` プロセスにアタッチ
4. コードにブレークポイントを設定
5. エクスプローラーで画像ファイルを選択してハンドラーをトリガー

## ロードマップ

### フェーズ 1 - MVP（現在）
- [x] 基本的な COM プレビューハンドラー
- [x] 分割レイアウトの WPF UI
- [x] EXIF GPS 抽出
- [x] Leaflet マップ統合
- [ ] 登録スクリプト
- [ ] マルチフォーマットテスト

### フェーズ 2 - 機能強化
- [ ] 分割比率の設定保存
- [ ] 逆ジオコーディング（場所名表示）
- [ ] パフォーマンス最適化
- [ ] エラー処理 UI の改善

## 既知の制限事項

- **HEIC サポート**には Windows HEIF コーデックが必要（追加セットアップが必要な場合あり）
- **WebView2 Runtime**が事前にインストールされている必要があります
- **大きな画像**はプレビューの読み込みに時間がかかる場合があります
- **GPS データ**が EXIF メタデータに埋め込まれている必要があります

## ライセンス

[MIT License](LICENSE)

## コントリビューション

コントリビューションを歓迎します！プルリクエストを送信する前に [CONTRIBUTING.md](CONTRIBUTING.md) をお読みください。

## トラブルシューティング

### プレビューが表示されない
- WebView2 Runtime がインストールされているか確認
- Windows イベントビューアーでエラーを確認
- 登録後にエクスプローラーを再起動

### マップが「ヌル島」を表示する
- 画像に GPS データが EXIF に含まれていません
- ExifTool などのツールを使用してメタデータを確認

### HEIC ファイルが動作しない
- Microsoft Store から「HEIF Image Extensions」をインストール
- Windows フォトアプリでコーデックサポートを確認

## クレジット

- **Leaflet.js** - インタラクティブマップライブラリ
- **OpenStreetMap** - マップタイルプロバイダー
- **MetadataExtractor** - EXIF 解析ライブラリ
- **WebView2** - Microsoft Edge WebView2

## お問い合わせ

問題や機能リクエストについては、[GitHub Issues](https://github.com/yourusername/PhotoGeoPreviewHandler/issues) ページをご利用ください。
