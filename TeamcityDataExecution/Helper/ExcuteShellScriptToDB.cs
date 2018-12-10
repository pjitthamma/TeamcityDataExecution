using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using TeamcityDataExecution.Models.Dto;

namespace TeamcityDataExecution.Helper
{
    class ExcuteShellScriptToDB
    {
        private static readonly string SHELL_SCRIPT = "###FILEPATH###";
        private static readonly string REGRESSION_FAILURE_JSON = "###FILEPATH###";
        private static readonly string REGRESSION_SUMMARY_JSON = "###FILEPATH###";

        public void GetFailureReport()
        {
            // 1) Pull Data from Teamcity
            FetchDataFromTeamcity();

            // 2) Get data from Json file
            var failureJsonResults = GetFailureSummaryFromJson();
            var summaryJsonResults = GetTotalSummaryFromJson();

            // 3) Insert data to Database (table : FailureDataReport) 
            InsertFailureSummaryToSQLdb(failureJsonResults);

            // 4) Insert data to Database (table : SummaryDataReport) 
            InsertToltalSummaryToSQLdb(summaryJsonResults);
        }

        #region Powershell
        public void FetchDataFromTeamcity()
        {
            if (File.Exists(SHELL_SCRIPT))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "Powershell.exe";
                startInfo.Arguments = SHELL_SCRIPT;
                startInfo.CreateNoWindow = false;

                Process process = new Process();
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
            }
            else
            {
                Console.WriteLine("Shell script does not exist.");
            }
        }
        #endregion

        #region SQL       
        public void InsertFailureSummaryToSQLdb(List<FailureReportDto> dataListDaily)
        {
            DatabaseUtils DB_MANAGER = new DatabaseUtils();
            // Handle null object
            if (dataListDaily == null || dataListDaily.Count < 1) { return; }

            DB_MANAGER.InsertFailureToDatabase(dataListDaily);
        }

        public void InsertToltalSummaryToSQLdb(List<BuildSummaryReportDto> dataListSummary)
        {
            DatabaseUtils DB_MANAGER = new DatabaseUtils();
            // Handle null object
            if (dataListSummary == null || dataListSummary.Count < 1) { return; }

            DB_MANAGER.CheckSummaryData(dataListSummary);
        }
        #endregion

        #region JSON
        public List<FailureReportDto> GetFailureSummaryFromJson()
        {
            var json = ReadJsonFile(REGRESSION_FAILURE_JSON);
            var failureResults = JsonConvert.DeserializeObject<List<FailureReportDto>>(json);
            return failureResults;
        }

        public List<BuildSummaryReportDto> GetTotalSummaryFromJson()
        {
            var json = ReadJsonFile(REGRESSION_SUMMARY_JSON);
            var totalsResults = JsonConvert.DeserializeObject<List<BuildSummaryReportDto>>(json);
            return totalsResults;
        }

        private string ReadJsonFile(string path)
        {
            string json = "";

            using (StreamReader r = new StreamReader(path))
            {
                json = r.ReadToEnd();
            }
            return json;
        }
        #endregion
    }
}
