using FluentAssertions;
using System.Net;
using System.Text.Json;
using Xunit;

namespace WebApi.Test.Users;

public class GetUserProfileTest : CashFlowClassFixture
{
    private const string METHOD = "api/user";

    private readonly string _token;
    private readonly string _userName;
    private readonly string _userEmail;

    public GetUserProfileTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _token = factory.User_Team_Member.GetToken();
        _userName = factory.User_Team_Member.GetName();
        _userEmail = factory.User_Team_Member.GetEmail();
    }

    [Fact]
    public async Task Success()
    {
        var result = await DoGet(METHOD, _token);

        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(body);

        response.RootElement.GetProperty("name").GetString().Should().Be(_userName);
        response.RootElement.GetProperty("email").GetString().Should().Be(_userEmail);
    }
}
