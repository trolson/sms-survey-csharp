using System;
using System.Threading.Tasks;
using Bandwidth.Net;
using Bandwidth.Net.Api;
using System.Net;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.IO;
using System.Net.Http;
using HtmlAgilityPack;
using System.Linq;
using Bandwidth.Net.Xml;
using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace Send_SMS_Practice
{
    public class Program
    {
        private const string UserId = ""; //{user_id}
        private const string Token = ""; //{token}
        private const string Secret = ""; //{secret}
        private const string pageURL = ""; //requestb.in url
        private const string outNumber = ""; //bandwidth app phone number

        private static string phoneNumber = ""; //leave blank
        private static bool stop = false;
        private static string MESSAGEID = ""; //leave blank


        public static void Main()
        {
            //first load question pack
            var lineCount = File.ReadLines("").Count(); //enter file path in between quotes
            var textLines = File.ReadAllLines(""); //enter file path in between quotes
            //Console.Out.WriteLine(lineCount);
            for (int i = 0; i < lineCount; i++)
            {
                Console.WriteLine(textLines[i]);
            }

            Console.Out.WriteLine();
            Console.Out.Write("Please enter the phone number of who you would like to send a message to: +1");
            phoneNumber = "+1" + Console.In.ReadLine();
            Console.Out.WriteLine();

            for (int i = 0; i <= lineCount; i++)
            {
                string message = "";
                if (i < lineCount)
                {
                    message = textLines[i];
                }
                else
                {
                    message = "Thank you for taking our survey. Further texts to this number will not invoke a reply.";
                }

                Console.Out.WriteLine("{0}: {1}", outNumber, message);
                //String message = Console.In.ReadLine();

                prepareSendMessage(phoneNumber, message);
                wait();
                if (i != lineCount)
                {
                    loadMessage();
                }
            }

        }

        private static async Task RunAsync(String phoneNumber, String message)
        {
            var client = new Client(UserId, Token, Secret);

            var sms = await client.Message.SendAsync(new MessageData
            {
                From = outNumber, // <-- This must be a Bandwidth number on your account
                To = phoneNumber,
                Text = message,
                CallbackUrl = pageURL
            });
        }

        private static void prepareSendMessage(string phoneNumber, string message)
        {
            try
            {
                RunAsync(phoneNumber, message).Wait();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                Environment.ExitCode = 1;
            }
        }

        private static void loadMessage()
        {
            string s = string.Empty;

            // loading html into HtmlDocument
            var doc = new HtmlWeb().Load(pageURL + "?inspect");
            var htmlNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'request-body')]").First().InnerText;

            //format string for json object
            string formatted = htmlNodes.Replace("&#34;", "\"");

            //Parse Json object...            
            var messageText = JObject.Parse(formatted)["text"];
            var direction = JObject.Parse(formatted)["direction"];
            var messageId = JObject.Parse(formatted)["messageId"];


            //check if it is inbound or outbound
            //if outbound, do nothing - already displayed on screen
            if (checkIfInbound(direction.ToString()) && messageId.ToString() != MESSAGEID) //make sure inbound message doesnt have same id as previous
            {
                Console.Out.WriteLine("{0}: {1}", phoneNumber, messageText);
                //need to save message text responses
                MESSAGEID = messageId.ToString();
            }
            else
            {
                loadMessage(); //if not new message, keep loading until new
            }

            return;
        }

        private static Boolean checkIfInbound(String direction)
        {
            if (direction == "in")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void wait()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (true)
            {
                if (stopwatch.ElapsedMilliseconds >= 2000)
                {
                    break;
                }
            }
        }
    }
}
