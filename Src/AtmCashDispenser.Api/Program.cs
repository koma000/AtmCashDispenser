using AtmCashDispenser.Api.Extensions;
using AtmCashDispenser.Application.DispenseCash;
using AtmCashDispenser.Domain.Dispensing;
using AtmCashDispenser.Domain.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<CashInventory>(sp =>
{
    // èâä˙ç›å…Çê›íË
    var initialInventory = new Dictionary<Denomination, int>
    {
        { Denomination.TenThousand, 10 },
        { Denomination.FiveThousand, 20 },
        { Denomination.TwoThousand, 10 },
        { Denomination.Thousand, 50 },
        { Denomination.FiveHundred, 100 },
        { Denomination.Hundred, 200 },
        { Denomination.Fifty, 100 },
        { Denomination.Ten, 100 },
        { Denomination.Five, 100 },
        { Denomination.One, 100 }
    };
    return new CashInventory(initialInventory);
});
builder.Services.AddSingleton<ICashDispenseCalculator, CashDispenseCalculator>();
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