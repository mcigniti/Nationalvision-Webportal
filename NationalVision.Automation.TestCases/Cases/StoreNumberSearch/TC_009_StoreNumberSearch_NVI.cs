/* 
*************************************************************************
Description : TC_009_StoreNumberSearch_NVI.cs is a test case which opens 
                 the menu and enters the store number in the store number 
                 field and opens the search result
Date :  26-Apr-2016
**********************************************************
*/
using Automation.Mercury;
using NationalVision.Automation.Pages;
using System;
using System.Collections.Generic;

namespace NationalVision.Automation.Tests.Cases.StoreNumberSearch.TC_009_StoreNumberSearch_NVI
{
    class TC_009_StoreNumberSearch_NVI : BaseCase
    {
        List<string> menulist = CommonPage.GetColoumnValues("TC_009_StoreNumberSearch_NVI", "Menu");
        List<string> submenulist = CommonPage.GetColoumnValues("TC_009_StoreNumberSearch_NVI", "SubMenu");
        List<string> externalapplicationmenulist = CommonPage.GetColoumnValues("TC_009_StoreNumberSearch_NVI", "ExternalApplicationMenu");
        List<string> externalapplicationsubmenulist = CommonPage.GetColoumnValues("TC_009_StoreNumberSearch_NVI", "ExternalApplicationSubMenu");
        bool isTrueBool = true;

        protected override void ExecuteTestCase()
        {
            Reporter.Chapter.Title = "Verifying the functaionality of External Application Module";
            Step = "Login into Web Portal Application";
            CommonPage.NavigateTo(Driver, Reporter, Util.EnvironmentSettings["Server"]);
            CommonPage.Login(Driver, Reporter, UserName, Password);

            //Select Application from the menu popup.
            Step = "Select " + TestData["APPLICATIONNAME"] + " application from the menu popup.";
            CommonPage.SelectApplication(Driver, Reporter, TestData["APPLICATIONNAME"]);

            //Assert page title of the application.
            Step = "Assert Page Title of " + TestData["PAGETITLE"] + " the application";
            CommonPage.AssertPageTitle(Driver, Reporter, TestData["PAGETITLE"]);

            //Click submenu.

            for (int i = 0; i < menulist.Count; i++)
            {
                try
                {
                    Step = (i + 1) + ":" + " Click>>" + submenulist[i] + " in " + menulist[i] + " menu ";
                    //AmericaBestHomePage.ClickOnMenu(Driver, Reporter, menulist[i]);
                    CommonPage.ClickSubMenuLink(Driver, Reporter, menulist[i], submenulist[i], i, resultsPath);
                    while (isTrueBool)
                    {
                        if (AmericaBestHomePage.IsMenuAnExternalApplication(Driver, Reporter, submenulist[i]))
                        {
                            //Step = "Click " + externalapplicationmenulist[i] + " in " + submenulist[i] + " Menu";
                            //AmericaBestHomePage.ClickExternalApplicationMenu(Driver, Reporter, externalapplicationmenulist[i],i);
                            Step = "Click " + externalapplicationsubmenulist[i] + " in " + externalapplicationmenulist[i] + " External Application Menu";
                            AmericaBestHomePage.ClickExternalApplicationSubMenu(Driver, Reporter, externalapplicationmenulist[i], externalapplicationsubmenulist[i], i, resultsPath);
                        }
                        Step = "Enter Store Number and Click on Search Button";
                        StoreSchedulerPage.TypeStoreNumber(Driver, Reporter, TestData["STORENUMBER"]);
                        StoreSchedulerPage.ClickSearchButton(Driver, Reporter, resultsPath);
                        ClickOnResults();
                        Selenide.SwitchToDefaultContent(Driver);
                        if (submenulist.Count > i + 1)
                        {
                            isTrueBool = submenulist[i + 1].Equals(submenulist[i]);
                            i++;
                        }
                        else
                        {
                            isTrueBool = false;
                        }
                    }
                    i--;
                    isTrueBool = true;
                }
                catch (Exception ex)
                {
                    CommonPage.AcceptOrDissmissAlertIfPresent(Driver, Reporter);
                    CommonPage.AcceptErrorMessageIfPresent(Driver, Reporter, resultsPath);
                    CommonPage.CloseBrowserNewTab(Driver);
                }
            }
        }
        public void ClickOnResults()
        {

            if (Selenide.IsElementExists(Driver, Util.GetLocator("ResultsTable_frm")))
            {
                Selenide.SwitchToFrame(Driver, Util.GetLocator("ResultsTable_frm"));
                if (Selenide.IsElementExists(Driver, Util.GetLocator("ResultsTable1_tbl")))
                {
                    Step = "Click on any store number";
                    StoreSchedulerPage.ClickOnAnyStoreNumber(Driver, Reporter, resultsPath);
                }
                else if (Selenide.IsElementExists(Driver, Util.GetLocator("ResultsTable2_tbl")))
                {
                    Step = "Click on any store number";
                    StoreSchedulerPage.ClickOnAnyStoreNumber(Driver, Reporter, resultsPath);
                }
                if (Selenide.IsElementExists(Driver, Util.GetLocator("StoreInfoPopUp_win")))
                {
                    Step = "Close store popup window";
                    StoreSchedulerPage.CloseStoreLocatorPopupWindow(Driver, Reporter, resultsPath);
                }
                else
                {
                    Step = "No Results Found";
                    Selenide.SwitchToDefaultContent(Driver);
                }

            }
        }
    }
}