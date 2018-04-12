namespace FwNs.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Runtime.InteropServices;

    public static class Xtnz
    {
        public static Dictionary<string, bool> TestSwtch;

        static Xtnz()
        {
            Dictionary<string, bool> dictionary1 = new Dictionary<string, bool>();
            dictionary1.Add("Test0" + 0.ToString(), false);
            dictionary1.Add("Test0" + 1.ToString(), false);
            dictionary1.Add("Test0" + 2.ToString(), false);
            dictionary1.Add("Test0" + 3.ToString(), false);
            dictionary1.Add("Test0" + 4.ToString(), false);
            dictionary1.Add("Test0" + 5.ToString(), false);
            dictionary1.Add("Test0" + 6.ToString(), false);
            dictionary1.Add("Test0" + 7.ToString(), false);
            dictionary1.Add("Test0" + 8.ToString(), false);
            dictionary1.Add("Test0" + 9.ToString(), false);
            dictionary1.Add("Test1" + 0.ToString(), false);
            dictionary1.Add("Test1" + 1.ToString(), false);
            dictionary1.Add("Test1" + 2.ToString(), true);
            dictionary1.Add("Test1" + 3.ToString(), false);
            dictionary1.Add("Test1" + 4.ToString(), false);
            dictionary1.Add("Test1" + 5.ToString(), false);
            dictionary1.Add("Test1" + 6.ToString(), false);
            dictionary1.Add("Test1" + 7.ToString(), false);
            dictionary1.Add("Test1" + 8.ToString(), false);
            dictionary1.Add("Test1" + 9.ToString(), false);
            TestSwtch = dictionary1;
        }

        public static HttpWebResponse HttpWebRspns(string s, out string responseFromServer)
        {
            WebRequest request1 = WebRequest.Create(s);
            request1.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse response = (HttpWebResponse) request1.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            responseFromServer = reader.ReadToEnd();
            reader.Close();
            responseStream.Close();
            response.Close();
            return response;
        }

        public static string JSonText1()
        {
            return "{\\\"itmsEnm\\\":[\r\n                    {\\\"u\\\":{\\\"OriginalPath\\\":\\\"atoz\\\",\\\"FullPath\\\":\\\"C:\\\\Lib\\\\[DwnlLib]\\\\componentsource\\\\atoz\\\"},\\\"rslts\\\":[false,false,true,true],\\\"rslt\\\":true},\r\n                    {\\\"u\\\":{\\\"OriginalPath\\\":\\\"bestsellers\\\",\\\"FullPath\\\":\\\"C:\\\\Lib\\\\[DwnlLib]\\\\componentsource\\\\bestsellers\\\"},\\\"rslts\\\":[false,false,true,true],\\\"rslt\\\":true},\r\n                    {\\\"u\\\":{\\\"OriginalPath\\\":\\\"features\\\",\\\"FullPath\\\":\\\"C:\\\\Lib\\\\[DwnlLib]\\\\componentsource\\\\features\\\"},\\\"rslts\\\":[false,false,true,true],\\\"rslt\\\":true},\r\n                    {\\\"u\\\":{\\\"OriginalPath\\\":\\\"http--dev11.componentsource.com\\\",\\\"FullPath\\\":\\\"C:\\\\Lib\\\\[DwnlLib]\\\\componentsource\\\\http--dev11.componentsource.com\\\"},\\\"rslts\\\":[false,false,true,true],\\\"rslt\\\":true},\r\n                    {\\\"u\\\":{\\\"OriginalPath\\\":\\\"http--ftp.componentsource.com\\\",\\\"FullPath\\\":\\\"C:\\\\Lib\\\\[DwnlLib]\\\\componentsource\\\\http--ftp.componentsource.com\\\"},\\\"rslts\\\":[false,false,true,true],\\\"rslt\\\":true},\r\n                    {\\\"u\\\":{\\\"OriginalPath\\\":\\\"https--ftp.componentsource.com\\\",\\\"FullPath\\\":\\\"C:\\\\Lib\\\\[DwnlLib]\\\\componentsource\\\\https--ftp.componentsource.com\\\"},\\\"rslts\\\":[false,false,true,true],\\\"rslt\\\":true},\r\n                    {\\\"u\\\":{\\\"OriginalPath\\\":\\\"index-zh.html\\\",\\\"FullPath\\\":\\\"C:\\\\Lib\\\\[DwnlLib]\\\\componentsource\\\\index-zh.html\\\"},\\\"rslts\\\":[false,false,true,false],\\\"rslt\\\":true},\r\n                    {\\\"u\\\":{\\\"OriginalPath\\\":\\\"index-zt.html\\\",\\\"FullPath\\\":\\\"C:\\\\Lib\\\\[DwnlLib]\\\\componentsource\\\\index-zt.html\\\"},\\\"rslts\\\":[false,false,true,false],\\\"rslt\\\":true},\r\n                    {\\\"u\\\":{\\\"OriginalPath\\\":\\\"index.html\\\",\\\"FullPath\\\":\\\"C:\\\\Lib\\\\[DwnlLib]\\\\componentsource\\\\index.html\\\"},\\\"rslts\\\":[false,false,true,false],\\\"rslt\\\":true},\r\n                    {\\\"u\\\":{\\\"OriginalPath\\\":\\\"newreleases\\\",\\\"FullPath\\\":\\\"C:\\\\Lib\\\\[DwnlLib]\\\\componentsource\\\\newreleases\\\"},\\\"rslts\\\":[false,false,true,true],\\\"rslt\\\":true},\r\n                    {\\\"u\\\":{\\\"OriginalPath\\\":\\\"news\\\",\\\"FullPath\\\":\\\"C:\\\\Lib\\\\[DwnlLib]\\\\componentsource\\\\news\\\"},\\\"rslts\\\":[false,false,true,true],\\\"rslt\\\":true},\r\n                    {\\\"u\\\":{\\\"OriginalPath\\\":\\\"products\\\",\\\"FullPath\\\":\\\"C:\\\\Lib\\\\[DwnlLib]\\\\componentsource\\\\products\\\"},\\\"rslts\\\":[false,false,true,true],\\\"rslt\\\":true},\r\n                    {\\\"u\\\":{\\\"OriginalPath\\\":\\\"publishers.html\\\",\\\"FullPath\\\":\\\"C:\\\\Lib\\\\[DwnlLib]\\\\componentsource\\\\publishers.html\\\"},\\\"rslts\\\":[false,false,true,false],\\\"rslt\\\":true},\r\n                    {\\\"u\\\":{\\\"OriginalPath\\\":\\\"res\\\",\\\"FullPath\\\":\\\"C:\\\\Lib\\\\[DwnlLib]\\\\componentsource\\\\res\\\"},\\\"rslts\\\":[false,false,true,true],\\\"rslt\\\":true},\r\n                    {\\\"u\\\":{\\\"OriginalPath\\\":\\\"services\\\",\\\"FullPath\\\":\\\"C:\\\\Lib\\\\[DwnlLib]\\\\componentsource\\\\services\\\"},\\\"rslts\\\":[false,false,true,true],\\\"rslt\\\":true},\r\n                    {\\\"u\\\":{\\\"OriginalPath\\\":\\\"topdownloads\\\",\\\"FullPath\\\":\\\"C:\\\\Lib\\\\[DwnlLib]\\\\componentsource\\\\topdownloads\\\"},\\\"rslts\\\":[false,false,true,true],\\\"rslt\\\":true},\r\n                    {\\\"u\\\":{\\\"OriginalPath\\\":\\\"topics\\\",\\\"FullPath\\\":\\\"C:\\\\Lib\\\\[DwnlLib]\\\\componentsource\\\\topics\\\"},\\\"rslts\\\":[false,false,true,true],\\\"rslt\\\":true},\r\n                    {\\\"u\\\":{\\\"OriginalPath\\\":\\\"topreviews\\\",\\\"FullPath\\\":\\\"C:\\\\Lib\\\\[DwnlLib]\\\\componentsource\\\\topreviews\\\"},\\\"rslts\\\":[false,false,true,true],\\\"rslt\\\":true}\r\n                ]}";
        }

        public static string PttrnUrl()
        {
            string str = "((ht|f)tps?)";
            str = str + @"(:\/\/)";
            str = "(" + str + ")";
            string str2 = @"((www|[^\/\.]{0,19})\.){0,5}";
            str2 = str2 + @"([^\/\.]+)" + @"((\.([^\.]{0,9})){0,5})";
            str2 = "(" + str2 + ")";
            string str3 = "[^\\/&\\s%\\\"]{0,199}";
            string[] textArray1 = new string[] { @"(\/?(", str3, @")){0,9}\/((", str3, ")?)" };
            str3 = string.Concat(textArray1);
            str3 = "(" + str3 + "){0,9}";
            str3 = "(" + str3 + ")";
            string text1 = str + str2 + str3;
            return "(((ht|f)tps?)(:\\/\\/))([^&\\s%\\\"]{0,499})";
        }

        public static void Test00()
        {
            throw new Exception();
        }

        public static void Test01()
        {
        }

        public static void Test04()
        {
            throw new Exception();
        }

        public static void Test06()
        {
            throw new Exception();
        }

        public static void Test07()
        {
            throw new Exception();
        }

        public static void Test08()
        {
            throw new Exception();
        }

        public static void Test09()
        {
            throw new Exception();
        }

        public static void Test10()
        {
            throw new Exception();
        }

        public static void Test11()
        {
            throw new Exception();
        }

        public static void Test12()
        {
            throw new Exception();
        }

        public static void Test13()
        {
        }

        public static void Test14()
        {
        }

        public static void Test15()
        {
        }

        public static void Test16()
        {
        }

        public static void Test17()
        {
        }

        public static void Test18()
        {
        }

        public static void Test19()
        {
        }

        public static void Test20()
        {
        }

        public static void Test21()
        {
        }

        public static string WebRspns(string s)
        {
            WebRequest request1 = WebRequest.Create(s);
            request1.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse response = (HttpWebResponse) request1.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader1 = new StreamReader(responseStream);
            string str = reader1.ReadToEnd();
            reader1.Close();
            try
            {
                responseStream.Close();
            }
            catch (Exception exception)
            {
                str = str + "\r\n" + exception.Message;
            }
            response.Close();
            return str;
        }
    }
}

