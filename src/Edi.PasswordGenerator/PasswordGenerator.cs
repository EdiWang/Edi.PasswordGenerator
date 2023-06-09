using System.Security.Cryptography;

namespace Edi.PasswordGenerator;

public class DefaultPasswordGenerator : IPasswordGenerator
{
    public string GeneratePassword(PasswordRule rule = null)
    {
        return rule == null ? GeneratePasswordDefault() : GeneratePasswordAspNetMembership(rule.Length, rule.NumberOfNonAlphanumericCharacters);
    }

    // https://referencesource.microsoft.com/#System.Web/Security/Membership.cs,fe744ec40cace139
    private static readonly char[] Punctuations = "!@#$%^&*()_-+=[{]};:>|./?".ToCharArray();

    public static string GeneratePasswordAspNetMembership(int length, int numberOfNonAlphanumericCharacters)
    {
        if (length < 1 || length > 128)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        if (numberOfNonAlphanumericCharacters > length || numberOfNonAlphanumericCharacters < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(numberOfNonAlphanumericCharacters));
        }

        string password;
        int index;

        do
        {
            var buf = new byte[length];
            var cBuf = new char[length];
            var count = 0;

            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(buf);

            for (int iter = 0; iter < length; iter++)
            {
                int i = (int)(buf[iter] % 87);
                switch (i)
                {
                    case < 10:
                        cBuf[iter] = (char)('0' + i);
                        break;
                    case < 36:
                        cBuf[iter] = (char)('A' + i - 10);
                        break;
                    case < 62:
                        cBuf[iter] = (char)('a' + i - 36);
                        break;
                    default:
                        cBuf[iter] = Punctuations[i - 62];
                        count++;
                        break;
                }
            }

            if (count < numberOfNonAlphanumericCharacters)
            {
                int j;
                var rand = new Random();

                for (j = 0; j < numberOfNonAlphanumericCharacters - count; j++)
                {
                    int k;
                    do
                    {
                        k = rand.Next(0, length);
                    }
                    while (!char.IsLetterOrDigit(cBuf[k]));

                    cBuf[k] = Punctuations[rand.Next(0, Punctuations.Length)];
                }
            }

            password = new(cBuf);
        }
        while (IsDangerousString(password, out index));

        return password;
    }

    private static readonly char[] StartingChars = { '<', '&' };
    private static bool IsDangerousString(string s, out int matchIndex)
    {
        //bool inComment = false;
        matchIndex = 0;

        for (int i = 0; ;)
        {
            // Look for the start of one of our patterns
            int n = s.IndexOfAny(StartingChars, i);

            // If not found, the string is safe
            if (n < 0) return false;

            // If it's the last char, it's safe
            if (n == s.Length - 1) return false;

            matchIndex = n;

            switch (s[n])
            {
                case '<':
                    // If the < is followed by a letter or '!', it's unsafe (looks like a tag or HTML comment)
                    if (IsAtoZ(s[n + 1]) || s[n + 1] == '!' || s[n + 1] == '/' || s[n + 1] == '?') return true;
                    break;
                case '&':
                    // If the & is followed by a #, it's unsafe (e.g. &#83;)
                    if (s[n + 1] == '#') return true;
                    break;
            }

            // Continue searching
            i = n + 1;
        }
    }

    private static bool IsAtoZ(char c)
    {
        return c is >= 'a' and <= 'z' or >= 'A' and <= 'Z';
    }

    private static string GeneratePasswordDefault()
    {
        string engLower = "aquickbrownfoxjumpedoverthelazydog";
        var engUpperArray = engLower.ToUpper().ToArray();

        var letters = engLower.ToArray().Concat(engUpperArray).ToArray();
        var nums = Enumerable.Range(0, 10).ToArray();
        var chars = new[] { '@', '$', '!', '%', '*', '#', '?', '&' };

        var pwdArray = new char[10];
        for (var i = 0; i < 6; i++)
        {
            var posL = RandomNumberGenerator.GetInt32(0, letters.Length);
            pwdArray[i] = (letters[posL]);
        }
        for (var i = 0; i < 3; i++)
        {
            var posN = RandomNumberGenerator.GetInt32(0, nums.Length);
            pwdArray[6 + i] = (char)(nums[posN] + 48);
        }
        var posC = RandomNumberGenerator.GetInt32(0, chars.Length);
        pwdArray[9] = chars[posC];

        var rndArray = pwdArray.OrderBy(p => Guid.NewGuid()).ToArray();
        var password = new string(rndArray);

        return password;
    }
}