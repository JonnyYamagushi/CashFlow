using FluentAssertions;
using System.Net;
using System.Net.Mime;
using Xunit;

namespace WebApi.Test.Expenses;

public class GenerateExpensesReportTest : CashFlowClassFixture
{
    private const string METHOD = "api/report";

    private readonly string _adminToken;
    private readonly string _teamMemberToken;
    private readonly DateTime _expenseDate;

    public GenerateExpensesReportTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _adminToken = factory.User_Admin.GetToken();
        _teamMemberToken = factory.User_Team_Member.GetToken();
        _expenseDate = factory.Expense_Admin.GetDate();
    }

    [Fact]
    public async Task Success_Pdf()
    {
        var result = await DoGet(requestUri: $"{METHOD}/pdf?month={_expenseDate:yyyy-MM-01}", token: _adminToken);

        result.StatusCode.Should().Be(HttpStatusCode.OK);

        result.Content.Headers.ContentType.Should().NotBeNull();
        result.Content.Headers.ContentType!.MediaType.Should().Be(MediaTypeNames.Application.Pdf);
    }

    [Fact]
    public async Task Success_Excel()
    {
        var result = await DoGet(requestUri: $"{METHOD}/excel?month={_expenseDate:yyyy-MM-01}", token: _adminToken);

        result.StatusCode.Should().Be(HttpStatusCode.OK);

        result.Content.Headers.ContentType.Should().NotBeNull();
        result.Content.Headers.ContentType!.MediaType.Should().Be(MediaTypeNames.Application.Octet);
    }

    [Fact]
    public async Task Erro_Forbidden_User_Not_Allowed_Excel()
    {
        var result = await DoGet(requestUri: $"{METHOD}/excel?month={_expenseDate:Y}", token: _teamMemberToken);

        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Erro_Forbidden_User_Not_Allowed_Pdf()
    {
        var result = await DoGet(requestUri: $"{METHOD}/pdf?month={_expenseDate:Y}", token: _teamMemberToken);

        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
