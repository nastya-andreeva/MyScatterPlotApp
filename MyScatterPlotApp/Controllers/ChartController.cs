using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyScatterPlotApp.Data;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

[Authorize]
public class ChartController : Controller
{
    private readonly ApplicationDbContext _context;

    public ChartController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index() => View();

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file, string title)
    {
        if (file == null || file.Length == 0)
        {
            return Json(new { status = "error", message = "Файл не был загружен." });
        }

        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        string filePath = Path.Combine(uploadsFolder, file.FileName);

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        List<float> xValues = new List<float>();
        List<float> yValues = new List<float>();

        var fileExtension = Path.GetExtension(file.FileName).ToLower();

        if (fileExtension == ".csv")
        {
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    var values = line.Split(',');

                    if (values.Length >= 2 && float.TryParse(values[0], out float x) && float.TryParse(values[1], out float y))
                    {
                        xValues.Add(x);
                        yValues.Add(y);
                    }
                }
            }
        }
        else if (fileExtension == ".xlsx" || fileExtension == ".xls")
        {
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 1; row <= rowCount; row++)
                {
                    if (float.TryParse(worksheet.Cells[row, 1].Text, out float x) && float.TryParse(worksheet.Cells[row, 2].Text, out float y))
                    {
                        xValues.Add(x);
                        yValues.Add(y);
                    }
                }
            }
        }
        else if (fileExtension == ".json")
        {
            using (var reader = new StreamReader(filePath))
            {
                var jsonString = await reader.ReadToEndAsync();
                try
                {
                    var jsonData = JsonSerializer.Deserialize<List<JsonData>>(jsonString);

                    if (jsonData != null)
                    {
                        foreach (var data in jsonData)
                        {
                            xValues.Add(data.x);
                            yValues.Add(data.y);
                        }
                    }
                }
                catch (JsonException)
                {
                    return Json(new { status = "error", message = "Неверный формат JSON." });
                }
            }
        }
        else
        {
            return Json(new { status = "error", message = "Неподдерживаемый формат файла." });
        }

        // Теперь добавим логику сохранения данных в базу

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Получаем ID пользователя

        // Генерация пути к изображению (можно использовать реальный путь к картинке, если генерируется изображение)
        string chartImagePath = Path.Combine("/images/charts", $"{Path.GetFileNameWithoutExtension(file.FileName)}.png");

        // Создаем новый объект ChartData для сохранения в базу данных
        var chartData = new ChartData
        {
            UserId = userId,
            XValues = JsonSerializer.Serialize(xValues), // Сохраняем X и Y как JSON
            YValues = JsonSerializer.Serialize(yValues),
            ChartImagePath = chartImagePath,
            CreatedAt = DateTime.UtcNow
        };

        // Сохраняем в БД
        _context.ChartDatas.Add(chartData);
        await _context.SaveChangesAsync();

        return Json(new { status = "success", xValues = xValues, yValues = yValues });
    }

    public class JsonData
    {
        public float x { get; set; }
        public float y { get; set; }
    }

    [HttpGet]
    public async Task<IActionResult> History()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var charts = await _context.ChartDatas
            .Where(c => c.UserId == userId)
            .ToListAsync();
        foreach (var chart in charts)
        {
            // Создаем JSON-данные на основе XValues и YValues
            var data = new List<JsonData>();

            // Десериализуем XValues и YValues
            var xValues = JsonSerializer.Deserialize<List<float>>(chart.XValues);
            var yValues = JsonSerializer.Deserialize<List<float>>(chart.YValues);

            for (int i = 0; i < xValues.Count; i++)
            {
                data.Add(new JsonData { x = xValues[i], y = yValues[i] });
            }

            // Генерируем путь к JSON-файлу
            string jsonFileName = $"chart_data_{chart.Id}.json";
            string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", jsonFileName);

            // Записываем данные в JSON-файл
            await System.IO.File.WriteAllTextAsync(jsonFilePath, JsonSerializer.Serialize(data));

            // Сохраняем путь к JSON-файлу в объекте ChartData для отображения
            chart.ChartImagePath = $"/uploads/{jsonFileName}";
        }

        return View(charts);
    }

    public class Request
    {
        public int Id { get; set; }
    }

    [HttpPost]
    public async Task<IActionResult> Delete([FromBody] Request request)
    {
        var chartData = await _context.ChartDatas.FindAsync(request.Id);

        if (chartData == null)
        {
            return Json(new { status = "error", message = "Диаграмма не найдена." });
        }

        _context.ChartDatas.Remove(chartData);
        await _context.SaveChangesAsync();

        return Json(new { status = "success", message = "Диаграмма удалена." });
    }


}
