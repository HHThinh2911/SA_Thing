using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/students")]
public class StudentController : ControllerBase
{
    private readonly AppDbContext _db;

    public StudentController(AppDbContext db)
    {
        _db = db;
    }

    // LẤY DANH SÁCH (Hiện người mới nhất lên đầu)
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_db.Students.OrderByDescending(s => s.Id).ToList());
    }

    // THÊM MỚI
    [HttpPost]
    public IActionResult Create(Student s)
    {
        _db.Students.Add(s);
        _db.SaveChanges();
        return Ok(s);
    }

    // SỬA THÔNG TIN
    [HttpPut("{id}")]
    public IActionResult Update(int id, Student s)
    {
        var sv = _db.Students.Find(id);
        if (sv == null) return NotFound();

        sv.Name = s.Name;
        sv.Email = s.Email;
        _db.SaveChanges();
        return Ok(sv);
    }

    // XÓA
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var sv = _db.Students.Find(id);
        if (sv == null) return NotFound();

        _db.Students.Remove(sv);
        _db.SaveChanges();
        return Ok();
    }
}