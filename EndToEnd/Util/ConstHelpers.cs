using System;
using System.Configuration;

namespace EndToEnd.Util
{
    public static class ConstHelpers
    {
        public static string Str_ADDomainName = Convert.ToString(ConfigurationManager.AppSettings["ADDomainName"]);
        public static string Str_ADLDAPPath = Convert.ToString(ConfigurationManager.AppSettings["ADLDAPPath"]);
        public static string Str_ADUserName = Convert.ToString(ConfigurationManager.AppSettings["ADUserName"]);
        public static string Str_ADPassword = Convert.ToString(ConfigurationManager.AppSettings["ADPassword"]);

        public const string Str_ADKey_AcctName = "samaccountname";
        public const string Str_ADKey_Mail = "mail";
        public const string Str_ADKey_UsrAcctCtrl = "useraccountcontrol";
        public const string Str_ADFilter = "(&(objectClass=user)(objectCategory=person))";


        //Mail Configuration
        public static string Mail_FromAddress= Convert.ToString(ConfigurationManager.AppSettings["Mail_FromAddress"]);
        public static string Mail_FromAddressPassword = Convert.ToString(ConfigurationManager.AppSettings["Mail_FromAddressPassword"]);
        public static string Mail_Subject = Convert.ToString(ConfigurationManager.AppSettings["Mail_Subject"]);
        public static string Mail_Host = Convert.ToString(ConfigurationManager.AppSettings["Mail_Host"]);
        public static string Mail_EnableSSL = Convert.ToString(ConfigurationManager.AppSettings["Mail_EnableSSL"]);
        public static string Mail_Port = Convert.ToString(ConfigurationManager.AppSettings["Mail_Port"]);
    }
}