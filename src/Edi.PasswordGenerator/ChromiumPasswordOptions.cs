namespace Edi.PasswordGenerator;

/// <summary>
/// Options for the Chromium-style password generator.
/// </summary>
public class ChromiumPasswordOptions
{
    public int Length { get; init; } = ChromiumPasswordGenerator.DefaultPasswordLength;

    public bool UseUppercase { get; init; } = true;

    public bool UseLowercase { get; init; } = true;

    public bool UseNumbers { get; init; } = true;

    public bool UseSymbols { get; init; }

    public bool ExcludeAmbiguous { get; init; } = true;
}
