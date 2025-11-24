using System.Runtime.CompilerServices;

namespace Vasconlabs.Aeolus.Client.Extensions;

public static class XxHash64
{
    private const ulong Prime1 = 11400714785074694791ul;
    private const ulong Prime2 = 14029467366897019727ul;
    private const ulong Prime3 =  1609587929392839161ul;
    private const ulong Prime4 =  9650029242287828579ul;
    private const ulong Prime5 =  2870177450012600261ul;

    public static ulong ComputeHash(ReadOnlySpan<byte> data, ulong seed = 0)
    {
        int length = data.Length;
        int remaining = length;
        int offset = 0;

        ulong hash;

        if (length >= 32)
        {
            ulong v1 = seed + Prime1 + Prime2;
            ulong v2 = seed + Prime2;
            ulong v3 = seed + 0;
            ulong v4 = seed - Prime1;

            while (remaining >= 32)
            {
                v1 = Round(v1, Read64(data, offset)); offset += 8;
                v2 = Round(v2, Read64(data, offset)); offset += 8;
                v3 = Round(v3, Read64(data, offset)); offset += 8;
                v4 = Round(v4, Read64(data, offset)); offset += 8;

                remaining -= 32;
            }

            hash = RotateLeft(v1, 1) + RotateLeft(v2, 7) + RotateLeft(v3, 12) + RotateLeft(v4, 18);

            hash = MergeRound(hash, v1);
            hash = MergeRound(hash, v2);
            hash = MergeRound(hash, v3);
            hash = MergeRound(hash, v4);
        }
        else
        {
            hash = seed + Prime5;
        }

        hash += (ulong)length;

        while (remaining >= 8)
        {
            ulong k = Round(0, Read64(data, offset));
            hash ^= k;
            hash = RotateLeft(hash, 27) * Prime1 + Prime4;

            offset += 8;
            remaining -= 8;
        }

        if (remaining >= 4)
        {
            hash ^= (ulong)Read32(data, offset) * Prime1;
            hash = RotateLeft(hash, 23) * Prime2 + Prime3;

            offset += 4;
            remaining -= 4;
        }

        while (remaining > 0)
        {
            hash ^= (ulong)data[offset] * Prime5;
            hash = RotateLeft(hash, 11) * Prime1;

            offset++;
            remaining--;
        }

        hash ^= hash >> 33;
        hash *= Prime2;
        hash ^= hash >> 29;
        hash *= Prime3;
        hash ^= hash >> 32;

        return hash;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong Read64(ReadOnlySpan<byte> data, int offset)
        => BitConverter.ToUInt64(data.Slice(offset, 8));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint Read32(ReadOnlySpan<byte> data, int offset)
        => BitConverter.ToUInt32(data.Slice(offset, 4));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong Round(ulong acc, ulong input)
    {
        acc += input * Prime2;
        acc = RotateLeft(acc, 31);
        acc *= Prime1;
        return acc;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong MergeRound(ulong acc, ulong val)
    {
        acc ^= Round(0, val);
        acc *= Prime1;
        acc += Prime4;
        return acc;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong RotateLeft(ulong value, int count)
        => (value << count) | (value >> (64 - count));

    public static ulong ComputeHash(string text, ulong seed = 0)
        => ComputeHash(System.Text.Encoding.UTF8.GetBytes(text), seed);
}