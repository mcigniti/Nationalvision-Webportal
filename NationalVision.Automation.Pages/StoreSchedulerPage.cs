/* **********************************************************************
 * Description : CommonPage.cs class having methods and objects common to all pages.
 *        Header links, Footer links, Menu Tabs, Search window objects.
 *        
 * Date  :  02-Feb-2016
 * 
 * **********************************************************************
 */

using System;
using Automation.Mercury;
using Automation.Mercury.Report;
using OpenQA.Selenium.Remote;
using System.Collections.Generic;
using OpenQA.Selenium.Interactions;
using System.Threading;
using OpenQA.Selenium;

namespace NationalVision.Automation.Pages
{
    public class StoreSchedulerPage : CommonPage
    {



        /// <summary>
        /// TypeStoreNumber method enters store number in store number field
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
                //WaitForPageLoad(driver, 10);
                //CommonPage.WaitUntilSpinnerDisappears(driver);
                CommonPage.AcceptOrDissmissAlertIfPresent(driver, reporter);
                CommonPage.AcceptErrorMessageIfPresent(driver, reporter, resultsPath);
                //Selenide.SwitchToFrame(driver,Locator.Get(LocatorType.ID, "dlg_ifrm_search"));
                Selenide.SwitchToDefaultContent(driver);
            }

            catch (SystemException sysex)
            {
                //    this.Reporter.Chapter.Step.Action.Extra = sysex.Message + "<br/>" + sysex.StackTrace;
                //    Reporter.Chapter.Step.Action.IsSuccess = false;
                CommonPage.AcceptOrDissmissAlertIfPresent(driver, reporter);
                CommonPage.AcceptErrorMessageIfPresent(driver, reporter, resultsPath);
            }

        }

        /// <summary>
        /// CloseStoreLocatorPopupWindow clicks on close icon of doctors entry popup
        /// </summary>
        /// <param name="Driver">Initialized RemoteWebDriver instance</param>
        /// <param name="reporter"></param>
        public static void CloseStoreLocatorPopupWindow(RemoteWebDriver driver, Iteration reporter, string resultsPath)
        {

            try
            {
                reporter.Add(new Act("Close Store Locator Popup Window"));
                //Selenide.SwitchToDefaultContent(driver);
                //Selenide.SwitchToFrame(driver, Locator.Get(LocatorType.ID, "dlg_ifrm_search"));
                Selenide.WaitForElementNotVisible(driver, Locator.Get(LocatorType.ClassName, "dlg_spinner"));
                if (Selenide.IsElementExists(driver, Locator.Get(LocatorType.ID, "createmsgdiv")))
                {
                    CommonPage.AcceptErrorMessageIfPresent(driver, reporter, resultsPath);
                }
                //if (Selenide.IsElementExists(driver, Util.GetLocator("Cancel_btn")))
                //{
                //    Selenide.Click(driver, Util.GetLocator("Cancel_btn"));
                //}
                if (Selenide.IsElementExists(driver, Util.GetLocator("StoreInfoCloseBtn_win")))
                {
                    Selenide.Click(driver, Util.GetLocator("StoreInfoCloseBtn_win"));
                }
                //CommonPage.WaitUntilSpinnerDisappears(driver);
                //CommonPage.AcceptOrDissmissAlertIfPresent(driver, reporter);
                //CommonPage.AcceptErrorMessageIfPresent(driver);
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

