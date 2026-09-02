namespace Edi.PasswordGenerator;

public class DefaultPasswordGenerator : IPasswordGenerator
{
    public string GeneratePassword(PasswordRule? rule = null)
    {
        rule ??= new PasswordRule(12, 1);
        return SecurePasswordGenerator.GenerateSecurePassword(
            rule.Length,
            rule.LeastNumberOfNonAlphanumericCharacters);
    }
}
