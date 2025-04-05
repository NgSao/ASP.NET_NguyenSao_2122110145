using Microsoft.AspNetCore.Mvc;

namespace NguyenSao_2122110145.Controllers;
[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
    // Dữ liệu giả lập về dự báo thời tiết
    private static readonly string[] Summaries = new[]
    {
        "Chilly", "Hot", "Warm", "Mild", "Freezing"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
