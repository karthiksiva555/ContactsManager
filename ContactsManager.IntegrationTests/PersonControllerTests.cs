using Fizzler.Systems.HtmlAgilityPack;
using FluentAssertions;
using HtmlAgilityPack;
using Xunit;

namespace ContactsManager.IntegrationTests;

public class PersonControllerTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Index_Person_Test()
    {
        var response = await _client.GetAsync("/person/index");

        response.Should().BeSuccessful();
        var responseBody = await response.Content.ReadAsStringAsync();
        
        // Create a document object and load the responseBody Html into it
        var html = new HtmlDocument();
        html.LoadHtml(responseBody);
        var document = html.DocumentNode;

        document.QuerySelectorAll("table.persons").Should().NotBeNull();
    }
}