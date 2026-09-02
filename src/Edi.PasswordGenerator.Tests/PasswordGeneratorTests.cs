using Xunit;

namespace Edi.PasswordGenerator.Tests;

public class PasswordGeneratorTests
{
    private static ChromiumPasswordOptions DefaultOptions => new();

    [Fact]
    public void GeneratePassword_WithDefaultOptions_GeneratesChromiumStylePassword()
    {
        var password = new PasswordGenerator().GeneratePassword();

        Assert.Equal(ChromiumPasswordGenerator.DefaultPasswordLength, password.Length);
        Assert.Contains(password, character => "abcdefghijkmnpqrstuvwxyz".Contains(character));
        Assert.Contains(password, character => "ABCDEFGHJKLMNPQRSTUVWXYZ".Contains(character));
        Assert.Contains(password, character => "23456789".Contains(character));
        Assert.DoesNotContain(password, character => "0O1Il|".Contains(character));
        Assert.DoesNotContain(password, character => "-_.:!".Contains(character));
    }

    [Fact]
    public void BuildChromiumPasswordSpec_UsesChromiumCharacterSetsByDefault()
    {
        var spec = ChromiumPasswordGenerator.BuildChromiumPasswordSpec(DefaultOptions);

        Assert.Equal(15, spec.TargetLength);
        Assert.Equal("abcdefghijkmnpqrstuvwxyz", spec.Lowercase.Set);
        Assert.Equal("ABCDEFGHJKLMNPQRSTUVWXYZ", spec.Uppercase.Set);
        Assert.Equal("23456789", spec.Numbers.Set);
        Assert.Equal("-_.:!", spec.Symbols.Set);
        Assert.Equal(1, spec.Lowercase.Min);
        Assert.Equal(1, spec.Uppercase.Min);
        Assert.Equal(1, spec.Numbers.Min);
        Assert.Equal(0, spec.Symbols.Min);
    }

    [Fact]
    public void GetChromiumPasswordCharset_UsesSymbolsOnlyWhenEnabled()
    {
        var charset = ChromiumPasswordGenerator.GetChromiumPasswordCharset(new ChromiumPasswordOptions
        {
            UseUppercase = false,
            UseLowercase = false,
            UseNumbers = false,
            UseSymbols = true,
        });

        Assert.Equal("-_.:!", charset);
    }

    [Fact]
    public void BuildChromiumPasswordSpec_AddsZeroAndOneWhenLettersAreDisabled()
    {
        var spec = ChromiumPasswordGenerator.BuildChromiumPasswordSpec(new ChromiumPasswordOptions
        {
            UseUppercase = false,
            UseLowercase = false,
            UseSymbols = false,
        });

        Assert.Equal("2345678901", spec.Numbers.Set);
    }

    [Fact]
    public void GenerateChromiumPassword_ReturnsEmptyWhenEveryClassIsDisabled()
    {
        var password = ChromiumPasswordGenerator.GenerateChromiumPassword(new ChromiumPasswordOptions
        {
            UseUppercase = false,
            UseLowercase = false,
            UseNumbers = false,
            UseSymbols = false,
        });

        Assert.Equal(string.Empty, password);
    }

    [Fact]
    public void GenerateChromiumPassword_RespectsEnabledCharacterClasses()
    {
        var options = new ChromiumPasswordOptions
        {
            Length = 32,
            UseUppercase = false,
            UseLowercase = false,
            UseNumbers = true,
            UseSymbols = true,
            ExcludeAmbiguous = false,
        };

        var password = ChromiumPasswordGenerator.GenerateChromiumPassword(options);

        Assert.Equal(32, password.Length);
        Assert.Contains(password, character => char.IsDigit(character));
        Assert.Contains(password, character => "-_.:!".Contains(character));
        Assert.All(password, character => Assert.Contains(character, "0123456789-_.:!"));
    }

    [Fact]
    public void GenerateChromiumPassword_NormalizesLengthLikeToolsGenerator()
    {
        var defaultLengthPassword = ChromiumPasswordGenerator.GenerateChromiumPassword(new ChromiumPasswordOptions
        {
            Length = 0,
        });
        var emptyPassword = ChromiumPasswordGenerator.GenerateChromiumPassword(new ChromiumPasswordOptions
        {
            Length = -1,
        });
        var maximumLengthPassword = ChromiumPasswordGenerator.GenerateChromiumPassword(new ChromiumPasswordOptions
        {
            Length = 500,
        });

        Assert.Equal(15, defaultLengthPassword.Length);
        Assert.Empty(emptyPassword);
        Assert.Equal(200, maximumLengthPassword.Length);
    }

    [Fact]
    public void GenerateChromiumPassword_WithSingleClassCanGenerateShortPasswords()
    {
        var password = ChromiumPasswordGenerator.GenerateChromiumPassword(new ChromiumPasswordOptions
        {
            Length = 3,
            UseUppercase = false,
            UseLowercase = false,
            UseNumbers = false,
            UseSymbols = true,
        });

        Assert.Equal(3, password.Length);
        Assert.All(password, character => Assert.Contains(character, "-_.:!"));
    }

    [Fact]
    public void DefaultPasswordGenerator_UsesTheNewPrimaryGenerator()
    {
        IPasswordGenerator generator = new DefaultPasswordGenerator();

        var password = generator.GeneratePassword();

        Assert.Equal(15, password.Length);
        Assert.DoesNotContain(password, character => "0O1Il|".Contains(character));
    }

    [Fact]
    public void PasswordGenerator_StaticHelpersForwardToChromiumLogic()
    {
        var options = new ChromiumPasswordOptions
        {
            Length = 10,
            UseUppercase = false,
            UseLowercase = false,
            UseNumbers = false,
            UseSymbols = true,
        };

        Assert.Equal("-_.:!", PasswordGenerator.GetChromiumPasswordCharset(options));
        Assert.Equal(10, PasswordGenerator.GenerateChromiumPassword(options).Length);
    }
}
