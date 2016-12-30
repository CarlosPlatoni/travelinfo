using System;
using System.Collections.Generic;
using System.Threading;

namespace TravelInfo.Model
{
    public enum eResultType { NR, TFL, TFLStatus };

    public class CallbackDetail
    {
        public CallbackDetail()
        {
            this.NrPredictions = new List<NrPrediction>();
            this.TflPredictions = new List<TflPrediction>();
            this.TflStatuses = new List<TflLine>();
        }

        public eResultType ResultType { get; set; }
        public int Running;
        public string StopPointId { get; set; }
        public int DueTime { get; set; }
        public int Period { get; set; }
        public DateTime Timestamp { get; set; }
        public List<NrPrediction> NrPredictions{ get; set; }
        public List<TflPrediction> TflPredictions { get; set; }
        public List<TflLine> TflStatuses { get; set; }
        public Timer CallbackTimer { get; set; }
    }
}
