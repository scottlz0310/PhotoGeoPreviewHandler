# PhotoGeoPreview

PowerToys をフォークして実装する、C++/WinRT + WebView2 ベースの画像ジオタグ表示用 Preview Handler です。

## 概要

このプロジェクトは **PowerToys のフォーク** として、C++ で実装されます。
- **技術**: C++/WinRT + WebView2 + HTML/CSS/JS
- **UI**: 単一の WebView2 内で画像と地図を表示
- **スプリッター**: HTML/CSS/JS による可変レイアウト

## 特徴
- ✅ PowerToys の標準構造（C++）に完全準拠
- ✅ WebView2 単一で UI 完結
- ✅ HTML/CSS/JS による柔軟な UI
- ✅ WIC による高速 EXIF 抽出

## 動作環境
- Windows 10 / 11 (x64 / ARM64)
- PowerToys (フォーク版)
- WebView2 Runtime
- **HEIC サポート**: Windows HEIF Image Extensions（Microsoft Store から入手）

## ビルド手順

### 1. PowerToys をフォーク

```bash
git clone https://github.com/YOUR_USERNAME/PowerToys.git
cd PowerToys
```

### 2. PhotoGeoPreview を追加

`src/modules/previewpane/PhotoGeoPreview/` にコードを配置。

### 3. `preview_handlers.json` に登録

`installer/PowerToysSetup/preview_handlers.json` に追加：

```json
{
  "id": "{YOUR-GUID}",
  "name": "PhotoGeoPreview",
  "extensions": [".jpg", ".jpeg", ".png", ".heic"],
  "clsid": "{YOUR-GUID}"
}
```

### 4. PowerToys をビルド

```bash
# Visual Studio 2026 で開く
start PowerToys.sln

# またはコマンドラインでビルド
.\build.cmd
```

## 技術詳細

### UI 構造（HTML）
```html
<div class="split-container">
  <div class="image-pane">
    <img src="{IMAGE_PATH}">
  </div>
  <div class="splitter"></div>
  <div class="map-pane"></div>
</div>
```

### EXIF 抽出（C++ + WIC）
```cpp
IWICImagingFactory* factory;
IWICBitmapDecoder* decoder;
IWICMetadataQueryReader* reader;
// GPS 座標を取得
```

## 参考実装
- `src/modules/previewpane/MarkdownPreviewHandler/` (C++)
- `src/modules/previewpane/SvgPreviewHandler/` (C++)

## ライセンス

PowerToys のライセンス（MIT License）に準拠します。

## お問い合わせ

[GitHub Issues](https://github.com/scottlz0310/PhotoGeoPreviewPane/issues)
