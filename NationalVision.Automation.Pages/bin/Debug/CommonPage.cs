/* **********************************************************************
 * Description : cs class having methods and objects common to all pages.
 *        Header links, Footer links, Menu Tabs, Search window objects.
 *        
 * Date  :  02-Feb-2016 
 * **********************************************************************
 */

using System;
using Automation.Mercury;
using Automation.Mercury.Report;
using OpenQA.Selenium.Remote;
using System.Collections.Generic;
using OpenQA.Selenium;
using System.Reflection;
using System.IO;
using System.Data;
using System.Drawing.Imaging;
using OpenQA.Selenium.Interactions;
using System.Configuration;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Globalization;

namespace NationalVision.Automation.Pages
{
    public class CommonPage : BaseCase
    {
        static bool IsExternalApplication = false, isSIMVisited = false, isAccountingVisited = false,
            isContentManagementVisited = false, isWellnessVisited = false, isETLVisited = false,
            isStoreSchedulerVisited = false, isPortalManagerVisited = false, isRedicallVisited = false,
            isWIPVisited = false, isAssessmentsVisited = false;


        /// <summary>
        /// This method naviagte the url
        /// </summary>
        /// <param name="driver">Initialized RemoteWebDriver instance</param>
        /// <param name="reporter">Initialized report instance</param>
        /// <param name="url">URL of the application</param>
        public static void NavigateTo(RemoteWebDriver driver, Iteration reporter, String url)
        {
            Selenide.NavigateTo(driver, url);
        }

        /// <summary>
        /// SelectApplication method selects the application at login screen
        /// </summary>
        /// <param name="Driver">Initialized RemoteWebDriver instance</param>
        /// <param name="applicationName">application Name</param>
        public static void SelectApplication(RemoteWebDriver driver, Iteration reporter, string applicationName)
        {
            reporter.Add(new Act(String.Format("Select the {0} link ", applicationName)));
            Selenide.SwitchToFrame(driver, Locator.Get(LocatorType.ID, "dlg_ifrm_modal2"));
            try
            {
                Actions action = new Actions(driver);
                IWebElement application = Selenide.GetElement(driver, Locator.Get(LocatorType.XPath, string.Format(@"//table[@id='tblSelect']/descendant::a[contains(@title,""{0}"")]/div", applicationName)));
                //Selenide.Click(driver,Locator.Get(LocatorType.XPath, string.Format(@"//table[@id='tblSelect']/descendant::a[contains(@title,""{0}"")]/div", applicationName)));
                //Selenide.Click(driver, Locator.Get(LocatorType.XPath, string.Format(@"//table[@id='tblSelect']/descendant::a[contains(@title,'{0}')]/div", applicationName)));
                action.MoveToElement(application).Click().Build().Perform();
                WaitForPageLoad(driver, 10);
                Selenide.WaitForElementNotVisible(driver, Locator.Get(LocatorType.ClassName, "dlg_spinner"));
                Selenide.SwitchToDefaultContent(driver);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("{ 0 } link not found in the website", applicationName));
            }
        }

        /// <summary>
        /// SwitchApplication method switches the application at login screen
        /// </summary>
        /// <param name="Driver">Initialized RemoteWebDriver instance</param>
        /// <param name="applicationName">application Name</param>
        public static void SwitchApplication(RemoteWebDriver driver, Iteration reporter, string applicationName)
        {
            reporter.Add(new Act(String.Format("Double click on the logo")));
            Selenide.SwitchToDefaultContent(driver);
            try
            {
                Selenide.DoubleClick(driver, Util.GetLocator("ApplicationLogo_Img"));
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("{ 0 } link not found in the website", applicationName));
            }
        }


        /// <summary>
        /// VerifyPortalLogo verifies portal logo availability at home page
        /// </summary>
        /// <param name="Driver">Initialized RemoteWebDriver instance</param>
        /// <param name="reporter">Initialized report instance</param>
        public static void VerifyPortalLogo(RemoteWebDriver driver, Iteration reporter, string screenshotName, string saveTo)
        {
            Selenide.VerifyVisible(driver, Util.GetLocator("Portal_Logo"));
            //takeScreenshot(driver, screenshotName, saveTo, reporter);
        }

        /// <summary>
        /// RefreshBrowser method for Refreshs The Browser
        /// </summary>
        /// <param name="Driver">Initialized RemoteWebDriver instance</param>
        /// <param name="reporter">Initialized report instance</param>
        /// <param name="location">Location to navigate</param>
        public static void RefreshBrowser(RemoteWebDriver driver, Iteration reporter)
        {
            Selenide.BrowserRefresh(driver);
        }

        /// <summary>
        /// WaitForPageLoad method holds the driver  until it loads
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        /// <param name="pageTitle"></param>
        public static void WaitForPageLoad(RemoteWebDriver driver, int seconds = 0)
        {
            string state = string.Empty;
            try
            {

                //int time = driver, GetWaiter(driver, seconds);
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(seconds > 0 ? seconds : Convert.ToInt32(ConfigurationManager.AppSettings.Get("ElementSyncTimeOut"))));

                //Checks every 500 ms whether predicate returns true if returns exit otherwise keep trying till it returns ture
                wait.Until(d =>
                {

                    try
                    {
                        state = ((IJavaScriptExecutor)driver).ExecuteScript(@"return document.readyState").ToString();
                    }
                    catch (InvalidOperationException)
                    {
                        //Ignore
                    }
                    catch (NoSuchWindowException)
                    {
                        //when popup is closed, switch to last windows
                        //driver.SwitchTo().Window(driver.WindowHandles.Last());
                    }
                    //In IE7 there are chances we may get state as loaded instead of complete
                    return (state.Equals("complete", StringComparison.InvariantCultureIgnoreCase) || state.Equals("loaded", StringComparison.InvariantCultureIgnoreCase));

                });
            }
            catch (TimeoutException)
            {
                //sometimes Page remains in Interactive mode and never becomes Complete, then we can still try to access the controls
                if (!state.Equals("interactive", StringComparison.InvariantCultureIgnoreCase))
                    throw;
            }
            catch (NullReferenceException)
            {
                //sometimes Page remains in Interactive mode and never becomes Complete, then we can still try to access the controls
                if (!state.Equals("interactive", StringComparison.InvariantCultureIgnoreCase))
                    throw;
            }
            catch (WebDriverException)
            {
                if (driver.WindowHandles.Count == 1)
                {
                    driver.SwitchTo().Window(driver.WindowHandles[0]);
                }
                state = ((IJavaScriptExecutor)driver).ExecuteScript(@"return document.readyState").ToString();
                if (!(state.Equals("complete", StringComparison.InvariantCultureIgnoreCase) || state.Equals("loaded", StringComparison.InvariantCultureIgnoreCase)))
                    throw;
            }
        }

        /// <summary>
        /// ClickExternalApplicationMenu clicks on the menu in the external application menu
        /// </summary>
        /// <param name="Driver">Initialized RemoteWebDriver instance</param>
        /// <param name="reporter"></param>
        /// <param name="extMenuName">menu Name</param>
        public static void ClickExternalApplicationMenu(RemoteWebDriver driver, Iteration reporter, string extMenuName, int i, string resultsPath)
        {
            reporter.Add(new Act(String.Format((i + 1) + ": Click on {0} menu in external application", extMenuName)));
            Selenide.WaitForElementVisible(driver, Locator.Get(LocatorType.XPath,
             string.Format(@"//div[@class='secmenu']/ul/li/a[contains(text(),""{0}"")]", extMenuName)));
            Selenide.Click(driver, Locator.Get(LocatorType.XPath,
             string.Format(@"//div[@class='secmenu']/ul/li/a[contains(text(),""{0}"")]", extMenuName)));
            Selenide.WaitForElementNotVisible(driver, Locator.Get(LocatorType.ID, "dlg_spinner"));
            AcceptOrDissmissAlertIfPresent(driver, reporter);
            AcceptErrorMessageIfPresent(driver, reporter, resultsPath);
        }

        /// <summary>
        /// ClickExternalApplicationSubMenu clicks on the menu in the external application submenu
        /// </summary>
        /// <param name="Driver">Initialized RemoteWebDriver instance</param>
        /// <param name="reporter"></param>
        /// <param name="extSubMenuName">menu Name</param>
        public static void ClickExternalApplicationSubMenu(RemoteWebDriver driver, Iteration reporter, string extMenuName, string extSubMenuName, int i, string resultsPath)
        {
            int count = 0;
            try
            {
                reporter.Add(new Act(String.Format((i + 1) + ": Click the following ext application menu: <b>{0}</b>>><b>{1}</b>", extMenuName, extSubMenuName)));

                //Mandatory step:
                count = 0;
                while (count < 3)
                {
                    //Click on external menu
                    if (Selenide.IsElementExists(driver, Locator.Get(LocatorType.XPath, string.Format(@"//div[@class='secmenu']/ul/li/a[contains(text(),""{0}"")]", extMenuName))))
                    {
                        Selenide.Click(driver, Locator.Get(LocatorType.XPath,
                  string.Format(@"//div[@class='secmenu']/ul/li/a[contains(text(),""{0}"")]", extMenuName)));
                    }

                    //Verifies whether submenu is visible or not
                    if (Selenide.IsElementExists(driver, Locator.Get(LocatorType.XPath,
                    string.Format(@"//div[@id='dvAppMenu']/descendant::a[contains(text(),""{0}"")]/following-sibling::ul//a[contains(text(),""{1}"")]", extMenuName, extSubMenuName))))
                    {
                        //Click on external sub menu
                        Selenide.Click(driver, Locator.Get(LocatorType.XPath,
                           string.Format(@"//div[@id='dvAppMenu']/descendant::a[contains(text(),""{0}"")]/following-sibling::ul//a[contains(text(),""{1}"")]", extMenuName, extSubMenuName)));
                        break;
                    }
                    else
                    {
                        count++;
                    }
                }
                Selenide.WaitForElementNotVisible(driver, Locator.Get(LocatorType.ClassName, "dlg_spinner"));
                AcceptOrDissmissAlertIfPresent(driver, reporter);
                AcceptErrorMessageIfPresent(driver, reporter, resultsPath);
                CloseBrowserNewTab(driver);
                FocusOnContent(driver, reporter);
            }
            catch (Exception ex)
            {
                AcceptOrDissmissAlertIfPresent(driver, reporter);
                AcceptErrorMessageIfPresent(driver, reporter, resultsPath);
                CloseBrowserNewTab(driver);
            }
        }

        /// <summary>
        /// ClickSearchButton method clicks on search button in Stores Directory Page
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        public static void ClickSearchButton(RemoteWebDriver driver, Iteration reporter, string resultsPath)
        {
            reporter.Add(new Act("Click on search button in Stores Directory Page"));
            try
            {
                Actions action = new Actions(driver);
                action.SendKeys(OpenQA.Selenium.Keys.Enter).Build().Perform();
                AcceptOrDissmissAlertIfPresent(driver, reporter);
                AcceptErrorMessageIfPresent(driver, reporter, resultsPath);
                Selenide.WaitForElementNotVisible(driver, Locator.Get(LocatorType.ClassName, "dlg_spinner"));

                Selenide.SwitchToDefaultContent(driver);
                if (Selenide.IsElementExists(driver, Locator.Get(LocatorType.XPath, "//div[@id='createsearchdiv']//img")))
                {
                    Selenide.Click(driver, Locator.Get(LocatorType.XPath, "//div[@id='createsearchdiv']//img"));
                }
            }
            catch (SystemException sysex)
            {
                AcceptOrDissmissAlertIfPresent(driver, reporter);
                AcceptErrorMessageIfPresent(driver, reporter, resultsPath);
            }
        }
        public static void ClickSubMenuLink(RemoteWebDriver driver, Iteration reporter,
            string menuName, string submenuname, int i, string resultsPath)
        {
            Selenide.SwitchToDefaultContent(driver);

            reporter.Add(new Act(String.Format((i + 1) + ": Click the {0} submenu under {1} menu", submenuname, menuName)));
            Selenide.Click(driver, Locator.Get(LocatorType.XPath, string.Format(@"//a[normalize-space(text())='{0}']", menuName)));
            //Selenide.Focus(driver, Locator.Get(LocatorType.XPath, string.Format(@"//a[normalize-space(text())='{0}']", submenuname)));
            if (Selenide.IsElementExists(driver, Locator.Get(LocatorType.XPath, string.Format(@"//a[normalize-space(text())='{0}']", submenuname))))
            {
                Selenide.Click(driver, Locator.Get(LocatorType.XPath, string.Format(@"//a[normalize-space(text())='{0}']", submenuname)));
                Selenide.WaitForElementNotVisible(driver, Locator.Get(LocatorType.ClassName, "dlg_spinner"));
            }
            else if (Selenide.IsElementExists(driver, Locator.Get(LocatorType.XPath, string.Format(@"//a/nobr[normalize-space(text())='{0}']", submenuname))))
            {
                Selenide.Click(driver, Locator.Get(LocatorType.XPath, string.Format(@"//a/nobr[normalize-space(text())='{0}']", submenuname)));
            }
            //WaitUntilSpinnerDisappears(driver);
            //WaitForPageLoad(driver,10);
            AcceptOrDissmissAlertIfPresent(driver, reporter);
            AcceptErrorMessageIfPresent(driver, reporter, resultsPath);
            CloseBrowserNewTab(driver);
            FocusOnContent(driver, reporter);
        }

        /// <summary>
        /// AssertPageTitle verify page title macth
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        /// <param name="pageTitle"></param>
        public static void AssertPageTitle(RemoteWebDriver driver, Iteration reporter, string pTitle = "")
        {
            reporter.Add(new Act("Waiting for page title"));

            Selenide.WaitForTitle(driver, pTitle);
            string title = Selenide.GetTitle(driver);
            //title = title.Replace("'", "");
            if (!title.ToLower().Contains(pTitle.ToLower()))
            {
                reporter.Add(new Act("Asserting current page title"));
                throw new Exception(string.Format(@"Page Title not matched: Expected Title: ""{0}"" <br> Current Page Title: ""{1}""", pTitle, title));
            }
            else
            {
                reporter.Add(new Act("Asserted current page title : " + title));
            }
        }

        /// <summary>
        /// ClickHomePageTabs method click on Home Page tabs
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        /// <param name="tabName">Tab Name where User wish to switch</param>
        public static void ClickHomePageTabs(RemoteWebDriver driver, Iteration reporter, string tabName)
        {
            reporter.Add(new Act("Click on Home Page Tab: " + tabName));
            Selenide.Click(driver, Locator.Get(LocatorType.XPath, string.Format(@"//ul/descendant::a[normalize-space()='{0}']", tabName)));
        }

        /// <summary>
        /// Performs login
        /// </summary>
        /// <param name="Driver">Initialized RemoteWebDriver instance</param>
        /// <param name="username">Login Username</param>
        /// <param name="password">Login Password</param>
        public static void Login(RemoteWebDriver driver, Iteration reporter, string username, string password)
        {
            try
            {
                reporter.Add(new Act(String.Format("Set Username {0}, Password {1} and Click Login", username, password)));
                Selenide.WaitForElementNotVisible(driver, Locator.Get(LocatorType.ID, "dlg_spinner"));
                Selenide.WaitForElementVisible(driver, Util.GetLocator("UserName_txt"));
                Selenide.SetText(driver, Util.GetLocator("UserName_txt"), Selenide.ControlType.Textbox, username);
                Selenide.WaitForElementVisible(driver, Util.GetLocator("Password_txt"));
                Selenide.SetText(driver, Util.GetLocator("Password_txt"), Selenide.ControlType.Textbox, password);
                Selenide.WaitForElementVisible(driver, Util.GetLocator("Login_btn"));
                Selenide.JS.Click(driver, Util.GetLocator("Login_btn"));
                Selenide.WaitForElementNotVisible(driver, Locator.Get(LocatorType.ID, "dlg_spinner"));
                System.Threading.Thread.Sleep(5000);
            }
            catch (Exception ex)
            {

            }

        }

        /// <summary>
        /// TypeSearchText method type search key words in Search text box
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        /// <param name="searchKey">Keywords wish to search</param>
        public static void TypeSearchText(RemoteWebDriver driver, Iteration reporter, String searchKey)
        {
            reporter.Add(new Act(String.Format("Type '{0}' search keyword(s)/ character(s)  at Search Text box", searchKey)));
            Selenide.SetText(driver, Util.GetLocator("search_txt"), Selenide.ControlType.Textbox, searchKey);
        }

        /// <summary>
        /// ClearSearchText method clear text box
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        public static void ClearSearchText(RemoteWebDriver driver, Iteration reporter)
        {
            Selenide.Clear(driver, Util.GetLocator("search_txt"), Selenide.ControlType.Textbox);
        }

        /// <summary>
        /// WaitLoadingComplete method load until spinner disappers
        /// Use for EyeGlassesshelfpage, MyaccountPage for loading
        /// This method wait until spinner disappers, default time 30sec
        /// Spinner appears in Ajax calls also.
        /// </summary>
        /// <param name="driver"></param>
        public static void WaitLoadingComplete(RemoteWebDriver driver)
        {
            Selenide.WaitForAjax(driver);
        }

        /// <summary>
        /// WaitUntilSpinnerDisappears method waits until spinner disappers  
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="locator"></param>
        public static void WaitUntilSpinnerDisappears(RemoteWebDriver driver, string locator = null)
        {
            locator = !string.IsNullOrEmpty(locator) ? locator : "Spinner1_img";
            Selenide.WaitForElementNotVisible(driver, Locator.Get(LocatorType.XPath, locator));
        }

        /// <summary>
        /// takeScreenshot method screen capture at any perticular place and store in same resutls folder
        /// Customize naming convertion for screenshot
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="resultsPath">results path, this value capture forom BaseCase.cs file</param>
        public static void takeScreenshot(RemoteWebDriver driver, string link, string resultsPath, Iteration reporter)
        {
            Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
            string screenshot = ss.AsBase64EncodedString;
            byte[] screenshotAsByteArray = ss.AsByteArray;
            ss.SaveAsFile(Path.Combine(resultsPath, "Screenshots", String.Format("{0}.Png", link, DateTime.Now.ToString("hhmmssfff"))), ImageFormat.Png);
            ss.ToString();
            reporter.Add(new Act("Click on the " + link + " PDF <a href='" + Path.Combine("Screenshots",
            String.Format("{0}.png", link)) + "'><span class='glyphicon glyphicon-paperclip normal'></span></a>&nbsp;"));
            //reporter.Add(new Act("<span class='glyphicon glyphicon-remove green'></span>"));          
        }

        public static void TakeScreenShotAndAttachToReport(RemoteWebDriver driver, string link, string resultsPath, Iteration reporter)
        {
            Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
            string screenshot = ss.AsBase64EncodedString;
            byte[] screenshotAsByteArray = ss.AsByteArray;
            ss.SaveAsFile(Path.Combine(resultsPath, "Screenshots", String.Format("{0}.Png",
            link, DateTime.Now.ToString("hhmmssfff"))), ImageFormat.Png);
            ss.ToString();
            reporter.Add(new Act(" " + link + "<a href='" + Path.Combine("Screenshots", String.Format("{0}.png", link)) + "'><span class='glyphicon glyphicon-paperclip normal' align='right'></span></a>"));
            //reporter.Add(new Act("<span class='glyphicon glyphicon-remove green'></span>"));  
        }

        /// <summary>
        /// WaitUntilSpinnerDisappears method waits until spinner disappers.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        public static void WaitUntilSpinnerDisappears(RemoteWebDriver driver)
        {
            WaitUntilSpinnerDisappears(driver, (Util.GetLocator("Spinner1_img")).ToString());
        }

        /// <summary>
        /// CloseNewTab method Closes opened new tab
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>//
        public static void CloseBrowserNewTab(RemoteWebDriver driver)
        {
            ReadOnlyCollection<string> handles = driver.WindowHandles;
            if (handles.Count > 1)
            {
                driver.SwitchTo().Window(handles[1]);
                //string parentWindow3 = driver.CurrentWindowHandle;
                driver.Close();
                driver.SwitchTo().Window(handles[0]);
            }
        }

        /// <summary>
        /// AcceptOrDissmissAlertIfPresent method Accepts/dismiss alert if present.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        public static void AcceptOrDissmissAlertIfPresent(RemoteWebDriver driver, Iteration reporter)
        {
            //reporter.Add(new Act(string.Format(@"Accepts if alert present")));
            if (Selenide.IsAlertPresent(driver))
            {
                Selenide.AcceptorDismissAlert(driver);
            }
        }

        /// <summary>
        /// AcceptErrorMessageIfPresent method Closes errormessage if error present
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        /// <param name="postion">Position of the link </param>
        /// <returns></returns>
        public static void AcceptErrorMessageIfPresent(RemoteWebDriver driver, Iteration reporter, string resultsPath)
        {

            bool error = Selenide.IsElementExists(driver, Locator.Get(LocatorType.XPath, "//div[@id='createmsgdiv']/descendant::img"));

            if (error)
            {
                Selenide.Click(driver, Locator.Get(LocatorType.XPath, string.Format(@"//div[@id='createmsgdiv']/descendant::img")));
                takeScreenshot(driver, "screenshotName", resultsPath, reporter);
            }

        }

        /// <summary>
        /// ReturnTabName returns the tabname
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        /// <param name="postion">Position of the link </param>
        /// <returns></returns>
        public static void ReturnTabName(RemoteWebDriver driver, int indexfromClickMethod)
        {
        }

        /// <summary>
        /// GetMenuCount returns menu count  
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        /// <param name="postion">Position of the link </param>
        /// <returns></returns>
        public static int GetMenuCount(RemoteWebDriver driver, Iteration reporter,
            string Application)
        {
            //Step = "Get the menu count of "+ Application +" ";
            reporter.Add(new Act(string.Format(@"Get the menu count in {0}", Application)));
            return Selenide.GetElementCount(driver, (Util.GetLocator("MenuGrid_lbl")));
        }

        /// <summary>
        /// MenuNames returns menu text of menu link
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        /// <param name="postion">Position of the link </param>
        /// <returns></returns>
        public static List<string> GetMenuNames(RemoteWebDriver driver, Iteration reporter,
            int menuIndex, int menucount)
        {
            List<string> menuNames = new List<string>();

            for (menuIndex = 1; menuIndex <= menucount; menuIndex++)
            {//reporter.Add(new Act(string.Format(@"Gets the menu names")));
                menuNames.Add(Selenide.GetText(driver, Locator.Get(LocatorType.XPath,
                    string.Format(@"//div[@class='menu']/ul/li[{0}]", menuIndex)), Selenide.ControlType.Label));
            }
            return menuNames;
        }

        /// <summary>
        /// ClickAllSubMenusInMenu method click the links in each menu of WebPortal
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        /// <param name="tabName">tabName</param>
        /// <param name="resultsPath">resultsPath</param>
        /// 
        public static void ClickAllSubMenusInEachMenu(RemoteWebDriver driver, Iteration reporter, string resultsPath, string menuItem, List<string> menuNames)
        {
            string subMenu = string.Empty, subMenuAttribute = string.Empty;
            int linksCountInEachMenu, linksCountInSubMenu, subMenuCount, subMenuCountInEachMenu, clickableLinksInEachMenu, linksCountInSubMenu1 = 0;

            //Clicks menu
            Selenide.Click(driver, Locator.Get(LocatorType.XPath, string.Format(@"//div[@class='menu']/ul/li/a[contains(text(),'{0}')]", menuItem)));

            //Number of submenu's in a menu
            //subMenuCountInEachMenu = Selenide.GetElementCount(driver, Locator.Get(LocatorType.XPath,string.Format(@"//div[@class='menu']/ul/li/a[contains(text(),'{0}')]/following-sibling::ul/li", menu)));
            subMenuCountInEachMenu = Selenide.GetElementCount(driver, Locator.Get(LocatorType.XPath, string.Format
                (@"//div[@class='menu']/ul/li/a[normalize-space(text())='{0}']/following-sibling::ul/li", menuItem)));

            //Number of clickable submenu's in a menu
            linksCountInEachMenu = Selenide.GetElementCount(driver, Locator.Get(LocatorType.XPath,
                    string.Format(@"//div[@class='menu']/ul/li/a[contains(text(),'{0}')]/following-sibling::ul/li/a[@onclick]", menuItem)));

            //Number of clickable submenu2's in a menu
            linksCountInSubMenu = Selenide.GetElementCount(driver, Locator.Get(LocatorType.XPath,
                    string.Format(@"//div[@class='menu']/ul/li/a[contains(text(),'{0}')]/following-sibling::ul/li/a[@class='drop']/following-sibling::ul/li/a[@onclick]", menuItem)));

            //Total clickable links in a menu
            clickableLinksInEachMenu = linksCountInEachMenu + linksCountInSubMenu;

            reporter.Add(new Act(string.Format(@"Number of clickable links under <b>{0}</b> menu are: <b>{1}</b>", menuItem, clickableLinksInEachMenu)));

            for (subMenuCount = 1; subMenuCount <= subMenuCountInEachMenu; subMenuCount++)
            {
                try
                {

                    #region Method0_Decides_ClickableLink(Ex:AboutNationalVision)orNot(Ex:SupplyOrdering)
                    //The below step decides whether a SubMenu1 has SubMenu2 or Not: 
                    // if subMenuAttribute has non-null value then it contains further more submenus's under it(Ex: Supply Ordering), 
                    // else it is direct clickable link (Ex: About National Vision)
                    subMenuAttribute = Selenide.GetElement(driver, Locator.Get(LocatorType.XPath, string.Format
                            (@"//div[@class='menu']/ul/li/a[contains(text(),'{0}')]/following-sibling::ul/li[{1}]/a", menuItem, subMenuCount))).GetAttribute("onclick");

                    #endregion

                    #region Method1_NoSubMenus

                    if (subMenuAttribute != null)
                    {
                        if (subMenuAttribute.Contains("PortalMenu") || subMenuAttribute.Contains("Schedule"))
                        {
                            subMenu = string.Empty;
                            while (subMenu == null || subMenu == string.Empty)
                            {
                                //Clicks menu
                                Selenide.Click(driver, Locator.Get(LocatorType.XPath, string.Format(@"//div[@class='menu']/ul/li/a[contains(text(),'{0}')]", menuItem)));

                                subMenu = Selenide.GetText(driver, Locator.Get(LocatorType.XPath,
                                string.Format(@"//div[@class='menu']/ul/li/a[contains(text(),'{0}')]/following-sibling::ul/li[{1}]/a", menuItem, subMenuCount)),
                                Selenide.ControlType.Label);
                            }
                            reporter.Add(new Act(String.Format("Click the following navigation: <b>{0}</b>>><b>{1}</b>", menuItem, subMenu)));

                            //Clicks submenu
                            Selenide.Click(driver, Locator.Get(LocatorType.XPath,
                                string.Format(@"//div[@class='menu']/ul/li/a[contains(text(),'{0}')]/following-sibling::ul/li[{1}]/a", menuItem, subMenuCount)));

                            AcceptOrDissmissAlertIfPresent(driver, reporter);
                            AcceptErrorMessageIfPresent(driver, reporter, resultsPath);
                            if (Selenide.IsElementExists(driver, Util.GetLocator("Spinner1_img")) ||
                                    Selenide.IsElementExists(driver, Util.GetLocator("Spinner2_img")))
                            {
                                Thread.Sleep(5000);
                            }
                            //WaitUntilSpinnerDisappears(driver);
                            CloseBrowserNewTab(driver);
                            FocusOnContent(driver, reporter);

                            VerifyPageHeading(driver, reporter);
                            //Clicks external applications
                            ClickExternalApplicationMenus(driver, reporter, resultsPath);
                            //Thread.Sleep(500);
                        }
                    }
                    #endregion

                    #region Method2_MoreSubMenus(Sub-SubMenu)

                    if (subMenuAttribute == null)
                    {
                        string submenuname = string.Empty;
                        while (submenuname == null || submenuname == string.Empty)
                        {
                            //Clicks menu
                            Selenide.Click(driver, Locator.Get(LocatorType.XPath, string.Format
                                (@"//div[@class='menu']/ul/li/a[contains(text(),'{0}')]", menuItem)));

                            submenuname = Selenide.GetText(driver, Locator.Get(LocatorType.XPath, string.Format
                                (@"//div[@class='menu']/ul/li/a[contains(text(),'{0}')]/following-sibling::ul/li[{1}]/a/span", menuItem, subMenuCount)), Selenide.ControlType.Label);
                        }
                        reporter.Add(new Act(string.Format(@"Sub Menu Name is: <b>{0}</b>", submenuname)));

                        //Clicks menu
                        Selenide.Click(driver, Locator.Get(LocatorType.XPath,
                        string.Format(@"//div[@class='menu']/ul/li/a[contains(text(),'{0}')]", menuItem)));

                        //Clicks submenu1
                        Selenide.Click(driver, Locator.Get(LocatorType.XPath, string.Format
                                 (@"//div[@class='menu']/ul/li/a[contains(text(),'{0}')]/following-sibling::ul/li[{1}]/a", menuItem, subMenuCount)));

                        //Number of submenu2 in submenu1
                        linksCountInSubMenu1 = Selenide.GetElementCount(driver, Locator.Get(LocatorType.XPath, string.Format
                            (@"//div[@class='menu']/ul/li/a[contains(text(),'{0}')]/following-sibling::ul/li[{1}]/ul/li/a", menuItem, subMenuCount)));

                        for (int eachSubLink = 1; eachSubLink <= linksCountInSubMenu1; eachSubLink++)
                        {
                            //Clicks menu
                            Selenide.Click(driver, Locator.Get(LocatorType.XPath, string.Format
                                (@"//div[@class='menu']/ul/li/a[contains(text(),'{0}')]", menuItem)));

                            //Clicks submenu1
                            Selenide.Click(driver, Locator.Get(LocatorType.XPath, string.Format
                                (@"//div[@class='menu']/ul/li/a[contains(text(),'{0}')]/following-sibling::ul/li[{1}]/a", menuItem, subMenuCount)));

                            string sublinkname = Selenide.GetText(driver, Locator.Get(LocatorType.XPath, string.Format
                                (@"//div[@class='menu']/ul/li/a[contains(text(),'{0}')]/following-sibling::ul/li[{1}]/ul/li[{2}]/a", menuItem, subMenuCount, eachSubLink)), Selenide.ControlType.Label);

                            reporter.Add(new Act(String.Format
                                ("Click submenu link " + "<b>" + "{0}" + "</b>" + " of " + "<b>" + "{1}" + "</b>" + " submenu under " + "<b>" + "{2}" + "</b>" + " menu", sublinkname, submenuname, menuItem)));

                            //Clicks submenu2
                            Selenide.Click(driver, Locator.Get(LocatorType.XPath, string.Format
                                (@"//div[@class='menu']/ul/li/a[contains(text(),'{0}')]/following-sibling::ul/li[{1}]/ul/li[{2}]/a", menuItem, subMenuCount, eachSubLink)));

                            AcceptOrDissmissAlertIfPresent(driver, reporter);
                            AcceptErrorMessageIfPresent(driver, reporter, resultsPath);

                            Selenide.WaitForElementNotVisible(driver, Locator.Get(LocatorType.ClassName, "dlg_spinner"));
                            //Selenide.WaitForElementNotVisible(driver, Util.GetLocator("Spinner1_img"));
                            //Selenide.WaitForElementNotVisible(driver, Util.GetLocator("Spinner2_img"));

                            WaitUntilSpinnerDisappears(driver);
                            if (Selenide.IsElementExists(driver, Util.GetLocator("Spinner1_img")) ||
                                Selenide.IsElementExists(driver, Util.GetLocator("Spinner2_img")))
                            { Thread.Sleep(5000); }
                            CloseBrowserNewTab(driver);
                            ClickExternalApplicationMenus(driver, reporter, resultsPath);
                        }
                    }
                    #endregion

                }


                catch (Exception Ex)
                {
                    AcceptOrDissmissAlertIfPresent(driver, reporter);
                    Selenide.WaitForElementNotVisible(driver, Locator.Get(LocatorType.ClassName, "dlg_spinner"));
                    Selenide.WaitForElementNotVisible(driver, Util.GetLocator("Spinner1_img"));
                    //Selenide.WaitForElementNotVisible(driver, Util.GetLocator("Spinner2_img"));
                    AcceptErrorMessageIfPresent(driver, reporter, resultsPath);
                }

            }
        }

        /// <summary>
        /// FocusOnContent method focuses in the content
        /// </summary>
        /// <param name="TestcaseName">Testcasename(classname)</param>
        /// <param name="valueundermenucoloumn">value under the menu coloumn</param>
        public static void FocusOnContent(RemoteWebDriver driver, Iteration reporter)
        {
            if (Selenide.IsElementExists(driver, Locator.Get(LocatorType.XPath, "//td[@class='left']")))
            {
                Selenide.Focus(driver, Locator.Get(LocatorType.XPath, string.Format(@"//td[@class='left']")));
            }
        }

        /// <summary>
        /// GetMenuColoumnValues method gets the all menu values mentioned in the testdata csv
        /// </summary>
        /// <param name="TestcaseName">Testcasename(classname)</param>
        /// <param name="valueundermenucoloumn">value under the menu coloumn</param>
        public static List<string> GetColoumnValues(string TestCaseName, string valueundermenucoloumn)
        {
            List<string> menulist = new List<string>();
            string menu = string.Empty;
            string menuTitlecase = string.Empty;
            string workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
            DataTable table = new DataTable();
            int temp = 0;
            String[] foundFiles = Directory.GetFiles(workingDirectory, String.Format("{0}.csv", TestCaseName), SearchOption.AllDirectories);
            string[] lines = File.ReadAllLines(foundFiles[0]);

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
                string[] data1 = data.Split(new Char[] { ',' });

                if (data1[1].ToUpper().Equals("YES"))
                {
                    table.Rows.Add(data.Split(new Char[] { ',' }));

                }
            }
            menu = valueundermenucoloumn.ToUpper();

            foreach (DataRow row in table.Rows)
            {
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                menuTitlecase = textInfo.ToTitleCase(row[menu].ToString());
                menulist.Add(row[menu].ToString());

            }

            return menulist;

        }

        /// <summary>
        /// ClickContentLinks method clicks the content links in the WebPortal
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        public static void ClickExternalApplicationMenus(RemoteWebDriver driver, Iteration reporter, string resultsPath)
        {
            int ExternalMenuCount = 0, LinksCountInExternalMenu = 0;
            //bool IsExternalApplication = false, isSIMVisited = false, isAccountingVisited = false,
            //    isContentManagementVisited = false, isWellnessVisited = false, 
            //    isETLVisited = false, isStoreSchedulerVisited = false, isPortalManagerVisited;
            int NumberOfExternalMenu = 0;
            int NumberOfSubmenuInExternalMenu = 0;
            string LinkTextInExternalMenu = string.Empty;
            string ExternalMenuText = string.Empty;
            string submenu1 = string.Empty;

            //Checks for external application
            IsExternalApplication = Selenide.IsElementExists(driver, Locator.Get(LocatorType.XPath, "//div[@class='secmenu']/ul/li/a"));

            if (IsExternalApplication)
            {
                if (Selenide.IsElementExists(driver, Locator.Get(LocatorType.XPath, "//span[@id='spSiteTitle' and text()='Accounting']")))
                {
                    if (!isAccountingVisited)
                    {
                        ExternalApplicationResuable(driver, reporter, resultsPath, NumberOfExternalMenu, NumberOfSubmenuInExternalMenu,
                        LinkTextInExternalMenu, ExternalMenuText, submenu1, ExternalMenuCount, LinksCountInExternalMenu);
                        isAccountingVisited = true;
                    }
                }
                else if (Selenide.IsElementExists(driver, Locator.Get(LocatorType.XPath, "//span[@id='spSiteTitle' and text()='SIM']")))
                {
                    if (!isSIMVisited)
                    {
                        ExternalApplicationResuable(driver, reporter, resultsPath, NumberOfExternalMenu, NumberOfSubmenuInExternalMenu,
                        LinkTextInExternalMenu, ExternalMenuText, submenu1, ExternalMenuCount, LinksCountInExternalMenu);
                        isSIMVisited = true;
                    }
                }
                else if (Selenide.IsElementExists(driver, Locator.Get(LocatorType.XPath, "//span[@id='spSiteTitle' and text()='Portal Manager']")))
                {
                    if (!isPortalManagerVisited)
                    {
                        ExternalApplicationResuable(driver, reporter, resultsPath, NumberOfExternalMenu, NumberOfSubmenuInExternalMenu,
                        LinkTextInExternalMenu, ExternalMenuText, submenu1, ExternalMenuCount, LinksCountInExternalMenu);
                        isPortalManagerVisited = true;
                    }
                }
                else if (Selenide.IsElementExists(driver, Locator.Get(LocatorType.XPath, "//span[@id='spSiteTitle' and text()='Wellness']")))
                {
                    if (!isWellnessVisited)
                    {
                        ExternalApplicationResuable(driver, reporter, resultsPath, NumberOfExternalMenu, NumberOfSubmenuInExternalMenu,
                        LinkTextInExternalMenu, ExternalMenuText, submenu1, ExternalMenuCount, LinksCountInExternalMenu);
                        isWellnessVisited = true;
                    }
                }
                else if (Selenide.IsElementExists(driver, Locator.Get(LocatorType.XPath, "//span[@id='spSiteTitle' and text()='Content Management']")))
                {
                    if (!isContentManagementVisited)
                    {
                        ExternalApplicationResuable(driver, reporter, resultsPath, NumberOfExternalMenu, NumberOfSubmenuInExternalMenu,
                        LinkTextInExternalMenu, ExternalMenuText, submenu1, ExternalMenuCount, LinksCountInExternalMenu);
                        isContentManagementVisited = true;
                    }
                }

                else if (Selenide.IsElementExists(driver, Locator.Get(LocatorType.XPath, "//span[@id='spSiteTitle' and text()='ETL']")))
                {
                    if (!isETLVisited)
                    {
                        ExternalApplicationResuable(driver, reporter, resultsPath, NumberOfExternalMenu, NumberOfSubmenuInExternalMenu,
                        LinkTextInExternalMenu, ExternalMenuText, submenu1, ExternalMenuCount, LinksCountInExternalMenu);
                        isETLVisited = true;
                    }
                }

                else if (Selenide.IsElementExists(driver, Locator.Get(LocatorType.XPath, "//span[@id='spSiteTitle' and text()='Store Scheduler']")))
                {
                    if (!isStoreSchedulerVisited)
                    {
                        ExternalApplicationResuable(driver, reporter, resultsPath, NumberOfExternalMenu, NumberOfSubmenuInExternalMenu,
                        LinkTextInExternalMenu, ExternalMenuText, submenu1, ExternalMenuCount, LinksCountInExternalMenu);
                        isStoreSchedulerVisited = true;
                    }
                }
                else if (Selenide.IsElementExists(driver, Locator.Get(LocatorType.XPath, "//span[@id='spSiteTitle' and text()='Redicall']")))
                {
                    if (!isRedicallVisited)
                    {
                        ExternalApplicationResuable(driver, reporter, resultsPath, NumberOfExternalMenu, NumberOfSubmenuInExternalMenu,
                        LinkTextInExternalMenu, ExternalMenuText, submenu1, ExternalMenuCount, LinksCountInExternalMenu);
                        isRedicallVisited = true;
                    }
                }
                else if (Selenide.IsElementExists(driver, Locator.Get(LocatorType.XPath, "//span[@id='spSiteTitle' and text()='WIP']")))
                {
                    if (!isWIPVisited)
                    {
                        ExternalApplicationResuable(driver, reporter, resultsPath, NumberOfExternalMenu, NumberOfSubmenuInExternalMenu,
                        LinkTextInExternalMenu, ExternalMenuText, submenu1, ExternalMenuCount, LinksCountInExternalMenu);
                        isWIPVisited = true;
                    }
                }
                else if (Selenide.IsElementExists(driver, Locator.Get(LocatorType.XPath, "//span[@id='spSiteTitle' and text()='Assessments']")))
                {
                    if (!isAssessmentsVisited)
                    {
                        ExternalApplicationResuable(driver, reporter, resultsPath, NumberOfExternalMenu, NumberOfSubmenuInExternalMenu,
                        LinkTextInExternalMenu, ExternalMenuText, submenu1, ExternalMenuCount, LinksCountInExternalMenu);
                        isAssessmentsVisited = true;
                    }
                }
            }
        }

        /// <summary>
        /// ExternalApplicationResuable is method called in the above ClickExternalApplicationMenus method
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        public static void ExternalApplicationResuable(RemoteWebDriver driver, Iteration reporter, string resultsPath, int NumberOfMenusInExternalMenu, int NumberOfSubmenuInExternalMenu,
        string LinkTextInExternalMenu,
        string ExternalMenuText,
        string submenu1,
        int ExternalMenuCount, int LinksCountInExternalMenu)
        {

            //Below step gets the menu count in external application
            NumberOfMenusInExternalMenu = Selenide.GetElementCount(driver, Locator.Get(LocatorType.XPath, "//div[@class='secmenu']/ul/li/a"));

            #region ClicksExternalApplicationMenu

            for (ExternalMenuCount = 1; ExternalMenuCount <= NumberOfMenusInExternalMenu; ExternalMenuCount++)
            {
                //Clicks a menu in external application
                Selenide.Click(driver, Locator.Get(LocatorType.XPath, string.Format(@"//div[@class='secmenu']/ul/li[{0}]/a", ExternalMenuCount + 1)));

                //Thread.Sleep(500);

                //Gets the external menu text
                ExternalMenuText = Selenide.GetText(driver, Locator.Get(LocatorType.XPath, string.Format
                    (@"//div[@class='secmenu']/ul/li[{0}]/a", ExternalMenuCount + 1)), Selenide.ControlType.Label);

                //reporter.Add(new Act(string.Format(@"Click on <b>{0}</b> menu in the external application", ExternalMenuText)));

                //Selenide.Click(driver, Locator.Get(LocatorType.XPath, string.Format(@"//div[@class='secmenu']/ul/li[{0}]/a", ExternalMenuCount + 1)));

                //Gets the sub menu's count in the external menu
                NumberOfSubmenuInExternalMenu = Selenide.GetElementCount(driver, Locator.Get(LocatorType.XPath, string.Format
                    (@"//div[@class='secmenu']/ul/li[{0}]/ul/li", ExternalMenuCount + 1)));

                reporter.Add(new Act(string.Format
                    (@"Links count in <b>{0} is:</b> <b> {1}</b>", ExternalMenuText, NumberOfSubmenuInExternalMenu)));

                #region ClickSubMenusInExternalApplicationMenu

                for (LinksCountInExternalMenu = 1; LinksCountInExternalMenu <= NumberOfSubmenuInExternalMenu; LinksCountInExternalMenu++)
                {
                    Selenide.Click(driver, Locator.Get(LocatorType.XPath, string.Format
                        (@"//div[@class='secmenu']/ul/li[{0}]/a", ExternalMenuCount + 1)));

                    LinkTextInExternalMenu = Selenide.GetText(driver, Locator.Get(LocatorType.XPath, string.Format
                        (@"//div[@class='secmenu']/ul/li[{0}]/ul/li[{1}]", ExternalMenuCount + 1, LinksCountInExternalMenu)), Selenide.ControlType.Label);

                    reporter.Add(new Act(string.Format
                        (@"" + LinksCountInExternalMenu + ".Click the the following navigation:" + "<b>" + "{0}>>" + "</b>" + "<b>{1}</b>", ExternalMenuText, LinkTextInExternalMenu)));

                    try
                    {
                        Selenide.Click(driver, Locator.Get(LocatorType.XPath, string.Format
                            (@"//div[@class='secmenu']/ul/li[{0}]/a", ExternalMenuCount + 1)));

                        Selenide.Click(driver, Locator.Get(LocatorType.XPath, string.Format
                            (@"//div[@class='secmenu']/ul/li[{0}]/ul/li[{1}]/a", ExternalMenuCount + 1, LinksCountInExternalMenu)));

                        CloseBrowserNewTab(driver);

                        FocusOnContent(driver, reporter);

                        VerifyPageHeading(driver, reporter);
                    }
                    catch (Exception ex)
                    {
                        AcceptOrDissmissAlertIfPresent(driver, reporter);
                        AcceptErrorMessageIfPresent(driver, reporter, resultsPath);

                        Selenide.WaitForElementNotVisible(driver, Locator.Get(LocatorType.ClassName, "dlg_spinner"));
                        Selenide.WaitForElementNotVisible(driver, Util.GetLocator("Spinner1_img"));
                        reporter.Add(new Act(string.Format
                            (@"Error found in external application menu link: <b>{0}</b>", LinkTextInExternalMenu)));

                        TakeScreenShotAndAttachToReport(driver, LinkTextInExternalMenu, resultsPath, reporter);
                    }
                }
                #endregion
            }
            #endregion
        }


        ///<summary>
        /// VerifyPageHeading is method used to verify page heading
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        public static void VerifyPageHeading(RemoteWebDriver driver, Iteration reporter)
        {
            Selenide.SwitchToFrame(driver, Locator.Get(LocatorType.ID, "iFrameSiteContents"));
            if (Selenide.IsElementExists(driver, Locator.Get(LocatorType.XPath, "//div[@class='SEARCH_TITLE']")))
            {
                string pageHeading = Selenide.GetText(driver, Locator.Get(LocatorType.XPath, string.Format
                (@"//div[@class='SEARCH_TITLE']")), Selenide.ControlType.Label);
                reporter.Add(new Act(string.Format(@"Heading on the page is: <b>{0}</b>", pageHeading)));
            }
            Selenide.SwitchToDefaultContent(driver);
        }

        ///<summary>
        /// ClickAddNewButton is method used to click add new button
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        public static void ClickAddNewButton(RemoteWebDriver driver, Iteration reporter)
        {
            Selenide.SwitchToFrame(driver, Locator.Get(LocatorType.ID, "iFrameSiteContents"));
            Selenide.Click(driver, Locator.Get(LocatorType.XPath, string.Format(@"//a[@id='aAddNew1']")));
            Selenide.SwitchToDefaultContent(driver);
            if (Selenide.IsElementExists(driver, Locator.Get(LocatorType.ID, "dlg_title_search")))
            {
                Selenide.WaitForElementVisible(driver, Locator.Get(LocatorType.ID, "createsearchheader"));
            }
            if (Selenide.IsElementExists(driver, Locator.Get(LocatorType.ID, "submodal_dlg2")))
            {
                Selenide.SwitchToFrame(driver, Locator.Get(LocatorType.ID, "dlg_ifrm_modal2"));
                reporter.Add(new Act(string.Format(@"Click on menu in the external application")));
                Selenide.Click(driver, Locator.Get(LocatorType.ID, "cmdCancel"));
            }
            Selenide.SwitchToDefaultContent(driver);
        }
    }
}

