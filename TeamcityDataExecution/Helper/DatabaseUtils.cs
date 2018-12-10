using Agoda.Automata.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using TeamcityDataExecution.Models.Dto;
using static TeamcityDataExecution.Models.Dto.BuildSummaryReportDto;

namespace TeamcityDataExecution.Helper
{
    public class DatabaseUtils
    {
        DatabaseManager dbManager;
        string Detail;
        private static readonly string ServerName = "###SERVERNAME###";
        private static readonly string DatabaseName = "###DBNAME###";
        private static readonly string Username = "###USERNAME###";
        private static readonly string Password = "###PASSWORD###";
        private static readonly string DATE_FORMAT = "yyyy-MM-dd";

        private static readonly string INSERTION_DIALYREPORT = "###QUERY###";
        private static readonly string INSERTION_SUMMARYREPORT = "###QUERY###";
        private static readonly string SELECT_DATE = "###QUERY###";


        public DatabaseUtils()
        {
            dbManager = new DatabaseManager(ServerName, DatabaseName, Username, Password);
        }

        public void CheckSummaryData(List<BuildSummaryReportDto> dataList)
        {
            var summaryDbResults = FetchBuildSummaryReport(DateTime.Now);

            var isDataForThatDateExist = false;
            var id = 0;
            foreach (var data in summaryDbResults)
            {
                if (data.Time.ToString(DATE_FORMAT) == DateTime.Now.ToString(DATE_FORMAT))
                {
                    isDataForThatDateExist = true;
                    id = data.Id;
                    break;
                }
            }

            if (isDataForThatDateExist)
            {
                // update
                UpdateSummaryReport(dataList, id);
            }
            else
            {
                // insert
                InsertSummaryToDatabase(dataList);
            }
        }
        
        public void InsertFailureToDatabase(List<FailureReportDto> dataList)
        {
            try
            {
                ConnectDatabase();

                foreach (var data in dataList)
                {
                    if(!string.IsNullOrEmpty(data.Detail))
                    {
                        Detail = StripHTML(data.Detail);
                    }
                    if (string.IsNullOrEmpty(data.Detail))
                    {
                        Detail = data.Detail;
                    }

                    var failure_data_list_insert = string.Format(INSERTION_DIALYREPORT,
                                                                data.Hash,
                                                                data.TestCase,
                                                                data.RGNumber,
                                                                data.time,
                                                                data.Category,
                                                                data.Comment,
                                                                Detail,
                                                                data.TestReport);

                    //Send to DB
                    dbManager.Query(failure_data_list_insert);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void InsertSummaryToDatabase(List<BuildSummaryReportDto> dataList)
        {
            try
            {
                ConnectDatabase();

                foreach (var data in dataList)
                {
                    var summary_data_list_insert = string.Format(INSERTION_SUMMARYREPORT,
                                                                DateTime.Now.ToString(DATE_FORMAT),
                                                                data.Total,
                                                                data.Failed,
                                                                data.Passed,
                                                                data.Stability);

                    //Send to DB
                    dbManager.Query(summary_data_list_insert);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void UpdateSummaryReport(List<BuildSummaryReportDto> dataList, int idToUpdate)
        {
            try
            {
                ConnectDatabase();
                DataTable ResultTableName = null;

                foreach (var data in dataList)
                {
                    // Read all records from 
                    var update_daily_data = "###QUERY###";

                    //Send to DB
                    ResultTableName = dbManager.Query(update_daily_data);
                }
            }
            catch { }
        }

        public List<BuildSummaryReportDto> FetchBuildSummaryReport(DateTime date)
        {
            return FetchBuildSummaryFromSQL(date);
        }

        private List<BuildSummaryReportDto> FetchBuildSummaryFromSQL(DateTime time)
        {
            // QL
            var queryDate = time.ToString(DATE_FORMAT);
            var tomorrowDate = time.AddDays(1).ToString(DATE_FORMAT);
            var fechting_summary_data = string.Format(SELECT_DATE, queryDate, tomorrowDate);

            // Execute DB
            ConnectDatabase();
            DataTable ResultTableName = dbManager.Query(fechting_summary_data);

            // Read data
            List<BuildSummaryReportDto> buildSummaryReport = new List<BuildSummaryReportDto>();
            if (ResultTableName != null && ResultTableName.Rows != null && ResultTableName.Rows.Count > 0)
            {
                foreach (DataRow dtRow in ResultTableName.Rows)
                {
                    buildSummaryReport.Add(new BuildSummaryReportDto((int)dtRow["ID"], (int)dtRow["Total"], (int)dtRow["Failed"],
                                                                     (int)dtRow["Passed"], (int)dtRow["Stability"]));

                }
            }

            return buildSummaryReport;
        }

        public string StripHTML(string input)
        {
            return Regex.Replace(input, "'", String.Empty);
        }

        public void ConnectDatabase()
        {
            dbManager.Connect();
        }
    }
}
