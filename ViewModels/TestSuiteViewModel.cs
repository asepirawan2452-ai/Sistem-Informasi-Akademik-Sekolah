using SistemInformasiNilaiAkademik.Models;
namespace SistemInformasiNilaiAkademik.ViewModels
{
    public class TestSuiteResultViewModel
    {
        public int TotalTests { get; set; }
        public int PassedTests { get; set; }
        public int FailedTests { get; set; }
        public long ExecutionTimeMs { get; set; }
        public DateTime ExecutedAt { get; set; } = DateTime.Now;
        public List<TestCaseResult> TestCases { get; set; } = new();
    }

    public class TestCaseResult
    {
        public string Code { get; set; } = string.Empty; // e.g. TC-01
        public string Title { get; set; } = string.Empty;
        public string AreaPengujian { get; set; } = string.Empty;
        public string InputDescription { get; set; } = string.Empty;
        public string ExpectedOutput { get; set; } = string.Empty;
        public string ActualOutput { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
