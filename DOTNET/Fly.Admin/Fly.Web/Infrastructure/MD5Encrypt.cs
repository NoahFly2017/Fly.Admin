using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fly.Web.Infrastructure
{
    public static class MD5Encrypt
    {
        /// <summary>
        /// md5加密
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public static string Encrypt(string original)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(original, "MD5");//用的是默认的格式
        }
    }
    /// <summary>
    /// 用户密码加密
    /// </summary>
    public static class EncryptionMode
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="password"></param>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public static string MemberEncryption(string password, string loginName)
        {
            return MD5Encrypt.Encrypt(MD5Encrypt.Encrypt(password).ToLower() + loginName).ToLower();
        }
    }
}