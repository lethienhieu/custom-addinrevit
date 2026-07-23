# Handoff — TUAN Dimension Below add-in (Revit 2026)

**Thời điểm:** 2026-07-23 16:08

## Mục tiêu phiên
Viết add-in Revit 2026 (1 command duy nhất): tab ribbon `TUAN` → button `Set Below Text`.
Click button → nhập text → Apply → click liên tiếp vào từng dimension để điền text vào phần **Below**,
ESC để dừng. Đóng gói `.addin` + `.dll`, kèm `install.ps1` cài vào `%AppData%\...\Addins\2026`.
Push lên repo GitHub **public** mới `custom-addinrevit`.

## Đã xong
- `Source.cs`: `App` (IExternalApplication, tạo tab TUAN + button) + `SetBelowCommand`
  (IExternalCommand) + `DimensionSelectionFilter` + `TextInputForm` (WinForms dialog).
  Loop `PickObject` bắt `OperationCanceledException` để dừng khi ESC. Set `dim.Below` (multi-segment
  thì set mọi segment). Đã alias Form/TextBox/Label/Button/TaskDialog để tránh trùng tên Revit API.
- `TuanDimensionBelow.csproj`: net8.0-windows, UseWindowsForms, reference RevitAPI/RevitAPIUI
  (Private=false) từ `C:\Program Files\Autodesk\Revit 2026`.
- `TuanDimensionBelow.addin`: manifest, assembly path tương đối, AddInId cố định.
- `install.ps1`: cài/gỡ (`-Uninstall`), tìm file trong `dist/` rồi copy vào Addins\2026.
- `dist/`: chứa bản build sẵn `.dll` + `.addin`.
- `README.md`, `.gitignore`.
- **Build Release THÀNH CÔNG** → `bin/Release/TuanDimensionBelow.dll` (đã copy vào dist).

## Cập nhật 16:13
- User đã **test trong Revit 2026 → chạy thành công**.
- Đổi **toàn bộ text hiển thị sang tiếng Anh** (tooltip, dialog label/title, pick prompt,
  TaskDialog kết quả, message "No active document"). Rebuild OK, cập nhật `dist/`.
- Push lần 2 lên repo public `custom-addinrevit`.

## Đã push
- Repo public: https://github.com/lethienhieu/custom-addinrevit (branch master).

## Lỗi / blocker hiện tại
- Không có. (2 lỗi build ban đầu do trùng tên `Form`/`TextBox`/`TaskDialog` đã fix bằng alias.)

## Bước tiếp theo
1. Push xong → user chạy `install.ps1` → mở Revit 2026 → test tab TUAN.
2. Nếu `dim.Below` báo lỗi với 1 số dimension (vd đã override "Replace With Text"): xử lý thêm.

## File / lệnh quan trọng
- Thư mục dự án: `E:\THBIM-CODE\2025\CODE\ATUAN\custom-addinrevit`
- Build: `dotnet build -c Release`
- Cài: `powershell -ExecutionPolicy Bypass -File .\install.ps1`

## Ràng buộc cần nhớ
- `~/.claude/CLAUDE.md`: gh CLI ở `C:\Users\Le_ThienHieu\tools\gh\bin\gh.exe` (không trên PATH),
  account `lethienhieu`. KHÔNG push/release khi chưa xin phép từng lần (lần này user ĐÃ cho phép).
- Surgical changes, simplicity first.
