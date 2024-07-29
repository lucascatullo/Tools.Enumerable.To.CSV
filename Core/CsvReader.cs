

using Microsoft.AspNetCore.Http;
using System.Data;
using System.Reflection;
using Tools.Enumerable.To.CSV.Attribute;
using Tools.Enumerable.To.CSV.Extension;
using Tools.Enumerable.To.CSV.Model;

namespace Tools.Enumerable.To.CSV.Core;

public class CsvReader : ICsvReader
{

    private readonly string[] _csvLinesString;


    /// <summary>
    /// Create a new instance of CSV reader.
    /// </summary>
    /// <param name="file">File must be a CSV, and the separator must be ';'</param>
    /// <exception cref="ArgumentException">If the file is not CSV</exception>
    public CsvReader(IFormFile file)
    {
        if (file.ContentType.ToLower() != "text/csv")
            throw new ArgumentException("file should have contentType 'text/csv'");

        _csvLinesString = ReadFileAsStringArray(file);
    }


    /// <summary>
    /// Converts the CSV file to string.
    /// </summary>
    /// <param name="file">CSV file.</param>
    /// <returns>First line of the array is the headers of the csv, and the following lines are the values.</returns>
    private string[] ReadFileAsStringArray(IFormFile file)
    {
        string[] response = [];

        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            while (reader.Peek() >= 0)
                response.Append(reader.ReadLine());
        }

        return response;
    }

    /// <summary>
    /// Reads the CSV file and cast it as T.
    /// </summary>
    /// <typeparam name="T">Output class</typeparam>
    /// <returns>List of the readed values on the CSV</returns>
    public IEnumerable<T> Read<T>() where T : notnull, new()
    {
        IList<T> response = [];

        var csvHeaders = _csvLinesString[0].Split(';');
        for (int i = 1; i < _csvLinesString.Length; i++)
        {
            var objectCells = CsvCell.MapValues(csvHeaders, _csvLinesString[i].Split(';'));
            response.Add(Map<T>(objectCells));
        }

        return response;
    }


    /// <summary>
    /// Maps all cells into an object of Type T
    /// </summary>
    /// <typeparam name="T">Output type. T must have an empty constructor.</typeparam>
    /// <param name="cells">Key value of the cells on the CSV</param>
    /// <returns>Created T object.</returns>
    private T Map<T>(IEnumerable<CsvCell> cells) where T : notnull, new()
    {
        var response = new T();

        var responseType = response.GetType();
        var responseProperties = responseType.GetProperties();

        responseProperties.RunOverPropertiesWithAttribute((PropertyInfo property, AppendPropertiesToCsvAttribute attribute) => ReadAppendPropertiesToCsvAttribute(property, attribute, cells, response));

        responseProperties.RunOverPropertiesWithAttribute((PropertyInfo property, CsvDataAttribute attribute) => MapCsvDataAttribute(property, attribute, cells, response));

        return response;
    }

    /// <summary>
    /// Searchs for AppentPropertiesCSVAttribute and map the values into the target.
    /// </summary>
    /// <param name="property">Target Property</param>
    /// <param name="attribute">-</param>
    /// <param name="cells">Lis of cells for the object</param>
    /// <param name="target">Target in wich the data is going to be mapped.</param>
    private void ReadAppendPropertiesToCsvAttribute(PropertyInfo property, AppendPropertiesToCsvAttribute attribute, IEnumerable<CsvCell> cells, object target)
    {
        var innerObjectMapped = Map(cells, property.GetModifiedPropertyType());

        property.SetValue(target, innerObjectMapped, null);
    }

    /// <summary>
    /// Maps all properties with CSvDataAttribute.
    /// </summary>
    /// <param name="property">TargetProperty</param>
    /// <param name="attribute">Csv Data Attribute</param>
    /// <param name="cells">Cells of the current object</param>
    /// <param name="target">Target object</param>
    private void MapCsvDataAttribute(PropertyInfo property, CsvDataAttribute attribute, IEnumerable<CsvCell> cells, object target)
    {
        var cell = cells.FirstOrDefault(c => c.Name != string.Empty && c.Name == attribute.GetName());

        if (cell != null)
        {
            var castedValue = Convert.ChangeType(cell.Value, property.PropertyType);
            property.SetValue(target, castedValue, null);
        }
    }

    /// <summary>
    /// Creates an object of the Type and maps the cells into it.
    /// </summary>
    /// <param name="cells">Cells of the current object</param>
    /// <param name="type">Type of the new object. This Type should have an empty constructor.</param>
    /// <returns>new object with mapped values</returns>
    /// <exception cref="ArgumentNullException">Is the Type doesn't have an empty constructor.</exception>
    private object Map(IEnumerable<CsvCell> cells, Type type)
    {
        var classConstructor = type.UnderlyingSystemType.GetConstructor(Type.EmptyTypes)
            ?? throw new ArgumentNullException("All classes with the AppendPropertiesToCsv attribute should have an empty constructor.");

        var response = classConstructor.Invoke(null);
        var properties = type.UnderlyingSystemType.GetProperties();

        properties.RunOverPropertiesWithAttribute((PropertyInfo property, AppendPropertiesToCsvAttribute attribute) => ReadAppendPropertiesToCsvAttribute(property, attribute, cells, response));
        properties.RunOverPropertiesWithAttribute((PropertyInfo property, CsvDataAttribute attribute) => MapCsvDataAttribute(property, attribute, cells, response));

        return response;
    }

}
