using Microsoft.EntityFrameworkCore;
using MinimalApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TestDbContext>(opt => opt.UseInMemoryDatabase("TestDb"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/todoitems/{id}", async (Guid id, TestDbContext context) =>
{
    var todo = await context.ToDos.FindAsync(id);

    return todo is not null ? Results.Ok(todo) : Results.NotFound();
});
app.MapGet("/todoitems", async (TestDbContext context) => await context.ToDos.ToListAsync() );
app.MapPost("/todoitems", async (ToDo toDo, TestDbContext context) =>
{
    context.ToDos.Add(toDo);
    await context.SaveChangesAsync();

    return Results.Created($"/todoitems/{toDo.Id}", toDo);
});
app.MapPut("/todoitems/{id}", async (Guid id, ToDo newToDo, TestDbContext context) =>
{
    var todo = await context.ToDos.FindAsync(id);

    if (todo is null) return Results.NotFound();

    todo.Name = newToDo.Name;
    todo.IsComplete = newToDo.IsComplete;

    await context.SaveChangesAsync();

    return Results.NoContent();
});
app.MapDelete("/todoitems/{id}", async (Guid id, TestDbContext context) =>
{
    if (await context.ToDos.FindAsync(id) is ToDo todo)
    {
        context.ToDos.Remove(todo);
        await context.SaveChangesAsync();
        return Results.Ok(todo);
    }

    return Results.NotFound();
});

app.Run();


