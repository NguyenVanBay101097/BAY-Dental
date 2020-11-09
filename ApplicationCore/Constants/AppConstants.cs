using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Constants
{
    public static class AppConstants
    {
        public const string LockRequestKeyTemplate = "{0}_{1}";

        public static string GetLockRequestKey(string host, string name)
        {
            host = host.Replace(".", "_");
            return string.Format(LockRequestKeyTemplate, host, name);
        }
    }
}
