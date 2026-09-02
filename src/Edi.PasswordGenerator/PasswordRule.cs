namespace Edi.PasswordGenerator;

/// <summary>
/// Rules understood by <see cref="LegacyPasswordGenerator"/>.
/// </summary>
[Obsolete("PasswordRule is supported only by LegacyPasswordGenerator. Use ChromiumPasswordOptions with PasswordGenerator instead.")]
public class PasswordRule(int length, int leastNumberOfNonAlphanumericCharacters)
{
    public int Length { get; set; } = length;

    public int LeastNumberOfNonAlphanumericCharacters { get; set; } = leastNumberOfNonAlphanumericCharacters;
}
