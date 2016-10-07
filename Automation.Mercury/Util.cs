using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Safari;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace Automation.Mercury
{
    public class Util
    {
        private static Dictionary<string, Locator> locators = new Dictionary<string, Locator>();
        private static Dictionary<string, string> commonTestData = new Dictionary<string, string>();
        private static Dictionary<string, string> environmentSettings = new Dictionary<string, string>();

        /// <summary>
        /// Gets settings for current environment
        /// </summary>
        public static Dictionary<string, string> EnvironmentSettings
        {
            get
            {
                String environment = ConfigurationManager.AppSettings.Get("Environment");
                if (environmentSettings.Count > 0) return environmentSettings;
                String[] KeyValue = null;

                lock (environmentSettings)
                {
                    foreach (String setting in ConfigurationManager.AppSettings.Get(environment).Split(new Char[] { ';' }))
                    {
                        KeyValue = setting.Split(new Char[] { '=' }, 2);
                        if (KeyValue.Length > 1)
                        {
                            environmentSettings.Add(KeyValue[0].Trim(), KeyValue[1].Trim());
                        }
                    }
                }
                return environmentSettings;
            }
        }

        /// <summary>
        /// Gets all iteration related test data of specified test case
        /// </summary>
        /// <param name="testCaseId"></param>
        /// <returns></returns>
        public static DataTable GetIterationsTestData(String location, String testCaseId)
        {
            lock (commonTestData)
            {
                if (commonTestData.Count == 0) LoadCommonTestData(location);
            }

            String[] foundFiles = Directory.GetFiles(location, String.Format("{0}.csv", testCaseId), SearchOption.AllDirectories);

            if (foundFiles.Length == 0)
                throw new FileNotFoundException(String.Format("Test Data file not found at {0}", location), String.Format("{0}.csv", testCaseId));

            DataTable tableTestData = ReadCSVContent("", foundFiles[0]);
            foreach (DataRow eachRow in tableTestData.Rows)
            {
                foreach (DataColumn eachColumn in tableTestData.Columns)
                {
                    if (eachRow[eachColumn].ToString().StartsWith("#"))
                    {
                        eachRow[eachColumn] = commonTestData[eachRow[eachColumn].ToString().Replace("#", "")];
                    }
                }
            }
            return tableTestData;
        }

        /// <summary>
        /// Gets all iteration related test data of specified test case
        /// </summary>
        /// <param name="testCaseId"></param>
        /// <returns></returns>
        public static DataTable GetIterationTestData(String location, String testCaseId)
        {
            lock (commonTestData)
            {
                if (commonTestData.Count == 0) LoadCommonTestData(location);
            }

            String[] foundFiles = Directory.GetFiles(location, String.Format("{0}.csv", testCaseId), SearchOption.AllDirectories);

            if (foundFiles.Length == 0)
                throw new FileNotFoundException(String.Format("Test Data file not found at {0}", location), String.Format("{0}.csv", testCaseId));

            DataTable tableTestData = ReadCSVContent("", foundFiles[0]);
           
            foreach( DataRow eachRow in tableTestData.Rows)
            {
                foreach (DataColumn eachColumn in tableTestData.Columns)
                {
                    if (eachRow[eachColumn].ToString().StartsWith("#"))
                    {
                        eachRow[eachColumn] = commonTestData[eachRow[eachColumn].ToString().Replace("#", "")];
                    }
                }
            }
            return tableTestData;
        }

        /// <summary>
        /// Loads Common Test Data from Common.csv
        /// </summary>
        /// <returns></returns>
        public static void LoadCommonTestData(String location)
        {
            String ColumnValue = String.Empty;
            DataTable tableCommonData = ReadCSVContent(location, EnvironmentSettings["CommonData"]);

            foreach (DataRow eachRow in tableCommonData.Rows)
            {
                commonTestData.Add(eachRow["Key"].ToString(), eachRow["Value"].ToString());
            }
        }

        /// <summary>
        /// Loads specified CSV content to DataTable
        /// </summary>
        /// <param name="filename">Filename of CSV</param>
        /// <returns>DataTable</returns>
        public static DataTable ReadCSVContent(String fileDirectory, String filename)
        {
            DataTable table = new DataTable();
            int temp = 0;

            foreach (String fName in filename.Split(','))
            {
                string[] lines = File.ReadAllLines(Path.Combine(fileDirectory, fName));

                if (temp == 0)
                {
                    temp = 1;
                    // identify columns
                    foreach (String columnName in lines[0].Split(new char[] { ',' }))
                    {
                        table.Columns.Add(columnName, typeof(String));
                    }
                }
                foreach (String data in lines.Where((val, index) => index != 0))
                {
                    table.Rows.Add(data.Split(new Char[] { ',' }));
                }
            }
            return table;
        }


        /// <summary>
        /// Loads specified CSV content to DataTable
        /// </summary>
        /// <param name="filename">Filename of CSV</param>
        /// <returns>DataTable</returns>
        public static DataTable ReadFirstLineOfCSVContent(String fileDirectory, String filename)
        {
            DataTable table = new DataTable();
            int temp = 0;

            foreach (String fName in filename.Split(','))
            {
                string[] lines = File.ReadAllLines(Path.Combine(fileDirectory, fName));

               

                if (temp == 0)
                {
                    temp = 1;
                    // identify columns
                    foreach (String columnName in lines[0].Split(new char[] { ',' }))
                    {
                        table.Columns.Add(columnName, typeof(String));
                    }
                }
                foreach (String data in lines.Where((val, index) => index ==1))
                {
                    table.Rows.Add(data.Split(new Char[] { ',' }));
                }
            }
            return table;
        }

        /// <summary>
        /// Utility functions that wraps object repository.
        /// Loads and maintains object locators
        /// </summary>
        /// <param name="name">Name of locator</param>
        /// <returns><see cref="By"/></returns>
        public static Locator GetLocator(String name)
        {
            if (locators.Count == 0)
            {
                lock (locators)
                {

                    // load all for one time
                    XmlDocument objectRepository = new XmlDocument();
                    //TODO: Assume ObjectRepository is always @ exe location. Set project build to deploy it to bin
                    objectRepository.Load(Path.Combine(Directory.GetParent(Assembly.GetEntryAssembly().Location).ToString(), "Objects.xml"));

                    foreach (XmlNode page in objectRepository.SelectNodes("/PageFactory/page"))
                    {
                        foreach (XmlNode eachObject in page.ChildNodes)
                        {
                            Locator locator = null;

                            switch (eachObject.SelectSingleNode("identifyBy").InnerText.ToLower())
                            {
                                case "linktext":
                                    locator = Locator.Get(LocatorType.LinkText, eachObject.SelectSingleNode("value").InnerText);
                                    break;

                                case "id":
                                    locator = Locator.Get(LocatorType.ID, eachObject.SelectSingleNode("value").InnerText);
                                    break;

                                case "xpath":
                                    locator = Locator.Get(LocatorType.XPath, eachObject.SelectSingleNode("value").InnerText);
                                    break;

                                case "classname":
                                    locator = Locator.Get(LocatorType.ClassName, eachObject.SelectSingleNode("value").InnerText);
                                    break;

                                case "name":
                                    locator = Locator.Get(LocatorType.Name, eachObject.SelectSingleNode("value").InnerText);
                                    break;

                                case "css":
                                    locator = Locator.Get(LocatorType.CSS, eachObject.SelectSingleNode("value").InnerText);
                                    break;
                            }
                            
                            //Console.WriteLine(eachObject.SelectSingleNode("name").InnerText);
                            locators.Add(eachObject.SelectSingleNode("name").InnerText, locator);
                        }
                    }
                }
            }
            
            return locators[name];
        }

        /// <summary>
        /// Gets Browser related configuration data from App.Config
        /// </summary>
        /// <param name="browserId">Identity of Browser</param>
        /// <returns><see cref="Dictionary<String, String>"/></returns>
        public static Dictionary<String, String> GetBrowserConfig(String browserId)
        {
            Dictionary<String, String> config = new Dictionary<string, string>();
            String[] KeyValue = null;

            foreach (String attribute in ConfigurationManager.AppSettings.Get(browserId).Split(new Char[] { ';' }))
            {
                KeyValue = attribute.Split(new Char[] { ':' });
                config.Add(KeyValue[0].Trim(), KeyValue[1].Trim());
            }
            return config;
        }

        /// <summary>
        /// Prepares RemoteWebDriver basing on configuration supplied
        /// </summary>
        /// <param name="browserConfig"></param>
        /// <returns></returns>
        public static RemoteWebDriver GetDriver(Dictionary<String, String> browserConfig)
        {
            RemoteWebDriver driver = null;

            //exePath get the assembly location it used for know the exe drivers location for other than FF browser
            string exePath = Directory.GetParent(Assembly.GetEntryAssembly().Location).ToString();

            try
            {
                if (browserConfig["target"] == "local")
                {
                    if (browserConfig["browser"] == "Firefox")
                    {
                        driver = new FirefoxDriver();
                    }
                    else if (browserConfig["browser"] == "IE")
                    {
                        //TODO: Get rid of Framework Path
                        driver = new InternetExplorerDriver(exePath);

                        //DesiredCapabilities capabilities = new DesiredCapabilities();
                        //capabilities.setCapability(CapabilityType.ACCEPT_SSL_CERTS, true);
                        //driver = new InternetExplorerDriver(capabilities);
                    }
                    else if (browserConfig["browser"] == "Chrome")
                    {
                        ChromeOptions options = new ChromeOptions();
                        //options.AddArgument("no-sandbox");
                        options.AddArguments("chrome.switches", "--disable-extensions");
                        driver = new ChromeDriver(exePath,options);
                    }
                    else if (browserConfig["browser"] == "PhantomJS")
                    {
                        PhantomJSDriverService service = PhantomJSDriverService.CreateDefaultService(exePath);
                        service.IgnoreSslErrors = true;
                        driver = new PhantomJSDriver(service);
                    }
                    else if (browserConfig["browser"] == "MSEdge")
                    {
                        EdgeOptions options = new EdgeOptions();
                        options.PageLoadStrategy = EdgePageLoadStrategy.Eager;
                        driver = new EdgeDriver(exePath, options);
                    }
                    else if (browserConfig["browser"] == "Safari")
                    {
                        driver = new SafariDriver();
                    }

                    driver.Manage().Window.Maximize();
                    //driver.Manage().Window.Size = new Size(680,1000); 
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
                }
                else if (browserConfig["target"] == "browserstack")
                {
                    DesiredCapabilities desiredCapabilities = new DesiredCapabilities();

                    String[] bsCredentials = ConfigurationManager.AppSettings.Get("BrowserStackCredentials").Split(new Char[] { ':' });
                    desiredCapabilities.SetCapability("browserstack.user", bsCredentials[0].Trim());
                    desiredCapabilities.SetCapability("browserstack.key", bsCredentials[1].Trim());

                    foreach (KeyValuePair<String, String> capability in browserConfig)
                    {
                        if (capability.Key != "target")
                            desiredCapabilities.SetCapability(capability.Key, capability.Value);
                    }

                    driver = new RemoteWebDriver(new Uri("http://hub.browserstack.com/wd/hub/"), desiredCapabilities);
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
                }
                driver.Manage().Cookies.DeleteAllCookies();
            }
            catch (Exception ex)
            {
                // this is not a good idea to suppress
                // we should better handle the situation where RemoteWebDriver initiation fail,
                // should not let entire execution halt
            }
            return driver;
        }

        /// <summary>
        /// Replaces first occurence
        /// </summary>
        /// <param name="s"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static string ReplaceFirstOccurrence(string s, string oldValue, string newValue)
        {
            int i = s.IndexOf(oldValue);
            return s.Remove(i, oldValue.Length).Insert(i, newValue);
        }
    }
}
