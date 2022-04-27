using Microsoft.EntityFrameworkCore;

namespace MinimalApi;

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {

    }

    public DbSet<ToDo> ToDos => Set<ToDo>();
}

