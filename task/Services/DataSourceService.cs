using System.Globalization;
using System.Text.Json;
using task.Contract;
using task.Entities;
using task.Extensions;

namespace task.Services;

internal sealed class DataSourceService() : IDataSourceService
{
    async Task<IList<Office>> IDataSourceService.LoadAsync(string filePath, CancellationToken cancellationToken)
    {
        var result = new List<Office>();

        using (var stream = File.OpenRead(filePath))
        {
            using (JsonDocument doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken))
            {
                var cityElements = doc.RootElement.EnumerateObject().FirstOrDefault().Value;

                if (cityElements.ValueKind == JsonValueKind.Undefined)
                    return await Task.FromResult(Enumerable.Empty<Office>().ToList());

                foreach (var cityElement in cityElements.EnumerateArray())
                {
                    var terminalsElements = cityElement
                        .GetPropertyIgnoreCase("terminals")
                        .GetPropertyIgnoreCase("terminal");

                    foreach (var terminalNode in terminalsElements.EnumerateArray())
                    {
                        var office = new Office
                        {
                            Id = int.TryParse(terminalNode.GetPropertyIgnoreCase("id").GetString(), out var id) ? id : 0,
                            Code = cityElement.GetPropertyIgnoreCase("code").GetString(),
                            CityCode = cityElement.GetPropertyIgnoreCase("cityID").GetInt32(),
                            Uuid = null, // Не нашел подходящего поля в JSON
                            Type = GetOfficeType(terminalNode),
                            CountryCode = "", // Не нашел подходящего поля в JSON
                            Coordinates = new Coordinates
                            {
                                Latitude = double.Parse(terminalNode.GetPropertyIgnoreCase("latitude").GetString(), CultureInfo.InvariantCulture),
                                Longitude = double.Parse(terminalNode.GetPropertyIgnoreCase("longitude").GetString(), CultureInfo.InvariantCulture),
                            },
                            AddressRegion = null,
                            AddressCity = cityElement.GetPropertyIgnoreCase("name").GetString(),
                            AddressStreet = terminalNode.GetPropertyIgnoreCase("address").GetString(),
                            AddressHouseNumber = terminalNode.GetPropertyIgnoreCase("fullAddress").GetString(),
                            AddressApartment = 0, // Не нашел подходящего поля в JSON
                            WorkTime = "" // Не нашел подходящего поля в JSON
                        };

                        office.Phones ??= [];

                        foreach (var phoneNode in terminalNode.GetPropertyIgnoreCase("phones").EnumerateArray())
                        {
                            office.Phones.Add(new Phone
                            {
                                OfficeId = office.Id,
                                PhoneNumber = phoneNode.GetPropertyIgnoreCase("number").GetString(),
                                Additional = phoneNode.GetPropertyIgnoreCase("comment").GetString(),
                                Office = office
                            });
                        }

                        result.Add(office);
                    }
                }
            }

            return await Task.FromResult(result);
        }
    }

    private static OfficeType? GetOfficeType(JsonElement terminalNode)
    {
        return terminalNode.GetPropertyIgnoreCase("isPVZ").GetBoolean()
                            ? OfficeType.PVZ
                            : terminalNode.GetPropertyIgnoreCase("isOffice").GetBoolean()
                            ? OfficeType.POSTAMAT
                            : terminalNode.GetPropertyIgnoreCase("storage").GetBoolean()
                            ? OfficeType.WAREHOUSE
                            : null;
    }
}
