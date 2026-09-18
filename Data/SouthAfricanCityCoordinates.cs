namespace TertiaryInstitutions.Data;

/// <summary>
/// Approximate (city-centroid, not campus-precise) coordinates for the South African cities used
/// as University.City values in Data/SouthAfricanUniversities.cs. Used to backfill
/// University.Latitude/Longitude for straight-line distance calculations - see Services/GeoDistance.cs.
/// </summary>
public static class SouthAfricanCityCoordinates
{
    public static readonly IReadOnlyDictionary<string, (double Lat, double Lng)> ByCity =
        new Dictionary<string, (double, double)>(StringComparer.OrdinalIgnoreCase)
        {
            ["Cape Town"] = (-33.9249, 18.4241),
            ["Johannesburg"] = (-26.2041, 28.0473),
            ["Stellenbosch"] = (-33.9321, 18.8602),
            ["Pretoria"] = (-25.7479, 28.2293),
            ["Durban"] = (-29.8587, 31.0218),
            ["Makhanda"] = (-33.3106, 26.5341),
            ["Potchefstroom"] = (-26.7145, 27.0980),
            ["Bloemfontein"] = (-29.0852, 26.1596),
            ["Gqeberha"] = (-33.9608, 25.6022),
            ["Polokwane"] = (-23.9045, 29.4689),
            ["Thohoyandou"] = (-22.9769, 30.4831),
            ["KwaDlangezwa"] = (-28.8360, 31.8420),
            ["Mthatha"] = (-31.5889, 28.7844),
            ["Vanderbijlpark"] = (-26.7113, 27.8393),
            ["Mbombela"] = (-25.4753, 30.9694),
            ["Kimberley"] = (-28.7282, 24.7499),
            ["Alice"] = (-32.7833, 26.8333),
        };
}
