using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Helper;

public static class Encrypt
{
    /// <summary>
    ///     8位字符的密钥字符串
    /// </summary>
    public static readonly string Key = "RetoolTe";

    /// <summary>
    ///     8位字符的初始化向量字符串
    /// </summary>
    public static readonly string Iv = "19970718";

    /// <summary>
    ///     Des加密
    /// </summary>
    /// <param name="data">加密数据</param>
    /// <returns></returns>
    public static string DesEncrypt(string data)
    {
        var byKey = Encoding.ASCII.GetBytes(Key);
        var byIv = Encoding.ASCII.GetBytes(Iv);

        var crypto = new DESCryptoServiceProvider();
        var i = crypto.KeySize;
        var ms = new MemoryStream();
        var cst = new CryptoStream(ms, crypto.CreateEncryptor(byKey, byIv), CryptoStreamMode.Write);

        var sw = new StreamWriter(cst);
        sw.Write(data);
        sw.Flush();
        cst.FlushFinalBlock();
        sw.Flush();
        return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
    }

    /// <summary>
    ///     Des解密
    /// </summary>
    /// <param name="data">解密数据</param>
    /// <returns></returns>
    public static string DesDecrypt(string data)
    {
        var byKey = Encoding.ASCII.GetBytes(Key);
        var byIv = Encoding.ASCII.GetBytes(Iv);

        byte[] byEnc;
        try
        {
            byEnc = Convert.FromBase64String(data);
        }
        catch
        {
            return null;
        }

        var cryptoProvider = new DESCryptoServiceProvider();
        var ms = new MemoryStream(byEnc);
        var cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIv), CryptoStreamMode.Read);
        var sr = new StreamReader(cst);
        return sr.ReadToEnd();
    }
}