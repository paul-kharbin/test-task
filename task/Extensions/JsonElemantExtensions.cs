using System.Text.Json;

namespace task.Extensions;

internal static class JsonElemantExtensions
{
    public static bool TryGetPropertyIgnoreCase(this JsonElement element, string propertyName, out JsonElement jsonElement, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
    {
        try
        {
            jsonElement = element.GetPropertyIgnoreCase(propertyName, comparisonType);
            return true;
        }
        catch (ArgumentNullException)
        {
            throw;
        }
        catch (Exception)
        {
            jsonElement = default;
            return false;
        }
    }

    public static JsonElement GetPropertyIgnoreCase(this JsonElement element, string propertyName, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
    {
        var jsonElement = element.EnumerateObject().First(o => propertyName.Equals(o.Name, comparisonType));
        return jsonElement.Value;
    }
}
