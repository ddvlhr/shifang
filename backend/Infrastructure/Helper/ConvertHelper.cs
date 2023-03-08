namespace Infrastructure.Helper;

public static class ConvertHelper
{
    public static double paToMMWG(double resistanceValuePa)
    {
        var ret = resistanceValuePa * 0.10197162;
        return ret;
    }
    
    public static double mmWGToPa(double resistanceValueMmWg)
    {
        return resistanceValueMmWg * 9.80665;
    }

}