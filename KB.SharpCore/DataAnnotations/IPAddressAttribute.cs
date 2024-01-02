using KB.SharpCore.Utils;
using System.ComponentModel.DataAnnotations;

namespace KB.SharpCore.DataAnnotations;

/// <summary>
///     Validation attribute to indicate that a property field or parameter is a ip address.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
    AllowMultiple = false)]
public class IPAddressAttribute : ValidationAttribute
{
    private bool _isIPv4;
    private bool _isIPv6;

    /// <summary>
    ///     Default constructor.
    /// </summary>
    /// <remarks>
    ///     This constructor selects a reasonable default error message for
    ///     <see cref="ValidationAttribute.FormatErrorMessage" />
    /// </remarks>
    public IPAddressAttribute(bool isIPv4, bool isIPv6, string errorMessage) : base(errorMessage)
    {
        _isIPv4 = isIPv4;
        _isIPv6 = isIPv6;
    }

    public IPAddressAttribute(bool isIPv4, bool isIPv6) : this(isIPv4, isIPv6, "Invalid IP Address")
    {
    }

    /// <summary>
    ///     Override of <see cref="ValidationAttribute.IsValid(object)" />
    /// </summary>
    /// <param name="value">The value to test</param>
    /// <returns>
    ///     <c>false</c> if the <paramref name="value" /> is a valid IP address.
    /// </returns>
    public override bool IsValid(object? value)
    {
        if (value == null)
        {
            return false;
        }

        string? ip = value as string;

        bool result = false;
        if (String.IsNullOrWhiteSpace(ip))
        {
            return false;
        }

        if (_isIPv4)
        {
            result = RegexHelper.Network.IsIPv4(ip!);
        }

        if (!result && _isIPv6)
        {
            result = RegexHelper.Network.IsIPv6(ip!);
        }

        return result;
    }
}
