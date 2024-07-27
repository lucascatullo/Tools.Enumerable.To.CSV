

using Tools.Enumerable.To.CSV.Attribute;

namespace Tools.Enumerable.To.CSV.Model;

internal class CsvCell(string name , string value)
{
    public string Name { get; } = name;
    public string Value { get; } = value;


    public static IList<CsvCell> ExtractLines(object myObject)
    {
        List<CsvCell> response = [];

        var type = myObject.GetType();
        var properties = type.GetProperties();

        foreach (var property in properties)
        {

            var hasAppendInsideAttribute = property.GetCustomAttributes(typeof(AppendPropertiesToCsvAttribute), true).Any();
            if (hasAppendInsideAttribute)
            {
                var innerObject = property.GetValue(myObject, null);
                if (innerObject != null)
                    foreach (CsvCell cell in ExtractLines(innerObject))
                        response.Add(cell);
            }
            else
            {
                var attributes = property.GetCustomAttributes(typeof(CsvDataAttribute), true);

                if (attributes.Length == 0) continue;

                var csvAttribute = attributes.First() as CsvDataAttribute;
                var propertyValue = property.GetValue(myObject, null);
                var stringValue = propertyValue != null ? propertyValue.ToString() : "";

                response.Add(new CsvCell(csvAttribute!.GetName(), stringValue!));
            }
        }

        if (AreCellNamesRepeated(response))
            throw new Exception("Names with the CSV Attribute must be unique. (Check for inneer properties)");  
        return response;
    }


    private static bool AreCellNamesRepeated(IList<CsvCell> cells) => cells.Count != cells.DistinctBy(c => c.Name).Count();

}
