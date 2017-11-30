using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Life.Tests
{
    public class AuthorizationTests : IntegrationTests
    {
        static readonly Guid Id = Guid.NewGuid();

        [Fact]
        public Task ShouldReturnAuthorizedForInvalidCredentials() =>
            TestAsync(UseNothing, async http =>
            {
                var response = await http.GetAsync("/life/Nothing");
                Assert.Equal(StatusCodes.Status401Unauthorized, (int)response.StatusCode);
            });

        [Fact]
        public Task ShouldReturnOKForValidCredentials() =>
            TestAsync(UseNothing, async http =>
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "/life/Nothing");
                request.Headers.Add("Authorization", Id.ToString());
                var response = await http.SendAsync(request);
                Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
            });


        static ILifeBuilder AuthorizeSome(Guid id, ILifeBuilder options)
        {
            options.AuthorizeDetails = async ctx =>
            {
                await Task.CompletedTask;
                var headers = ctx.Request.Headers;
                if (headers.TryGetValue("Authorization", out var authorization))
                    return Guid.TryParse(authorization, out var guid) && guid == id;
                return false;
            };
            return options;
        }

        class Nothing { }
        static Task<ComponentStatus> NothingFunc(Nothing _) =>
            Task.FromResult(ComponentStatus.Up("Nothing"));
        static void UseNothing(ILifeBuilder options) =>
            AuthorizeSome(Id, options).AddEvaluator<Nothing>("Nothing", NothingFunc).Services.AddSingleton<Nothing>();
    }
}
