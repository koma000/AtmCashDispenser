using AtmCashDispenser.Api.Requests;
using AtmCashDispenser.Api.Responses;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;


namespace AtmCashDispenser.Api.Tests
{
    public class DispenseCashEndPointTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public DispenseCashEndPointTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Post_Dispense_正常な金額の場合_200OKと正しいJSONが返ること()
        {
            // Arrange
            var request = new DispenseRequest(16000);

            // Act
            var response = await _client.PostAsJsonAsync("/dispense", request);

            // Assert: ① HTTPステータスコードが200 OKであること
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert: ② レスポンスのJSONが正しく復元でき、中身が期待通りであること
            var content = await response.Content.ReadFromJsonAsync<DispenseResponse>();
            Assert.NotNull(content);
            var actual = content.Items
                .OrderByDescending(i => i.Denomination)
                .Select(i => (i.Denomination, i.Count));

            var expected = new[]
            {
                (10000, 1),
                (5000, 1),
                (1000, 1)
            };
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task Post_Dispense_払い出し不可能な金額の場合_400BadRequestが返ること()
        {
            // Arrange
            var request = new DispenseRequest(12345);

            // Act
            var response = await _client.PostAsJsonAsync("/dispense", request);

            // Assert: ① HTTPステータスコードが400 Bad Requestであること
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // Assert: ② エラーメッセージが期待通りであること
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("払い出し不可能な金額です", content);
        }
    }
}
