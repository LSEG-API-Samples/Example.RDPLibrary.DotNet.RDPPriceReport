 using System.Collections.Generic;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace RdpRealTimePricing.Model.Data
{
    public class MarketPriceReport:IMarketData
    {
        public enum PriceChangeEnum
        {
            NoChange=0,
            Up=1,
            Down=2
        };
        public bool IsSelected{ get; set; }
        public string RicName { get; set; }
        public int? StreamId { get; set; }
        public IDictionary<string, JToken> _Attribute { get; set; }
        private IDictionary<string, dynamic> _fields;
        public IDictionary<string, dynamic> Fields
        {
            get => _fields;
            set
            {
                _fields = value;
                var item = new ExpandoObject();
                var dataItem = item as IDictionary<string, dynamic>;

                if (Fields != null)
                {
                    foreach (var (key, val) in Fields)
                    {
                       
                        dataItem.Add(key, val);
                    }
                }

                _dynamicFields = item;
            }
        }
        private ExpandoObject _dynamicFields = new ExpandoObject();

        public ExpandoObject DynamicFields => _dynamicFields;
    }
}