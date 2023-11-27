
using System.Runtime.CompilerServices;

namespace KB.SharpCore.Utils;
public static class BitWiseHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int FlagToBit(int flag)
    {
        return (int)(1 << flag);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int RaiseFlag(int flags, int flag)
    {
        return flags | BitWiseHelper.FlagToBit(flag);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum RaiseFlag<TEnum>(TEnum enumValue, TEnum flag)
        where TEnum : Enum
    {
        BitWiseHelper.s_AssertEnumCheck(enumValue, flag);
        int result = BitWiseHelper.RaiseFlag(Convert.ToInt32(enumValue), Convert.ToInt32(flag));
        return (TEnum)Enum.ToObject(typeof(TEnum), result);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ClearFlag(int flags, int flag)
    {
        return flags & ~flag;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum ClearFlag<TEnum>(TEnum enumValue, TEnum flag)
        where TEnum : Enum
    {
        BitWiseHelper.s_AssertEnumCheck(enumValue, flag);
        int result = BitWiseHelper.ClearFlag(Convert.ToInt32(enumValue), Convert.ToInt32(flag));
        return (TEnum)Enum.ToObject(typeof(TEnum), result);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasFlag(int flags, int flag)
    {
        return (flags & flag) != 0;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasFlag<TEnum>(TEnum enumValue, TEnum flag)
        where TEnum : Enum
    {
        BitWiseHelper.s_AssertEnumCheck(enumValue, flag);
        return BitWiseHelper.HasFlag(Convert.ToInt32(enumValue), Convert.ToInt32(flag));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GetBit(int number, int bit)
    {
        return (number & ((int)1 << bit)) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SetBit(int number, int bit)
    {
        return number | (1 << bit);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ClearBit(int number, int bit)
    {
        return number & ~(1 << bit);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToggleBit(int number, int bit)
    {
        return number ^ (1 << bit);
    }

    
    private static void s_AssertEnumCheck<TEnum>(TEnum enumValue, TEnum flag)
        where TEnum : Enum
    {
        if (enumValue.GetType() != flag.GetType())
        {
            throw new ArgumentException("The enum values are not of the same enum type.");
        }
    }
}