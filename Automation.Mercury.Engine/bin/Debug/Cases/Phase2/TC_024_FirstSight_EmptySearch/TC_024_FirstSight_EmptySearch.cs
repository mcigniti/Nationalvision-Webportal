﻿/* Description : TC_024_NVI_EmptySearch.cs is a test case  which opens the menu 
                 and searches without passing any parameter.

Date :  26-Apr-2016
*/

using System;
using Automation.Mercury;
using System.Collections.Generic;
using NationalVision.Automation.Pages;

namespace NationalVision.Automation.Tests.Cases.Phase2.TC_024_NVI_EmptySearch
{
    class TC_024_NVI_EmptySearch : BaseCase
    {
        List<string> menulist = CommonPage.GetColoumnValues("TC_024_NVI_EmptySearch", "Menu");
        List<string> submenulist = CommonPage.GetColoumnValues("TC_024_NVI_EmptySearch", "SubMenu");
        List<string> externalapplicationmenulist = CommonPage.GetColoumnValues("TC_024_NVI_EmptySearch", "ExternalApplicationMenu");
        List<string> externalapplicationsubmenulist = CommonPage.GetColoumnValues("TC_024_NVI_EmptySearch", "ExternalApplicationSubMenu");
        bool isTrue = true;
        protected override void ExecuteTestCase()
        {
            Reporter.Chapter.Title = "Verifying the functionality of Cost center number";
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
                    Step = (i + 1) + ":" + " Click following navigation" + "<b>" + menulist[i] + "</b>" +">>"+ "<b>" + submenulist[i] + "</b>";
                    //AmericaBestHomePage.ClickOnMenu(Driver, Reporter, menulist[i]);
                    CommonPage.ClickSubMenuLink(Driver, Reporter, menulist[i], submenulist[i], i, resultsPath);
                    while (isTrue)
                    {
                        if (AmericaBestHomePage.IsMenuAnExternalApplication(Driver, Reporter, submenulist[i]))
                        {
                            //Step = "Click " + externalapplicationmenulist[i] + " in " + submenulist[i] + " Menu";
                            //AmericaBestHomePage.ClickExternalApplicationMenu(Driver, Reporter, externalapplicationmenulist[i],i);

                            Step = (i + 1) + ":" + " Click the following external application menu " + "<b>" + externalapplicationmenulist[i] + "</b>" + ">>" + "<b>" + externalapplicationsubmenulist[i] + "</b>";                                                       
                            AmericaBestHomePage.ClickExternalApplicationSubMenu(Driver, Reporter, externalapplicationmenulist[i], externalapplicationsubmenulist[i], i, resultsPath);
                        }
                        if (i + 1 < submenulist.Count)
                        {
                            isTrue = submenulist[i + 1].Equals(submenulist[i]);
                        }
                        StoreSchedulerPage.ClickSearchButton(Driver, Reporter, resultsPath);
                        ClickOnResults();
                        Selenide.SwitchToDefaultContent(Driver);
                        i++;
                    }
                    i--;
                    isTrue = true;
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