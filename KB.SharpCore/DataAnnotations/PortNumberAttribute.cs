using KB.SharpCore.Utils;
using System.ComponentModel.DataAnnotations;

namespace KB.SharpCore.DataAnnotations;

/// <summary>
///     Validation attribute to indicate that a property field or parameter is a port number.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
    AllowMultiple = false)]
public class PortNumberAttribute : ValidationAttribute
{
    /// <summary>
    ///     Default constructor.
    /// </summary>
    /// <remarks>
    ///     This constructor selects a reasonable default error message for
    ///     <see cref="ValidationAttribute.FormatErrorMessage" />
    /// </remarks>
    public PortNumberAttribute(string errorMessage) : base(errorMessage)
    {
    }

    /// <summary>
    ///     Default constructor.
    /// </summary>
    /// <remarks>
    ///     This constructor selects a reasonable default error message for
    ///     <see cref="ValidationAttribute.FormatErrorMessage" />
    /// </remarks>
    public PortNumberAttribute() : this("Invalid Port Number")
    {
    }

    /// <summary>
    ///     Override of <see cref="ValidationAttribute.IsValid(object)" />
    /// </summary>
    /// <param name="value">The value to test</param>
    /// <returns>
    ///     <c>false</c> if the <paramref name="value" /> is a valid Port Number.
    /// </returns>
    public override bool IsValid(object? value)
    {
        if (value == null)
        {
            return false;
        }

        string? port = value as string;
        if (String.IsNullOrWhiteSpace(port))
        {
            return false;
        }

        return RegexHelper.Network.IsPort(port!);
    }
}