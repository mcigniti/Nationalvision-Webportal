﻿/* Description : TC_017_ClickAddNewButton_NVI.cs is a test case which opens 
                 all the menus and links under them and also any sub 
         links if available.
Date :  26-Apr-2016
*/
using Automation.Mercury;
using NationalVision.Automation.Pages;
using System;
using System.Collections.Generic;

namespace NationalVision.Automation.Tests.Cases.ClickAddNewButton.TC_017_ClickAddNewButton_NVI
{
    class TC_017_ClickAddNewButton_NVI : BaseCase
    {
        List<string> menulist = CommonPage.GetColoumnValues("TC_017_ClickAddNewButton_NVI", "Menu");
        List<string> submenulist = CommonPage.GetColoumnValues("TC_017_ClickAddNewButton_NVI", "SubMenu");
        List<string> externalapplicationmenulist = CommonPage.GetColoumnValues("TC_017_ClickAddNewButton_NVI", "ExternalApplicationMenu");
        List<string> externalapplicationsubmenulist = CommonPage.GetColoumnValues("TC_017_ClickAddNewButton_NVI", "ExternalApplicationSubMenu");
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
                    CommonPage.ClickSubMenuLink(Driver, Reporter, menulist[i], submenulist[i], i, resultsPath);
                    int count = 0;
                    while (isTrueBool && count < 3)
                    {
                        if (CommonPage.IsMenuAnExternalApplication(Driver, Reporter, submenulist[i]))
                        {
                            Step = "Click " + externalapplicationsubmenulist[i] + " in " + externalapplicationmenulist[i] + " External Application Menu";
                            CommonPage.ClickExternalApplicationSubMenu(Driver, Reporter, externalapplicationmenulist[i], externalapplicationsubmenulist[i], i, resultsPath);
                        }
                        Step = "Click add new button";
                        CommonPage.ClickAddNewButton(Driver, Reporter);
                        Step = "Close store popup window";
                        CommonPage.CloseStoreLocatorPopupWindow(Driver, Reporter, resultsPath);
                        isTrueBool = submenulist[i + 1].Equals(submenulist[i]);
                        i++;
                        count++;
                    }
                    i--;
                    isTrueBool = true;
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Unable to locate element"))
                    {
                        this.Reporter.Chapter.Step.Action.IsSuccess = false;
                        this.Reporter.Chapter.Step.Action.Extra = "Exception Message : " + ex.Message + "<br/>" + ex.InnerException + ex.StackTrace;
                        break;
                    }
                    CommonPage.AcceptOrDissmissAlertIfPresent(Driver, Reporter);
                    CommonPage.AcceptErrorMessageIfPresent(Driver, Reporter, resultsPath);
                    CommonPage.CloseBrowserNewTab(Driver);
                }
            }
        }

    }
}