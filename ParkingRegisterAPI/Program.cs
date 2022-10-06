using ParkingRegisterAPI;
using ParkingRegisterAPI.ParkingRegistry;

var builder = WebApplication.CreateBuilder(args);

LotSizeInfo.SetupLots(builder.Configuration.GetValue<int>("ParkingLotSize"));
builder.Services.AddSingleton<ISlotRegistry, SlotRegistry>();
builder.Services.AddSingleton<ICarRegistry, CarRegistry>();
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseRateLimiting();

app.MapControllers();

app.Run();
