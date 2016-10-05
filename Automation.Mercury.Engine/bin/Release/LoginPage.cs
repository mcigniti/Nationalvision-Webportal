/* *******************************************************
 * Description : LoginPage.cs contains all the methods, objects related to Login Page
 *               Enter UserID, Password, Forgot Password and ForgotUserID    
 *               
 * Date : 04-Feb-2016
 * 
 * *******************************************************
 */

using Automation.Mercury;
using Automation.Mercury.Report;
using System;
using OpenQA.Selenium.Remote;


namespace NationalVision.Automation.Pages
{
    public class LoginPage : CommonPage
    {
        /// <summary>
        /// ClickForgotPasswordLink method clicks on forgot password click here link 
        /// </summary>
        /// <param name="uname">User Name </param>
        /// <param name="password">Password to login into the application</param>
        public static void ClickForgotPasswordLink(RemoteWebDriver driver, Iteration reporter)
        {
            reporter.Add(new Act("Click on forgot password link"));
            Selenide.Click(driver, Util.GetLocator("ForgotPassword_lnk"));
        }

        /// <summary>
        /// Click on logout button
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        public static void clickLogout(RemoteWebDriver driver, Iteration reporter)
        {
            reporter.Add(new Act("Click on  Logout button"));
            Selenide.Click(driver, Util.GetLocator("acclogout_btn"));
        }

        /// <summary>
        /// ClickCreateNewAccount method click on create new account button
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        public static void ClickCreateNewAccount(RemoteWebDriver driver, Iteration reporter)
        {
            reporter.Add(new Act("Click on Create New Account button"));
            Selenide.Click(driver, Util.GetLocator("createaccount_btn"));

            // *** Create New account redirects to AccountRegister Page *** //
        }

        /// <summary>
        /// VerifyMessageWrongPassword method  verifies the error message when we enter the wrong password
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        /// <param name="textToAssert">Expected message</param>
        public static void VerifyMessageWrongPassword(RemoteWebDriver driver, Iteration reporter,
            String textToAssert)
        {
            reporter.Add(new Act("Verify Error message when user enter wrong password"));

            String actualText = Selenide.GetText(driver,
                Locator.Get(LocatorType.XPath, "//div[@class='login-form']/descendant::li"), Selenide.ControlType.Label);

            if (!textToAssert.Equals(actualText))
            {
                reporter.Chapter.Step.Action.IsSuccess = false;
                throw new Exception(String.Format(
                    @"Expected Error message not appeared, Expected: {0}, Actual: {1}", textToAssert, actualText));
            }
        }

        /// <summary>
        /// VerifyMessageInavalidDetails method verifies the message when we enter the invalid details
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="reporter"></param>
        /// <param name="textToAssert">Expected message</param>
        public static void VerifyMessageInavalidDetails(RemoteWebDriver driver, Iteration reporter, String textToAssert)
        {
            reporter.Add(new Act("Verify error message when user enter invalid details"));
            String actualText = Selenide.GetText(driver, 
                Locator.Get(LocatorType.XPath, "//div[@class='login-form']/descendant::li"), Selenide.ControlType.Label);

            if (!textToAssert.Equals(actualText))
            {
                reporter.Chapter.Step.Action.IsSuccess = false;
                throw new Exception(String.Format(
                    @"Expected Error message not appeared, Expected: {0}, Actual: {1}", textToAssert, actualText));
            }
        }
    }
}
