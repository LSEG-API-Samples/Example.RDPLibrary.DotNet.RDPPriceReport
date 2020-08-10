using ChainExpanderLib.Models.Enum;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ChainExpanderLib.Models.Message
{
    internal class PingPongMessage
    {
        [Newtonsoft.Json.JsonProperty("Type", DefaultValueHandling = DefaultValueHandling.Ignore, Required = Newtonsoft.Json.Required.DisallowNull,
            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageTypeEnum Type { get; set; }
        public string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        }
        public static PingPongMessage FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<PingPongMessage>(data);
        }
    }
}