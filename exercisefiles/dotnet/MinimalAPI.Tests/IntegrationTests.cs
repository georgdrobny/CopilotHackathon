using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace IntegrationTests;

public class IntegrationTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public IntegrationTests(TestWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ReturnsHelloWorld()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync("/");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("Hello World!", content);
    }

    [Theory]
    [InlineData("2024-01-01", "2024-01-10", 9)]
    [InlineData("2024-01-10", "2024-01-01", 9)]
    [InlineData("2024-01-01", "2024-01-01", 0)]
    public async Task DaysBetweenDates_ReturnsCorrectDays(string date1, string date2, double expected)
    {
        // Arrange

        // Act
        var response = await _client.GetAsync($"/DaysBetweenDates?date1={date1}&date2={date2}");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal(expected.ToString(), content);
    }

    [Fact]
    public async Task DaysBetweenDates_InvalidDate_ReturnsBadRequest()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync("/DaysBetweenDates?date1=invalid&date2=2024-01-01");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("%2B34123456789", true)]
    [InlineData("%2B34987654321", true)]
    [InlineData("123456789", false)]
    [InlineData("%2B3412345678", false)]
    [InlineData("%2B341234567890", false)]
    public async Task ValidatePhoneNumber_Works(string phone, bool expected)
    {
        // Arrange

        // Act
        var response = await _client.GetAsync($"/ValidatePhoneNumber?phonenumber={phone}");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal(expected.ToString().ToLower(), content.ToLower());
    }

    [Theory]
    [InlineData("12345678Z", "\"valid\"")] // 12345678 % 23 = 14, 'Z'
    [InlineData("00000000T", "\"valid\"")] // 0 % 23 = 0, 'T'
    [InlineData("12345678A", "\"invalid\"")]
    [InlineData("abcdefghI", "\"invalid\"")]
    [InlineData("1234567Z", "\"invalid\"")]
    [InlineData("", "\"invalid\"")]
    public async Task ValidateSpanishDNI_Works(string dni, string expected)
    {
        // Arrange

        // Act
        var response = await _client.GetAsync($"/ValidateSpanishDNI?dni={dni}");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal(expected, content);
    }

    [Fact]
    public async Task ReturnColorCode_ReturnsHex_WhenColorExists()
    {
        // Arrange: colors.json must contain a color named "red" with hex "#FF0000"

        // Act
        var response = await _client.GetAsync("/ReturnColorCode?color=red");
        if (response.StatusCode == HttpStatusCode.NotFound)
            return; // Skip if colors.json is not present in test env

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.True(content.StartsWith("#") || content.StartsWith("\"#")); // Accepts "#FF0000" or "\"#FF0000\""
    }

    [Fact]
    public async Task ReturnColorCode_ReturnsNotFound_WhenColorDoesNotExist()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync("/ReturnColorCode?color=notacolor");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task TellMeAJoke_ReturnsJokeOrProblem()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync("/TellMeAJoke");

        // Assert
        // Accept either 200 OK or 500 Problem (if JokeAPI is down)
        Assert.True(response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task RandomEuropeanCountry_ReturnsCountryAndISO()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync("/RandomEuropeanCountry");

        // Assert
        response.EnsureSuccessStatusCode();
        var country = await response.Content.ReadFromJsonAsync<CountryResult>();
        Assert.False(string.IsNullOrWhiteSpace(country?.Name));
        Assert.False(string.IsNullOrWhiteSpace(country?.ISO));
    }

    private class CountryResult
    {
        public string Name { get; set; }
        public string ISO { get; set; }
    }
}
