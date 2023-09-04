using System.Collections.Generic;

namespace WebDispacher.ViewModels.Driver
{
    public class CheckDriverViewModel
    {
        public DriverSearchViewModel Search { get; set; }
        public List<DriverReportViewModel> DriverReports { get; set; }
    }
}
