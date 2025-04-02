#  Website Bán Hàng

##  Giới thiệu
Đây là dự án website bán hàng được phát triển trong môn **Lập trình Web**. Dự án xây dựng hệ thống thương mại điện tử cơ bản với các tính năng như quản lý sản phẩm, giỏ hàng, thanh toán và quản lý đơn hàng.

##  Công nghệ sử dụng
- **Front-end**: HTML, CSS, JavaScript, Bootstrap
- **Back-end**: ASP.NET MVC5
- **Cơ sở dữ liệu**: SQL Server
- **ORM**: Entity Framework

##  Tính năng chính
-  **Trang chủ**: Hiển thị danh sách sản phẩm
-  **Tìm kiếm**: Tìm sản phẩm theo tên, danh mục
-  **Giỏ hàng**: Thêm/xóa sản phẩm, tính tổng tiền
-  **Thanh toán**: Hỗ trợ thanh toán đơn hàng
-  **Quản lý đơn hàng**: Theo dõi trạng thái đơn hàng
-  **Quản lý tài khoản**: Đăng ký, đăng nhập, quyền admin

## 📂 Cấu trúc thư mục
```
Lab04.WebsiteBanhang/
│-- Controllers/      # Xử lý logic
│-- Models/           # Định nghĩa dữ liệu
│-- Views/            # Giao diện người dùng
│-- wwwroot/          # Tài nguyên tĩnh (CSS, JS, Images)
│-- appsettings.json  # Cấu hình ứng dụng
│-- Startup.cs        # Cấu hình chính
```

##  Hướng dẫn chạy dự án
### 1️ Clone repository
```sh
git clone https://github.com/tranlongvu-02-10/Web-Programming-Lab.git
cd Web-Programming-Lab/Lab04.WebsiteBanhang
```

### 2️ Cấu hình cơ sở dữ liệu
- Mở **SQL Server** và tạo database `BanHangDB`
- Chạy lệnh **Entity Framework Migrations**:
  ```sh
  update-database
  ```

### 3️ Chạy ứng dụng
```sh
dotnet run
```
Mở trình duyệt và truy cập: **http://localhost:5000**

##  Ghi chú
- Nếu gặp lỗi kết nối DB, kiểm tra lại chuỗi kết nối trong `appsettings.json`
- Nếu chưa cài **Entity Framework**, chạy lệnh:
  ```sh
  dotnet tool install --global dotnet-ef
  ```

##  Liên hệ
📧 Email: tranlongvu02102004@gmail.com  
🌐 GitHub: [Your GitHub](https://github.com/tranlongvu-02-10)



