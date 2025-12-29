using AssetShareLib;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDb"));

builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<MachineRepository>();
builder.Services.AddScoped<ListingRepository>();
builder.Services.AddScoped<BookingRepository>();

builder.Services.AddControllers();



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();
app.UseDeveloperExceptionPage();


app.UseSwagger();
app.UseSwaggerUI();

<<<<<<< HEAD
// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

=======
>>>>>>> Test-branch

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
