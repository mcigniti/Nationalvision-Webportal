/* **********************************************************************
 * Description : CommonPage.cs class having methods and objects common to all pages.
 *        Header links, Footer links, Menu Tabs, Search window objects.
 *        
 * Date  :  20-Apr-2016
 * 
 * **********************************************************************
 */

using System;
using Automation.Mercury;
using Automation.Mercury.Report;
using OpenQA.Selenium.Remote;
using System.Collections.Generic;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading;


namespace NationalVision.Automation.Pages
{
    public class AmericaBestHomePage : CommonPage
    {


        /// <summary>
        /// IsMenuAnExternalApplication checks whether menu external application
        /// </summary>
        /// <param name="driver">Initialized RemoteWebDriver instance</param>
        /// <param name="reporter"></param>
        /// <param name="submenuname">Link Name</param> 
        public static bool IsMenuAnExternalApplication(RemoteWebDriver driver, Iteration reporter, string submenuname)
        {
            reporter.Add(new Act(String.Format("Verify whether the menu {0} is an external application", submenuname)));
            return Selenide.IsElementExists(driver, Util.GetLocator("ExternalApplication_menu"));
        } 
        
      }      
    }



