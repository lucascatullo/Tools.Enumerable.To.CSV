

using System.Text;
using Tools.Enumerable.To.CSV.Model;

namespace Tools.Enumerable.To.CSV.Core;

public class CsvWritter
{
    private readonly StringBuilder _stringBuilder = new();
    public MemoryStream WriteAsMemoryStream(IEnumerable<object> csvObject) 
    {
        var response = new MemoryStream();

        var streamWritter = new StreamWriter(response);

        streamWritter.Write(ConvertCsvString(csvObject));
        streamWritter.Flush();
        response.Position = 0;
        return response; 
    }


    private string ConvertCsvString(IEnumerable<object> csvObject)
    {
        var objectCells = csvObject.Select(CsvCell.ExtractLines);

        WriteHeaders(objectCells.First());
        foreach (var cells in objectCells)
            WriteValues(cells);

        return _stringBuilder.ToString();
    }

    private void WriteValues(IEnumerable<CsvCell> cells)
    {
        string _tempLine = string.Empty;
        foreach(var value in cells.Select(c => c.Value))
        {
            _tempLine += value + ";"; 
        }

        _stringBuilder.AppendLine(_tempLine);
    }

    private void WriteHeaders(IEnumerable<CsvCell> cells)
    {
        string _tempLine = string.Empty;

        foreach (var header in GetHeadersName(cells))
        {
            _tempLine += header + ";";
        }

        _stringBuilder.AppendLine(_tempLine);
    }

    private IEnumerable<string> GetHeadersName(IEnumerable<CsvCell> allCells) => allCells.Select(cell => cell.Name);
}
