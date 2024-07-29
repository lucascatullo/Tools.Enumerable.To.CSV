

using System.Text;
using Tools.Enumerable.To.CSV.Model;

namespace Tools.Enumerable.To.CSV.Core;

public class CsvWritter : ICsvWritter
{
    private readonly StringBuilder _stringBuilder = new();


    /// <summary>
    /// Writes a CSV object into a memory stream.
    /// </summary>
    /// <param name="csvObject">Target object</param>
    /// <returns>Memory strream of the csv object</returns>
    public MemoryStream WriteAsMemoryStream(IEnumerable<object> csvObject)
    {
        var response = new MemoryStream();

        var streamWritter = new StreamWriter(response);

        streamWritter.Write(ConvertCsvString(csvObject));
        streamWritter.Flush();
        response.Position = 0;
        return response;
    }

    /// <summary>
    /// Converts the CSV objects into a CSV formated String with separation ','
    /// </summary>
    /// <param name="csvObject">Target object</param>
    /// <returns>Full string in CSV format</returns>
    private string ConvertCsvString(IEnumerable<object> csvObject)
    {
        var objectCells = csvObject.Select(CsvCell.ExtractLines);

        WriteHeaders(objectCells.First());
        foreach (var cells in objectCells)
            WriteValues(cells);
        var temp = _stringBuilder.ToString();
        return _stringBuilder.ToString();
    }

    /// <summary>
    /// Write all cells values into string using ; as separator.
    /// </summary>
    /// <param name="cells">Cells of the Object</param>
    private void WriteValues(IEnumerable<CsvCell> cells)
    {
        string _tempLine = string.Empty;
        foreach (var value in cells.Select(c => c.Value))
        {
            _tempLine += value + ";";
        }

        _stringBuilder.AppendLine(_tempLine);
    }


    /// <summary>
    /// Write the headers of the object. Using ; as separator.
    /// </summary>
    /// <param name="cells">All cells of the object.</param>
    private void WriteHeaders(IEnumerable<CsvCell> cells)
    {
        string _tempLine = string.Empty;

        foreach (var header in GetHeadersName(cells))
        {
            _tempLine += header + ";";
        }

        _stringBuilder.AppendLine(_tempLine);
    }

    /// <summary>
    /// Selects all names in the cells.
    /// </summary>
    /// <param name="allCells">Cells of the object</param>
    /// <returns>Array of string with cell names.</returns>
    private IEnumerable<string> GetHeadersName(IEnumerable<CsvCell> allCells) => allCells.Select(cell => cell.Name);
}
