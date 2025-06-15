/*
 * Program.cs
 * ----------
 * Main entry point for the MinimalAPI project.
 * 
 * This file configures and runs a minimal ASP.NET Core Web API with several endpoints:
 * 
 * Endpoints:
 *   GET /
 *     - Returns "Hello World!".
 * 
 *   GET /DaysBetweenDates?date1={date1}&date2={date2}
 *     - Returns the absolute number of days between two date strings.
 * 
 *   GET /ValidatePhoneNumber?phonenumber={phonenumber}
 *     - Validates if the given phone number matches the Spanish format (+34 followed by 9 digits).
 * 
 *   GET /ValidateSpanishDNI?dni={dni}
 *     - Validates a Spanish DNI (8 digits + 1 letter). Returns "valid" or "invalid".
 * 
 *   GET /ReturnColorCode?color={color}
 *     - Reads colors.json and returns the HEX code for the specified color name.
 * 
 *   GET /TellMeAJoke
 *     - Fetches and returns a random joke from the JokeAPI.
 * 
 *   GET /RandomEuropeanCountry
 *     - Returns a random European country and its ISO code.
 * 
 * Usage:
 *   - Run the application and use the above endpoints via browser or HTTP client.
 *   - Swagger UI is enabled in development mode for easy testing.
 * 
 * Dependencies:
 *   - Requires colors.json file in the application base directory for /ReturnColorCode.
 *   - Uses MinimalAPI.Color and MinimalAPI.ColorCode classes for color deserialization.
 * 
 * -------------------------------------------------------------------------------
 */

using Color = MinimalAPI.Color;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ADD NEW ENDPOINTS HERE
app.MapGet("/", () => "Hello World!");
app.MapGet("/DaysBetweenDates", (string date1, string date2) =>
{
    if (!DateTime.TryParse(date1, out var d1) || !DateTime.TryParse(date2, out var d2))
    {
        return Results.BadRequest("Invalid date format. Please use a valid date string.");
    }
    var daysBetween = Math.Abs((d2 - d1).TotalDays);
    return Results.Ok(daysBetween);
});
app.MapGet("/ValidatePhoneNumber", (string phonenumber) =>
{
    // Spanish phone number format: +34 followed by 9 digits
    var regex = new System.Text.RegularExpressions.Regex(@"^\+34\d{9}$");
    bool isValid = regex.IsMatch(phonenumber);
    return Results.Ok(isValid);
});
app.MapGet("/ValidateSpanishDNI", (string dni) =>
{
    // Spanish DNI: 8 digits + 1 letter (e.g., 12345678Z)
    if (string.IsNullOrWhiteSpace(dni) || dni.Length != 9)
        return Results.Ok("invalid");

    var numberPart = dni.Substring(0, 8);
    var letterPart = dni.Substring(8, 1).ToUpper();

    if (!int.TryParse(numberPart, out int dniNumber))
        return Results.Ok("invalid");

    string letters = "TRWAGMYFPDXBNJZSQVHLCKE";
    char correctLetter = letters[dniNumber % 23];

    if (letterPart == correctLetter.ToString())
        return Results.Ok("valid");
    else
        return Results.Ok("invalid");
});
app.MapGet("/ReturnColorCode", async (string color) =>
{
    var filePath = Path.Combine(AppContext.BaseDirectory, "colors.json");
    if (!System.IO.File.Exists(filePath))
        return Results.NotFound("colors.json file not found.");

    var json = await System.IO.File.ReadAllTextAsync(filePath);
    var colors = System.Text.Json.JsonSerializer.Deserialize<List<Color>>(json);

    if (colors == null)
        return Results.NotFound("No colors found.");

    var found = colors.FirstOrDefault(c => string.Equals(c.Name, color, StringComparison.OrdinalIgnoreCase));
    if (found == null)
        return Results.NotFound("Color not found.");

    return Results.Ok(found.Code.HEX);
});
app.MapGet("/TellMeAJoke", async () =>
{
    using (var httpClient = new HttpClient())
    {
        var response = await httpClient.GetAsync("https://v2.jokeapi.dev/joke/Any?type=single");
        if (!response.IsSuccessStatusCode)
            return Results.Problem("Failed to fetch a joke.");

        var json = await response.Content.ReadAsStringAsync();
        using var doc = System.Text.Json.JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (root.TryGetProperty("joke", out var jokeElement))
            return Results.Ok(jokeElement.GetString());

        return Results.Problem("No joke found.");
    }
});
app.MapGet("/RandomEuropeanCountry", () =>
{
    var countries = new[]
    {
        new { Name = "Spain", ISO = "ES" },
        new { Name = "France", ISO = "FR" },
        new { Name = "Germany", ISO = "DE" },
        new { Name = "Italy", ISO = "IT" },
        new { Name = "Portugal", ISO = "PT" },
        new { Name = "Netherlands", ISO = "NL" },
        new { Name = "Belgium", ISO = "BE" },
        new { Name = "Sweden", ISO = "SE" },
        new { Name = "Norway", ISO = "NO" },
        new { Name = "Denmark", ISO = "DK" },
        new { Name = "Finland", ISO = "FI" },
        new { Name = "Poland", ISO = "PL" },
        new { Name = "Austria", ISO = "AT" },
        new { Name = "Switzerland", ISO = "CH" },
        new { Name = "Greece", ISO = "GR" },
        new { Name = "Ireland", ISO = "IE" },
        new { Name = "Czech Republic", ISO = "CZ" },
        new { Name = "Hungary", ISO = "HU" },
        new { Name = "Romania", ISO = "RO" },
        new { Name = "Bulgaria", ISO = "BG" }
    };

    var random = new Random();
    var country = countries[random.Next(countries.Length)];
    return Results.Ok(country);
});

// END OF NEW ENDPOINTS
app.Run();

// Needed to be able to access this type from the MinimalAPI.Tests project.
public partial class Program
{ }


