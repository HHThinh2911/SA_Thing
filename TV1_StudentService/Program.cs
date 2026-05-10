using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. THÊM CORS Ở ĐÂY (Cho phép Frontend gọi API mà không bị chặn)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("Default"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Default"))));

// API + gRPC
builder.Services.AddControllers();
builder.Services.AddGrpc();

// THÊM DÒNG NÀY CHO GIAO DIỆN C# (Khai báo sử dụng Razor Pages)
builder.Services.AddRazorPages(); 
// Đăng ký gRPC Client gọi sang cổng 5167 của NotificationService
builder.Services.AddGrpcClient<StudentService.Protos.Notification.NotificationClient>(o =>
{
    o.Address = new Uri("http://localhost:5167");
});
var app = builder.Build();

// 2. KÍCH HOẠT CORS Ở ĐÂY (Bắt buộc phải đặt trước MapControllers)
app.UseCors("AllowAll");

app.MapControllers();
app.MapGrpcService<StudentGrpcService>();

// THÊM DÒNG NÀY CHO GIAO DIỆN C# (Mở đường dẫn cho các trang HTML)
app.MapRazorPages(); 

app.Run();