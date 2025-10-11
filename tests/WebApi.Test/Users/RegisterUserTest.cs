using CashFlow.Exception;
using CommonTestUtilities.Requests;
using FluentAssertions;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;
using Xunit;

namespace WebApi.Test.Users;

public class RegisterUserTest : CashFlowClassFixture
{
    private const string METHOD = "api/user";

    public RegisterUserTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Success()
    {
        var request = RequestRegisterUserJsonBuilder.Build();

        var result = await DoPost(METHOD, request);

        result.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var bodyResponse = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(bodyResponse);

        response.RootElement.GetProperty("name").GetString().Should().Be(request.Name);
        response.RootElement.GetProperty("token").GetString().Should().NotBeNullOrEmpty();
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Empty_Name(string cultureInfo)
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Name = string.Empty;

        var result = await DoPost(
            requestUri: METHOD, 
            request: request, 
            culture: cultureInfo
        );

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var bodyResponse = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(bodyResponse);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        var expenctedMessage = ResourceErrorMessages.ResourceManager.GetString("NAME_EMPTY", new CultureInfo(cultureInfo));

        errors.Should().HaveCount(1).And.Contain(error => error.GetString()!.Equals(expenctedMessage));
    }
}
