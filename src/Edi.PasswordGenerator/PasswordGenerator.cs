namespace Edi.PasswordGenerator;

/// <summary>
/// The primary password generator, using the Chromium-style password logic.
/// </summary>
public class PasswordGenerator : IPasswordGenerator
{
    public string GeneratePassword(ChromiumPasswordOptions? options = null)
    {
        return ChromiumPasswordGenerator.GenerateChromiumPassword(options);
    }

    public static string GenerateChromiumPassword(ChromiumPasswordOptions? options = null) =>
        ChromiumPasswordGenerator.GenerateChromiumPassword(options);

    public static ChromiumPasswordSpec BuildChromiumPasswordSpec(ChromiumPasswordOptions options) =>
        ChromiumPasswordGenerator.BuildChromiumPasswordSpec(options);

    public static string GetChromiumPasswordCharset(ChromiumPasswordOptions options) =>
        ChromiumPasswordGenerator.GetChromiumPasswordCharset(options);
}

/// <summary>
/// Compatibility name for the pre-3.0.0 default generator entry point.
/// </summary>
public class DefaultPasswordGenerator : PasswordGenerator
{
}
