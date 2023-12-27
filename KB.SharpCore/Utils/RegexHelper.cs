using System.Globalization;
using System.Text.RegularExpressions;

namespace KB.SharpCore.Utils;

public static class RegexHelper
{
    public static string GetMatch(string input, string pattern)
    {
        var match = Regex.Match(input, pattern);
        return match.Success ? match.Value : String.Empty;
    }

    public static string GetMatch(string input, string pattern, int group)
    {
        var match = Regex.Match(input, pattern);
        return match.Success ? match.Groups[group].Value : String.Empty;
    }

    public static string[] GetMatches(string input, string pattern)
    {
        var matches = Regex.Matches(input, pattern);
        return matches.Cast<Match>().Select(m => m.Value).ToArray();
    }

    public static string[] GetMatches(string input, string pattern, int group)
    {
        var matches = Regex.Matches(input, pattern);
        return matches.Cast<Match>().Select(m => m.Groups[group].Value).ToArray();
    }

    public static string SplitByUpperCase(string input)
    {
        return Regex.Replace(input, @"([A-Z])", " $1", RegexOptions.Compiled).Trim();
    }

    public static class Network
    {
        public static bool IsIPAddress(string ip)
        {
            return IsIPv4(ip) || IsIPv6(ip);
        }

        public static bool IsPort(string port)
        {
            return Regex.IsMatch(port, @"^\d{1,5}$");
        }

        public static bool IsPassword(string password)
        {
            return Regex.IsMatch(password, @"^[a-zA-Z0-9]+$");
        }

        public static bool IsHostname(string hostname)
        {
            return Regex.IsMatch(hostname, @"^[a-zA-Z0-9]+$");
        }

        public static bool IsURL(string url)
        {
            return Regex.IsMatch(url, @"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$");
        }

        public static bool IsEmail(string email)
        {
            return Regex.IsMatch(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        }

        public static bool IsMACAddress(string mac)
        {
            return Regex.IsMatch(mac, @"^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$");
        }

        public static bool IsIPv4(string ip)
        {
            return Regex.IsMatch(ip, @"^(\d{1,3}\.){3}\d{1,3}$");
        }

        public static bool IsIPv6(string ip)
        {
            return Regex.IsMatch(ip, @"^([0-9A-Fa-f]{1,4}:){7}[0-9A-Fa-f]{1,4}$");
        }

        public static bool IsIPv4MappedIPv6(string ip)
        {
            return Regex.IsMatch(ip, @"^::[fF]{4}:\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$");
        }

        public static bool IsIPv4EmbeddedIPv6(string ip)
        {
            return Regex.IsMatch(ip, @"^::\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$");
        }

        public static bool IsIPv4CompatibleIPv6(string ip)
        {
            return Regex.IsMatch(ip, @"^::\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$");
        }

        public static bool IsIPv6UniqueLocalAddress(string ip)
        {
            return Regex.IsMatch(ip, @"^fc00:(::|[\w:]{1,4}){0,4}/\d{1,3}$");
        }

        public static bool IsIPv6LinkLocalAddress(string ip)
        {
            return Regex.IsMatch(ip, @"^fe80:(::|[\w:]{1,4}){0,4}/\d{1,3}$");
        }

        public static bool IsIPv6SiteLocalAddress(string ip)
        {
            return Regex.IsMatch(ip, @"^fec0:(::|[\w:]{1,4}){0,4}/\d{1,3}$");
        }

        public static bool IsIPv6MulticastAddress(string ip)
        {
            return Regex.IsMatch(ip, @"^ff[0-9a-f]{2}:(::|[\w:]{1,4}){0,4}/\d{1,3}$");
        }

        public static bool IsIPv6LoopbackAddress(string ip)
        {
            return Regex.IsMatch(ip, @"^::1$");
        }

        public static bool IsIPv6GlobalUnicastAddress(string ip)
        {
            return Regex.IsMatch(ip, @"^([0-9A-Fa-f]{1,4}:){7}[0-9A-Fa-f]{1,4}$");
        }
    }

    public static class Numeric
    {
        public static bool IsNumeric(string value, CultureInfo cultureInfo)
        {
            string decimalSeparator = cultureInfo.NumberFormat.NumberDecimalSeparator;
            string pattern = $@"^[-+]?[0-9]*[{Regex.Escape(decimalSeparator)}]?[0-9]+$";

            return Regex.IsMatch(value, pattern);
        }

        public static bool IsNumeric(string value)
        {
            return IsNumeric(value, CultureInfo.CurrentCulture);
        }

        public static bool HasDecimals(string value, CultureInfo cultureInfo)
        {
            string decimalSeparator = cultureInfo.NumberFormat.NumberDecimalSeparator;
            return Regex.IsMatch(value, $@"^\d+{Regex.Escape(decimalSeparator)}\d+$");
        }

        public static bool HasDecimals(string value)
        {
            return HasDecimals(value, CultureInfo.CurrentCulture);
        }

        public static bool IsInteger(string value)
        {
            return IsNumeric(value) && !HasDecimals(value);
        }

        public static bool IsFloat(string value)
        {
            return IsNumeric(value) && HasDecimals(value);
        }

        public static bool IsPositive(string value)
        {
            return IsNumeric(value) && !value.StartsWith("-");
        }

        public static bool IsNegative(string value)
        {
            return IsNumeric(value) && value.StartsWith("-");
        }
        public static bool IsZero(string value, CultureInfo cultureInfo)
        {
            return IsNumeric(value, cultureInfo) && Double.Parse(value, cultureInfo) == 0.0d;
        }

        public static bool IsZero(string value)
        {
            return IsZero(value, CultureInfo.CurrentCulture);
        }

        public static bool IsBetween(string value, string min, string max, CultureInfo cultureInfo)
        {
            if(IsNumeric(value, cultureInfo) && IsNumeric(min, cultureInfo) && IsNumeric(max, cultureInfo))
            {
                return Double.Parse(value, cultureInfo) >= Double.Parse(min, cultureInfo) && Double.Parse(value, cultureInfo) <= Double.Parse(max, cultureInfo);
            }

            return false;
        }

        public static bool IsBetween(string value, string min, string max)
        {
            return IsBetween(value, min, max, CultureInfo.CurrentCulture);
        }
    }
}
