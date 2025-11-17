using CashFlow.Communication.Requests;
using CashFlow.Exception;
using CommonTestUtilities.Requests;
using FluentAssertions;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;
using Xunit;

namespace WebApi.Test.Login;

public class DoLoginTest : CashFlowClassFixture
{
    private const string METHOD = "api/login";

    private readonly string _email;
    private readonly string _name;
    private readonly string _password;

    public DoLoginTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _email = factory.User_Team_Member.GetEmail();
        _name = factory.User_Team_Member.GetName();
        _password = factory.User_Team_Member.GetPassword();
    }

    [Fact]
    public async Task Success()
    {
        var request = new RequestLoginJson
        {
            Email = _email,
            Password = _password
        };

        var result = await DoPost(METHOD, request);

        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var bodyResponse = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(bodyResponse);

        response.RootElement.GetProperty("name").GetString().Should().Be(_name);
        response.RootElement.GetProperty("token").GetString().Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Login_Invalid(string cultureInfo)
    {
        var request = RequestLoginJsonBuilder.Build();

        var result = await DoPost(
            requestUri: METHOD,
            request: request,
            culture: cultureInfo
        );

        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var bodyResponse = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(bodyResponse);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        var expenctedMessage = ResourceErrorMessages.ResourceManager.GetString("EMAIL_OR_PASSWORD_INVALID", new CultureInfo(cultureInfo));

        errors.Should().HaveCount(1).And.Contain(error => error.GetString()!.Equals(expenctedMessage));
    }
}
