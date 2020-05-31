using Leaf.xNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;
using System.Net;
using System.Web;
using SteamWeb;
using System.Net.Http;
using Newtonsoft.Json;

namespace Steamer
{
    class CodeGen
    {
        public static string Wallet()
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string Code = "";
            var random = new Random();
            for (int i = 0; i < 15; i++)
                Code += chars[random.Next(chars.Length)];
            return Code.Substring(0, 5) + "-" + Code.Substring(5, 5) + "-" + Code.Substring(10, 5);
        }
    }
    class Checker
    {
        public static string ParseResponse(string response, string attritube, char left, char right)
        {
            if (response.Contains(attritube))
            {
                int start = response.IndexOf(attritube) + attritube.Length;
                if (response[start] == left)
                {
                    int end = response.IndexOf(right, start + 1);
                    int length = end - start - 1;
                    return response.Substring(start + 1, length);
                }
            }
            return "";
        }

        public static async Task DoSteamAsync()
        {
            Web steamWeb = new Web();
            await steamWeb.Login("valerycheck1", "*Cpt_Shad*");
            if (steamWeb.LoginSuccess)
                CheckWalletCode(CodeGen.Wallet(), steamWeb.Cookies);
        }
        public static bool CheckWalletCode(string Code, CookieCollection Cookies)
        {
            string postdata = $"wallet_code={Code}&{Cookies["sessionid"].ToString()}";
            Leaf.xNet.HttpRequest m_HttpClient = new Leaf.xNet.HttpRequest();
            m_HttpClient.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.61 Safari/537.36";
            m_HttpClient.AddHeader("Host", "store.steampowered.com");
            m_HttpClient.AddHeader("X-Requested-With", "XMLHttpRequest");
            m_HttpClient.UseCookies = true;
            m_HttpClient.AllowAutoRedirect = false;
            m_HttpClient.Cookies = new CookieStorage();
            m_HttpClient.Cookies.Set(Cookies);
            m_HttpClient.Cookies.Set("https://store.steampowered.com/account/validatewalletcode/", Web.steamLoginSecure);

            var response = m_HttpClient.Post("https://store.steampowered.com/account/validatewalletcode/", postdata, "application/x-www-form-urlencoded; charset=UTF-8");
            dynamic eval = JsonConvert.DeserializeObject(response.ToString());
            if (eval.success == 1 && eval.detail == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    class Program
    {
        public static void GetInput()
        {

        }
        public static void PrintTitle()
        {

        }
        static void Main(string[] args)
        {
            PrintTitle();
            Checker.DoSteamAsync();
            Colorful.Console.ReadLine();
            Environment.Exit(0);
        }
    }
}
