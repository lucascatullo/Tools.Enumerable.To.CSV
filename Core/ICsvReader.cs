
namespace Tools.Enumerable.To.CSV.Core;

public interface ICsvReader
{
    IEnumerable<T> Read<T>() where T : notnull, new();
}