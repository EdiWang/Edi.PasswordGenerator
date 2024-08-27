namespace Edi.PasswordGenerator;

public class DefaultPasswordGenerator : IPasswordGenerator
{
    public string GeneratePassword(PasswordRule rule = null)
    {
        return rule == null ? 
            SecurePasswordGenerator.GenerateSecurePassword() : 
            AspNetMembershipPasswordGenerator.GeneratePassword(rule.Length, rule.LeastNumberOfNonAlphanumericCharacters);
    }
}