using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using SimpleBrowser.WebDriver;

namespace HolidayCalendarDownloader
{
    class Program
    {
        private enum ExitCode
        {
            Success,
            InvalidParameters,
            ErrorOccured
        }

        static ExitCode DownloadAndSaveLeave(string[] args)
        {

            if (args.Length != 2)
            {
                Console.WriteLine("Usage HolidayCalendarDownloader.exe <username> <password>");
                return ExitCode.InvalidParameters;
            }

            var userName = args[0];
            var password = args[1];

            try
            {
                var webDriver = new SimpleBrowserDriver();
                webDriver.Navigate().GoToUrl("https://ewassist.co.uk/CC/Areas_Holidays_Cal.aspx?Token=AS&Ar=5");


                var loginForm = webDriver.FindElement(By.Name("ctl00"));
                if (loginForm != null)
                {
                    var loginElement = webDriver.FindElement(By.Name("LoginLogin"));
                    var passwordElement = webDriver.FindElement(By.Name("LoginPassword"));
                    var loginButton = webDriver.FindElement(By.Id("LoginButton_DoLogin"));

                    loginElement.SendKeys(userName);
                    passwordElement.SendKeys(password);

                    loginButton.Click();
                }

                CalendarParser.ParseEWAHtmlString(webDriver.PageSource).Save("Leave.xml");
                return ExitCode.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine();
                Console.WriteLine(ex.StackTrace);
                return ExitCode.ErrorOccured;
            }
        }

        static int Main(string[] args)
        {
            return (int) DownloadAndSaveLeave(args);
        }
    }
}
