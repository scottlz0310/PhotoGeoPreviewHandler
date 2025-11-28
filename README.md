# PhotoGeoPreview

A C++/WinRT + WebView2 based PowerToys fork add-on for displaying image geotags.

## Overview

Implemented as a **PowerToys fork** using C++.
- **Tech**: C++/WinRT + WebView2 + HTML/CSS/JS
- **UI**: Single WebView2 hosting image and map
- **Splitter**: HTML/CSS/JS resizable layout

## Features
- ✅ Fully compliant with PowerToys standard structure (C++)
- ✅ Complete UI within single WebView2
- ✅ Flexible UI via HTML/CSS/JS
- ✅ Fast EXIF extraction via WIC

## Requirements
- Windows 10 / 11 (x64 / ARM64)
- PowerToys (forked version)
- WebView2 Runtime
- **HEIC Support**: Windows HEIF Image Extensions (from Microsoft Store)

## Build Instructions

### 1. Fork PowerToys

```bash
git clone https://github.com/YOUR_USERNAME/PowerToys.git
cd PowerToys
```

### 2. Add PhotoGeoPreview

Place code in `src/modules/previewpane/PhotoGeoPreview/`.

### 3. Register in `preview_handlers.json`

Add to `installer/PowerToysSetup/preview_handlers.json`:

```json
{
  "id": "{YOUR-GUID}",
  "name": "PhotoGeoPreview",
  "extensions": [".jpg", ".jpeg", ".png", ".heic"],
  "clsid": "{YOUR-GUID}"
}
```

### 4. Build PowerToys

```bash
# Open in Visual Studio 2026
start PowerToys.sln

# Or build via command line
.\build.cmd
```

## Technical Details

### UI Structure (HTML)
```html
<div class="split-container">
  <div class="image-pane">
    <img src="{IMAGE_PATH}">
  </div>
  <div class="splitter"></div>
  <div class="map-pane"></div>
</div>
```

### EXIF Extraction (C++ + WIC)
```cpp
IWICImagingFactory* factory;
IWICBitmapDecoder* decoder;
IWICMetadataQueryReader* reader;
// Extract GPS coordinates
```

## Reference Implementations
- `src/modules/previewpane/MarkdownPreviewHandler/` (C++)
- `src/modules/previewpane/SvgPreviewHandler/` (C++)

## License

Follows PowerToys license (MIT License).

## Contact

[GitHub Issues](https://github.com/scottlz0310/PhotoGeoPreviewPane/issues)
