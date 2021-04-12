using Newtonsoft.Json;

namespace notifier.Models
{
    public class DeviceTelemetry
    {
        public string DeviceId { get; set; }

        [JsonProperty("temperature")]
        public int Temperature { get; set; }
    }
}
