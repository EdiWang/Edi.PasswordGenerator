using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Edi.PasswordGenerator;

public class DefaultPasswordGenerator : IPasswordGenerator
{
    public string GeneratePassword(PasswordRule rule = null)
    {
        if (rule == null)
        {
            return GeneratePasswordDefault();
        }

        throw new NotImplementedException();
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