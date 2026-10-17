using System.Numerics;
using System.Security.Cryptography;
using SmartNav.Application.Abstractions;

namespace SmartNav.Infrastructure.Qr;

/// <summary>128 bits of CSPRNG entropy, base62-encoded to a fixed 22 chars (URL-safe, compact QR).</summary>
public sealed class QrTokenGenerator : IQrTokenGenerator
{
    private const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    public const int TokenLength = 22;

    public string NewToken()
    {
        Span<byte> bytes = stackalloc byte[16];
        RandomNumberGenerator.Fill(bytes);
        var value = new BigInteger(bytes, isUnsigned: true);
        var chars = new char[TokenLength];
        for (var i = TokenLength - 1; i >= 0; i--)
        {
            value = BigInteger.DivRem(value, 62, out var rem);
            chars[i] = Alphabet[(int)rem];
        }
        return new string(chars);
    }
}
