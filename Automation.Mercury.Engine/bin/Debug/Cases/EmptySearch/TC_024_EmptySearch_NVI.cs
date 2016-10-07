/* Description : TC_024_EmptySearch_NVI.cs is a test case  which opens the menu 
                 and searches without passing any parameter.

Date :  26-Apr-2016
*/

using System;
using Automation.Mercury;
using System.Collections.Generic;
using NationalVision.Automation.Pages;

namespace NationalVision.Automation.Tests.Cases.StoreNumberSearch.TC_024_EmptySearch_NVI
{
    class TC_024_EmptySearch_NVI : BaseCase
    {
        List<string> menulist = CommonPage.GetColoumnValues("TC_024_EmptySearch_NVI", "Menu");
        List<string> submenulist = CommonPage.GetColoumnValues("TC_024_EmptySearch_NVI", "SubMenu");
        List<string> externalapplicationmenulist = CommonPage.GetColoumnValues("TC_024_EmptySearch_NVI", "ExternalApplicationMenu");
        List<string> externalapplicationsubmenulist = CommonPage.GetColoumnValues("TC_024_EmptySearch_NVI", "ExternalApplicationSubMenu");
        bool isTrueBool = true;
        protected override void ExecuteTestCase()
        {
            Reporter.Chapter.Title = "Verifying test results without any search parameter";
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
                    // To click the menu and submenu under it
                    Step = (i + 1) + ":" + " Click the following navigation: " + "<i>" + menulist[i] + "</i>" + ">>" + "<i>" + submenulist[i] + "</i>";
                    CommonPage.ClickSubMenuLink(Driver, Reporter, menulist[i], submenulist[i], i, resultsPath);

                    if (CommonPage.IsMenuAnExternalApplication(Driver, Reporter, submenulist[i]))
                    {
                        while (isTrueBool)
                        {
                            Step = "<i>" + (i + 1) + ":" + " Click the following external application menu " + "<b>" + externalapplicationmenulist[i] + "</b>" + ">>" + "<b>" + externalapplicationsubmenulist[i] + "</b>" + "</i>";
                            CommonPage.ClickExternalApplicationSubMenu(Driver, Reporter, externalapplicationmenulist[i], externalapplicationsubmenulist[i], i, resultsPath);

                            CommonPage.ClickSearchButton(Driver, Reporter, resultsPath);
                            VerifyResults();
                            Selenide.SwitchToDefaultContent(Driver);
                            if (i + 1 < submenulist.Count)
                            {
                                isTrueBool = submenulist[i + 1].Equals(submenulist[i]);
                            }
                            i++;
                        }
                        i--;
                        isTrueBool = true;
                    }
                }
                catch (Exception ex)
                {
                    CommonPage.AcceptOrDissmissAlertIfPresent(Driver, Reporter);
                    CommonPage.AcceptErrorMessageIfPresent(Driver, Reporter, resultsPath);
                    CommonPage.CloseBrowserNewTab(Driver);
                }
            }
        }
        public void VerifyResults()
        {

            if (Selenide.IsElementExists(Driver, Util.GetLocator("ResultsTable_frm")))
            {
                Selenide.SwitchToFrame(Driver, Util.GetLocator("ResultsTable_frm"));
                if (Selenide.IsElementExists(Driver, Util.GetLocator("ResultsTable1_tbl")))
                {
                    Step = "Results Found";
                }
                else if (Selenide.IsElementExists(Driver, Util.GetLocator("ResultsTable2_tbl")))
                {
                    Step = "Results Found";
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