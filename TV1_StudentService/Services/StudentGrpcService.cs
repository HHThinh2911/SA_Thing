using Grpc.Core;
using StudentService;

public class StudentGrpcService : StudentGrpc.StudentGrpcBase
{
    private readonly AppDbContext _db;

    public StudentGrpcService(AppDbContext db)
    {
        _db = db;
    }

    public override Task<StudentResponse> GetStudent(StudentRequest request, ServerCallContext context)
    {
        var s = _db.Students.Find(request.Id);

        if (s == null)
            return Task.FromResult(new StudentResponse());

        return Task.FromResult(new StudentResponse
        {
            Id = s.Id,
            Name = s.Name,
            Email = s.Email,
            Age = s.Age
        });
    }
}