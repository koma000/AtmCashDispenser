using AtmCashDispenser.Api.Requests;
using AtmCashDispenser.Api.Responses;
using AtmCashDispenser.Application.DispenseCash;

namespace AtmCashDispenser.Api.Endpoints
{
    public static class DispenseCashEndPoint
    {
        public static void MapDispenseCash(this IEndpointRouteBuilder app)
        {
            app.MapPost("/transactions/dispense", Handle);
        }

        private static IResult Handle(
            DispenseRequest request, 
            DispenseCashUseCase useCase)
        {
            var result = useCase.Execute(request.Amount);

            return result switch
            {
                DispenseUseCaseResult.Success success => Results.Ok(new DispenseResponse(
                    success.TransactionId.Value,
                    success.DispenseDetails
                    .Select(x => new DispenseItem(x.Denomination, x.Count))
                    .ToList()
                )),
                DispenseUseCaseResult.Failure failure => Results.BadRequest(new ErrorResponse(
                    failure.Error.Code,
                    failure.Error.Message
                )),
                _ => Results.StatusCode(500)
            };
        }
    }
}
