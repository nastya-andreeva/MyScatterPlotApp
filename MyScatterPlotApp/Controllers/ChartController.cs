using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using MyScatterPlotApp.Data;
using MyScatterPlotApp.Models;

[Authorize]
public class ChartController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;

    public ChartController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public IActionResult Index() => View();

    [HttpPost]
    public async Task<IActionResult> SaveData(string xValues, string yValues)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var chartData = new ChartData
        {
            XValues = xValues,
            YValues = yValues,
            UserId = userId
        };
        _context.ChartDatas.Add(chartData);
        await _context.SaveChangesAsync();

        // Вызов PHP скрипта для генерации диаграммы
        var client = _httpClientFactory.CreateClient();
        var phpUrl = "http://localhost/php/generate_chart.php"; // Убедитесь, что путь правильный

        var payload = new
        {
            chartId = chartData.Id,
            xValues = xValues,
            yValues = yValues
        };
        var content = new StringContent(JsonConvert.SerializeObject(payload), System.Text.Encoding.UTF8, "application/json");
        var response = await client.PostAsync(phpUrl, content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);
            if (result.ContainsKey("status") && result["status"] == "success")
            {
                chartData.ChartImagePath = result["imagePath"];
                await _context.SaveChangesAsync();
            }
        }

        return RedirectToAction("History");
    }

    [HttpGet]
    public async Task<IActionResult> History()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var charts = await _context.ChartDatas
                                   .Where(c => c.UserId == userId)
                                   .ToListAsync();
        return View(charts);
    }
}
