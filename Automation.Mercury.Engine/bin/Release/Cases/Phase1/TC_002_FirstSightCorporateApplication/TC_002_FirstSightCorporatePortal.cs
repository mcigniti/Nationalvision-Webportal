/* **********************************************************
 * Description : TC_002_FirstSightCorporateApplication.cs is a test case which opens 
                 all the tabs and links under them and also any content 
                 links if available.
                                           
 *              
 * Date :  26-Apr-2016
 * **********************************************************
 */
using Automation.Mercury;
using NationalVision.Automation.Pages;
using System.Collections.Generic;

namespace NationalVision.Automation.Tests.Cases.Phase1.TC_002_FirstSightCorporateApplication
{
    class TC_002_FirstSightCorporateApplication : BaseCase
    {
        List<string> MenuNames = new List<string>();

        /// <summary>
        /// 
        /// </summary>
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

            int menuCount = CommonPage.GetMenuCount(Driver, Reporter, TestData["APPLICATIONNAME"]);
            for (int menu = 1; menu <= menuCount; menu++)
            {
                MenuNames.Add(CommonPage.GetMenuNames(Driver, Reporter, menu));
            }

            Step = "Number of menu items in  " + "<b>" + TestData["APPLICATIONNAME"] + "</b>" + "menu is:" + menuCount + "";
            foreach (string menu in MenuNames)
            {
                Step = "Click on " + menu + " menu and click links under it";
                CommonPage.ClickAllSubMenusInMenu(Driver, Reporter, resultsPath, menu);
            }

        }
    }
}
