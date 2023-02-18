using IdentityModel.Client;

Console.WriteLine("Merchant in town...");

// Find out who is the mighty authority,
// i.e., discover endpoints from metadata
var client = new HttpClient();
var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
if (disco.IsError)
{
    Console.WriteLine(disco.Error);
    return;
}

// Request a ticket to the farmland,
// i.e., request for access token
var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
{
    Address = disco.TokenEndpoint,

    ClientId = "merchant",
    ClientSecret = "511536EF-F270-4058-80CA-1C89C192F69A",
    Scope = "farmlandwebapi"
});

if (tokenResponse.IsError)
{
    Console.WriteLine(tokenResponse.Error);
    return;
}

Console.WriteLine($"Merchant got a ticket to the farmland: {tokenResponse.AccessToken}");


// Visit the farmland using the ticket
// i.e., call the web API
var apiClient = new HttpClient();
apiClient.SetBearerToken(tokenResponse.AccessToken);

var response = await apiClient.GetAsync("https://localhost:6001/WeatherForecast");
if (!response.IsSuccessStatusCode)
{
    Console.WriteLine(response.StatusCode);
}
else
{
    string farmlandResource = await response.Content.ReadAsStringAsync();
    Console.WriteLine($"Merchant got resource from the farmland: {farmlandResource}");
}
