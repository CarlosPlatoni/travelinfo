using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace TravelInfo.Model
{
    public class TflLine
    {
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }

        [JsonProperty("modified")]
        public DateTime Modified { get; set; }

        [JsonProperty("lineStatuses")]
        public List<TflLineStatus> LineStatuses { get; set; }

        public string StatusSeverity
        {
            get
            {
                if (this.LineStatuses.Any())
                {
                    return this.LineStatuses[0].StatusSeverity;
                }

                return string.Empty;
            }
        }

        public string StatusSeverityDescription
        {
            get
            {
                if (this.LineStatuses.Any())
                {
                    return this.LineStatuses[0].StatusSeverityDescription;
                }

                return string.Empty;
            }
        }
    }
}

