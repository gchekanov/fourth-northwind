using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NorthwindDemo.API.Middleware;
using NorthwindDemo.Application.Interfaces;
using NorthwindDemo.Application.Models;
using NorthwindDemo.Application.Services;
using NorthwindDemo.Application.Validators;
using NorthwindDemo.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<NorthwindDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddTransient<IValidator<CustomerQuery>, CustomerQueryValidator>();

builder.Services.AddCors(o =>
{
    o.AddPolicy("mvc", p =>
        p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddResponseCaching();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseCors("mvc");
app.UseResponseCaching();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "NorthwindDemo API V1");
    });
}

app.MapControllers();

app.Run();