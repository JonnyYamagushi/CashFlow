using CashFlow.Communication.Requests;
using CashFlow.Exception;
using CommonTestUtilities.Requests;
using FluentAssertions;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using WebApi.Test.InlineData;
using Xunit;

namespace WebApi.Test.Login;

public class DoLoginTest : IClassFixture<CustomWebApplicationFactory>
{
    private const string METHOD = "api/login";

    private readonly HttpClient _httpClient;
    private readonly string _email;
    private readonly string _name;
    private readonly string _password;

    public DoLoginTest(CustomWebApplicationFactory factory)
    {
        _httpClient = factory.CreateClient();
        _email = factory.GetUserEmail();
        _name = factory.GetUserName();
        _password = factory.GetUserPassword();
    }

    [Fact]
    public async Task Success()
    {
        var request = new RequestLoginJson
        {
            Email = _email,
            Password = _password
        };

        var result = await _httpClient.PostAsJsonAsync(METHOD, request);

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

        _httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(cultureInfo));

        var result = await _httpClient.PostAsJsonAsync(METHOD, request);

        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var bodyResponse = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(bodyResponse);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        var expenctedMessage = ResourceErrorMessages.ResourceManager.GetString("EMAIL_OR_PASSWORD_INVALID", new CultureInfo(cultureInfo));

        errors.Should().HaveCount(1).And.Contain(error => error.GetString()!.Equals(expenctedMessage));
    }
}
