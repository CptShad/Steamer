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

        static SteamClient steamClient;
        static CallbackManager manager;

        static SteamUser steamUser;
        static SteamUser.LogOnDetails logonDetails;

        static bool isRunning;

        static string user, pass;

        public static string Token;
        public static string TokenSecure;
        public static string SessionId;

        public static void Connect()
        { 
            // save our logon details
            user = "valerycheck1";
            pass = "*Cpt_Shad*";

            // create our steamclient instance
            steamClient = new SteamClient();
            // create the callback manager which will route callbacks to function calls
            manager = new CallbackManager(steamClient);

            // get the steamuser handler, which is used for logging on after successfully connecting
            steamUser = steamClient.GetHandler<SteamUser>();

            // register a few callbacks we're interested in
            // these are registered upon creation to a callback manager, which will then route the callbacks
            // to the functions specified
            manager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
            manager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);

            manager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
            manager.Subscribe<SteamUser.LoggedOffCallback>(OnLoggedOff);

            manager.Subscribe<SteamUser.LoginKeyCallback>(OnLoginIn);
            manager.Subscribe<SteamUser.SessionTokenCallback>(SessionToken);

            isRunning = true;

            Console.WriteLine("Connecting to Steam...");

            // initiate the connection
            steamClient.Connect();

            // create our callback handling loop
            while (isRunning)
            {
                // in order for the callbacks to get routed, they need to be handled by the manager
                manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
            }
        }

        static void OnConnected(SteamClient.ConnectedCallback callback)
        {
            Console.WriteLine("Connected to Steam! Logging in '{0}'...", user);

            steamUser.LogOn(new SteamUser.LogOnDetails
            {
                Username = user,
                Password = pass,
            });
        }

        static void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            Console.WriteLine("Disconnected from Steam");

            isRunning = false;
        }

        static void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            if (callback.Result != EResult.OK)
            {
                if (callback.Result == EResult.AccountLogonDenied)
                {
                    // if we recieve AccountLogonDenied or one of it's flavors (AccountLogonDeniedNoMailSent, etc)
                    // then the account we're logging into is SteamGuard protected
                    // see sample 5 for how SteamGuard can be handled

                    Console.WriteLine("Unable to logon to Steam: This account is SteamGuard protected.");

                    isRunning = false;
                    return;
                }

                Console.WriteLine("Unable to logon to Steam: {0} / {1}", callback.Result, callback.ExtendedResult);

                isRunning = false;
                return;
            }


            Console.WriteLine("Successfully logged on!");
            Console.WriteLine($"Logon result: {callback.Result} {callback.ExtendedResult}");
            //SteamUser.LogOnDetails logonDetails = steamClient.GetHandler<SteamUser.LogOnDetails>();
            Console.WriteLine("Steam ID {0}", steamClient.SteamID);
            Console.WriteLine("Universe {0}", steamClient.Universe);
            Console.WriteLine("SessionID {0}", steamClient.SessionID);
            Console.WriteLine("Cell ID {0}", steamClient.CellID);
            Console.WriteLine("Session Token {0}", steamClient.SessionToken);
            //Console.WriteLine("Steam ID {0}", steamUser.SteamID);
            //Console.WriteLine("Steam Token {0}", steamUser.SessionToken);
            //Console.WriteLine("Cell ID {0}", steamUser.CellID);

            // at this point, we'd be able to perform actions on Steam

            // for this sample we'll just log off
            //Console.ReadLine();
            //steamUser.LogOff();
        }

        static void OnLoggedOff(SteamUser.LoggedOffCallback callback)
        {
            Console.WriteLine("Logged off of Steam: {0}", callback.Result);
        }
        static void OnLoginIn(SteamUser.LoginKeyCallback callback)
        {
            Console.WriteLine("Login Key : {0}", callback.LoginKey);
            //GetCookies(callback.UniqueID.ToString(), callback.LoginKey);
        }
        static void SessionToken(SteamUser.SessionTokenCallback callback)
        {
            Console.WriteLine("Session Token : {0}", callback.SessionToken);
        }

        public static async Task DoSteamAsync()
        {
            Web steamWeb = new Web();
            await steamWeb.Login("valerycheck1", "*Cpt_Shad*");
            if (steamWeb.LoginSuccess)
                CheckWalletCode(CodeGen.Wallet(), steamWeb.Cookies);
        }
        public static void CheckWalletCode(string Code, CookieCollection Cookies)
        {
            string postdata = $"wallet_code={Code}&sessionid={Cookies["sessionid"].ToString()}";
            Leaf.xNet.HttpRequest m_HttpClient = new Leaf.xNet.HttpRequest();
            m_HttpClient.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.61 Safari/537.36";
            m_HttpClient.AddHeader("Host", "store.steampowered.com");
            m_HttpClient.AddHeader("X-Requested-With", "XMLHttpRequest");
            m_HttpClient.Cookies.Set(Cookies);

            var response = m_HttpClient.Post("https://store.steampowered.com/account/validatewalletcode/", postdata, "application/x-www-form-urlencoded; charset=UTF-8");
            Console.WriteLine(response);    
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
