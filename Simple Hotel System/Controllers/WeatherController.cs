using Microsoft.AspNetCore.Mvc;
using Simple_Hotel_System.Models;

namespace Simple_Hotel_System.Controllers
{
    public class WeatherController : Controller
    {
        private readonly HttpClient _httpClient;
        public WeatherController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<IActionResult> Weather()
        {
            var data = await _httpClient.GetFromJsonAsync<List<WeatherForecastInfo>>(
                "https://localhost:7049/WeatherForecast");

            return View(data);
        }

        public async Task<IActionResult> Refresh()
        {
            var data = await _httpClient.GetFromJsonAsync<List<WeatherForecastInfo>>(
                "https://localhost:7049/WeatherForecast");

            return PartialView("_WeatherPartial", data);
        }

    }
}
