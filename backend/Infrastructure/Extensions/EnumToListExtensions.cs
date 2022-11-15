using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Core.Dtos;

namespace Infrastructure.Extensions;

public static class EnumToListExtensions
{
    public static string toDescription(this Enum enumName)
    {
        string description;
        var fieldInfo = enumName.GetType().GetField(enumName.ToString());
        var attributes = GetDescriptionArr(fieldInfo);
        if (attributes != null && attributes.Length > 0)
            description = attributes[0].Description;
        else
            description = enumName.ToString();
        return description;
    }

    private static DescriptionAttribute[] GetDescriptionArr(FieldInfo fieldInfo)
    {
        if (fieldInfo != null)
            return (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
        return null;
    }

    public static IEnumerable<BaseOptionDto> ToOptions(this Type type)
    {
        var dic = GetNameAndValue(type);
        if (dic == null) return null;
        var list = dic.Select(c => new BaseOptionDto
        {
            Value = Convert.ToInt32(c.Key),
            Text = c.Value.ToString()
        }).ToList();
        return list;
    }

    public static Dictionary<object, object> GetNameAndValue(this Type type)
    {
        if (type.IsEnum)
        {
            var dic = new Dictionary<object, object>();
            var enumValues = Enum.GetValues(type);
            foreach (Enum value in enumValues) dic.Add(value.GetHashCode(), toDescription(value));
            return dic;
        }

        return null;
    }
}