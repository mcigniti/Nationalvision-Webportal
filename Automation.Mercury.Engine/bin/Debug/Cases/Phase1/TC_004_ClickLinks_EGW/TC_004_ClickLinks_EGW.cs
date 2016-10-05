/* **********************************************************
 * Description : TC_004_ClickLinks_EGW.cs is a test case which opens 
                 all the tabs and links under them and also any content 
                 links if available.
                                           
 *              
 * Date :  26-Apr-2016
 * **********************************************************
 */
using Automation.Mercury;
using NationalVision.Automation.Pages;
using System.Collections.Generic;

namespace NationalVision.Automation.Tests.Cases.Phase1.TC_004_ClickLinks_EGW
{
    class TC_004_ClickLinks_EGW : BaseCase
    {
        List<string> menuNames = new List<string>();
        int menuIndex, menuCount;
        int intiterator = 1;
        protected override void ExecuteTestCase()
        {
            Reporter.Chapter.Title = "Clicks all the links in the webportal";
            Step = "Login into web portal application";
            CommonPage.NavigateTo(Driver, Reporter, Util.EnvironmentSettings["Server"]);
            CommonPage.Login(Driver, Reporter, UserName, Password);

            Step = "Select " + TestData["APPLICATIONNAME"] + " link from the navigation menu";
            CommonPage.SelectApplication(Driver, Reporter, TestData["APPLICATIONNAME"]);

            Step = "Assert page title of " + TestData["TITLE"] + "";
            CommonPage.AssertPageTitle(Driver, Reporter, TestData["TITLE"]);

            Step = "Get the menu count  and menu names in " + "<b>" + TestData["APPLICATIONNAME"] + "</b>" + "";
            menuCount = CommonPage.GetMenuCount(Driver, Reporter, TestData["APPLICATIONNAME"]);
            menuNames = CommonPage.GetMenuNames(Driver, Reporter, menuIndex, menuCount);

            Step = "Number of menu items in  " + "<b>" + TestData["APPLICATIONNAME"] + "</b>" + " menu is:" + "<b>" + menuCount + "</b>" + ", Check below";

            foreach (string menuItem in menuNames)
            {
                Step = (intiterator++) + ". Click the " + "<b>" + menuItem + "</b>" + " menu and click all sub menus under it";
                CommonPage.ClickAllSubMenusInEachMenu(Driver, Reporter, resultsPath, menuItem, menuNames);
            }
        }
    }
}