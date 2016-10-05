using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.IO;
using System.Drawing.Imaging;
using System.Net;


namespace Automation.Mercury
{
    /// <summary>
    /// Mercury is wrapper around Selenium that abstracts many functions
    /// 
    /// Selenium is produced from Selenides (Salts of Selenium)
    /// Mercury Selenide [HgSe] is one of Selenide
    /// </summary>
    public static class Selenide
    {

        public static class Browser
        {
            /// <summary>
            /// Gets value indicating whether the current associated browser is Internet Explorer or not
            /// </summary>
            public static bool isInternetExplorer(RemoteWebDriver driver)
            {

                return (driver.Capabilities.BrowserName.ToUpper().Equals("IE") ||
                        driver.Capabilities.BrowserName.ToUpper().Equals("INTERNET EXPLORER"))
                    ? true
                    : false;

            }

            /// <summary>
            /// Gets value indicating whether the current associated browser is iPad Safari or not
            /// </summary>
            public static bool isIPadSafari(RemoteWebDriver driver)
            {

                return (driver.Capabilities.BrowserName.ToUpper().Equals("IPAD") ||
                        driver.Capabilities.BrowserName.ToUpper().Equals("IOS_SAFARI_IPADAIR_REAL"))
                    ? true
                    : false;

            }

        }

        /// <summary>
        /// Enumeration of Control Type
        /// </summary>
        public enum ControlType
        {
            Textbox,
            Select,
            Label,
            Checkbox,
            Button,
            IFrame,
            Listbox
        }

        private static Nullable<Boolean> shouldHighlight;

        /// <summary>
        /// Gets ShouldHighlight
        /// </summary>
        private static Boolean ShouldHighlight
        {
            get
            {
                if (!shouldHighlight.HasValue)
                {
                    shouldHighlight = ConfigurationManager.AppSettings.Get("ElementHighlight").ToString().ToLower() == "true" ? true : false;
                }
                return shouldHighlight.Value;
            }
        }

        /// <summary>
        /// Navigates to specified URL
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="location"></param>
        public static void NavigateTo(RemoteWebDriver driver, String location)
        {
            driver.Navigate().GoToUrl(location);
        }

        /// <summary>
        /// Refreshs The Browser
        /// </summary>
        /// <param name="Driver"></param>
        /// <param name="location"></param>
        public static void BrowserRefresh(RemoteWebDriver driver)
        {
            driver.Navigate().Refresh();
        }

        /// <summary>
        /// Removing Escape Sequences
        /// </summary>
        /// <param name="dataValue"></param>
        /// <returns></returns>
        public static string RemoveEscapeChars(string dataValue)
        {
            if (!String.IsNullOrEmpty(dataValue))
            {
                dataValue = dataValue.Replace("\r\n", "");
            }
            return dataValue;
        }
        /// <summary>
        /// Initializes and reuses WebDriverWait instance
        /// </summary>
        /// <param name="Driver"><see cref="RemoteWebDriver"/></param>
        /// <param name="seconds">Seconds to wait, or 0 to use default from config</param>
        /// <returns>WebDriverWait instance</returns>
        private static WebDriverWait GetWaiter(RemoteWebDriver driver, int seconds = 0)
        {
            return new WebDriverWait(driver, TimeSpan.FromSeconds(seconds > 0 ? seconds : Convert.ToInt32(ConfigurationManager.AppSettings.Get("ElementSyncTimeOut"))));
        }

        /// <summary>
        /// Executes JavaScript on an element
        /// </summary>
        /// <param name="Driver"><see cref="RemoteWebDriver"/></param>
        /// <param name="element"><see cref="IWebElement"/></param>
        /// <param name="js">Javascript code to execute</param>
        private static void ExecuteJS(RemoteWebDriver driver, IWebElement element, String js)
        {
            IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
            jsExecutor.ExecuteScript(js, new object[] { element });
        }

        public static void ExecuteJS(RemoteWebDriver driver, Locator locator, String js)
        {
            IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
            jsExecutor.ExecuteScript(js, new object[] { GetElement(driver, locator) });
        }

        /// <summary>
        /// Highlights an element by drawing border
        /// </summary>
        /// <param name="Driver"><see cref="RemoteWebDriver"/></param>
        /// <param name="element"><see cref="IWebElement"/></param>
        /// <param name="isOrange">Orange default, Red otherwise</param>
        /// <returns><see cref="IWebElement"/></returns>
        private static IWebElement Highlight(RemoteWebDriver driver, IWebElement element, Boolean isOrange = true)
        {
            if (ShouldHighlight)
            {
                String script = String.Format(@"arguments[0].style.cssText = ""border-width: 4px; border-style: solid; border-color: {0}"";", isOrange ? "orange" : "darkred");
                IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
                jsExecutor.ExecuteScript(script, new object[] { element });
            }
            return element;
        }

        /// <summary>
        /// Waits for specified amount time
        /// </summary>
        /// <param name="Driver"><see cref="RemoteWebDriver"/></param>
        /// <param name="seconds">Number of seconds to wait</param>
        /// <param name="forceThread">Specify whether to wait at Driver level or Thread</param>
        public static void Wait(RemoteWebDriver driver, int seconds, bool threadLevel = false)
        {
            if (threadLevel)
            {
                Thread.Sleep(seconds * 1000);
            }
            else
            {
                driver.Manage().Timeouts().ImplicitlyWait(new TimeSpan(0, 0, seconds));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="seconds"></param>
        /// <param name="threadLevel"></param>
        public static void Wait(RemoteWebDriver driver, Double seconds, bool threadLevel = false)
        {
            int sec = Convert.ToInt16(seconds * 1000);
            if (threadLevel)
            {
                Thread.Sleep(sec);
            }
            else
            {
                driver.Manage().Timeouts().ImplicitlyWait(new TimeSpan(0, 0, Convert.ToInt16(seconds)));
            }
        }

        /// <summary>
        /// Wait until Ajax or JQuery complete
        /// </summary>
        /// <param name="driver"></param>
        public static void WaitForAjax(RemoteWebDriver driver)
        {
            while (true) // Handle timeout somewhere
            {
                var ajaxIsComplete = (bool)(driver as IJavaScriptExecutor).ExecuteScript("return jQuery.active == 0");
                if (ajaxIsComplete)
                    break;
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// WaitForElementNotVisible wait for until specific element disappears
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="locator"></param>
        public static void WaitForElementNotVisible(RemoteWebDriver driver, Locator locator)
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(Convert.ToInt32(ConfigurationManager.AppSettings.Get("ElementSyncTimeOut")))).Until(ExpectedConditions.InvisibilityOfElementLocated(locator.GetBy()));
        }

        /// <summary>
        /// WaitFor Page Title load for 15 Sec
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="title"></param>
        public static void WaitForTitle(RemoteWebDriver driver, string title)
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(15)).Until(ExpectedConditions.TitleContains(title));
        }

        /// <summary>
        /// Waits for an element to be visible on page
        /// </summary>
        /// <param name="locator"><see cref="Automation.Mercury.Locator"/></param>
        /// <param name="seconds">Number of Seconds to wait. 0 (Default) means, globale wait time (from Settings)</param>
        public static void WaitForElementVisible(RemoteWebDriver driver, Locator locator, int seconds = 0)
        {
            Highlight(driver, GetWaiter(driver, seconds).Until(ExpectedConditions.ElementIsVisible(locator.GetBy())));
        }

        /// <summary>
        /// Waits for an element to be Clickble on page
        /// </summary>
        /// <param name="locator"><see cref="Automation.Mercury.Locator"/></param>
        /// <param name="seconds">Number of Seconds to wait. 0 (Default) means, globale wait time (from Settings)</param>
        public static void WaitForElementClickble(RemoteWebDriver driver, Locator locator, int seconds = 0)
        {
            Highlight(driver, GetWaiter(driver, seconds).Until(ExpectedConditions.ElementToBeClickable(locator.GetBy())));
        }

        /// <summary>
        /// Waits for an element to be exist on page
        /// </summary>
        /// <param name="locator"><see cref="Automation.Mercury.Locator"/></param>
        /// <param name="seconds">Number of Seconds to wait. 0 (Default) means, globale wait time (from Settings)</param>
        public static void WaitForElementExist(RemoteWebDriver driver, Locator locator, int seconds = 0)
        {
            Highlight(driver, GetWaiter(driver).Until(ExpectedConditions.ElementExists(locator.GetBy())));
        }

        /// <summary>
        /// Verifies specified element is enabled or disabled
        /// </summary>
        /// <param name="Driver"><see cref="RemoteWebDriver"/></param>
        /// <param name="by"><see cref="by"/></param>
        /// <param name="shouldEnabled">True if expectation is 'Enabled'</param>
        public static void VerifyEnabledOrDisabled(RemoteWebDriver driver, Locator locator, Boolean shouldEnabled = true, ControlType controlType = ControlType.Textbox)
        {
            IWebElement element = null;
            element = GetElement(driver, locator);

            if (!Highlight(driver, element).Enabled == shouldEnabled)
            {
                throw new Exception("Expected State of Element is not Matching");
            }
        }

        /// <summary>
        /// Verifies specified element is visible or invisible
        /// </summary>
        /// <param name="by"><see cref="by"/></param>
        /// <param name="shouldEnabled">True if expectation is 'Visible'</param>
        public static Boolean VerifyVisible(RemoteWebDriver driver, Locator locator)
        {
            try
            {
                // throw new Exception("Element selection state is not matching with expected");
                if (driver.FindElement(locator.GetBy()).Displayed)
                {
                    Highlight(driver, GetElement(driver, locator), false);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Element state is not matching with expected: <br>" + locator.GetBy());
            }
        }

        /// <summary>
        /// IsElementExists method check Element Prescence in the Page, if element not exists it will return False
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="locator"></param>
        /// <returns>If Element Exists= true, else false</returns>
        public static Boolean IsElementExists(RemoteWebDriver driver, Locator locator)
        {
            try
            {

                if (driver.FindElement(locator.GetBy()).Displayed)
                {
                    Highlight(driver, GetElement(driver, locator), false);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Verifies specified element is visible or invisible
        /// </summary>
        /// <param name="by"><see cref="by"/></param>
        /// <param name="shouldEnabled">True if expectation is 'Visible'</param>
        public static Boolean VerifyInvisible(RemoteWebDriver driver, Locator locator)
        {
            try
            {
                Highlight(driver, GetWaiter(driver, Convert.ToInt32(ConfigurationManager.AppSettings.Get("ElementSyncTimeOut"))).Until(ExpectedConditions.ElementIsVisible(locator.GetBy())));
                if (!driver.FindElement(locator.GetBy()).Displayed)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Element selection state is not matching with expected");
            }
        }

        /// <summary>
        /// Verifies specified element is selected or not
        /// </summary>
        /// <param name="Driver"><see cref="RemoteWebDriver"/></param>
        /// <param name="by"><see cref="by"/></param>
        /// <param name="selected">True if expectation is 'Checked'</param>        
        public static void VerifyCheckedOrUnchecked(RemoteWebDriver driver, Locator locator, Boolean selected = true)
        {
            if (GetElement(driver, locator).Selected != selected)
            {
                throw new Exception("Element selection state is not matching with expected");
            }
        }

        /// <summary>
        /// Verifies specified element is selected or not
        /// </summary>
        /// <param name="Driver"><see cref="RemoteWebDriver"/></param>
        /// <param name="by"><see cref="by"/></param>
        /// <param name="selected">True if expectation is 'Checked'</param>        
        public static Boolean GetCheckedStatus(RemoteWebDriver driver, Locator locator)
        {
            return GetElement(driver, locator).Selected;
        }

        /// <summary>
        /// Gets element count 
        /// </summary>
        /// <param name="locator"></param>
        /// <returns></returns>
        public static Int32 GetElementCount(RemoteWebDriver driver, Locator locator)
        {
            IList<IWebElement> elements = driver.FindElements(locator.GetBy());
            return elements.Count;
        }

        /// <summary>
        /// Gets element collection 
        /// </summary>        
        /// <returns></returns>
        public static IList<String> GetElementNames(RemoteWebDriver driver, Locator locator)
        {
            List<String> result = new List<string>();
            foreach (IWebElement element in driver.FindElements(locator.GetBy()))
            {
                result.Add(element.Text);
            }
            return result;
        }

        /// <summary>
        /// Gets an element from DOM
        /// </summary>
        /// <param name="Driver"><see cref="RemoteWebDriver"/></param>
        /// <param name="by"><see cref="by"/></param>
        /// <returns><see cref="IWebElement"/></returns>
        public static IWebElement GetElement(RemoteWebDriver driver, Locator locator)
        {
            return Highlight(driver, driver.FindElement(locator.GetBy()));
        }


        /// <summary>
        /// Returns page title
        /// </summary>
        /// <param name="driver"></param>
        /// <returns></returns>
        public static string GetTitle(RemoteWebDriver driver)
        {
            return driver.Title;
        }

        /// <summary>
        /// returns current page url
        /// </summary>
        /// <param name="driver"></param>
        /// <returns></returns>
        public static string GetCurrentPageUrl(RemoteWebDriver driver)
        {
            return driver.Url;
        }

        public class Query
        {
            /// <summary>
            /// Verifies visibility of an element
            /// </summary>
            /// <param name="Driver"><see cref="RemoteWebDriver"/></param>
            /// <param name="by"><see cref="by"/></param>
            /// <param name="wait">Flag to indicate whether to wait while looking for element</param>            
            /// <returns>Boolean value representing existence of element</returns>
            public static bool isElementVisible(RemoteWebDriver driver, Locator locator, Boolean wait = true)
            {
                IWebElement element = null;
                try
                {
                    if (wait)
                    {
                        WaitForElementVisible(driver, locator);
                        element = GetElement(driver, locator);
                    }
                    else
                    {
                        element = GetElement(driver, locator);
                    }
                    return element.Displayed;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            /// <summary>
            /// Verifies visibility of an element
            /// </summary>
            /// <param name="Driver"><see cref="RemoteWebDriver"/></param>
            /// <param name="by"><see cref="by"/></param>
            /// <param name="wait">Flag to indicate whether to wait while looking for element</param>            
            /// <returns>Boolean value representing existence of element</returns>
            public static bool isElementInvisible(RemoteWebDriver driver, Locator locator, Boolean wait = true)
            {
                IWebElement element = null;
                bool isException = true;

                if (!wait)
                {
                    try
                    {
                        element = GetElement(driver, locator);
                        if (element.Displayed != wait)
                        {
                            isException = false;
                            throw new Exception("Element selection state is not matching with expected");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (isException == false)
                        {
                            throw new Exception("Element selection state is not matching with expected");
                        }

                        isException = true;
                    }
                }
                else
                {
                    try
                    {
                        element = GetElement(driver, locator);
                        if (element.Displayed)
                        {
                            isException = false;
                        }
                        else if (element.Displayed == wait)
                        {
                            isException = false;
                            throw new Exception("Element selection state is not matching with expected");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (wait == true)
                        {
                            throw new Exception("Element selection state is not matching with expected");
                        }

                        isException = true;
                    }
                }
                return isException;
            }

            /// <summary>
            /// Verifies visibility of an element for Check Annotation [PDM]
            /// </summary>
            /// <param name="Driver"><see cref="RemoteWebDriver"/></param>
            /// <param name="by"><see cref="by"/></param>
            /// <param name="wait">Flag to indicate whether to wait while looking for element</param>            
            /// <returns>Boolean value representing existence of element</returns>
            public static bool isElementVisibleForAnnotation(RemoteWebDriver driver, Locator locator, Boolean wait = true)
            {
                IWebElement element = null;
                bool isException = false;

                if (wait)
                {
                    WaitForElementVisible(driver, locator);
                    element = GetElement(driver, locator);
                }
                else
                {
                    try
                    {
                        element = GetElement(driver, locator);
                        if (element.Displayed != wait)
                        {
                            isException = true;
                            throw new Exception("Element selection state is not matching with expected");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (isException == true)
                        {
                            throw new Exception("Element selection state is not matching with expected");
                        }

                        return true;
                    }
                }
                return element.Displayed;
            }

            /// <summary>
            /// Verifies existence of an element
            /// </summary>
            /// <param name="Driver"><see cref="RemoteWebDriver"/></param>
            /// <param name="by"><see cref="by"/></param>
            /// <param name="wait">Flag to indicate whether to wait while looking for element</param>            
            /// <returns>Boolean value representing existence of element</returns>
            public static bool isElementExist(RemoteWebDriver driver, Locator locator, Boolean wait = true)
            {
                try
                {
                    if (wait)
                    {
                        WaitForElementExist(driver, locator);
                    }
                    else
                    {
                        Highlight(driver, GetElement(driver, locator));
                    }
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            /// <summary>
            /// Verifies existence of an element
            /// </summary>
            /// <param name="Driver"><see cref="RemoteWebDriver"/></param>
            /// <param name="by"><see cref="by"/></param>
            /// <param name="wait">Flag to indicate whether to wait while looking for element</param>
            /// <returns>Boolean value representing whether it is enabled or not</returns>
            public static bool isElementEnabled(RemoteWebDriver driver, Locator locator, Boolean wait = true)
            {
                IWebElement element = null;

                try
                {
                    if (wait)
                    {
                        WaitForElementVisible(driver, locator);
                        element = Highlight(driver, GetElement(driver, locator));
                    }
                    else
                    {
                        element = Highlight(driver, GetElement(driver, locator));
                    }
                    return element.Enabled;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public class JS
        {
            /// <summary>
            /// Performs Click with JavaScript
            /// </summary>
            /// <param name="locator"></param>
            public static void Click(RemoteWebDriver driver, Locator locator)
            {
                IWebElement element = Highlight(driver, GetElement(driver, locator), false);
                //JQuery trigger onClick event only. Never Clicks. So, do native JS Click
                ExecuteJS(driver, element, @"$(arguments[0])[0].click();");
            }

            /// <summary>
            /// DoubleClick method double click on a elemnt using JavaScript executor
            /// </summary>
            /// <param name="driver"></param>
            /// <param name="locator"></param>
            public static void DoubleClick(RemoteWebDriver driver, Locator locator)
            {
                IWebElement element = Highlight(driver, GetElement(driver, locator), false);
                ((IJavaScriptExecutor)driver).ExecuteScript("var evt = document.createEvent('MouseEvents');"
+ "evt.initMouseEvent('dblclick',true, true, window, 0, 0, 0, 0, 0, false, false, false, false, 0,null);"
+ "arguments[0].dispatchEvent(evt);", element);
            }

            /// <summary>
            /// Accept alert message but this is only for PhantomJS
            /// </summary>
            /// <param name="driver"></param>
            /// <returns></returns>
            public static void AcceptAlert(RemoteWebDriver driver)
            {
                if (driver.Capabilities.BrowserName.ToUpper().Equals("PHANTOMJS"))
                {
                    var js = driver as IJavaScriptExecutor;
                    var result = js.ExecuteScript("window.confirm = function(){return true;}") as string;
                    ((PhantomJSDriver)driver).ExecutePhantomJS("var page = this;" +
                                                 "page.onConfirm = function(msg) {" +
                                                 "console.log('CONFIRM: ' + msg);return true;" +
                                                    "};");
                    Wait(driver, 1, true);
                }
                else
                {
                    try
                    {
                        driver.SwitchTo().Alert().Accept();
                    }
                    catch (NoAlertPresentException exp)
                    {
                        throw new Exception(exp.InnerException.ToString());
                    }
                }
            }


            /// <summary>
            /// Sets Text/Value/Selection with JavaScript
            /// </summary>
            public static void SetText(RemoteWebDriver driver, Locator locator, ControlType controlType, String value)
            {
                IWebElement element = Highlight(driver, GetElement(driver, locator), false);
                switch (controlType)
                {
                    case ControlType.Textbox:
                        ExecuteJS(driver, element, String.Format(@"$(arguments[0]).val('{0}');", value));
                        break;
                }

                ExecuteJS(driver, element, @"$(arguments[0]).change();");
            }


            /// <summary>
            /// Executes any JS commands
            /// refer: http://selenium.googlecode.com/svn/trunk/docs/api/dotnet/html/AllMembers_T_OpenQA_Selenium_IJavaScriptExecutor.htm
            /// </summary>
            /// <param name="js">JS to execute</param>
            /// <returns>An object representing the return value</returns>
            public static object GetObject(RemoteWebDriver driver, String js)
            {
                IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
                return jsExecutor.ExecuteScript(js, new object[] { Locator.Get(LocatorType.XPath, "\\body").Location });
            }

            public static void Enter(RemoteWebDriver driver, Locator locator)
            {
                IWebElement element = Highlight(driver, GetElement(driver, locator), false);
                new Actions(driver).KeyDown(OpenQA.Selenium.Keys.Enter);
            }

            public static void Focus(RemoteWebDriver driver, Locator locator)
            {
                IWebElement element = GetElement(driver, locator);
                ExecuteJS(driver, element, String.Format(@"$(arguments[0].scrollIntoView(true));"));
                //Wait(driver, 0, true);
            }
        }

        /// <summary>
        /// Gets text from an element
        /// </summary>
        /// <param name="Driver"><see cref="RemoteWebDriver"/></param>
        /// <param name="by"><see cref="by"/></param>
        /// <param name="controlType"><see cref="ControlType"/></param>
        /// <returns>Text of element</returns>
        public static string GetText(RemoteWebDriver driver, Locator locator, ControlType controlType)
        {
            IWebElement element = GetElement(driver, locator);
            String value = String.Empty;

            switch (controlType)
            {
                case ControlType.Select:
                    value = new SelectElement(element).SelectedOption.Text;
                    break;

                case ControlType.Textbox:
                    value = element.GetAttribute("value");
                    break;

                case ControlType.Label:
                    value = element.Text;
                    break;

                case ControlType.Button:
                    value = element.GetAttribute("value");
                    break;
            }

            return value;
        }

        /// <summary>
        /// returns the attribute value of object
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="locator"></param>
        /// <param name="attribute">attribute name; eg: value, id, etc..</param>
        /// <returns></returns>
        public static string GetAttributeValue(RemoteWebDriver driver, Locator locator,
            string attribute)
        {
            IWebElement element = driver.FindElement(locator.GetBy());
            return element.GetAttribute(attribute);
        }

        /// <summary>
        /// SetAttributeValue method set a value for an attribute for specific object(element)
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="locator">Element details</param>
        /// <param name="attribute">attribute name</param>
        /// <param name="attributeValue">attribute value wish to set</param>
        public static void SetAttributeValue(RemoteWebDriver driver, Locator locator,
            string attribute,
            string attributeValue)
        {
            IWebElement element = driver.FindElement(locator.GetBy());
            IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
            jsExecutor.ExecuteScript(@"(arguments[0].setAttribute(arguments[1], arguments[2]))",
                element, attribute, attributeValue);
        }


        /// <summary>
        /// Sets text to an element
        /// </summary>
        /// <param name="Driver"><see cref="RemoteWebDriver"/></param>
        /// <param name="by"><see cref="by"/></param>
        /// <param name="controlType"><see cref="ControlType"/></param>
        /// <param name="text">Text to set</param>
        public static void SetText(RemoteWebDriver driver, Locator locator, ControlType controlType, String text, bool split = false, bool JSForIE = true)
        {
            IWebElement element = Highlight(driver, GetElement(driver, locator), false);

            switch (controlType)
            {
                case ControlType.Select:
                    new SelectElement(element).SelectByText(text);
                    break;

                case ControlType.Listbox:
                    SelectElement se = new SelectElement(element);
                    se.SelectByText(text);
                    break;

                case ControlType.Textbox:
                    element.SendKeys(text);
                    break;

                case ControlType.IFrame:
                    element.Clear();
                    if (text.Contains("'") || text.Contains(@""""))
                    {
                        text = text.Replace("'", "\\'");
                        element.SendKeys(text);
                    }
                    else
                    {
                        element.SendKeys(text);
                    }
                    break;
            }
        }

        /// <summary>
        /// Gets list of options avaialble
        /// </summary>
        /// <param name="Driver"><see cref="RemoteWebDriver"/></param>
        /// <param name="by"><see cref="By"/></param>
        /// <param name="controlType"><see cref="ControlType"/></param>
        /// <returns>Avaialble options as <see cref="List<String>"/></returns>
        public static List<String> GetOptions(RemoteWebDriver driver, Locator locator, ControlType controlType)
        {
            IWebElement element = Highlight(driver, GetElement(driver, locator), false);
            List<String> options = new List<string>();

            switch (controlType)
            {
                case ControlType.Select:
                    foreach (IWebElement each in new SelectElement(element).Options)
                    {
                        options.Add(each.Text);
                    }
                    break;
            }

            return options;
        }

        /// <summary>
        /// Sets specified state to switch (Radio, Checkbox)
        /// </summary>
        /// <param name="Driver"><see cref="RemoteWebDriver"/></param>
        /// <param name="by"><see cref="by"/></param>
        /// <param name="check">Should check or not</param>
        public static void SetSwitch(RemoteWebDriver driver, Locator locator, bool check = true)
        {
            IWebElement element = GetElement(driver, locator);
            if (element.Selected != check)
            {
                element.Click();
            }
        }

        /// <summary>
        /// Gets state of switch
        /// </summary>
        /// <param name="Driver"><see cref="RemoteWebDriver"/></param>
        /// <param name="by"><see cref="by"/></param>
        /// <returns>Boolean value representing current state</returns>
        public static bool GetSwitch(RemoteWebDriver driver, Locator locator)
        {
            return GetElement(driver, locator).Selected;
        }

        /// <summary>
        /// Clears value of an element
        /// </summary>
        /// <param name="Driver"><see cref="RemoteWebDriver"/></param>
        /// <param name="by"><see cref="by"/></param>
        /// <param name="controlType"><see cref="ControlType"/></param>
        public static void Clear(RemoteWebDriver driver, Locator locator, ControlType controlType)
        {
            IWebElement element = Highlight(driver, GetElement(driver, locator), false);

            switch (controlType)
            {
                case ControlType.Select:
                    new SelectElement(element).DeselectAll();
                    break;

                case ControlType.Textbox:
                    element.Clear();
                    break;
            }
        }

        /// <summary>
        /// Moves Focus to an element
        /// </summary>
        /// <param name="locator"></param>
        public static void Focus(RemoteWebDriver driver, Locator locator)
        {
            // HACK: Control's that out of Window visible area are not receving click (Not always)
            // So, the idea is to set focus to it by calling MoveToElement (Mimics mouse move)
            // However, this kind of stuff not working in Touch devices

            IWebElement element = GetElement(driver, locator);
            new Actions(driver).MoveToElement(element).Perform();
            // Wait(driver, 1, true);
        }

        /// <summary>
        /// DragandDrop help to click on element and drag using co-ordinates
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="locator"></param>
        public static void DragandDrop(RemoteWebDriver driver, Locator locator,
            string xvalue,
            string yvalue)
        {
            IWebElement element = GetElement(driver, locator);
            new Actions(driver).DragAndDropToOffset(element, Int32.Parse(xvalue), Int32.Parse(yvalue)).Build().Perform();
        }

        /// <summary>
        /// Clicks an element
        /// </summary>
        /// <param name="Driver"><see cref="RemoteWebDriver"/></param>
        /// <param name="by"><see cref="by"/></param>
        public static void Click(RemoteWebDriver driver, Locator locator)
        {
            IWebElement element = GetElement(driver, locator);

            // HACK: Control's that out of Window visible area are not receving click (Not always)
            // So, the idea is to set focus to it by calling MoveToElement (Mimics mouse move)
            // However, this kind of stuff not working in Touch devices

            Highlight(driver, element, false).Click();
        }

        /// <summary>
        /// Onclick method fires onClick event of specific object
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="locator"></param>
        public static void OnClick(RemoteWebDriver driver, Locator locator)
        {
            IWebElement element = GetElement(driver, locator);
            // IJavaScriptExecutor jsExecutor1 = 
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].ng-click();", element);
        }

        /// <summary>
        /// Scroll browser window 
        /// </summary>
        /// <param name="driver"></param>
        public static void Scroll(RemoteWebDriver driver)
        {
            IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
            jsExecutor.ExecuteScript("window.scrollBy(0,250)", "");
        }

        /// <summary>
        /// DoubleClick method DoubleClick method 
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="locator"></param>
        public static void DoubleClick(RemoteWebDriver driver, Locator locator)
        {
            IWebElement element = Highlight(driver, GetElement(driver, locator), false);
            new Actions(driver).DoubleClick(element).Build().Perform();
        }

        /// <summary>
        /// Accept /Dismiss the alert message.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="accept">default value true;  False: for dismiss alert</param>
        public static void AcceptorDismissAlert(RemoteWebDriver driver,
            bool accept = true)
        {
            if (accept)
                driver.SwitchTo().Alert().Accept();
            else
                driver.SwitchTo().Alert().Dismiss();
        }



        /// <summary>
        /// IsAlertPresent method return true if alert present else false.
        /// </summary>
        /// <param name="driver"></param>
        /// <returns></returns>
        public static bool IsAlertPresent(RemoteWebDriver driver)
        {
            bool foundAlert = false;
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
            try
            {
                wait.Until(ExpectedConditions.AlertIsPresent());
                foundAlert = true;
            }
            catch (Exception eTO)
            {
                foundAlert = false;
            }

            return foundAlert;
        }

        /// <summary>
        /// Switch To Frame Element
        /// </summary>
        /// <param name="Driver"><see cref="RemoteWebDriver"/></param>
        /// <param name="by"><see cref="by"/></param>
        public static void SwitchToFrame(RemoteWebDriver driver, Locator locator)
        {
            driver.SwitchTo().Frame(GetElement(driver, locator));
        }

        /// <summary>
        /// Swicth To alert 
        /// </summary>
        /// <param name="driver"></param>
        public static string SwitchToAlertandGetAlertText(RemoteWebDriver driver)
        {
            try
            {
                return driver.SwitchTo().Alert().Text.ToString();
            }
            catch (NoAlertPresentException ex)
            {
                throw new Exception(ex.InnerException.ToString() + "<br>" + ex.StackTrace.ToString());
            }
        }

        /// <summary>
        /// Switch To Window Element
        /// </summary>
        /// <param name="Driver"><see cref="RemoteWebDriver"/></param>
        /// <param name="by"><see cref="by"/></param>
        public static void SwitchToWindow(RemoteWebDriver driver)
        {
            var currentWindow = driver.CurrentWindowHandle;
            var availableWindows = new List<string>(driver.WindowHandles);

            foreach (string window in availableWindows)
            {
                if (window != currentWindow)
                {
                    driver.SwitchTo().Window(window);
                }
            }

        }

        /// <summary>
        /// Switch To Window Element
        /// </summary>
        /// <param name="Driver"><see cref="RemoteWebDriver"/></param>
        /// <param name="by"><see cref="by"/></param>
        public static string GetWindowHandle(RemoteWebDriver driver)
        {
            return driver.CurrentWindowHandle;
        }

        /// <summary>
        /// Switch To Window Element
        /// </summary>
        /// <param name="Driver"><see cref="RemoteWebDriver"/></param>
        /// <param name="by"><see cref="by"/></param>
        public static void SwitchToWindow(RemoteWebDriver driver, string handler)
        {
            driver.SwitchTo().Window(handler);
        }

        /// <summary>
        /// Switch To Default Content
        /// </summary>
        /// <param name="Driver"><see cref="RemoteWebDriver"/></param>
        /// <param name="by"><see cref="by"/></param>
        public static void SwitchToDefaultContent(RemoteWebDriver driver)
        {
            driver.SwitchTo().DefaultContent();
        }

        /// <summary>
        /// Sends Enter Key
        /// </summary>
        /// <param name="locator"></param>
        public static void Enter(RemoteWebDriver driver, Locator locator)
        {
            IWebElement element = Highlight(driver, GetElement(driver, locator), false);
            element.SendKeys(Keys.Enter);
        }

        /// <summary>
        /// Clicks an element
        /// </summary>
        /// <param name="Driver"><see cref="RemoteWebDriver"/></param>
        /// <param name="by"><see cref="by"/></param>
        public static void ClickAddFile(RemoteWebDriver driver, Locator locator, string path)
        {
            //Driver.FindElement(By.XPath("//input[@id='fileupload']")).SendKeys(path);            
            driver.FindElement(locator.GetBy()).SendKeys(path);
        }

        /// <summary>
        /// Get Time Stamp
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="timeFormat"></param>
        public static string GetTimeStamp(string dateTime, string timeFormat)
        {
            DateTime dt = DateTime.Parse(dateTime);
            return dt.ToString(timeFormat);
        }

        /// <summary>
        /// Get Date Time Stamp
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="timeFormat"></param>
        public static string GetDateTimeStamp()
        {
            string dateTimeStamp = String.Empty;
            dateTimeStamp = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Millisecond.ToString();
            return dateTimeStamp;
        }

        public static bool IsDownloadable(string Source, RemoteWebDriver driver, Locator locator, string actualSizeOfPDF)
        {
            WebDriverWait wait = new WebDriverWait(driver, System.TimeSpan.FromSeconds(160));
            string text = string.Empty;
            bool status = false;
            bool status2 = false;
            int sizeofPDF = Convert.ToInt32(actualSizeOfPDF);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Source);
            request.Method = "GET";
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        wait.Until(CheckClipboardContent(driver, locator, Source, sizeofPDF));

                        status = true;
                    }

                    else
                    {

                        status = false;
                    }

            }
            catch (Exception e)
            {


                if (e.Message.Contains("404"))
                {
                    status = false;
                }


            }
            return status;
        }


        public static Func<IWebDriver, bool> CheckClipboardContent(RemoteWebDriver driver, Locator locator, string url, int actualsize)
        {
            int copied = 0;
            int copiedlatest = 0;
            int copiedlatest1 = 0;
            bool status = false;
            Thread.Sleep(2000);

            return d =>
            {
                try
                {
                    status = true;
                    if (status)
                    {
                        PDFreraderFactory e = new PDFreraderFactory();
                        copied = e.pdfText(driver);

                        Thread.Sleep(5000);
                        PDFreraderFactory e1 = new PDFreraderFactory();
                        copiedlatest = e1.pdfText(driver);
                        if (copied == copiedlatest && actualsize <= 250)
                        {
                            copiedlatest = copied + 2;
                        }
                        //PDFreraderFactory e3 = new PDFreraderFactory();
                        //copiedlatest1 = e3.pdfText(driver);


                    }
                    //copied = string.Empty;

                }

                catch (Exception e)
                {

                }

                return (copiedlatest > copied) ? true : false;

            };
        }
    }
    public class PDFreraderFactory
    {
        private Object thisLock = new Object();

        public int pdfText(RemoteWebDriver driver)
        {

            string screenshot = string.Empty;

            lock (thisLock)
            {
                Exception threadEx = null;

                Thread staThread = new Thread(
                    delegate ()
                    {
                        try
                        {
                            Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
                            screenshot = ss.AsBase64EncodedString;
                        }
                        catch (Exception ex)
                        {
                            threadEx = ex;
                        }
                    });
                staThread.SetApartmentState(ApartmentState.STA);
                staThread.Start();
                staThread.Join();

            }


            return screenshot.Length;
        }

    }
}
