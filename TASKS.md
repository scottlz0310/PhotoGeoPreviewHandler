# タスクリスト（C++/WinRT + WebView2 Implementation）

フェーズ 0 — 設計と計画
- [x] `ARCHITECTURE.md` の更新（C++/WinRT + WebView2 構造）
- [x] `TECHSTACK.md` の更新（C++ 技術スタック）
- [x] `ImplementationPlan.md` の更新（C++ 実装計画）

フェーズ 1 — PowerToys セットアップ
1. PowerToys フォーク & ビルド
   - [ ] PowerToys リポジトリをフォーク
   - [ ] Visual Studio 2026 でビルド確認
   - [ ] 既存 Preview Handler の実装調査：
     - `MarkdownPreviewHandler` (C++)
     - `SvgPreviewHandler` (C++)

フェーズ 2 — PhotoGeoPreview Add-on 作成（C++）
1. プロジェクト作成
   - [ ] `src/modules/previewpane/PhotoGeoPreview/` ディレクトリ作成
   - [ ] `PhotoGeoPreview.vcxproj` 作成
   - [ ] `module.def` 作成（COM エクスポート）
   - [ ] `preview_handlers.json` に登録
2. COM ハンドラ実装（C++/WinRT）
   - [ ] `PhotoGeoPreviewHandler.h/.cpp` 作成
   - [ ] `IPreviewHandler` インターフェイス実装
   - [ ] `IInitializeWithFile` 実装
   - [ ] WebView2 ホスト実装
3. UI 実装（HTML/CSS/JS）
   - [ ] `template.html` 作成：
     - 画像ペイン（`<img>`）
     - スプリッター（`<div>`）
     - 地図ペイン（Leaflet）
   - [ ] `styles.css` 作成（Flexbox レイアウト）
   - [ ] スプリッターのドラッグ処理（JavaScript）
4. ロジック実装（C++）
   - [ ] WIC で EXIF GPS 抽出
   - [ ] HTML テンプレート生成
   - [ ] WebView2 に `NavigateToString()`

フェーズ 3 — ビルド & 検証
1. ビルド
   - [ ] PowerToys 全体をビルド（CMake + VS）
   - [ ] `PhotoGeoPreview` が正しく含まれることを確認
2. テスト & 検証
   - [ ] 動作確認（JPEG / PNG / HEIC）
   - [ ] PowerToys Runner 経由での自動登録確認
   - [ ] スプリッターのドラッグ動作確認
   - [ ] テーマ切り替え（Dark/Light）確認

フェーズ 4 — 配布準備
- [ ] 配布用パッケージ作成（zip または installer）
- [ ] README 更新（ビルド・インストール手順）

備考:
- PowerToys をフォークし、C++ Add-on として実装（PR は送らない）
- WebView2 + HTML で UI 完結（WPF/XAML は使用しない）
- WIC による EXIF 抽出（MetadataExtractor は使用しない）
