using Newtonsoft.Json;

class Program
{
    private static readonly HttpClient httpClient = new HttpClient();

    static async Task Main(string[] args)
    {
        string apiKey = "4xCl+xjXRLnKPYDfgymZRw==Y3nxQGKuhGSLiRX9";
        string city = null;
        string country = null;

        while (string.IsNullOrEmpty(city) || string.IsNullOrEmpty(country))
        {
            Console.WriteLine("City: ");
            city = Console.ReadLine();
            Console.WriteLine("Country: ");
            country = Console.ReadLine();
        }

        try
        {
            (string latitude, string longitude) = await GetGeocodingData(apiKey, city, country);
            string temp = await GetWeatherData(latitude, longitude);
            Console.WriteLine($"The temperature in {city} is {temp} degrees Celcius");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static async Task<(string, string)> GetGeocodingData(string apiKey, string city, string country)
    {
        try
        {
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("X-Api-Key", apiKey);

            string apiUrl = $"https://api.api-ninjas.com/v1/geocoding?city={Uri.EscapeDataString(city)}&country={Uri.EscapeDataString(country)}";

            HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(responseBody);

            string latitude = jsonData[0].latitude;
            string longitude = jsonData[0].longitude;

            return (latitude, longitude);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to retrieve geocoding data: {ex.Message}");
        }
    }

    static async Task<string> GetWeatherData(string latitude, string longitude)
    {
        try
        {
            string apiUrl = $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&hourly=temperature_2m&current_weather=true";

            HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(responseBody);

            return jsonData.current_weather.temperature;



        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to retrieve weather data: {ex.Message}");
        }
    }
}
