#nullable enable
using System.ComponentModel.DataAnnotations;
using Path = KBAvaloniaCore.IO.Path;

namespace KBAvaloniaCore.DataAnnotations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class RequiredPathAttribute : RequiredAttribute
{
    public RequiredPathAttribute()
    {
        AllowEmptyStrings = false;
    }

    public bool AllowNonExistingPath { get; set; }

    public override bool IsValid(object? value)
    {
        if (base.IsValid(value))
        {
            if (!AllowNonExistingPath)
            {
                Path path = (Path)(string)value!;
                return path.Exists();
            }

            return true;
        }

        return false;
    }
}