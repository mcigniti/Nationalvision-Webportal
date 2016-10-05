/* Description : TC_019_ClickAddNewButton_EGW.cs is a test case which opens 
                 all the menus and links under them and also any sub 
         links if available.
Date :  26-Apr-2016
*/
using Automation.Mercury;
using NationalVision.Automation.Pages;
using System;
using System.Collections.Generic;

namespace NationalVision.Automation.Tests.Cases.Phase2.TC_018_ClickAddNewButton_FirstSight
{
    class TC_019_ClickAddNewButton_EGW : BaseCase
    {
        List<string> menulist = CommonPage.GetColoumnValues("TC_019_ClickAddNewButton_EGW", "Menu");
        List<string> submenulist = CommonPage.GetColoumnValues("TC_019_ClickAddNewButton_EGW", "SubMenu");
        List<string> externalapplicationmenulist = CommonPage.GetColoumnValues("TC_019_ClickAddNewButton_EGW", "ExternalApplicationMenu");
        List<string> externalapplicationsubmenulist = CommonPage.GetColoumnValues("TC_019_ClickAddNewButton_EGW", "ExternalApplicationSubMenu");
        bool isTrueBool = true;

        protected override void ExecuteTestCase()
        {
            Reporter.Chapter.Title = "Verifying the functaionality of External Application Module";
            Step = "Login into Web Portal Application";
            CommonPage.NavigateTo(Driver, Reporter, Util.EnvironmentSettings["Server"]);
            CommonPage.Login(Driver, Reporter, UserName, Password);

            //Select Application from the menu popup.
            Step = "Select " + TestData["APPLICATIONNAME"] + " application from the menu popup";
            CommonPage.SelectApplication(Driver, Reporter, TestData["APPLICATIONNAME"]);

            //Assert page title of the application.
            Step = "Assert Page Title of " + TestData["PAGETITLE"] + " the application";
            CommonPage.AssertPageTitle(Driver, Reporter, TestData["PAGETITLE"]);

            //Click submenu.

            for (int i = 0; i < menulist.Count; i++)
            {
                try
                {
                    Step = (i + 1) + ":" + " Click " + submenulist[i] + " in " + menulist[i] + " menu ";
                    //AmericaBestHomePage.ClickOnMenu(Driver, Reporter, menulist[i]);
                    AmericaBestHomePage.ClickSubMenuLink(Driver, Reporter, menulist[i], submenulist[i], i, resultsPath);
                    int count = 0;
                    while (isTrueBool && count < 3)
                    {
                        if (AmericaBestHomePage.IsMenuAnExternalApplication(Driver, Reporter, submenulist[i]))
                        {
                            //Step = "Click " + externalapplicationmenulist[i] + " in " + submenulist[i] + " Menu";
                            //AmericaBestHomePage.ClickExternalApplicationMenu(Driver, Reporter, externalapplicationmenulist[i],i);
                            Step = "Click " + externalapplicationsubmenulist[i] + " in " + externalapplicationmenulist[i] + " External Application Menu";
                            AmericaBestHomePage.ClickExternalApplicationSubMenu(Driver, Reporter, externalapplicationmenulist[i], externalapplicationsubmenulist[i], i, resultsPath);
                        }
                        Step = "Click add new button";
                        CommonPage.ClickAddNewButton(Driver, Reporter);
                        Step = "Close store popup window";
                        StoreSchedulerPage.CloseStoreLocatorPopupWindow(Driver, Reporter, resultsPath);
                        isTrueBool = submenulist[i + 1].Equals(submenulist[i]);
                        i++;
                        count++;
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

    }
}