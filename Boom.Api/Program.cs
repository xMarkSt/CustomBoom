using Boom.Business.MappingProfiles;
using Boom.Business.Services;
using Boom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string? connectionString = builder.Configuration.GetConnectionString("BoomConnection");
builder.Services.AddDbContext<BoomDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

builder.Services.AddScoped(typeof(IRepository), typeof(Repository<BoomDbContext>));
builder.Services.AddScoped<ITournamentService, TournamentService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddSingleton<IPlistSerializationService, PlistSerializationService>();
builder.Services.AddSingleton<IEncryptionService, EncryptionService>();
builder.Services.AddAutoMapper(typeof(TournamentGroupProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();