# custom-addinrevit — TUAN Dimension Below (Revit 2026)

Add-in Revit 2026 với **1 lệnh duy nhất**: điền text vào phần **Below** của dimension bằng cách click liên tiếp.

## Tính năng
- Tạo tab ribbon **`TUAN`** → panel *Dimension Tools* → button **`Set Below Text`**.
- Click button → hộp thoại nhập text → **Apply**.
- Sau đó click liên tiếp vào từng **dimension**, mỗi lần click sẽ điền text vừa nhập vào phần **Below**.
- Nhấn **ESC** để dừng.

## Yêu cầu
- **Revit 2026** (chạy trên .NET 8).
- Build từ source: **.NET SDK 8+**, có Revit 2026 cài tại `C:\Program Files\Autodesk\Revit 2026`.

## Cài đặt nhanh (dùng bản build sẵn trong `dist/`)
```powershell
powershell -ExecutionPolicy Bypass -File .\install.ps1
```
Script copy `TuanDimensionBelow.addin` + `TuanDimensionBelow.dll` vào:
```
%AppData%\Autodesk\Revit\Addins\2026\
```
Khởi động lại Revit 2026 → tab **TUAN**.

### Gỡ cài đặt
```powershell
powershell -ExecutionPolicy Bypass -File .\install.ps1 -Uninstall
```

## Build lại từ source
```powershell
dotnet build -c Release
```
File `.dll` xuất ra `bin\Release\TuanDimensionBelow.dll`.

## Cấu trúc
| File | Vai trò |
|------|---------|
| `Source.cs` | ExternalApplication (ribbon) + ExternalCommand (logic) + WinForms dialog |
| `TuanDimensionBelow.csproj` | Project net8.0-windows, reference RevitAPI/RevitAPIUI |
| `TuanDimensionBelow.addin` | Manifest, assembly path tương đối |
| `install.ps1` | Script cài / gỡ vào thư mục Addins\2026 |
| `dist/` | Bản build sẵn (.dll + .addin) để cài ngay |

## Ghi chú
- Với dimension nhiều đoạn (multi-segment), text được gán cho **mọi segment**.
- Chỉ ảnh hưởng phần chú thích (Below), **không đổi hình học** mô hình.
