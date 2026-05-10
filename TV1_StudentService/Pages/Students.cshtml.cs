using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentService.Protos; // THÊM DÒNG NÀY ĐỂ LẤY CẤU TRÚC TIN NHẮN gRPC

namespace StudentService.Pages;

public class StudentsModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly Notification.NotificationClient _grpcClient; // THÊM BIẾN gRPC CLIENT

    // TIÊM CẢ DB VÀ gRPC VÀO ĐÂY
    public StudentsModel(AppDbContext db, Notification.NotificationClient grpcClient)
    {
        _db = db;
        _grpcClient = grpcClient;
    }

    public List<Student> StudentList { get; set; } = new();

    [BindProperty]
    public Student InputStudent { get; set; } = new();

    public void OnGet(int? editId)
    {
        if (editId.HasValue)
        {
            var sv = _db.Students.Find(editId.Value);
            if (sv != null) InputStudent = sv;
        }

        StudentList = _db.Students.OrderByDescending(s => s.Id).Take(50).ToList();
    }

    // ĐỔI THÀNH async Task ĐỂ CHỜ GỬI TIN NHẮN gRPC
    public async Task<IActionResult> OnPostSave()
    {
        string logMsg = ""; // Biến chứa câu chữ để gửi đi

        if (InputStudent.Id == 0)
        {
            _db.Students.Add(InputStudent);
            logMsg = $"Hệ thống vừa THÊM MỚI sinh viên: {InputStudent.Name}";
        }
        else
        {
            _db.Students.Update(InputStudent);
            logMsg = $"Hệ thống vừa CẬP NHẬT thông tin sinh viên ID {InputStudent.Id}";
        }
        
        _db.SaveChanges(); // Lưu vào DB của Student trước

        // BẮN THÔNG BÁO SANG NOTIFICATION SERVICE
        try 
        {
            await _grpcClient.SendLogAsync(new LogRequest { Message = logMsg });
        } 
        catch 
        { 
            // Nếu NotificationService lỡ bị tắt thì kệ nó, web vẫn không bị lỗi sập
        }

        return RedirectToPage("/Students"); 
    }

    // ĐỔI THÀNH async Task
    public async Task<IActionResult> OnPostDelete(int id)
    {
        var sv = _db.Students.Find(id);
        if (sv != null)
        {
            string logMsg = $"Hệ thống vừa XÓA sinh viên: {sv.Name}";
            
            _db.Students.Remove(sv);
            _db.SaveChanges();

            // BẮN THÔNG BÁO SANG NOTIFICATION SERVICE
            try 
            {
                await _grpcClient.SendLogAsync(new LogRequest { Message = logMsg });
            } 
            catch { }
        }
        return RedirectToPage("/Students");
    }
}