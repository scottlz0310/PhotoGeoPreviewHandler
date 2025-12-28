# PhotoGeoExplorer

PhotoGeoExplorer は、写真の位置情報を地図上に表示する Windows デスクトップアプリです。
Windows 10/11 を対象とし、WinUI 3 と WebView2 でローカルの Leaflet マップを描画します。

## ステータス

このリポジトリは初期開発段階です。UI シェルと地図読み込みは実装済みですが、
ファイルブラウザ、EXIF 抽出、地図プロットのロジックは開発中です。

## 現在の機能

- WinUI 3 の単一ウィンドウ構成（ファイルブラウザ、画像プレビュー、地図の各ペイン）
- WebView2 でローカルの `wwwroot/index.html` マップページを読み込み
- `%LocalAppData%\\PhotoGeoExplorer\\Logs\\app.log` へのアプリログ出力

## 予定している機能

- フォルダナビゲーションとサムネイル付きファイル一覧
- EXIF/GPS 抽出と地図マーカー表示
- 画像プレビュー操作（ズーム/パン）
- オフライン向け地図タイルキャッシュ

## 技術スタック

- .NET 10 / C# (WinUI 3, Windows App SDK)
- WebView2 + Leaflet による地図描画
- EXIF 抽出に MetadataExtractor
- サムネイル/画像処理に SixLabors.ImageSharp

## 前提条件

- Windows 10/11
- .NET 10 SDK
- Visual Studio 2026 (WinUI 3 ワークロード。IDE で使う場合は任意)
- WebView2 Runtime

## ビルド

```powershell
dotnet restore PhotoGeoExplorer.sln
dotnet build PhotoGeoExplorer.sln -c Release -p:Platform=x64
```

## 実行

```powershell
dotnet run --project PhotoGeoExplorer/PhotoGeoExplorer.csproj -c Release -p:Platform=x64
```

## フォーマット/品質チェック

```powershell
dotnet format --verify-no-changes PhotoGeoExplorer.sln
dotnet build PhotoGeoExplorer.sln -c Release -p:Platform=x64 -p:TreatWarningsAsErrors=true -p:AnalysisLevel=latest
```

任意のフック:

```powershell
lefthook install
```

## リリース成果物

タグの push (例: `v0.1.0`) を契機に、`win-x64` 向けの未署名 MSIX インストーラーを作成します。

## ライセンス

`LICENSE` が追加されている場合はそちらを参照してください。未追加の場合は現状未ライセンスです。
