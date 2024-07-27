
namespace Tools.Enumerable.To.CSV.Core;

public interface ICsvWritter
{
    MemoryStream WriteAsMemoryStream(IEnumerable<object> csvObject);
}