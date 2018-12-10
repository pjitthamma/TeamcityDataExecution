using System;

namespace TeamcityDataExecution.Models.Dto
{
    public class FailureReportDto
    {
        public string Hash
        {
            get
            {
                int hash = 123;

                hash += TestCase?.GetHashCode() ?? 0;
                hash += RGNumber?.GetHashCode() ?? 0;
                hash += (time != null) ? time.GetHashCode() : 0;
                hash += Category?.GetHashCode() ?? 0;
                hash += Comment?.GetHashCode() ?? 0;
                hash += Detail?.GetHashCode() ?? 0;
                hash += TestReport?.GetHashCode() ?? 0;

                return hash.ToString();
            }
        }
        public string TestCase { get; set; }
        public string RGNumber { get; set; }
        public DateTime time { get; set; }
        public string Category { get; set; }
        public string Comment { get; set; }
        public string Detail { get; set; }
        public string TestReport { get; set; }
    }
}
