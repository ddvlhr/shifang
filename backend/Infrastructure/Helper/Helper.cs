using System;
using System.Collections.Generic;
using System.Security.Claims;
using Core.Enums;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Helper;

public static class Helper
{
    public static List<int> Deduplication(List<int> now, List<int> old)
    {
        var temp = new List<int>();
        return temp;
    }

    public static int getUserId(this HttpContext context)
    {
        var userId = context.User.FindFirst(c => c.Type == ClaimTypes.UserData).Value;
        return Convert.ToInt32(userId);
    }

    public static string getUserName(this HttpContext context)
    {
        var userName = context.User.Identity.Name;
        return userName;
    }

    public static int getUserRoleId(this HttpContext context)
    {
        var roleId = context.User.FindFirst("roleId").Value;
        return Convert.ToInt32(roleId);
    }

    public static string getUserRoleName(this HttpContext context)
    {
        var roleName = context.User.FindFirst(c => c.Type == ClaimTypes.Role).Value;
        return roleName;
    }

    public static bool getCanSeeOtherData(this HttpContext context)
    {
        var can = context.User.FindFirst("canSeeOtherData").Value;
        return can == "1";
    }

    public static DepartmentType getDepartmentType(this HttpContext context)
    {
        var departmentType = context.User.FindFirst("equipmentType")?.Value;
        return (DepartmentType) Convert.ToInt32(departmentType ?? "0");
    }

    public static string getStandard(string standard, string upper, string lower)
    {
        var sta = Convert.ToDouble(standard);
        var up = Convert.ToDouble(upper);
        var low = Convert.ToDouble(lower);
        var result = "";
        if (!sta.Equals(0))
        {
            if (up.Equals(low))
            {
                result = $"{standard}±{upper}";
            }
            else if (up.Equals(0) && low.Equals(sta))
            {
                result = $"≥{standard}";
            }
            else if (low.Equals(0) && up.Equals(sta))
            {
                result = $"≤{standard}";
            }
            else
            {
                var upLimit = sta + up;
                var downLimit = sta - low;
                result = $"{downLimit}-{upLimit}";
            }
        }

        return result;
    }
}