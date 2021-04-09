using Newtonsoft.Json;

namespace notifier.Models
{
    public class DeviceTelemetry
    {
        [JsonProperty("temperature")]
        public int Temperature { get; set; }
    }
}
