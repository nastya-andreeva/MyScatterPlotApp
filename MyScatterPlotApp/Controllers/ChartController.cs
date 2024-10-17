using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

[Authorize]
public class ChartController : Controller
{
    [HttpGet]
    public IActionResult Index() => View();

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
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
        else
        {
            return Json(new { status = "error", message = "Неподдерживаемый формат файла." });
        }

        // Возвращаем JSON с данными для диаграммы
        return Json(new { status = "success", xValues = xValues, yValues = yValues });
    }
}
