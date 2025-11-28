# 実装計画書（PowerToys Fork + C++/WinRT Implementation）

## 目的
PowerToys をフォークし、**C++/WinRT + WebView2 + HTML** で `PhotoGeoPreview` Preview Handler を実装する。
画像プレビューとジオタグ地図を **HTML 内で** 上下に配置し、CSS/JS による可変スライダーで調整可能にする。

## 技術スタック（PowerToys 準拠）
- **言語**: C++/WinRT (C++17/20)
- **UI**: WebView2 + HTML/CSS/JavaScript
- **EXIF**: WIC (Windows Imaging Component)
- **地図**: Leaflet.js (CDN)
- **プロジェクト**: `.vcxproj` (Visual Studio C++ プロジェクト)

## アプローチ
PowerToys の既存 Preview Handler（Markdown, SVG, DevFilePreview）と同じパターンで実装：
1. **単一の WebView2** をホスト
2. **HTML テンプレート** で画像と地図を配置
3. **CSS/JS** で resizable splitter を実装
4. **C++ 側** で EXIF 抽出 + HTML 生成

## スコープ（Phase 1 - MVP）

### 1. PowerToys フォーク & セットアップ
- [ ] PowerToys リポジトリをフォーク
- [ ] Visual Studio 2026 でビルド確認
- [ ] 既存 Preview Handler の実装調査：
  - `src/modules/previewpane/MarkdownPreviewHandler/` (C++)
  - `src/modules/previewpane/SvgPreviewHandler/` (C++)

### 2. PhotoGeoPreview Add-on 作成（C++）
- [ ] `src/modules/previewpane/PhotoGeoPreview/` ディレクトリ作成
- [ ] `PhotoGeoPreview.vcxproj` 作成
- [ ] `PhotoGeoPreviewHandler.h/.cpp` 作成
- [ ] `module.def` 作成（COM エクスポート）
- [ ] `preview_handlers.json` に登録

### 3. UI 実装（HTML/CSS/JS）
**HTML テンプレート** (`template.html`):
```html
<div class="container">
  <div class="image-pane" id="imagePane">
    <img id="photo" src="{IMAGE_PATH}" />
  </div>
  <div class="splitter" id="splitter"></div>
  <div class="map-pane" id="mapPane"></div>
</div>
<script src="https://unpkg.com/leaflet/dist/leaflet.js"></script>
<script>
  const map = L.map('mapPane').setView([{LAT}, {LON}], 13);
  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png').addTo(map);
  L.marker([{LAT}, {LON}]).addTo(map);
</script>
```

**CSS**: Flexbox + resizable splitter
**JS**: マウスドラッグで上下比率調整

### 4. ロジック実装（C++）
- [ ] **WIC で EXIF GPS 抽出**:
  ```cpp
  IWICImagingFactory* factory;
  IWICBitmapDecoder* decoder;
  IWICMetadataQueryReader* reader;
  // GPS 座標を取得
  ```
- [ ] **HTML 生成**:
  - テンプレートに画像パス、緯度経度を埋め込み
  - WebView2 に `NavigateToString()` で渡す

### 5. ビルド & 配布
- [ ] PowerToys 全体をビルド（CMake + Visual Studio）
- [ ] 動作確認（JPEG, PNG, HEIC）
- [ ] 配布用パッケージ作成

## 非スコープ
- WPF/XAML（使用しない）
- C#/.NET（使用しない）
- 複数 UI コントロール（WebView2 単一で完結）

## 成功基準
- PowerToys Runner 経由で自動登録される
- Explorer で画像選択時、HTML 内に画像と地図が表示される
- スライダーで上下比率を調整可能
- PowerToys の他の機能と共存できる

## 技術的利点
- ✅ PowerToys の標準構造（C++/WinRT）に完全準拠
- ✅ WebView2 の単一ホストで UI 完結
- ✅ HTML/CSS/JS による柔軟な UI 実装
- ✅ WIC による高速 EXIF 抽出

## 実装時の重要な注意点（PowerToys ベストプラクティス）

### ✅ 最重要

#### 1. HTML の読み込み方法
**推奨**: `NavigateToString()` ではなく、**一時ファイル経由**で読み込む
```cpp
// ❌ 非推奨（大きな HTML で遅い、CSP 問題）
webview->NavigateToString(htmlContent);

// ✅ 推奨（PowerToys の標準パターン）
std::wstring tempPath = GetTempHtmlPath();
WriteFile(tempPath, htmlContent);
webview->Navigate(L"file:///" + tempPath);
```

**理由**:
- `NavigateToString()` は大きな HTML で遅い
- Leaflet のような外部ライブラリと CSP の相性が悪い
- ローカルファイルの方がパフォーマンスと安定性が高い

#### 2. Leaflet の CDN とオフライン対応
**推奨**: CDN + ローカル fallback の 2 段構成
```html
<script src="https://unpkg.com/leaflet/dist/leaflet.js"
        onerror="this.onerror=null; this.src='resources/leaflet.js'">
</script>
```

**理由**:
- オフライン環境や社内ネットワークでも動作
- PowerToys ユーザーの多様な環境に対応

#### 3. ファイルパスの URI エスケープ
**必須**: 日本語・空白を含むパスは URI エンコード
```cpp
// ❌ 危険（日本語/空白でエラー）
html.replace("{IMAGE_PATH}", L"C:\\Users\\太郎\\Pictures\\旅行 2024\\IMG.jpg");

// ✅ 安全（PowerToys のユーティリティ関数を使用）
std::wstring escapedPath = UriEscape(imagePath);
html.replace("{IMAGE_PATH}", escapedPath);
```

### 🟦 中程度の重要度

#### 4. JavaScript の分離
**推奨**: HTML 内に直書きせず、外部ファイル化
```
PhotoGeoPreview/
├── Resources/
│   ├── template.html
│   ├── styles.css
│   ├── map.js          # Leaflet 初期化
│   └── splitter.js     # スプリッター処理
```

**理由**: PowerToys の既存 Add-on もこの方式でメンテナンス性が高い

#### 5. EXIF なし画像のフォールバック
**推奨**: GPS データがない場合の UI 対応
```cpp
if (!hasGPS) {
    // オプション1: 地図を非表示
    html.replace("{MAP_DISPLAY}", "display:none");

    // オプション2: 「位置情報なし」メッセージ
    html.replace("{MAP_CONTENT}", "<p>位置情報がありません</p>");
}
```

### 🟩 軽微な改善点

#### 6. HEIC サポートの前提条件
**メモ**: HEIC は Windows の HEIF 拡張が必要
- PowerToys の既存 Image Preview と同じ前提
- README に記載推奨
