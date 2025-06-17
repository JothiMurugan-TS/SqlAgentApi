var builder = WebApplication.CreateBuilder(args);

// ✅ Configure services BEFORE builder.Build()
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ CORS Configuration (before build)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ✅ Middleware setup
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll"); // Use the CORS policy

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
