using CashFlow.Communication.Enums;
using FluentAssertions;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;
using Xunit;

namespace WebApi.Test.Expenses;

public class GetExpenseByIdTest : CashFlowClassFixture
{
    private const string METHOD = "api/expenses";

    private readonly string _token;
    private readonly long _expenseId;

    public GetExpenseByIdTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _token = factory.User_Team_Member.GetToken();
        _expenseId = factory.Expense_MemberTeam.GetId();
    }

    [Fact]
    public async Task Success()
    {
        var result = await DoGet($"{METHOD}/{_expenseId}", _token);

        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await result.Content.ReadAsStreamAsync();

        var response = JsonDocument.Parse(body);

        response.RootElement.GetProperty("id").GetInt64().Should().Be(_expenseId);
        response.RootElement.GetProperty("title").GetString().Should().NotBeNullOrWhiteSpace();
        response.RootElement.GetProperty("description").GetString().Should().NotBeNullOrWhiteSpace();
        response.RootElement.GetProperty("date").GetDateTime().Should().NotBeAfter(DateTime.Today);
        response.RootElement.GetProperty("amount").GetDecimal().Should().BeGreaterThan(0);
        response.RootElement.GetProperty("tags").EnumerateArray().Should().NotBeNullOrEmpty();

        var paymentType = response.RootElement.GetProperty("paymentType").GetInt32();
        Enum.IsDefined(typeof(PaymentType), paymentType).Should().BeTrue();
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_expense_Not_Found(string culture)
    {
        
    }
}