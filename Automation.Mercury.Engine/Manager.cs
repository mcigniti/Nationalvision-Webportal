using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Configuration;
using Automation.Mercury.Report;
using System.Text;
using System.IO;
using System.IO.Compression;
using RestSharp;
using System.Threading.Tasks;

namespace Automation.Mercury.Engine
{
    class Manager
    {
        public static List<Object[]> works = new List<object[]>();
        public static bool reportSummary = true;
        public static bool detailedreportflag = true;
        public static string browserTypeValue = null;
        public static Dictionary<String, String> qualifiedNames = new Dictionary<string, string>();

        static void Main(string[] args)
        {
            string workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
            Report.Engine reportEngine = new Report.Engine(Util.EnvironmentSettings["ReportsPath"], Util.EnvironmentSettings["Server"]);

            // Get all Test Cases (Derived from BaseTestCase) 
            // Have a ClassName vs Qualified ClassName Dictionary
            // Peformance is not an issue here. Flexibility is needed.
            // We want to place Test Cases in varying levels of Directories
            Type typeBaseCase = typeof(BaseCase);

            foreach (Type type in Assembly.LoadFrom("NationalVision.Automation.Tests.dll").GetTypes().Where(x => x.IsSubclassOf(typeBaseCase)))
            {
                qualifiedNames.Add(type.Name, type.AssemblyQualifiedName);
            }

            // test cases reading logic from test suites from automation.mercury.engine
            foreach (DataRow eachRow in Util.ReadCSVContent(workingDirectory, Util.EnvironmentSettings["TestSuite"]).Rows)
            {
                try
                {
                    if (eachRow["Run"].ToString().ToUpper() != "YES")
                        continue;

                    String testCaseId = eachRow["TestCaseID"].ToString().Trim();
                    String testCaseName = eachRow["TestCaseTitle"].ToString().Trim();
                    String testCaseRequirementFeature = eachRow["RequirementFeature"].ToString().Trim();

                    Report.TestCase testCaseReporter = new Report.TestCase(testCaseId, testCaseName, testCaseRequirementFeature);
                    testCaseReporter.Summary = reportEngine.Reporter;
                    reportEngine.Reporter.TestCases.Add(testCaseReporter);

                    // browsers in test suite
                    foreach (String browserId in eachRow["Browser"].ToString().Split(new char[] { ';' }))
                    {
                        String overRideBrowserId = String.Empty;

                        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("DefaultBrowser").ToString()))
                            overRideBrowserId = ConfigurationManager.AppSettings.Get("DefaultBrowser").ToString();

                        Report.Browser browserReporter = new Report.Browser(overRideBrowserId != String.Empty ? overRideBrowserId : browserId);
                        browserReporter.TestCase = testCaseReporter;
                        testCaseReporter.Browsers.Add(browserReporter);

                        // Logic for reading iterative data from the respective test cases (reads from automation.tests)- Changed for project need reads only first iteration
                        foreach (DataRow iterationTestData in Util.GetIterationTestData(Path.Combine(workingDirectory, "Data"), testCaseId).Rows)
                        {
                            if (iterationTestData["Run"].ToString().ToUpper() != "YES")
                                continue;

                            Dictionary<String, String> testData = iterationTestData.Table.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => iterationTestData.Field<string>(col.ColumnName));

                            Dictionary<String, String> browserConfig = Util.GetBrowserConfig(overRideBrowserId != String.Empty ? overRideBrowserId : browserId);
                            String iterationId = iterationTestData["TDID"].ToString();
                            String defectID = iterationTestData["DefectID"].ToString();
                            String TestcaseID = iterationTestData["TestCaseID"].ToString();

                            Report.Iteration iterationReporter = new Report.Iteration(TestcaseID, defectID);
                            iterationReporter.Browser = browserReporter;
                            browserReporter.Iterations.Add(iterationReporter);


                            works.Add(new Object[] { browserConfig, testCaseId, iterationId, iterationReporter, testData, reportEngine, TestcaseID });
                        }
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            // Parllel Processing                     
            Processor(Int32.Parse(ConfigurationManager.AppSettings.Get("MaxDegreeOfParallelism")));
            reportEngine.Summarize();

            // generate re-run suite
            StringBuilder suiteContent = new StringBuilder();
            suiteContent.AppendLine("TestCaseID,TestCaseTitle,RequirementFeature,Run,Browser");
            foreach (TestCase eachCase in reportEngine.Reporter.TestCases)
            {
                if (!eachCase.IsSuccess)
                {
                    String browsers = String.Empty;

                    foreach (Browser eachBrowser in eachCase.Browsers)
                    {
                        if (!eachBrowser.IsSuccess)
                        {
                            browsers += String.Format("{0};", eachBrowser.Title);
                        }
                    }

                    browsers = browsers.TrimEnd(new char[] { ';' });
                    suiteContent.AppendLine(String.Format("{0},{1},{2},Yes,{3}", eachCase.Title, eachCase.Name, eachCase.RequirementFeature, browsers));
                }
            }

            String fileName = Path.Combine(reportEngine.ReportPath, "FailedSuite.csv");

            using (StreamWriter output = new StreamWriter(fileName))
            {
                output.Write(suiteContent.ToString());
            }

            String[] mailConfig = ConfigurationManager.AppSettings.Get("EmailReports").Split(new char[] { ';' });
            if (mailConfig.Length > 0 && mailConfig[0].ToUpper() == "TRUE")
            {
                // Prepare to send reports via Email
                String zipFilePath = Path.Combine(
                        Directory.GetParent(reportEngine.ReportPath).ToString(),
                        reportEngine.Timestamp + ".zip");
                ZipFile.CreateFromDirectory(reportEngine.ReportPath, zipFilePath);

                // Compose Email
                RestClient client = new RestClient();
                client.BaseUrl = new Uri("https://api.mailgun.net/v2");
                client.Authenticator = new HttpBasicAuthenticator("api", "key-a790236dd7b2bff508eaf13509a2d72a");
                RestRequest request = new RestRequest();
                request.AddParameter("domain", "sandbox1190ad81def541bc9dfac73ed08c16b8.mailgun.org", ParameterType.UrlSegment);
                request.Resource = "{domain}/messages";
                request.AddParameter("from", "automation@mailgun.org");
                request.AddParameter("to", mailConfig[1]);
                if (mailConfig[2].ToString() != null)
                {
                    request.AddParameter("cc", mailConfig[2]);
                    request.AddParameter("cc", mailConfig[3]);
                    request.AddParameter("cc", mailConfig[4]);
                    request.AddParameter("bcc", mailConfig[5]);
                }
                request.AddParameter("subject", "National Vision Web Portal Test Automation Report");
                request.AddParameter("html", File.ReadAllText(Path.Combine(reportEngine.ReportPath, "summary.html")));
                //request.AddParameter("html", File.ReadAllText(Path.Combine(reportEngine.ReportPath, "summary.html")));
                //request.AddFile("attachment", Path.Combine(reportEngine.ReportPath, "summary.html"));
                request.AddFile("attachment", zipFilePath);
                request.Method = Method.POST;
                IRestResponse response = client.Execute(request);
            }
        }

        static void Processor(int maxDegree)
        {
            Parallel.ForEach(works,
                new ParallelOptions { MaxDegreeOfParallelism = maxDegree },
                work =>
                {
                    try
                    {
                        ProcessEachWork(work);
                    }
                    catch (Exception e)
                    {


                    }
                });
        }

        static void ProcessEachWork(Object[] data)
        {
            Type typeTestCase = Type.GetType(qualifiedNames[data[1].ToString()]); //data[1]: TestCaseId
            Console.WriteLine("Started Executing following test script: " + data[1].ToString() + " in " + data[6].ToString());
            BaseCase baseCase = Activator.CreateInstance(typeTestCase) as BaseCase;

            typeTestCase.GetMethod("Execute").Invoke(
                baseCase, data);
        }
    }
}