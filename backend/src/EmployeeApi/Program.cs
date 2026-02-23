using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(opts =>
{
    opts.SerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<EmployeeDbContext>(opt =>
    opt.UseInMemoryDatabase("EmployeeDb"));

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var api = app.MapGroup("/api/v1/employees");

api.MapGet("", async (IEmployeeRepository repo, int page = 1, int pageSize = 20) =>
{
    if (page <= 0 || pageSize <= 0) return Results.BadRequest(new { error = "page and pageSize must be positive" });
    var (items, total) = await repo.GetPagedAsync(page, pageSize);
    return Results.Ok(new { items, page, pageSize, total });
});

api.MapGet("/{id:guid}", async (IEmployeeRepository repo, Guid id) =>
{
    var emp = await repo.GetByIdAsync(id);
    return emp is null ? Results.NotFound(new { error = "Employee not found" }) : Results.Ok(emp);
});

api.MapPost("", async (IEmployeeRepository repo, EmployeeCreateDto dto) =>
{
    var validation = dto.Validate();
    if (validation is not null) return Results.BadRequest(validation);

    if (await repo.ExistsByEmailAsync(dto.Email))
        return Results.Conflict(new { error = "Email already exists" });

    var created = await repo.CreateAsync(dto);
    return Results.Created($"/api/v1/employees/{created.Id}", created);
});

api.MapPut("/{id:guid}", async (IEmployeeRepository repo, Guid id, EmployeeUpdateDto dto) =>
{
    if (id != dto.Id) return Results.BadRequest(new { error = "Id in path and body must match" });
    var validation = dto.Validate();
    if (validation is not null) return Results.BadRequest(validation);

    var exists = await repo.GetByIdAsync(id);
    if (exists is null) return Results.NotFound(new { error = "Employee not found" });

    if (await repo.ExistsByEmailForOtherAsync(dto.Email, id))
        return Results.Conflict(new { error = "Email already exists" });

    var updated = await repo.UpdateAsync(dto);
    return Results.Ok(updated);
});

api.MapDelete("/{id:guid}", async (IEmployeeRepository repo, Guid id) =>
{
    var exists = await repo.GetByIdAsync(id);
    if (exists is null) return Results.NotFound(new { error = "Employee not found" });

    await repo.DeleteAsync(id);
    return Results.NoContent();
});

app.Run();

// ------------------ Models, DTOs, DbContext, Repository ------------------

public class Employee
{
    public Guid Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public decimal Salary { get; set; }
    public DateTime DateOfJoining { get; set; }
}

public record EmployeeReadDto(Guid Id, string FirstName, string LastName, string Email, decimal Salary, DateTime DateOfJoining);

public class EmployeeCreateDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public decimal? Salary { get; set; }
    public DateTime? DateOfJoining { get; set; }

    public object? Validate()
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(FirstName)) errors[nameof(FirstName)] = new[] { "FirstName is required" };
        if (string.IsNullOrWhiteSpace(LastName)) errors[nameof(LastName)] = new[] { "LastName is required" };
        if (string.IsNullOrWhiteSpace(Email)) errors[nameof(Email)] = new[] { "Email is required" };
        else if (!new EmailAddressAttribute().IsValid(Email)) errors[nameof(Email)] = new[] { "Email must be a valid email address" };
        if (Salary is null) errors[nameof(Salary)] = new[] { "Salary is required" };
        else if (Salary < 0) errors[nameof(Salary)] = new[] { "Salary must be >= 0" };
        if (DateOfJoining is null) errors[nameof(DateOfJoining)] = new[] { "DateOfJoining is required" };
        else if (DateOfJoining > DateTime.UtcNow) errors[nameof(DateOfJoining)] = new[] { "DateOfJoining cannot be in the future" };

        return errors.Count == 0 ? null : new { type = "https://example.com/probs/validation", title = "Validation Failed", status = 400, errors };
    }
}

public class EmployeeUpdateDto : EmployeeCreateDto
{
    public Guid Id { get; set; }
}

public class EmployeeDbContext : DbContext
{
    public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : base(options) { }
    public DbSet<Employee> Employees => Set<Employee>();
}

public interface IEmployeeRepository
{
    Task<(IEnumerable<EmployeeReadDto> items, int total)> GetPagedAsync(int page, int pageSize);
    Task<EmployeeReadDto?> GetByIdAsync(Guid id);
    Task<EmployeeReadDto> CreateAsync(EmployeeCreateDto dto);
    Task<EmployeeReadDto> UpdateAsync(EmployeeUpdateDto dto);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByEmailForOtherAsync(string email, Guid id);
}

public class EmployeeRepository : IEmployeeRepository
{
    private readonly EmployeeDbContext _db;
    public EmployeeRepository(EmployeeDbContext db) { _db = db; }

    public async Task<(IEnumerable<EmployeeReadDto> items, int total)> GetPagedAsync(int page, int pageSize)
    {
        var total = await _db.Employees.CountAsync();
        var items = await _db.Employees
            .OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new EmployeeReadDto(e.Id, e.FirstName, e.LastName, e.Email, e.Salary, e.DateOfJoining))
            .ToListAsync();
        return (items, total);
    }

    public async Task<EmployeeReadDto?> GetByIdAsync(Guid id)
    {
        var e = await _db.Employees.FindAsync(id);
        return e is null ? null : new EmployeeReadDto(e.Id, e.FirstName, e.LastName, e.Email, e.Salary, e.DateOfJoining);
    }

    public async Task<EmployeeReadDto> CreateAsync(EmployeeCreateDto dto)
    {
        var e = new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName!.Trim(),
            LastName = dto.LastName!.Trim(),
            Email = dto.Email!.Trim(),
            Salary = dto.Salary!.Value,
            DateOfJoining = dto.DateOfJoining!.Value
        };
        _db.Employees.Add(e);
        await _db.SaveChangesAsync();
        return new EmployeeReadDto(e.Id, e.FirstName, e.LastName, e.Email, e.Salary, e.DateOfJoining);
    }

    public async Task<EmployeeReadDto> UpdateAsync(EmployeeUpdateDto dto)
    {
        var e = await _db.Employees.FindAsync(dto.Id);
        if (e is null) throw new InvalidOperationException("Employee not found");
        e.FirstName = dto.FirstName!.Trim();
        e.LastName = dto.LastName!.Trim();
        e.Email = dto.Email!.Trim();
        e.Salary = dto.Salary!.Value;
        e.DateOfJoining = dto.DateOfJoining!.Value;
        await _db.SaveChangesAsync();
        return new EmployeeReadDto(e.Id, e.FirstName, e.LastName, e.Email, e.Salary, e.DateOfJoining);
    }

    public async Task DeleteAsync(Guid id)
    {
        var e = await _db.Employees.FindAsync(id);
        if (e is null) return;
        _db.Employees.Remove(e);
        await _db.SaveChangesAsync();
    }

    public Task<bool> ExistsByEmailAsync(string email)
        => _db.Employees.AnyAsync(e => e.Email.ToLower() == email.ToLower());

    public Task<bool> ExistsByEmailForOtherAsync(string email, Guid id)
        => _db.Employees.AnyAsync(e => e.Email.ToLower() == email.ToLower() && e.Id != id);
}
