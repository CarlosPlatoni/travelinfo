using System;
using Newtonsoft.Json;

namespace TravelInfo.Model
{
    public class TflPrediction
    {
        [JsonProperty("stationName")]
        public string StationName {get; set;}

        [JsonProperty("direction")]
        public string Direction { get; set; }

        [JsonProperty("currentLocation")]
        public string CurrentLocation { get; set; }

        [JsonProperty("destinationName")]
        public string DestinationName { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("timeToStation")]
        public int TimeToStation { get; set; }

        [JsonProperty("expectedArrival")]
        public DateTime ExpectedArrival { get; set; }

        [JsonProperty("lineId")]
        public string LineId { get; set; }

        [JsonProperty("towards")]
        public string Towards { get; set; }

        [JsonProperty("timeToLive")]
        public DateTime TimeToLive { get; set; }

        [JsonProperty("modeName")]
        public string ModeName { get; set; }

        [JsonProperty("lineName")]
        public string LineName { get; set; }

        public string ShortDestinationName
        {
            get
            {
                return this.DestinationName.Substring(0, this.DestinationName.IndexOf(" "));
            }
        }

        public string TimeToStationInMinutes
        {
            get
            {
                int mins = this.TimeToStation / 60;
                int secs = this.TimeToStation % 60;
                return string.Format("{0:D2}:{1:D2}", mins, secs);
            }
        }
    }
}
