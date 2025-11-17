using CashFlow.Communication.Requests;
using FluentAssertions;
using System.Net;
using Xunit;

namespace WebApi.Test.Users;

public class DeleteUserTest : CashFlowClassFixture
{
    private const string METHOD = "api/user";

    private readonly string _token;
    private readonly string _email;
    private readonly string _password;

    public DeleteUserTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _token = factory.User_Team_Member.GetToken();
        _email = factory.User_Team_Member.GetEmail();
        _password = factory.User_Team_Member.GetPassword();
    }

    [Fact]
    public async Task Success()
    {
        var result = await DoDelete(METHOD, _token);

        result.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var request = new RequestLoginJson
        {
            Email = _email,
            Password = _password
        };

        result = await DoPost("api/login", request);

        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
