using AtmCashDispenser.Api.Endpoints;

namespace AtmCashDispenser.Api.Extensions
{
    public static class EndpointRouteBuilderExtensions
    {
        public static void MapEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapDispenseCash();
        }
    }
}
