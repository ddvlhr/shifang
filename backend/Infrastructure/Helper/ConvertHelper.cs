using System;
using RTLib;

namespace Infrastructure.Helper;

public static class ConvertHelper
{
    public static double paToMMWG(double resistanceValuePa)
    {
        var ret = RTHelper.getConvertedValue(resistanceValuePa, DataItemUnitEnum.pa, DataItemUnitEnum.mmwg);
        return ret;
    }
    
    public static double mmWGToPa(double resistanceValueMmWg)
    {
        return RTHelper.getConvertedValue(resistanceValueMmWg, DataItemUnitEnum.mmwg, DataItemUnitEnum.pa);
    }

}