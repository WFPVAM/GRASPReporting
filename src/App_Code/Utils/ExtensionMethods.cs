using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ExtensionMethods
/// </summary>
public static class ExtensionMethods
{
    public static string GetSubstringAfterLastChar(this string source, char lastChar)
    {
        if (!string.IsNullOrEmpty(source))
        {
            int lastCharIndex = source.LastIndexOf(lastChar);
            if (lastCharIndex > 0)
            {
                return source.Substring(lastCharIndex + 1);
            }
            else
                return null;
        }
        else
            return null;
    }

    public static string GetSubstringBeforeLastChar(this string source, char lastChar)
    {
        if (!string.IsNullOrEmpty(source))
        {
            int lastCharIndex = source.LastIndexOf(lastChar);
            if (lastCharIndex > 0)
            {
                return source.Substring(0, lastCharIndex);
            }
            else
                return null;
        }
        else
            return null;
    }

    public static string GetSubstringAfterFirstChar(this string source, char firstChar)
    {
        if (!string.IsNullOrEmpty(source))
        {
            int firstCharIndex = source.IndexOf(firstChar);
            if (firstCharIndex > 0)
            {
                return source.Substring(firstCharIndex + 1, source.Length - firstCharIndex - 1);
            }
            else
                return null;
        }
        else
            return null;
    }
}