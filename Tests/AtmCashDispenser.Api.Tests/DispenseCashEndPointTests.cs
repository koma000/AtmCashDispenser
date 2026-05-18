using AtmCashDispenser.Api.Requests;
using AtmCashDispenser.Api.Responses;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;


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
        public async Task Post_Dispense_払い出し可能な金額の場合_200OKと正しいJSONが返ること()
        {
            // Arrange
            var request = new DispenseRequest(16000);

            // Act
            var response = await _client.PostAsJsonAsync("/transactions/dispense", request);

            // Assert: ① HTTPステータスコードが200 OKであること
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert: ② レスポンスのJSONが正しく復元でき、中身が期待通りであること
            var content = await response.Content.ReadFromJsonAsync<DispenseResponse>();
            Assert.NotNull(content);
            Assert.NotNull(content.Items);
            Assert.NotEmpty(content.Items);
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

        [Theory]
        [InlineData(1)]
        [InlineData(100000)]
        public async Task Post_Dispense_払い出し可能な金額の場合_境界値_200OKと正しいJSONが返ること(int amount)
        {
            // Arrange
            var request = new DispenseRequest(amount);

            // Act
            var response = await _client.PostAsJsonAsync("/transactions/dispense", request);
            
            // Assert: ① HTTPステータスコードが200 OKであること
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            // Assert: ② レスポンスのJSONが正しく復元でき、中身が期待通りであること
            var content = await response.Content.ReadFromJsonAsync<DispenseResponse>();
            Assert.NotNull(content);
            Assert.NotNull(content.Items);
            Assert.NotEmpty(content.Items);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Post_Dispense_金額が0以下の場合_400BadRequestが返ること(int amount)
        {
            // Arrange
            var request = new DispenseRequest(amount);

            // Act
            var response = await _client.PostAsJsonAsync("/transactions/dispense", request);

            // Assert: ① HTTPステータスコードが400 Bad Requestであること
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // Assert: ② エラーメッセージが期待通りであること
            var content = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            Assert.NotNull(content);
            Assert.Equal("INVALID_AMOUNT", content.Code);
            Assert.Equal("金額は1以上", content.Message);
        }

        [Fact]
        public async Task Post_Dispense_金額が10万より大きい場合_400BadRequestが返ること()
        {
            // Arrange
            var request = new DispenseRequest(100001);

            // Act
            var response = await _client.PostAsJsonAsync("/transactions/dispense", request);

            // Assert: ① HTTPステータスコードが400 Bad Requestであること
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // Assert: ② エラーメッセージが期待通りであること
            var content = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            Assert.NotNull(content);
            Assert.Equal("LIMIT_EXCEEDS", content.Code);
            Assert.Equal("上限超過", content.Message);
        }

        [Fact]
        public async Task Post_Dispense_払い出し不可な金額の場合_400BadRequestが返ること()
        {
            // Arrange
            var request = new DispenseRequest(12345);

            // Act
            var response = await _client.PostAsJsonAsync("/transactions/dispense", request);

            // Assert: ① HTTPステータスコードが400 Bad Requestであること
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // Assert: ② エラーメッセージが期待通りであること
            var content = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            Assert.NotNull(content);
            Assert.Equal("NOT_DISPENSABLE", content.Code);
            Assert.Equal("払い出し不可", content.Message);
        }
    }
}
