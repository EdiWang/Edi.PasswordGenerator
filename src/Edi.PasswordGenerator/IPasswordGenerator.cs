namespace Edi.PasswordGenerator;

public interface IPasswordGenerator
{
    string GeneratePassword(PasswordRule rule = null);
}

public class PasswordRule
{
    public PasswordRule(int length, int numberOfNonAlphanumericCharacters)
    {
        Length = length;
        NumberOfNonAlphanumericCharacters = numberOfNonAlphanumericCharacters;
    }

    public int Length { get; set; }

    public int NumberOfNonAlphanumericCharacters { get; set; }
}