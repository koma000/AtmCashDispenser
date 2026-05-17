using AtmCashDispenser.Api.Extensions;
using AtmCashDispenser.Application.DispenseCash;
using AtmCashDispenser.Domain.Dispensing;
using AtmCashDispenser.Domain.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<CashInventory>(sp =>
{
    // ‰ŠúİŒÉ‚ğİ’è
    var initialInventory = new Dictionary<Denomination, int>
    {
        { Denomination.TenThousand, 10 },
        { Denomination.FiveThousand, 20 },
        { Denomination.Thousand, 50 },
        { Denomination.FiveHundred, 100 },
        { Denomination.Hundred, 200 }
    };
    return new CashInventory(initialInventory);
});
builder.Services.AddTransient<DispenseCashUseCase>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapEndpoints();

app.Run();

public partial class Program { }