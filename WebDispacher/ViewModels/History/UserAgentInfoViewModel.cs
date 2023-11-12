namespace WebDispacher.ViewModels.History
{
    public class UserAgentInfoViewModel
    {
        public bool IsBot { get; set; }
        public string BrowserName { get; set; } = string.Empty;
        public string BrowserVersion { get; set; } = string.Empty;
        public string OSName { get; set; } = string.Empty;
        public string OSVersion { get; set; } = string.Empty;
        public string OSPlatform { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public string DeviceBrand { get; set; } = string.Empty;
        public string DeviceModel { get; set; } = string.Empty;
        public bool IsMobile { get; set; }
        public bool IsDesktop { get; set; }
    }
}