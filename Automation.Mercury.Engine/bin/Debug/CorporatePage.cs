﻿/* **********************************************************************
 * Description : CorporatePage.cs class having methods and objects common to all pages.
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

namespace NationalVision.Automation.Pages
{
    public class CorporatePage : CommonPage
    {

        /// <summary>
        /// ClickCorporateLink selects the Corporate link
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        /// <param name="menuname"></param>
        public static void ClickCorporateLink(RemoteWebDriver driver, Iteration reporter, string menuname)
        {

            reporter.Add(new Act(String.Format("Click the {0} link ", menuname)));
            Selenide.Focus(driver, Locator.Get(LocatorType.XPath,
            string.Format(@"//table[@class='wide100']/descendant::div[@id='bannernav']/descendant::a[text()[normalize-space()='{0}']]", menuname)));
            Selenide.Click(driver, Locator.Get(LocatorType.XPath,
            string.Format(@"//table[@class='wide100']/descendant::div[@id='bannernav']/descendant::a[text()[normalize-space()='{0}']]", menuname)));
           
        }

        /// <summary>
        /// ClickCorporateSubLink selects sub links in Corporate
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        public static void ClickCorporateSubLink(RemoteWebDriver driver, Iteration reporter, string submenuname)
        {

            reporter.Add(new Act(String.Format("Click the {0} link under corporate sublink", submenuname)));
            Selenide.Focus(driver, Locator.Get(LocatorType.XPath,
                string.Format(@"//table[@class='wide100']/descendant::div[@id='bannernav']/descendant::a[text()='{0}']", submenuname)));
            Selenide.Click(driver, Locator.Get(LocatorType.XPath,
                string.Format(@"//table[@class='wide100']/descendant::div[@id='bannernav']/descendant::a[text()='{0}']", submenuname)));
            
        }

        /// <summary>
        /// TypeAccountingDeatls enters details in Accounting Page
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        /// <param name="costcenternumber">First Name</param>
        public static void TypeAccountingDeatls(RemoteWebDriver driver, Iteration reporter,
            string costcenternumber)
        {
            reporter.Add(new Act("Entered Cost center text"));
            Selenide.SwitchToFrame(driver, Locator.Get(LocatorType.ID, "iFrameSiteContents"));
            Selenide.SetText(driver, Util.GetLocator("CostCenter_txt"), Selenide.ControlType.Textbox,costcenternumber);
        }

        /// <summary>
        /// ClickSearch performs Search Operation
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        public static void ClickSearch(RemoteWebDriver driver, Iteration reporter)
        {
            reporter.Add(new Act("Click on Search button"));
            Selenide.Click(driver, Util.GetLocator("AccountingSearch_btn"));
        }

        /// <summary>
        /// VerifyCostCenterNumber method verifies the CC Number in Search Results Page
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        /// <param name="costcenternumber"></param>
        public static void VerifyCostCenterNumber(RemoteWebDriver driver, Iteration reporter, string costcenternumber)
        {

            reporter.Add(new Act(String.Format("Verify {0} cost ceter number is visible", costcenternumber)));
            Selenide.VerifyVisible(driver, Locator.Get(LocatorType.XPath,
                string.Format(@"//tr[@class='GridRow_altN_selN_hovN']/td[2]/a[text()='{0}']", costcenternumber)));
            
        }

        /// <summary>
        /// ClickCostCenterAndVerify performs Search Operation
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        /// <param name="costcenternumber"></param>
        public static void ClickCostCenterAndVerify(RemoteWebDriver driver, Iteration reporter,string costcenternumber)
        {
            reporter.Add(new Act("Click on Search button"));
            Selenide.Click(driver, Locator.Get(LocatorType.XPath,
                string.Format(@"//tr[@class='GridRow_altN_selN_hovN']/td[2]/a[text()='{0}']", costcenternumber)));
        }

        /// <summary>
        /// VerifyNationalVisionDocumentDownload verfies pdf downloads
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>

        public static void VerifyNationalVisionDocumentDownload(RemoteWebDriver driver, Iteration reporter)
        {
            reporter.Add(new Act("Click on About National Vision Documents"));
            Selenide.SwitchToFrame(driver, Util.GetLocator("AboutNationalVisionFrame"));
            Selenide.JS.Click(driver, Util.GetLocator("NVDocument_lnk"));
            Selenide.SwitchToWindow(driver);          
            Selenide.VerifyVisible(driver, Util.GetLocator("VerifyAboutNVDocument_lnk"));
        }


        /// <summary>
        /// VerifyTabsInCostCenterPopUp verfies tabs in cost center popup
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>

        public static void VerifyTabsInCostCenterPopUp(RemoteWebDriver driver, Iteration reporter)
        {
            reporter.Add(new Act("Switch to cost center popup window"));
            reporter.Add(new Act("Click on each tab in the cost center popup window"));
            Selenide.SwitchToDefaultContent(driver);
            Selenide.SwitchToFrame(driver, Locator.Get(LocatorType.ID,"dlg_ifrm_search"));
            int index = Selenide.GetElementCount(driver, Locator.Get(LocatorType.XPath,
                "//div[@id='tab_ctrl']/ul/li"));
            for (int tab = 1; tab <= index; tab++)
            {
                Selenide.JS.Click(driver, Locator.Get(LocatorType.XPath,
                string.Format(@"//div[@id='tab_ctrl']/ul/li[{0}]/a/span", tab)));
            }
        }

        /// <summary>
        /// CloseCostCenterPopUp method closes Patient Information Poup in Scheduler Page
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        public static void CloseCostCenterPopUp(RemoteWebDriver driver, Iteration reporter)
        {
            reporter.Add(new Act("Close Patient Information Poup in Scheduler Page"));
            Selenide.SwitchToDefaultContent(driver);
            Selenide.JS.Click(driver, Locator.Get(LocatorType.XPath,
                string.Format(@"//img[@title='Close']")));
        }

    }
}

