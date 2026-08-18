using PharmacySpa.Api.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSingleton<MedicineRepository>();
builder.Services.AddSingleton<MedicineService>();
builder.Services.AddCors(options => options.AddPolicy("Spa", policy => policy
    .AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors("Spa");
app.MapControllers();
app.MapFallbackToFile("index.html");
app.Run();

public partial class Program { }
