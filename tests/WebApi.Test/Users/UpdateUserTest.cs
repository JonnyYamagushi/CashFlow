using CashFlow.Exception;
using CommonTestUtilities.Requests;
using FluentAssertions;
using System.Globalization;
using System.Text.Json;
using WebApi.Test.InlineData;
using Xunit;

namespace WebApi.Test.Users;

public class UpdateUserTest : CashFlowClassFixture
{
    private const string METHOD = "api/user";

    private readonly string _token;

    public UpdateUserTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _token = factory.User_Team_Member.GetToken();
    }

    [Fact]
    public async Task Success()
    {
        var request = RequestUpdateUserJsonBuilder.Build();

        var response = await DoPut(METHOD, request, _token);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Empty_Name(string culture)
    {
        var request = RequestUpdateUserJsonBuilder.Build();
        request.Name = string.Empty;

        var result = await DoPut(METHOD, request, _token, culture);

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        var bodyResponse = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(bodyResponse);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        var expenctedMessage = ResourceErrorMessages.ResourceManager.GetString("NAME_EMPTY", new CultureInfo(culture));

        errors.Should().HaveCount(1).And.Contain(error => error.GetString()!.Equals(expenctedMessage));
    }
}