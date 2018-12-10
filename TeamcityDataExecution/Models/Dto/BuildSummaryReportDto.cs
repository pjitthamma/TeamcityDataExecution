using System;

namespace TeamcityDataExecution.Models.Dto
{
    public class BuildSummaryReportDto
    {
        public BuildSummaryReportDto(int id, int total, int failed, int passed, int stability)
        {
            this.Id = id;
            this.Time = DateTime.Now;
            this.Total = total;
            this.Failed = failed;
            this.Passed = passed;
            this.Stability = stability;
        }

        //public BuildSummaryReportDto(int id, DateTime time, int total, int failed, int passed, int stability)
        //{
        //    this.Id = id;
        //    this.Time = time;
        //    this.Total = total;
        //    this.Failed = failed;
        //    this.Passed = passed;
        //    this.Stability = stability;
        //}

        public int Id { get; set; }
        public DateTime Time { get; set; }
        public int Total { get; set; }
        public int Failed { get; set; }
        public int Passed { get; set; }
        public int Stability { get; set; }
    }
}
