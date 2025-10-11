using FluentAssertions;
using System.Net;
using System.Text.Json;
using Xunit;

namespace WebApi.Test.Expenses;

public class GetAllExpensesTest : CashFlowClassFixture
{
    private const string METHOD = "api/expenses";

    private readonly string _token;

    public GetAllExpensesTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _token = factory.User_Team_Member.GetToken();
    }

    [Fact]
    public async Task Success()
    {
        var result = await DoGet(requestUri: METHOD, token: _token);

        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(body);

        response.RootElement.GetProperty("expenses").EnumerateArray().Should().NotBeNullOrEmpty();
    }
}
