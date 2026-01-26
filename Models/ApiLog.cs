namespace Api_Monitoring.Core
{
    public class ApiLog
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow; 
        public string Method { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public int StatusCode { get; set; } 
        public long DurationMs { get; set; }
        public string QueryString { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;   
        public string UserAgent { get; set; } = string.Empty;   
        public string ExceptionMessage { get; set; } = string.Empty;
        public string StackTrace { get; set; } = string.Empty;  


    }
}
