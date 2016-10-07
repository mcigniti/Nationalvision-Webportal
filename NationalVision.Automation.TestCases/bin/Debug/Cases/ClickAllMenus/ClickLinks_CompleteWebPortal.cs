/* **********************************************************
 * Description : ClickLinks_CompleteWebPortal.cs is a test case which opens 
                 all the tabs and links under them and also any content 
                 links if available.
                                           
 *              
 * Date :  26-Apr-2016
 * **********************************************************
 */
using Automation.Mercury;
using NationalVision.Automation.Pages;
using System.Collections.Generic;

namespace NationalVision.Automation.Tests.Cases.ClickAllMenus.ClickLinks_CompleteWebPortal
{
    class ClickLinks_CompleteWebPortal : BaseCase
    {
        List<string> applications = CommonPage.GetColoumnValues("ClickLinks_CompleteWebPortal", "APPLICATIONNAME");
        List<string> menuNames = new List<string>();
        int menuIndex, menuCount;
        int intiterator = 1;
        protected override void ExecuteTestCase()
        {
            Reporter.Chapter.Title = "Clicks all the links in the webportal";
            Step = "Login into web portal application";
            CommonPage.NavigateTo(Driver, Reporter, Util.EnvironmentSettings["Server"]);
            CommonPage.Login(Driver, Reporter, UserName, Password);
            try
            {
                foreach (string application in applications)
                {
                    Step = "Select " + application + " from the navigation menu";
                    CommonPage.SelectApplication(Driver, Reporter, application);

                    Step = "Get the menu count  and menu names in " + "<b>" + application + "</b>" + " application";
                    menuCount = CommonPage.GetMenuCount(Driver, Reporter, application);
                    menuNames = CommonPage.GetMenuNames(Driver, Reporter, menuIndex, menuCount);

                    Step = "Number of links in  " + "<b>" + application + "</b>" + " is:" + "<b>" + menuCount + "</b>" + ", Check below";

                    foreach (string menuItem in menuNames)
                    {
                        Step = (intiterator++) + ". Click " + "<b>" + menuItem + "</b>" + " and click sub menus under it";
                        CommonPage.ClickAllSubMenusInEachMenu(Driver, Reporter, resultsPath, menuItem, menuNames);
                    }

                    Step = "Switch to " + "<b>" + application + "</b>" + " application";
                    CommonPage.SwitchApplication(Driver, Reporter, application);
                }
            }
            catch (System.Exception)
            {
                CommonPage.AcceptOrDissmissAlertIfPresent(Driver, Reporter);
                CommonPage.AcceptErrorMessageIfPresent(Driver, Reporter, resultsPath);
            }

        }
    }
}