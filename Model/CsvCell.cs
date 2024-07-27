

using Tools.Enumerable.To.CSV.Attribute;

namespace Tools.Enumerable.To.CSV.Model;

public class CsvCell(string name , string value)
{
    public string Name { get; } = name;
    public string Value { get; } = value;


    public static IList<CsvCell> ExtractLines(object myObject)
    {
        IList<CsvCell> response = [];

        var type = myObject.GetType();
        var properties = type.GetProperties();

        foreach (var property in properties)
        {
            var attributes = property.GetCustomAttributes(typeof(CsvDataAttribute), true);


            foreach(var attribute in attributes.Where(a => a is CsvDataAttribute))
            {
                var csvAttribute = attribute as CsvDataAttribute;
                var propertyValue = property.GetValue(myObject, null);
                var stringValue = propertyValue != null ?  propertyValue.ToString() : "";

                response.Add(new CsvCell(csvAttribute!.GetName(), stringValue!));

            }

        }

        return response;
    }
}
