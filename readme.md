# Auto_Parking

T?ng quan
- `Auto_Parking` là ?ng d?ng Windows Forms ?? qu?n lý bãi ?? xe t? ??ng.
- Ch?c n?ng chính: ch?p và nh?n di?n bi?n s? (camera + EmguCV), OCR b?ng Tesseract, tích h?p th? RFID/Serial v?i STM/Arduino, báo tr?ng thái c?m bi?n, l?u l?ch s? vào c? s? d? li?u và th?ng kê doanh thu.

Tính n?ng
- Ghi nh?n t? hai camera (vào / ra).
- Phát hi?n bi?n s? b?ng Haar cascade + x? lý contour.
- Nh?n d?ng ký t? b?ng Tesseract (`TesseractProcessor`).
- X? lý th? RFID qua hai c?ng Serial (`STM1_Serial`, `STM2_Serial`).
- Báo cáo tr?ng thái ô ?? (sensor) và x? lý c?nh báo cháy.
- L?u/??c l?ch s?, ??m l??t vào/ra và t?ng doanh thu qua `clsCommon`/`DLDatabase`.
- Giao di?n h? tr? xem ?nh và import ?nh ?? test.

Các file quan tr?ng
- `MainFormtest.cs` — UI chính và lu?ng nghi?p v? (camera, serial, nh?n di?n bi?n, quy t?c vào/ra).
- `MainFormtest.Designer.cs`, `MainFormtest.resx` — tài nguyên WinForms.
- `Program.cs` — entry point.
- `ImageForm` — form h? tr? xem ?nh và k?t qu? nh?n d?ng.
- `clsCommon.cs`, `DLDatabase.cs` — helper thao tác v?i c? s? d? li?u và nghi?p v?.
- `WEBCAM` — l?p ti?n ích webcam.
- Th? m?c `\data\` — ch?a `testData` và file cascade (ví d? `output-hv-33-x25.xml`).

Yêu c?u tr??c khi ch?y
- Visual Studio (h? tr? .NET Framework 4.8).
- Target framework: .NET Framework 4.8.
- NuGet packages: `Emgu.CV`, `AForge.Video`, `AForge.Video.DirectShow`, và wrapper Tesseract t??ng thích.
- Hai thi?t b? camera (ho?c ?i?u ch?nh code n?u ch? có 1).
- (Tùy ch?n) Thi?t b? STM/Arduino ho?c gi? l?p c?ng Serial n?u mu?n test ph?n RFID.
- SQL Server Express (ho?c s?a connection string trong `clsCommon` / `DLDatabase`).

T?p tin d? li?u b?t bu?c
- `\data\testData\` — d? li?u ngôn ng? c?a Tesseract.
- `\output-hv-33-x25.xml` — Haar cascade s? d?ng cho phát hi?n bi?n s?.

C?u hình
- C?u hình COM/baud ??c t? `XINIFILE.ReadValue("COM_STM1")`, `XINIFILE.ReadValue("COM_STM2")`, `XINIFILE.ReadValue("BAURATE")` — ch?nh s?a `XINIFILE` ho?c gán c?ng cho th? nghi?m.
- `GetCameraInfor()` m?c ??nh ch?n `filterInfo[0]` và `filterInfo[1]` — ??m b?o có ít nh?t hai thi?t b? video ho?c thêm ki?m tra.
- Chu?i k?t n?i m?c ??nh trong `clsCommon`/`DLDatabase`: `Data Source=.\SQLEXPRESS;Initial Catalog=Haui_SmartParking;Integrated Security=True` — c?p nh?t n?u c?n.

H??ng d?n build & ch?y nhanh
1. M? solution trong Visual Studio.
2. Khôi ph?c NuGet packages.
3. Ki?m tra th? m?c `\data\` có `testData` và cascade xml.
4. Build solution.
5. Ch?y ?ng d?ng (F5 ho?c Start Without Debugging).

G?i ý test
- N?u không có thi?t b? Serial th?t, dùng gi? l?p c?ng Serial ?? g?i các chu?i nh?: `i_<RFID>...x`, `o_<RFID>...x`, `sXXXXXx`, `f_1x` (k?t thúc b?ng ký t? `x`).
- N?u không có camera, dùng ch?c n?ng `Ch?n ?nh` ?? m? ?nh m?u và test nh?n d?ng.
- Dùng c?a s? `ImageForm` (IF) ?? xem k?t qu? trung gian và t?ng ký t? nh?n d?ng.

Kh?c ph?c s? c? th??ng g?p
- L?i "Không tìm th?y thông tin camera": ki?m tra driver camera và quy?n truy c?p.
- Tesseract initialization failed: ??m b?o `testData` có m?t trong `\data\` và phiên b?n t??ng thích.
- L?i DB: ki?m tra SQL Server instance, chu?i k?t n?i và schema.
- L?i c?ng Serial: ki?m tra tên COM và baud rate, th? gi? l?p n?u c?n.

B?o m?t & l?u ý
- ?ng d?ng dùng `Integrated Security=True` cho SQL; thay ??i c?u hình n?u c?n b?o m?t.
- M?t s? x? lý ?nh và I/O th?c hi?n trên lu?ng UI — có th? gây treo giao di?n n?u x? lý n?ng; cân nh?c tách sang lu?ng n?n.

Nên ki?m tra ti?p
- `clsCommon` / `DLDatabase` ?? bi?t schema DB và stored procedures c?n thi?t.
- `XINIFILE` ?? bi?t cách c?u hình COM và tham s? runtime.
- N?i dung th? m?c `\data\` (testData + cascade xml).

Mu?n mình làm ti?p gì?
- Mình có th? thêm file này vào repo (?ã th?c hi?n), ho?c t?o script ki?m tra prerequisites (PowerShell).