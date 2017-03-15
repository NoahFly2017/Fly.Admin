using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Core.Identity
{
    public class PasswordHasher : Microsoft.AspNet.Identity.IPasswordHasher
    {
        private Microsoft.AspNet.Identity.PasswordHasher _passwordHasher;

        public PasswordHasher(Microsoft.AspNet.Identity.PasswordHasher passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        ///     生成密文字符串。
        /// </summary>
        /// <param name="password">明文字符串</param>
        /// <returns>密文字符串</returns>
        public virtual string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(password);
        }

        /// <summary>
        ///     验证明文字符串是否与密文匹配。
        /// </summary>
        /// <param name="hashedPassword">密文字符串</param>
        /// <param name="providedPassword">明文字符串</param>
        /// <returns></returns>
        public virtual PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            return _passwordHasher.VerifyHashedPassword(hashedPassword, providedPassword);
        }
    }
}
