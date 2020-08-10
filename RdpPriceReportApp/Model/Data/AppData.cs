using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections.Generic;


namespace RdpRealTimePricing.Model.Data
{
    public class AppData
    {
        public AppData()
        {
            AppMenuTxt = "Login";
            DataCache = new ConcurrentDictionary<string, MarketPriceReport>();
        }
        public string AppMenuTxt { get; set; }
        public bool UseRDP { get; set; } = true;
        public string CurrentUserName { get; set; }
        public string MinWidth { get; set; } = "60%";
        public string MaxWidth { get; set; } = "95%"; 
        public ConcurrentDictionary<string, MarketPriceReport> DataCache{ get; set; }
        public IEnumerable<string> columnValues = new List<string> { "DSPLY_NAME", "BID", "ASK", "TRDPRC_1", };
    }
}
