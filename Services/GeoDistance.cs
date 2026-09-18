namespace TertiaryInstitutions.Services;

/// <summary>
/// Straight-line (Haversine) distance between two coordinates, in kilometres. Used to approximate
/// distance-from-institution since this project has no external geocoding/routing service.
/// </summary>
public static class GeoDistance
{
    private const double EarthRadiusKm = 6371.0;

    public static double CalculateKm(double lat1, double lng1, double lat2, double lng2)
    {
        var dLat = ToRadians(lat2 - lat1);
        var dLng = ToRadians(lng2 - lng1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return EarthRadiusKm * c;
    }

    private static double ToRadians(double deg) => deg * Math.PI / 180.0;
}
