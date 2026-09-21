using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AICareerHub.API.Tests.Integration
{
    public class CustomWebApplicationFactory
        : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(
            IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.UseSetting(
                "AI:Provider",
                "Mock");

            builder.UseSetting(
                "Jwt:Key",
                "IntegrationTestJwtKeyThatIsLongEnough123456789");

            builder.UseSetting(
                "Jwt:Issuer",
                "AICareerHub");

            builder.UseSetting(
                "Jwt:Audience",
                "AICareerHubUsers");

            builder.UseSetting(
                "Jwt:ExpiryMinutes",
                "60");
        }
    }
}