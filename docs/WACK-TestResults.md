# Windows アプリ認定キット (WACK) テスト結果と対処法

最終更新: 2026年1月3日

## テスト結果サマリー

- **全体の結果**: 不合格
- **アプリ名**: PhotoGeoExplorer
- **バージョン**: 1.3.0.0
- **テスト日時**: 2026/01/03 2:00:57

## 重要な問題と対処法

### 🔴 Critical: 特殊用途の機能 (runFullTrust)

**問題**:
- WinUI 3 デスクトップアプリは `runFullTrust` capability が必須
- Microsoft Store では「特殊用途の機能」として扱われる
- **会社アカウント**が必要、または個人アカウントの場合は審査時に使用理由の説明が必要

**対処法**:
1. Partner Center で会社アカウントを取得（推奨）
2. または、個人アカウントの場合:
   - Store 提出時の「Notes for certification」に以下を記載:
   ```
   This app requires runFullTrust capability because:
   - It is a WinUI 3 desktop application that needs full file system access
   - It reads photo files from user-selected folders
   - It extracts EXIF/GPS metadata from photos
   - It uses native Windows APIs for file operations
   ```

**ステータス**: ⚠️ 会社アカウントの確認が必要

---

### 🔴 Critical: デバッグ構成

**問題**: デバッグビルドで MSIX パッケージを作成している

**対処法**: Release ビルドで再作成
```powershell
dotnet publish .\PhotoGeoExplorer\PhotoGeoExplorer.csproj -c Release -p:Platform=x64 `
  -p:WindowsPackageType=MSIX -p:GenerateAppxPackageOnBuild=true -p:AppxBundle=Never `
  -p:UapAppxPackageBuildMode=StoreUpload -p:AppxPackageSigningEnabled=false
```

**ステータス**: ⚠️ 要修正

---

### 🔴 Critical: プライベート コード署名キー

**問題**: パッケージに .pfx または .snk ファイルが含まれている

**対処法**:
- プロジェクトから .pfx, .snk ファイルを除外
- .gitignore に追加済みか確認
- パッケージング時に除外されるよう .csproj を確認

**ステータス**: ✅ 完了（該当ファイルなし）

---

### 🟡 High: 高 DPI サポート

**問題**: DPI 対応の宣言がない

**対処法**: Package.appxmanifest に DPI awareness を追加
```xml
<uap:VisualElements ... >
  ...
</uap:VisualElements>
<Extensions>
  <uap10:Extension Category="windows.runFullTrust" EntryPoint="Windows.FullTrustApplication">
    <uap10:RuntimeBehavior DpiAwareness="perMonitorV2" />
  </uap10:Extension>
</Extensions>
```

**ステータス**: ✅ 完了

---

### 🟡 High: ブランド化（既定のイメージ）

**問題**: テンプレートまたは SDK サンプルの既定イメージを使用している

**対処法**:
- すべてのアセットを実際の画像に置き換え済みか確認
- 特に以下を確認:
  - Square310x310Logo.png（まだ作成されていない）
  - その他のロゴが既定画像でないか確認

**ステータス**: ⚠️ Square310x310Logo.png 作成待ち

---

### 🟡 Medium: Windows ランタイム メタデータ検証

**問題**: 複数のメタデータエラー
- ExclusiveTo 属性
- 型のロケーション
- 型名の大文字小文字の区別
- 型名の正確性
- プロパティ

**対処法**:
- これらは WinUI 3 フレームワーク自体の問題の可能性
- 最新の Windows App SDK に更新
- または、Store 提出時に「Notes for certification」で説明

**ステータス**: ⚠️ 要調査

---

### 🟢 Low: その他のエラー

以下は情報提供のみで、Store 提出には直接影響しない可能性があります：
- 署名されていない実行可能ファイル（Store が自動署名）
- UAC 実行レベル
- アーカイブ ファイルの使用量
- ブロック済みの実行可能ファイル

---

## 修正の優先順位

### 最優先（Store 提出前に必須）
1. ⚠️ Release ビルドで MSIX を再作成（下記コマンドで実行）
2. ✅ DPI awareness を Package.appxmanifest に追加（完了）
3. ✅ プライベートキーがパッケージに含まれていないか確認（完了 - 該当ファイルなし）
4. ⚠️ Square310x310Logo.png を作成（310×310 px）

### 高優先度
5. ⚠️ runFullTrust の使用理由を「Notes for certification」に記載
6. ⚠️ 会社アカウントの確認（または個人アカウントでの申請方法確認）

### 中優先度
7. ⚠️ Windows ランタイム メタデータエラーの調査

## 参考リンク

- [アプリ認定要件](https://docs.microsoft.com/windows/uwp/publish/app-certification-requirements)
- [runFullTrust capability](https://docs.microsoft.com/windows/uwp/packaging/app-capability-declarations#restricted-capabilities)
- [高 DPI アプリの作成](https://docs.microsoft.com/windows/win32/hidpi/high-dpi-desktop-application-development-on-windows)
