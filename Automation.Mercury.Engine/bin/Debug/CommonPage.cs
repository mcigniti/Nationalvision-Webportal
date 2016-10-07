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
        static bool IsExternalApplication = false;
        static List<string> externalAppNames = new List<string>();

        /// <summary>
        /// NavigateTo method naviagtes the url
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        /// <param name="url"></param>
        public static void NavigateTo(RemoteWebDriver driver, Iteration reporter, String url)
        {
            Selenide.NavigateTo(driver, url);
        }

        /// <summary>
        /// SelectApplication method selects the application at login screen
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        /// <param name="applicationName"></param>
        public static void SelectApplication(RemoteWebDriver driver, Iteration reporter, string applicationName)
        {
            reporter.Add(new Act(String.Format("Select the {0} link ", applicationName)));
            Selenide.SwitchToFrame(driver, Locator.Get(LocatorType.ID, "dlg_ifrm_modal2"));
            try
            {
                Actions action = new Actions(driver);
                IWebElement application = Selenide.GetElement(driver, Locator.Get(LocatorType.XPath, string.Format(@"//table[@id='tblSelect']/descendant::a[contains(@title,""{0}"")]/div", applicationName)));
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
        /// IsMenuAnExternalApplication clicks the submenu link
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        /// <param name="submenuname"></param>
        /// <returns>bool</returns>
        public static bool IsMenuAnExternalApplication(RemoteWebDriver driver, Iteration reporter, string submenuname)
        {
            return Selenide.IsElementExists(driver, Util.GetLocator("ExternalApplication_menu"));
        }

        /// <summary>
        /// SwitchApplication method switches the application at login screen
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        /// <param name="applicationName"></param>
        public static void SwitchApplication(RemoteWebDriver driver, Iteration reporter, string applicationName)
        {
            reporter.Add(new Act(String.Format("Double click on the logo")));
            Selenide.SwitchToDefaultContent(driver);
            try
            {
                Selenide.DoubleClick(driver, Util.GetLocator("ApplicationLogo_img"));
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("{ 0 } link not found in the website", applicationName));
            }
        }


        /// <summary>
        /// WaitForPageLoad method holds the driver  until it loads
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="seconds"></param>
        public static void WaitForPageLoad(RemoteWebDriver driver, int seconds = 0)
        {
            string state = string.Empty;
            try
            {

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
        /// ClickExternalApplicationSubMenu clicks on the menu in the external application submenu
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        /// <param name="extMenuName"></param>
        /// <param name="extSubMenuName"></param>
        /// <param name="i"></param>
        /// <param name="resultsPath"></param>
        public static void ClickExternalApplicationSubMenu(RemoteWebDriver driver, Iteration reporter, string extMenuName, string extSubMenuName, int i, string resultsPath)
        {
            int count = 0;
            try
            {
                //reporter.Add(new Act(String.Format((i + 1) + ": Click the following ext application menu: <i>{0}</i>>><i>{1}</i>", extMenuName, extSubMenuName)));

                //Mandatory step:
                count = 0;
                while (count < 2)
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
        /// <param name="resultsPath"></param>
        public static void ClickSearchButton(RemoteWebDriver driver, Iteration reporter, string resultsPath)
        {
            reporter.Add(new Act("Click on search button"));
            try
            {
                Selenide.SwitchToFrame(driver, Locator.Get(LocatorType.Name, "iFrameSiteContents"));
                Selenide.Click(driver, Locator.Get(LocatorType.XPath, "//div[@id='divSearchFields']//input"));
                Selenide.SwitchToDefaultContent(driver);

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

        /// <summary>
        /// ClickSubMenuLink method clicks submenu's
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        /// <param name="menuName"></param>
        /// <param name="submenuname"></param>
        /// <param name="i"></param>
        /// <param name="resultsPath"></param>
        public static void ClickSubMenuLink(RemoteWebDriver driver, Iteration reporter,
            string menuName, string submenuname, int i, string resultsPath)
        {
            try
            {
                Selenide.SwitchToDefaultContent(driver);

                //reporter.Add(new Act(String.Format((i + 1) + ": Click the following navigtaion:<i> {0} >> {1} </i>", menuName,submenuname)));

                //Clicks menu
                Selenide.Click(driver, Locator.Get(LocatorType.XPath, string.Format(@"//a[normalize-space(text())='{0}']", menuName)));

                //Checks if submenu is available
                if (Selenide.IsElementExists(driver, Locator.Get(LocatorType.XPath, string.Format(@"//a[normalize-space(text())='{0}']", submenuname))))
                {
                    //Clicks submenu 
                    Selenide.Click(driver, Locator.Get(LocatorType.XPath, string.Format(@"//a[normalize-space(text())='{0}']", submenuname)));
                }

                //Checks if submenu is available
                else if (Selenide.IsElementExists(driver, Locator.Get(LocatorType.XPath, string.Format(@"//a/nobr[normalize-space(text())='{0}']", submenuname))))
                {
                    //Clicks submenu 
                    Selenide.Click(driver, Locator.Get(LocatorType.XPath, string.Format(@"//a/nobr[normalize-space(text())='{0}']", submenuname)));
                }

                //Waits until spinner dissappears
                Selenide.WaitForElementNotVisible(driver, Locator.Get(LocatorType.ClassName, "dlg_spinner"));
                //Accepts or dismisses if any alert came
                AcceptOrDissmissAlertIfPresent(driver, reporter);
                //Closes if any error popup comes
                AcceptErrorMessageIfPresent(driver, reporter, resultsPath);
                FocusOnContent(driver, reporter);
            }
            catch (Exception)
            {
                AcceptOrDissmissAlertIfPresent(driver, reporter);
                //Closes if any error popup comes
                AcceptErrorMessageIfPresent(driver, reporter, resultsPath);
                //Closes newly opened tab of the browser
                CloseBrowserNewTab(driver);
                //Focuses on the content
                FocusOnContent(driver, reporter);
            }
        }

        /// <summary>
        /// AssertPageTitle method verifies page title macth
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        /// <param name="pTitle"></param>
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
        /// Performs login
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
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
        /// <param name="link"></param>
        /// <param name="resultsPath"></param>
        /// <param name="reporter"></param>
        public static void takeScreenshot(RemoteWebDriver driver, string link, string resultsPath, Iteration reporter)
        {
            Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
            string screenshot = ss.AsBase64EncodedString;
            byte[] screenshotAsByteArray = ss.AsByteArray;
            ss.SaveAsFile(Path.Combine(resultsPath, "Screenshots", String.Format("{0}.Png", link, DateTime.Now.ToString("hhmmssfff"))), ImageFormat.Png);
            ss.ToString();
            reporter.Add(new Act("Click on the " + link + " PDF <a href='" + Path.Combine("Screenshots",
            String.Format("{0}.png", link)) + "'><span class='glyphicon glyphicon-paperclip normal'></span></a>&nbsp;"));

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

        }

        /// <summary>
        /// WaitUntilSpinnerDisappears method waits until spinner disappers.
        /// </summary>
        /// <param name="driver"></param>
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
                driver.Close();
                driver.SwitchTo().Window(handles[0]);
            }
        }

        /// <summary>
        /// AcceptOrDissmissAlertIfPresent method Accepts/dismisses alert if present.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        public static void AcceptOrDissmissAlertIfPresent(RemoteWebDriver driver, Iteration reporter)
        {
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
        /// <param name="resultsPath"></param>
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
        /// GetMenuCount method returns menu count  
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        /// <param name="Application"></param>
        /// <returns></returns>
        public static int GetMenuCount(RemoteWebDriver driver, Iteration reporter,
            string Application)
        {
            reporter.Add(new Act(string.Format(@"Get the menu count in {0}", Application)));
            return Selenide.GetElementCount(driver, (Util.GetLocator("MenuGrid_lbl")));
        }

        /// <summary>
        /// MenuNames returns menu text of menu link
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        /// <param name="menuIndex"></param>
        /// <param name="menucount"></param>
        /// <returns = "menuNames"></returns>
        public static List<string> GetMenuNames(RemoteWebDriver driver, Iteration reporter,
            int menuIndex, int menucount)
        {
            List<string> menuNames = new List<string>();

            if (menucount > 0)
            {
                for (menuIndex = 1; menuIndex <= menucount; menuIndex++)
                {
                    menuNames.Add(Selenide.GetText(driver, Locator.Get(LocatorType.XPath,
                    string.Format(@"//div[@class='menu']/ul/li[{0}]", menuIndex)), Selenide.ControlType.Label));
                }
            }
            return menuNames;
        }


        /// <summary>
        /// ClickAllSubMenusInMenu method clicks the links in each menu of WebPortal
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        /// <param name="resultsPath"></param>
        /// <param name="menuItem"></param>
        /// <param name="menuNames"></param>
        public static void ClickAllSubMenusInEachMenu(RemoteWebDriver driver, Iteration reporter, string resultsPath, string menuItem, List<string> menuNames)
        {
            string subMenu = string.Empty, subMenuAttribute = string.Empty, sublinkname= string.Empty;
            int linksCountInEachMenu, linksCountInSubMenu, subMenuCount, subMenuCountInEachMenu, clickableLinksInEachMenu, linksCountInSubMenu1 = 0;

            //Clicks menu
            Selenide.Click(driver, Locator.Get(LocatorType.XPath, string.Format(@"//div[@class='menu']/ul/li/a[contains(text(),'{0}')]", menuItem)));

            //Number of submenu's in a menu
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

                    #region Method1_Decides_ClickableLink(Ex:AboutNationalVision)orNot(Ex:SupplyOrdering)
                    //The below step decides whether a SubMenu1 has SubMenu2 or Not: 
                    // if subMenuAttribute has non-null value then it contains further more submenus's under it(Ex: Supply Ordering), 
                    // else it is direct clickable link (Ex: About National Vision)
                    subMenuAttribute = Selenide.GetElement(driver, Locator.Get(LocatorType.XPath, string.Format
                            (@"//div[@class='menu']/ul/li/a[contains(text(),'{0}')]/following-sibling::ul/li[{1}]/a", menuItem, subMenuCount))).GetAttribute("onclick");

                    #endregion

                    #region Method2_NoSubMenus

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

                    #region Method3_MoreSubMenus(Sub-SubMenu)

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

                            sublinkname = Selenide.GetText(driver, Locator.Get(LocatorType.XPath, string.Format
                                (@"//div[@class='menu']/ul/li/a[contains(text(),'{0}')]/following-sibling::ul/li[{1}]/ul/li[{2}]/a", menuItem, subMenuCount, eachSubLink)), Selenide.ControlType.Label);

                            reporter.Add(new Act(String.Format
                                ("Click submenu link " + "<b>" + "{0}" + "</b>" + " of " + "<b>" + "{1}" + "</b>" + " submenu under " + "<b>" + "{2}" + "</b>" + " menu", sublinkname, submenuname, menuItem)));

                            //Clicks submenu2
                            Selenide.Click(driver, Locator.Get(LocatorType.XPath, string.Format
                                (@"//div[@class='menu']/ul/li/a[contains(text(),'{0}')]/following-sibling::ul/li[{1}]/ul/li[{2}]/a", menuItem, subMenuCount, eachSubLink)));

                            AcceptOrDissmissAlertIfPresent(driver, reporter);
                            AcceptErrorMessageIfPresent(driver, reporter, resultsPath);

                            Selenide.WaitForElementNotVisible(driver, Locator.Get(LocatorType.ClassName, "dlg_spinner"));

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
                    AcceptErrorMessageIfPresent(driver, reporter, resultsPath);
                    reporter.Add(new Act(string.Format 
                            (@"Error found in application menu link: <b>{0}</b>", Ex.Message)));

                    TakeScreenShotAndAttachToReport(driver, sublinkname, resultsPath, reporter);
                }
            }
        }

        /// <summary>
        /// FocusOnContent method focuses in the content
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
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
        /// <param name="TestCaseName"></param>
        /// <param name="valueundermenucoloumn"></param>
        /// <returns></returns>
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
        /// <param name="resultsPath"></param>
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
            string extAppName;

            //Checks for external application
            IsExternalApplication = Selenide.IsElementExists(driver, Locator.Get(LocatorType.XPath, "//div[@class='secmenu']/ul/li/a"));

            //Execution enters into this below loop if the menu is an external application
            if (IsExternalApplication)
            {
                extAppName = Selenide.GetText(driver, Util.GetLocator("ExternalApplication_lbl"), Selenide.ControlType.Label);

                if (!externalAppNames.Contains(extAppName))
                {
                    ExternalApplicationResuable(driver, reporter, resultsPath, NumberOfExternalMenu, NumberOfSubmenuInExternalMenu,
                    LinkTextInExternalMenu, ExternalMenuText, submenu1, ExternalMenuCount, LinksCountInExternalMenu);
                }
                externalAppNames.Add(extAppName);
            }
        }

        /// <summary>
        /// ExternalApplicationResuable method clicks external applications
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        /// <param name="resultsPath"></param>
        /// <param name="NumberOfMenusInExternalMenu"></param>
        /// <param name="NumberOfSubmenuInExternalMenu"></param>
        /// <param name="LinkTextInExternalMenu"></param>
        /// <param name="ExternalMenuText"></param>
        /// <param name="submenu1"></param>
        /// <param name="ExternalMenuCount"></param>
        /// <param name="LinksCountInExternalMenu"></param>
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

                //Gets the external menu text
                ExternalMenuText = Selenide.GetText(driver, Locator.Get(LocatorType.XPath, string.Format
                    (@"//div[@class='secmenu']/ul/li[{0}]/a", ExternalMenuCount + 1)), Selenide.ControlType.Label);

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
                        AcceptOrDissmissAlertIfPresent(driver, reporter);
                        AcceptErrorMessageIfPresent(driver, reporter, resultsPath);

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
                            (@"Error found in external application menu link: <b>{0}</b>", ex.Message)));

                        TakeScreenShotAndAttachToReport(driver, LinkTextInExternalMenu, resultsPath, reporter);
                    }
                }
                #endregion
            }
            #endregion
        }


        ///<summary>
        /// VerifyPageHeading method used to verify page heading
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
        /// ClickAddNewButton method clicks add new button
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        public static void ClickAddNewButton(RemoteWebDriver driver, Iteration reporter)
        {
            Selenide.SwitchToFrame(driver, Locator.Get(LocatorType.ID, "iFrameSiteContents"));
            if (Selenide.IsElementExists(driver, Locator.Get(LocatorType.ID, "aAddNew1")))
            {
                Selenide.Click(driver, Locator.Get(LocatorType.XPath, string.Format(@"//a[@id='aAddNew1']")));
            }
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



















        /// <summary>
        /// ClickOnTabsInStoreInfoPopUpWindow clicks on each tab in the popup window
        /// </summary>
        /// <param name="Driver">Initialized RemoteWebDriver instance</param>
        /// <param name="reporter"></param>
        /// <param name="submenuname">Link Name</param>
        public static void CloseStoreInformationPoupWindow(RemoteWebDriver driver, Iteration reporter)
        {
            reporter.Add(new Act("Close Store Information Popup window"));
            Selenide.Click(driver, Locator.Get(LocatorType.XPath, "StoreInfoPopUpCloseBtn_btn"));
        }

        /// <summary>
        /// EnterStoreNumber method enters store number in store number field
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        /// <param name="storeNumber">Store Number</param>
        public static void TypeStoreNumber(RemoteWebDriver driver, Iteration reporter,
            string storeNumber)
        {
            reporter.Add(new Act("Enter store number in store number field"));
            Selenide.SwitchToFrame(driver, Locator.Get(LocatorType.ID, "iFrameSiteContents"));
            if (storeNumber.Equals(""))
            {
                return;
            }
            else if (Selenide.IsElementExists(driver, Util.GetLocator("StoreNumber1_txt")))
            {
                Selenide.Clear(driver, Util.GetLocator("StoreNumber1_txt"), Selenide.ControlType.Textbox);
                Selenide.SetText(driver, Util.GetLocator("StoreNumber1_txt"), Selenide.ControlType.Textbox, storeNumber);
            }

            //Selenide.SwitchToDefaultContent(driver);
        }

        /// <summary>
        /// TypeCostCenterNumber method enters cost center number in cost center number field
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        /// <param name="costCenterNumber">Store Number</param>
        public static void TypeCostCenterNumber(RemoteWebDriver driver, Iteration reporter,
            string costCenterNumber)
        {
            reporter.Add(new Act("Enter cost center number in cost center number field"));
            Selenide.SwitchToFrame(driver, Locator.Get(LocatorType.ID, "iFrameSiteContents"));
            if (costCenterNumber.Equals(""))
            {
                return;
            }
            else if (Selenide.IsElementExists(driver, Util.GetLocator("CostCenterNumber1_txt")))
            {
                Selenide.Clear(driver, Util.GetLocator("CostCenterNumber1_txt"), Selenide.ControlType.Textbox);
                Selenide.SetText(driver, Util.GetLocator("CostCenterNumber1_txt"), Selenide.ControlType.Textbox, costCenterNumber);
            }

            //Selenide.SwitchToDefaultContent(driver);
        }

        /// <summary>
        /// ClickOnAnyStoreNumber method clicks on store number in the results
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        /// <param name="resultsPath"></param>
        public static void ClickOnAnyStoreNumber(RemoteWebDriver driver, Iteration reporter, string resultsPath)
        {
            try
            {
                reporter.Add(new Act("Click on store number in results of Stores Directory Page"));
                //Selenide.SwitchToFrame(driver, Locator.Get(LocatorType.ID, "iFrameSiteContents"));
                if (Selenide.IsElementExists(driver, Locator.Get(LocatorType.XPath, "//table[@class='formview']/descendant::tr[2]/td//a/nobr")))
                {
                    Selenide.Click(driver, Locator.Get(LocatorType.XPath, ("//table[@class='formview']/descendant::tr[2]/td//a/nobr")));
                }
                else if (Selenide.IsElementExists(driver, Locator.Get(LocatorType.XPath, "//table[@class='formview']/descendant::tr[2]/td//a")))
                {
                    Selenide.Click(driver, Locator.Get(LocatorType.XPath, ("//table[@class='formview']/descendant::tr[2]/td//a")));
                }
                else
                {
                    reporter.Add(new Act("Results not found"));
                }

                Selenide.WaitForElementNotVisible(driver, Locator.Get(LocatorType.ClassName, "dlg_spinner"));
                CommonPage.AcceptOrDissmissAlertIfPresent(driver, reporter);
                CommonPage.AcceptErrorMessageIfPresent(driver, reporter, resultsPath);
                Selenide.SwitchToDefaultContent(driver);
            }

            catch (SystemException sysex)
            {
                CommonPage.AcceptOrDissmissAlertIfPresent(driver, reporter);
                CommonPage.AcceptErrorMessageIfPresent(driver, reporter, resultsPath);
            }

        }

        /// <summary>
        /// CloseStoreLocatorPopupWindow method clicks on close icon of doctors entry popup
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        /// <param name="resultsPath"></param>
        public static void CloseStoreLocatorPopupWindow(RemoteWebDriver driver, Iteration reporter, string resultsPath)
        {

            try
            {
                reporter.Add(new Act("Close Store Locator Popup Window"));
                Selenide.WaitForElementNotVisible(driver, Locator.Get(LocatorType.ClassName, "dlg_spinner"));
                if (Selenide.IsElementExists(driver, Locator.Get(LocatorType.ID, "createmsgdiv")))
                {
                    CommonPage.AcceptErrorMessageIfPresent(driver, reporter, resultsPath);
                }
                if (Selenide.IsElementExists(driver, Util.GetLocator("StoreInfoCloseBtn_win")))
                {
                    Selenide.Click(driver, Util.GetLocator("StoreInfoCloseBtn_win"));
                }
                Selenide.SwitchToDefaultContent(driver);
            }
            catch (Exception ex)
            {
                CommonPage.AcceptOrDissmissAlertIfPresent(driver, reporter);
                CommonPage.AcceptErrorMessageIfPresent(driver, reporter, resultsPath);
            }
        }
    }
}

