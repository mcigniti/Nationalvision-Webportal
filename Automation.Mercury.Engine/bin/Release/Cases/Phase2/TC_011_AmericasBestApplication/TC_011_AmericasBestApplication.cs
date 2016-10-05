/* **********************************************************
 * Description : TC_011_AmericasBestApplication.cs is a test case which opens 
                 all the tabs and links under them and also any content 
                 links if available.                                          
 *              
 * Date :  26-Apr-2016
 * **********************************************************
 */
using Automation.Mercury;
using NationalVision.Automation.Pages;
using System.Collections.Generic;
using System.Linq;

namespace NationalVision.Automation.Tests.Cases.Phase2.TC_011_AmericasBestApplication
{
    class TC_011_AmericasBestApplication : BaseCase
    {
        List<string> menulist = CommonPage.GetColoumnValues("TC_011_AmericasBestApplication", "Menu");
        List<string> submenulist = CommonPage.GetColoumnValues("TC_011_AmericasBestApplication", "SubMenu");
        List<string> externalapplicationmenulist = CommonPage.GetColoumnValues("TC_011_AmericasBestApplication", "ExternalApplicationMenu");
        List<string> externalapplicationsubmenulist = CommonPage.GetColoumnValues("TC_011_AmericasBestApplication", "ExternalApplicationSubMenu");

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

            for (int i = 0; i < menulist.Count(); i++)
            {
                Step = (i + 1) + ":" + " Click" + submenulist[i] + " in " + menulist[i] + " menu ";
                AmericaBestHomePage.ClickOnMenu(Driver, Reporter, menulist[i]);
                AmericaBestHomePage.ClickSubMenuLink(Driver, Reporter, menulist[i], submenulist[i], i);
                if (AmericaBestHomePage.IsMenuAnExternalApplication(Driver, Reporter, submenulist[i]))
                {
                    //Step = "Click " + externalapplicationmenulist[i] + " in " + submenulist[i] + " Menu";
                    //AmericaBestHomePage.ClickExternalApplicationMenu(Driver, Reporter, externalapplicationmenulist[i], i);
                    Step = "Click " + externalapplicationsubmenulist[i] + " in " + externalapplicationmenulist[i] + " External Application Menu";
                    AmericaBestHomePage.ClickExternalApplicationSubMenu(Driver, Reporter, externalapplicationmenulist[i], externalapplicationsubmenulist[i], i);
                }
                Step = "Enter Store Number and Click on Search Button";
                StoreSchedulerPage.TypeStoreNumber(Driver, Reporter, TestData["STORENUMBER"]);
                StoreSchedulerPage.ClickSearchButton(Driver, Reporter);
                ClickOnResults();
                Selenide.SwitchToDefaultContent(Driver);
            }
        }

        //Method clicks on results in the table
        public void ClickOnResults()
        {
            if (Selenide.IsElementExists(Driver, Util.GetLocator("ResultsTable1_tbl")))
            {
                Step = "Click on any store number";
                StoreSchedulerPage.ClickOnAnyStoreNumber(Driver, Reporter);
            }
            

            else if (Selenide.IsElementExists(Driver, Util.GetLocator("ResultsTable2_tbl")))
            {
                Step = "Click on any store number";
                StoreSchedulerPage.ClickOnAnyStoreNumber(Driver, Reporter);
            }
            if (Selenide.IsElementExists(Driver, Util.GetLocator("StoreInfoPopUp_win")))
            {
                Step = "Close store popup window";
                StoreSchedulerPage.CloseStoreLocatorPopupWindow(Driver, Reporter);
            }
            else
            {
                Step = "No Results Found";
            }
        }
    }
}