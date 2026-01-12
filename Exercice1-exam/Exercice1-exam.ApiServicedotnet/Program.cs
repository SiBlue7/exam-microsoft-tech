using Exercice1_exam.ApiServicedotnet.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IStockService, StockService>();
builder.AddServiceDefaults();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapDefaultEndpoints();
app.MapControllers();
app.Run();