using System.Buffers.Binary;
using System.Security.Cryptography;

namespace Edi.PasswordGenerator;

/// <summary>
/// Character classes exposed by the Chromium-style password generator.
/// </summary>
public enum PasswordCharacterClassKey
{
    Lowercase,
    Uppercase,
    Numbers,
    Symbols,
}

/// <summary>
/// The character set and bounds for one password character class.
/// </summary>
public sealed class PasswordCharacterClass
{
    internal PasswordCharacterClass(string set, int min, int max)
    {
        Set = set;
        Min = min;
        Max = max;
    }

    public string Set { get; }

    public int Min { get; internal set; }

    public int Max { get; internal set; }

    internal PasswordCharacterClass Clone() => new(Set, Min, Max);
}

/// <summary>
/// The normalized password-generation specification.
/// </summary>
public sealed class ChromiumPasswordSpec
{
    internal ChromiumPasswordSpec(
        int targetLength,
        IReadOnlyDictionary<PasswordCharacterClassKey, PasswordCharacterClass> classes)
    {
        TargetLength = targetLength;
        Classes = classes;
    }

    public int TargetLength { get; }

    public IReadOnlyDictionary<PasswordCharacterClassKey, PasswordCharacterClass> Classes { get; }

    public PasswordCharacterClass Lowercase => Classes[PasswordCharacterClassKey.Lowercase];

    public PasswordCharacterClass Uppercase => Classes[PasswordCharacterClassKey.Uppercase];

    public PasswordCharacterClass Numbers => Classes[PasswordCharacterClassKey.Numbers];

    public PasswordCharacterClass Symbols => Classes[PasswordCharacterClassKey.Symbols];
}

/// <summary>
/// Implements the password-generation logic used by the Tools password generator.
/// </summary>
public static class ChromiumPasswordGenerator
{
    public const int DefaultPasswordLength = 15;
    public const int MaxPasswordLength = 200;

    private const int BigMax = int.MaxValue;
    private const string ChromiumLowercase = "abcdefghijkmnpqrstuvwxyz";
    private const string ChromiumUppercase = "ABCDEFGHJKLMNPQRSTUVWXYZ";
    private const string ChromiumNumbers = "23456789";
    private const string FullLowercase = "abcdefghijklmnopqrstuvwxyz";
    private const string FullUppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string FullNumbers = "0123456789";
    private const string ChromiumSymbols = "-_.:!";

    private static readonly PasswordCharacterClassKey[] ClassOrder =
    [
        PasswordCharacterClassKey.Lowercase,
        PasswordCharacterClassKey.Uppercase,
        PasswordCharacterClassKey.Numbers,
        PasswordCharacterClassKey.Symbols,
    ];

    /// <summary>
    /// Builds the normalized specification used for generation.
    /// </summary>
    public static ChromiumPasswordSpec BuildChromiumPasswordSpec(ChromiumPasswordOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var lowerSet = options.ExcludeAmbiguous ? ChromiumLowercase : FullLowercase;
        var upperSet = options.ExcludeAmbiguous ? ChromiumUppercase : FullUppercase;
        var numberSet = options.ExcludeAmbiguous ? ChromiumNumbers : FullNumbers;

        if (!options.UseLowercase && !options.UseUppercase && options.ExcludeAmbiguous)
        {
            numberSet += "01";
        }

        var classes = new Dictionary<PasswordCharacterClassKey, PasswordCharacterClass>
        {
            [PasswordCharacterClassKey.Lowercase] = new(
                lowerSet,
                options.UseLowercase ? 1 : 0,
                options.UseLowercase ? BigMax : 0),
            [PasswordCharacterClassKey.Uppercase] = new(
                upperSet,
                options.UseUppercase ? 1 : 0,
                options.UseUppercase ? BigMax : 0),
            [PasswordCharacterClassKey.Numbers] = new(
                numberSet,
                options.UseNumbers ? 1 : 0,
                options.UseNumbers ? BigMax : 0),
            [PasswordCharacterClassKey.Symbols] = new(
                ChromiumSymbols,
                0,
                options.UseSymbols ? BigMax : 0),
        };

        foreach (var key in ClassOrder)
        {
            var characterClass = classes[key];
            if (characterClass.Set.Length == 0)
            {
                characterClass.Max = 0;
            }

            if (characterClass.Max < characterClass.Min)
            {
                characterClass.Min = characterClass.Max;
            }
        }

        return new ChromiumPasswordSpec(NormalizeLength(options.Length), classes);
    }

    /// <summary>
    /// Returns the character pool used by the normalized specification.
    /// </summary>
    public static string GetChromiumPasswordCharset(ChromiumPasswordOptions options)
    {
        var spec = BuildChromiumPasswordSpec(options);
        return string.Concat(
            ClassOrder
                .Select(key => spec.Classes[key].Max > 0 ? spec.Classes[key].Set : string.Empty));
    }

    /// <summary>
    /// Generates a password using cryptographically secure randomness.
    /// </summary>
    public static string GenerateChromiumPassword(ChromiumPasswordOptions? options = null)
    {
        options ??= new ChromiumPasswordOptions();

        using var rng = RandomNumberGenerator.Create();
        return GenerateChromiumPassword(options, rng);
    }

    private static string GenerateChromiumPassword(
        ChromiumPasswordOptions options,
        RandomNumberGenerator rng)
    {
        var spec = BuildChromiumPasswordSpec(options);
        var workingClasses = CloneClasses(spec.Classes);
        var activeClasses = ClassOrder
            .Where(key => workingClasses[key].Max > 0)
            .ToArray();

        if (activeClasses.Length == 0 || spec.TargetLength == 0)
        {
            return string.Empty;
        }

        var password = new List<GeneratedCharacter>(spec.TargetLength);

        foreach (var key in activeClasses)
        {
            var characterClass = workingClasses[key];
            while (characterClass.Min > 0 && password.Count < spec.TargetLength)
            {
                password.Add(new GeneratedCharacter(RandomChoice(rng, characterClass.Set), key));
                characterClass.Min -= 1;
                characterClass.Max -= 1;
            }
        }

        while (password.Count < spec.TargetLength)
        {
            var possibleCharacters = activeClasses
                .Where(key => workingClasses[key].Max > 0)
                .Sum(key => workingClasses[key].Set.Length);

            if (possibleCharacters == 0)
            {
                break;
            }

            var choice = RandomRange(rng, possibleCharacters);
            foreach (var key in activeClasses)
            {
                var characterClass = workingClasses[key];
                if (characterClass.Max <= 0)
                {
                    continue;
                }

                if (choice < characterClass.Set.Length)
                {
                    password.Add(new GeneratedCharacter(characterClass.Set[choice], key));
                    characterClass.Max -= 1;
                    break;
                }

                choice -= characterClass.Set.Length;
            }
        }

        if (password.Count < 4 && spec.TargetLength >= 4)
        {
            return GenerateChromiumPassword(
                new ChromiumPasswordOptions
                {
                    Length = DefaultPasswordLength,
                    UseUppercase = true,
                    UseLowercase = true,
                    UseNumbers = true,
                    UseSymbols = false,
                    ExcludeAmbiguous = true,
                },
                rng);
        }

        var shuffled = password;
        var remainingAttempts = 5;
        do
        {
            shuffled = Shuffle(shuffled, rng);
        }
        while (IsDifficultToRead(shuffled) && remainingAttempts-- > 0);

        return string.Concat(shuffled.Select(item => item.Character));
    }

    private static Dictionary<PasswordCharacterClassKey, PasswordCharacterClass> CloneClasses(
        IReadOnlyDictionary<PasswordCharacterClassKey, PasswordCharacterClass> classes)
    {
        return new Dictionary<PasswordCharacterClassKey, PasswordCharacterClass>
        {
            [PasswordCharacterClassKey.Lowercase] = classes[PasswordCharacterClassKey.Lowercase].Clone(),
            [PasswordCharacterClassKey.Uppercase] = classes[PasswordCharacterClassKey.Uppercase].Clone(),
            [PasswordCharacterClassKey.Numbers] = classes[PasswordCharacterClassKey.Numbers].Clone(),
            [PasswordCharacterClassKey.Symbols] = classes[PasswordCharacterClassKey.Symbols].Clone(),
        };
    }

    private static char RandomChoice(RandomNumberGenerator rng, string text)
    {
        return text[RandomRange(rng, text.Length)];
    }

    private static int RandomRange(RandomNumberGenerator rng, int range)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(range);

        var unsignedRange = (ulong)range;
        var maxAcceptable = ulong.MaxValue / unsignedRange * unsignedRange - 1;
        Span<byte> bytes = stackalloc byte[sizeof(ulong)];
        ulong value;

        do
        {
            rng.GetBytes(bytes);
            value = BinaryPrimitives.ReadUInt64LittleEndian(bytes);
        }
        while (value > maxAcceptable);

        return (int)(value % unsignedRange);
    }

    private static List<GeneratedCharacter> Shuffle(
        IReadOnlyList<GeneratedCharacter> items,
        RandomNumberGenerator rng)
    {
        var output = items.ToList();
        for (var i = output.Count - 1; i > 0; i--)
        {
            var j = RandomRange(rng, i + 1);
            (output[i], output[j]) = (output[j], output[i]);
        }

        return output;
    }

    private static bool IsDifficultToRead(IReadOnlyList<GeneratedCharacter> items)
    {
        for (var i = 1; i < items.Count; i++)
        {
            var previous = items[i - 1].Character;
            var current = items[i].Character;
            if (previous == current && (current == '-' || current == '_'))
            {
                return true;
            }
        }

        return false;
    }

    private static int NormalizeLength(int length)
    {
        // This mirrors Math.trunc(options.length || CHROMIUM_DEFAULT_PASSWORD_LENGTH)
        // for the integer API: zero selects the default, then the value is clamped.
        var requestedLength = length == 0 ? DefaultPasswordLength : length;
        return Math.Min(Math.Max(0, requestedLength), MaxPasswordLength);
    }

    private readonly record struct GeneratedCharacter(
        char Character,
        PasswordCharacterClassKey Source);
}
