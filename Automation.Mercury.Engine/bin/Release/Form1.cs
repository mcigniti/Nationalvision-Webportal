using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Reflection;
using Automation.Mercury;
using System.Xml;
using System.Diagnostics;
using Automation.Mercury.Report;

namespace Automation.Mercury.Engine
{
    public partial class form_SuiteRunner : Form
    {
        List<string> lst_AvailableModules = new List<string>();
        List<string> lst_AvailableSubModules = new List<string>();
        List<string> lst_AvailableCategories = new List<string>();
        DataView dvTestCaseDetails = null;
        DataTable dtTestCaseDetails = new DataTable();
        string strModuleFilterCriteria = string.Empty;
        string strSubModuleFilterCriteria = string.Empty;
        string strCategoryFilterCriteria = string.Empty;
        public static List<Object[]> testCaseToExecute = new List<object[]>();
        public static Dictionary<String, String> qualifiedNames = new Dictionary<string, string>();
        public form_SuiteRunner()
        {
            InitializeComponent();
        }

        private void btn_LoadTests_Click(object sender, EventArgs e)
        {
            string assemblyFileName = ConfigurationManager.AppSettings.Get("TestsDLLName").ToString();
            string strDllPath = Directory.GetCurrentDirectory();
            string strDLLPath = string.Concat(strDllPath, "\\", assemblyFileName);
            dtTestCaseDetails.Columns.Clear();
            dtTestCaseDetails.Columns.Add("ToExecute", typeof(bool));
            dtTestCaseDetails.Columns.Add("ModuleName", typeof(string));
            dtTestCaseDetails.Columns.Add("SubModuleName", typeof(string));
            dtTestCaseDetails.Columns.Add("TestCaseName", typeof(string));
            dtTestCaseDetails.Columns.Add("TestCaseDescription", typeof(string));
            dtTestCaseDetails.Columns.Add("ExecutionCategories", typeof(string));
            int i_Counter = 0;
            Assembly assembly = Assembly.LoadFrom(strDLLPath);
            Array.ForEach(assembly.GetTypes(), type =>
            {
                if (type.GetCustomAttributes(typeof(ScriptAttribute), true).Length > 0)
                {
                    i_Counter++;
                    var objTestCase = (ScriptAttribute)type.GetCustomAttribute(typeof(ScriptAttribute));
                    string strModuleName = objTestCase.ModuleName;
                    string strSubModuleName = objTestCase.SubModuleName;
                    string strTestCaseName = type.Name;
                    qualifiedNames.Add(strTestCaseName, type.AssemblyQualifiedName);
                    string strTestCaseDescription = objTestCase.TestCaseDescription;
                    string strExecutionCategories = objTestCase.ExecutionCategories;
                    dtTestCaseDetails.Rows.Add(false, strModuleName, strSubModuleName, strTestCaseName, strTestCaseDescription, strExecutionCategories);

                    if (!lst_AvailableModules.Contains(strModuleName))
                    {
                        lst_AvailableModules.Add(strModuleName);
                    }

                    if (!lst_AvailableSubModules.Contains(strSubModuleName))
                    {
                        lst_AvailableSubModules.Add(strSubModuleName);
                    }

                    Array.ForEach(strExecutionCategories.Split(','), x =>
                    {
                        if (!lst_AvailableCategories.Contains(x))
                        {
                            lst_AvailableCategories.Add(x);
                        }
                    });
                }
            });
            lstbox_Module.Items.AddRange(lst_AvailableModules.ToArray());
            lstbox_SubModule.Items.AddRange(lst_AvailableSubModules.ToArray());
            lstbox_Criteria.Items.AddRange(lst_AvailableCategories.ToArray());
            dvTestCaseDetails = new DataView(dtTestCaseDetails);
            dvTestCaseDetails.Sort = "ModuleName";
            dvTestCaseDetails.Sort = "SubModuleName";
            dvTestCaseDetails.Sort = "TestCaseName";
            dvTestCaseDetails.Sort = "TestCaseDescription";
            dvTestCaseDetails.Sort = "ExecutionCategories";
            dataGridView1.DataSource = dvTestCaseDetails;
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            }
        }


        private void btn_FilterData_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;

            if (lstbox_Module.SelectedIndex != -1)
                strModuleFilterCriteria = lstbox_Module.SelectedItem.ToString().Trim();
            if (lstbox_SubModule.SelectedIndex != -1)
                strSubModuleFilterCriteria = lstbox_SubModule.SelectedItem.ToString().Trim();
            if (lstbox_Criteria.SelectedIndex != -1)
                strCategoryFilterCriteria = lstbox_Criteria.SelectedItem.ToString().Trim();

            string strCondition = "ModuleName LIKE '*" + strModuleFilterCriteria + "*' AND SubModuleName LIKE '*" + strSubModuleFilterCriteria + "*'  AND ExecutionCategories LIKE '*" + strCategoryFilterCriteria + "*'";

            DataView dvFilteredTestCaseDetails = new DataView(dtTestCaseDetails);
            dvFilteredTestCaseDetails.Sort = "ModuleName";
            dvFilteredTestCaseDetails.Sort = "SubModuleName";
            dvFilteredTestCaseDetails.Sort = "TestCaseName";
            dvFilteredTestCaseDetails.Sort = "TestCaseDescription";
            dvFilteredTestCaseDetails.Sort = "ExecutionCategories";
            dvFilteredTestCaseDetails.RowFilter = strCondition;
            dataGridView1.DataSource = dvFilteredTestCaseDetails;
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            }
        }

        private void btn_ExecuteTests_Click(object sender, EventArgs e)
        {
            try
            {
                Automation.Mercury.Report.Engine reportEngine = new Automation.Mercury.Report.Engine(Util.EnvironmentSettings["ReportsPath"], Util.EnvironmentSettings["Server"]);
                try
                {

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Cells[0].Value.Equals(true))
                        {
                            string testCaseModuleName = row.Cells[1].Value.ToString().Trim();
                            string testCaseSubModuleName = row.Cells[2].Value.ToString().Trim();
                            string strBrowserName = ConfigurationManager.AppSettings.Get("DefaultBrowser").ToString();
                            string testCaseRequirementFeature = string.Concat(testCaseModuleName, " - ", testCaseSubModuleName);
                            string testCaseName = row.Cells[3].Value.ToString().Trim();
                            TestCase testCaseReporter = new TestCase(testCaseName.Split('_')[0], testCaseName, testCaseRequirementFeature);
                            testCaseReporter.Summary = reportEngine.Reporter;
                            reportEngine.Reporter.TestCases.Add(testCaseReporter);
                            string strBrowserId = string.Empty;
                            // browsers
                            foreach (String browserId in strBrowserName.ToString().Split(new char[] { ';' }))
                            {
                                strBrowserId = browserId != String.Empty ? browserId : ConfigurationManager.AppSettings.Get("DefaultBrowser").ToString();
                                Browser browserReporter = new Browser(strBrowserId);
                                browserReporter.TestCase = testCaseReporter;
                                testCaseReporter.Browsers.Add(browserReporter);

                                // Get the Test data details
                                XmlDocument xmlTestDataDoc = new XmlDocument();
                                xmlTestDataDoc.Load("TestData/" + testCaseModuleName + ".xml");
                                XmlNodeList testdataNodeList = xmlTestDataDoc.DocumentElement.SelectNodes("/TestData/" + testCaseName.Split(',')[0].Replace("CAO.Automation.Tests.", string.Empty));
                                //XmlNodeList testdataNodeList = xmlTestDataDoc.DocumentElement.SelectNodes("/TestData/" + testCaseName.Replace("CAO.Automation.Tests", string.Empty));
                                //Iterate for each data
                                foreach (XmlNode testDataNode in testdataNodeList)
                                {
                                    Dictionary<String, String> browserConfig = Util.GetBrowserConfig(strBrowserId);
                                    string iterationId = testDataNode.SelectSingleNode("TDID").InnerText;
                                    string defectID = testDataNode.SelectSingleNode("DefectID").InnerText;
                                    Iteration iterationReporter = new Iteration(iterationId, defectID);
                                    iterationReporter.Browser = browserReporter;
                                    browserReporter.Iterations.Add(iterationReporter);
                                    //testCaseToExecute.Add(new Object[] { testCaseName,browserConfig, testCaseId, iterationId, iterationReporter, null, testDataNode, reportEngine });
                                    testCaseToExecute.Add(new Object[] { testCaseReporter, browserConfig, testDataNode, iterationReporter, reportEngine });
                                }
                            }
                        }
                    }
                    Processor(Int32.Parse(ConfigurationManager.AppSettings.Get("MaxDegreeOfParallelism")));
                    reportEngine.Summarize();
                    this.Activate();
                    LinkLabel.Link link = new LinkLabel.Link();
                    String fileName = Path.Combine(reportEngine.ReportPath, "Summary.html");
                    link.LinkData = fileName;
                    lnklbl_OverallExeStatResult.Links.Add(link);
                    lnklbl_OverallExeStatResult.Text = "PASSED " + DateTime.Now;
                }
                catch (Exception)
                {
                    this.Activate();
                    LinkLabel.Link link = new LinkLabel.Link();
                    String fileName = Path.Combine(reportEngine.ReportPath, "Summary_Provisional.html");
                    link.LinkData = fileName;
                    lnklbl_OverallExeStatResult.Links.Add(link);
                    bool overallPassed = true;
                    foreach (TestCase testCase in reportEngine.Reporter.TestCases)
                    {
                        if (!testCase.IsSuccess)
                        {
                            overallPassed = false;
                            break;
                        }
                    }
                    if (!overallPassed)
                        lnklbl_OverallExeStatResult.Text = "FAILED " + DateTime.Now;
                    else
                        lnklbl_OverallExeStatResult.Text = "PASSED " + DateTime.Now;
                }
            }
            catch
            {

            }

        }

        static void Processor(int maxDegree)
        {
            try
            {
                if (ConfigurationManager.AppSettings.Get("ExecutionMode").ToLower().Equals("s"))
                {
                    ///Use this loop for sequential running of the test cases
                    foreach (object[] work in testCaseToExecute)
                    {
                        ProcessEachWork(work);
                    }
                }
                else if (ConfigurationManager.AppSettings.Get("ExecutionMode").ToLower().Equals("p"))
                {
                    /*Use this loop for parellel running of the test cases*/
                    Parallel.ForEach(testCaseToExecute,
                                     new ParallelOptions { MaxDegreeOfParallelism = maxDegree },
                                     work =>
                                     {
                                         ProcessEachWork(work);
                                     });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        static void ProcessEachWork(Object[] data)
        {
            try
            {
                TestCase objTestCase = (TestCase)data[0];
                string strTCName = objTestCase.Name.ToString().Trim();
                Type typeTestCase = Type.GetType(qualifiedNames[strTCName]);
                BaseCase baseCase = Activator.CreateInstance(typeTestCase) as BaseCase;
                try
                {
                    typeTestCase.GetMethod("Execute").Invoke(baseCase, data);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(strTCName + " execution has caught exception " + ex.Message);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Cells[0].Value = true;
            }
        }

        private void lnklbl_OverallExeStatResult_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(e.Link.LinkData as string))
                    // Send the URL to the operating system.
                    Process.Start(e.Link.LinkData as string);
            }
            catch { }

        }

        private void form_SuiteRunner_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }
    }
}
