namespace Edi.PasswordGenerator;

public interface IPasswordGenerator
{
    string GeneratePassword(PasswordRule rule = null);
}

public class PasswordRule
{
    public PasswordRule(int length, int leastNumberOfNonAlphanumericCharacters)
    {
        Length = length;
        LeastNumberOfNonAlphanumericCharacters = leastNumberOfNonAlphanumericCharacters;
    }

    public int Length { get; set; }

    public int LeastNumberOfNonAlphanumericCharacters { get; set; }
}