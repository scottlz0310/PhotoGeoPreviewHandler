# 技術スタック計画書 (C++/WinRT + WebView2)

## 基本プラットフォーム
- **Base**: PowerToys (Fork)
- **言語**: C++/WinRT (C++17/20)
- **Target**: Windows 10 / 11 (x64 / ARM64)
- **Host**: PowerToys Runner → Windows Explorer Preview Pane

## UI テクノロジー
- **WebView2 (Microsoft Edge based)**:
  - 単一の WebView2 内で全 UI を実装
  - HTML/CSS/JS による柔軟なレイアウト
- **HTML/CSS/JavaScript**:
  - **Layout**: Flexbox で画像と地図を上下配置
  - **Splitter**: CSS + JS でリサイズ可能なスプリッター実装
  - **Map**: Leaflet.js (CDN)

## ライブラリ & ツール
- **WIC (Windows Imaging Component)**:
  - ネイティブ C++ による高速 EXIF 解析
  - GPS メタデータ抽出
- **Leaflet.js**:
  - 軽量なオープンソース JavaScript マップライブラリ
- **OpenStreetMap**:
  - 地図タイルプロバイダ

## PowerToys インフラ
- **COM Preview Handler**: C++/WinRT による実装
- **PowerToys Runner**: 自動 COM 登録・管理
- **Settings UI**: PowerToys 設定画面で有効/無効切り替え
- **Theme System**: PowerToys のテーマ（Dark/Light）に自動追従

## 開発環境
- **IDE**: Visual Studio 2026
- **Build Tools**: CMake + MSBuild
- **Version Control**: Git (PowerToys fork)

## ビルド & デプロイ
- **ビルド方法**:
  - PowerToys 全体をビルド（`build.cmd` または Visual Studio）
  - `PhotoGeoPreview` は自動的に含まれる
- **配布方法**:
  - ビルド済み PowerToys 全体を zip で配布
  - または PowerToys インストーラーを再パッケージ
- **登録方法**:
  - PowerToys Runner が自動的に COM 登録
  - 手動登録は不要

## 技術選定の背景

### C++/WinRT (vs C#/.NET)
- **PowerToys の標準**: 既存の Preview Handler は全て C++ で実装
- **COM ネイティブ**: Explorer との統合がスムーズ
- **パフォーマンス**: ネイティブコードによる高速処理
- **一貫性**: 他の Preview Handler と同じ技術スタック

### WebView2 + HTML (vs WPF/XAML)
- **PowerToys の実装パターン**: Markdown, SVG などは全て WebView2 + HTML
- **単一ホスト**: WebView2 一つで UI 完結
- **柔軟性**: HTML/CSS/JS による自由な UI 設計
- **Web 標準**: Leaflet.js などの Web ライブラリを直接利用可能

### WIC (vs MetadataExtractor)
- **ネイティブ API**: Windows 標準の画像処理 API
- **高速**: C++ による直接アクセス
- **依存関係なし**: 外部ライブラリ不要
- **PowerToys との一貫性**: 他の Preview Handler も WIC を使用
