namespace Edi.PasswordGenerator;

public interface IPasswordGenerator
{
    string GeneratePassword(ChromiumPasswordOptions? options = null);
}
