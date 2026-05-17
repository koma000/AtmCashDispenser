using AtmCashDispenser.Api.Requests;
using AtmCashDispenser.Api.Responses;
using AtmCashDispenser.Application.DispenseCash;

namespace AtmCashDispenser.Api.Endpoints
{
    public static class DispenseCashEndPoint
    {
        public static void MapDispenseCash(this IEndpointRouteBuilder app)
        {
            app.MapPost("/dispense", (
                DispenseRequest request,
                DispenseCashUseCase useCase) =>
            {
                if (request.Amount <= 0)
                {
                    return Results.BadRequest("出金金額は1円以上にしてください");
                }

                var result = useCase.Execute(request.Amount);

                return result switch
                {
                    DispenseUseCaseResult.Success success => Results.Ok(new DispenseResponse(
                        success.DispenseDetails.Select(kvp => new DispenseItem(kvp.Key, kvp.Value))
                        .ToList()
                    )),
                    DispenseUseCaseResult.Failure failure => Results.BadRequest(failure.Reason),
                    _ => Results.StatusCode(500)
                };
            });
        }
    }
}
