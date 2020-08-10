using System.Collections.Concurrent;

namespace RdpRealTimePricing.Model.Data
{
    public class ExportData
    {
        public ExportData()
        {
            RawData = new ConcurrentDictionary<string, AppData>();
        }
        public ConcurrentDictionary<string, AppData> RawData { get; set; }
    }
}