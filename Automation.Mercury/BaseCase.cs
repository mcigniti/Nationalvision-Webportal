using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Management;
using System.Linq;


namespace Automation.Mercury
{
    /// <summary>
    /// Abstracts Common Test Case functionality. 
    /// Derived classes should implement specifics
    /// </summary>
    public abstract class BaseCase
    {
        /// <summary>
        /// Gets or Sets Driver
        /// </summary>
        public RemoteWebDriver Driver { get; set; }

        public String IterationTestCaseName { get; set; }

        /// <summary>
        /// Gets or Sets Reporter
        /// </summary>
        public Report.Iteration Reporter { get; set; }

        /// <summary>
        /// Gets or Sets Step
        /// </summary>
        protected string Step
        {
            get
            {
                //TODO: Get should go away
                return Reporter.Chapter.Step.Title;
            }
            set
            {
                Reporter.Add(new Report.Step(value));
            }
        }

        /// <summary>
        /// Gets or Sets Identity of Test Case
        /// </summary>
        public string TestCaseId { get; set; }

        /// <summary>
        /// Gets or Sets Identity of Test Data
        /// </summary>
        public string TestDataId { get; set; }

        /// <summary>
        /// Gets or Sets Test Data as Dictionary<string, string>
        /// </summary>
        public Dictionary<string, string> TestData { get; set; }

        /// <summary>
        /// Returns location where resutls stored.
        /// </summary>
        public string resultsPath = string.Empty;

        
        /// <summary>
        /// Executes Test Cases
        /// </summary>
        public void Execute(Dictionary<String, String> browserConfig,
            String testCaseId,
            String iterationId,
            Report.Iteration iteration,
            Dictionary<String, String> testData,
            Report.Engine reportEngine,
            string iterationTestCasename)
        {
            try
            {

                this.Driver = Util.GetDriver(browserConfig);
                this.Reporter = iteration;
                this.TestCaseId = testCaseId;
                this.TestDataId = iterationId;
                this.TestData = testData;
                this.resultsPath = reportEngine.ReportPath;
                this.IterationTestCaseName = iterationTestCasename;

                if (browserConfig["target"] == "local")
                {
                    var wmi = new ManagementObjectSearcher("select * from Win32_OperatingSystem").Get().Cast<ManagementObject>().First();

                    this.Reporter.Browser.PlatformName = String.Format("{0} {1}", ((string)wmi["Caption"]).Trim(), (string)wmi["OSArchitecture"]);
                    this.Reporter.Browser.PlatformVersion = ((string)wmi["Version"]);
                    this.Reporter.Browser.BrowserName = Driver.Capabilities.BrowserName;
                    this.Reporter.Browser.BrowserVersion = Driver.Capabilities.Version.Substring(0,2);

                }
                else
                {
                    this.Reporter.Browser.PlatformName = browserConfig.ContainsKey("os") ? browserConfig["os"] : browserConfig["device"];
                    this.Reporter.Browser.PlatformVersion = browserConfig.ContainsKey("os_version") ? browserConfig["os_version"] : browserConfig.ContainsKey("realMobile") ? "Real" : "Emulator";
                    this.Reporter.Browser.BrowserName = browserConfig.ContainsKey("browser") ? browserConfig["browser"] : "Safari";
                    this.Reporter.Browser.BrowserVersion = browserConfig.ContainsKey("browser_version") ? browserConfig["browser_version"].Substring(0,2) : "";
                }

                // Does Seed having anything?
                if (this.Reporter.Chapter.Steps.Count == 0)
                    this.Reporter.Chapters.RemoveAt(0);

                this.Reporter.StartTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));


                ExecuteTestCase();
            }
           catch (Exception ex)
            {
              
                this.Reporter.Chapter.Step.Action.IsSuccess = false;
                this.Reporter.Chapter.Step.Action.Extra =  "Exception Message : " + ex.Message + "<br/>" + ex.InnerException + ex.StackTrace;
            }
            finally
            {

                this.Reporter.IsCompleted = true;

                // If current iteration is a failure, get screenshot

                this.Reporter.EndTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

                try
                {
                    string s = Driver.CurrentWindowHandle;

                    ITakesScreenshot iTakeScreenshot = Driver;
                    this.Reporter.Screenshot = iTakeScreenshot.GetScreenshot().AsBase64EncodedString;

                    lock (reportEngine)
                    {
                        reportEngine.PublishIteration(this.Reporter);
                        reportEngine.Summarize(false);
                    }
                    Driver.Quit();
                }
                catch (Exception e)
                {
                    if (!(e.ToString().Contains("target window already closed") || e.ToString().Contains("chrome not reachable") || e.ToString().Contains("unexpected alert open") ||
                        (e.ToString().Contains("timed out after 60 seconds."))))
                    {
                        // If current iteration is a failure, get screenshot
                        if (!Reporter.IsSuccess)
                        {
                            ITakesScreenshot iTakeScreenshot = Driver;
                            this.Reporter.Screenshot = iTakeScreenshot.GetScreenshot().AsBase64EncodedString;
                        }
                    }
                    else
                    {
                        this.Reporter.Chapter.Step.Action.Extra = "Browser closed unexpectedly or chrome not reachable or unexpected alert open/not handled the alert";
                        lock (reportEngine)
                        {
                            reportEngine.PublishIteration(this.Reporter);
                            reportEngine.Summarize(false);
                        }
                        Driver.Quit();
                    }
                }

            }
        }

        /// <summary>
        /// Executes Test Case, should be overriden by derived
        /// </summary>
        protected virtual void ExecuteTestCase()
        {
            Reporter.Add(new Report.Chapter("Execute Test Case"));
        }

        /// <summary>
        /// Gets or Sets Chapter
        /// </summary>
        protected string Chapter
        {
            get
            {
                //TODO: Get should go away
                return Reporter.Chapter.Title;
            }
            set
            {
                Reporter.Add(new Report.Chapter(value));
            }
        }


        #region USER CREDENTAILS
        // ** Below veriables are used for to create new account
        protected string UserName = "mdarabastu";
        protected string Password = "nationalvision";
        #endregion

    }
}
