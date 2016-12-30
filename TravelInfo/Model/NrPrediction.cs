using TravelInfo.NRServiceReference;
using System;
using System.ComponentModel;

namespace TravelInfo.Model
{
    public class NrPrediction : INotifyPropertyChanged
    {
        private string std;
        private string etd;
        private string platform;
        private string destination;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Std
        {
            get
            {
                return this.std;
            }
            set
            {
                this.std = value;
                this.NotifyPropertyChanged("Std");
            }
        }

        public string Etd
        {
            get
            {
                return this.etd;
            }
            set
            {
                this.etd = value;
                this.NotifyPropertyChanged("Etd");
            }
        }

        public string Destination
        {
            get
            {
                return this.destination;
            }
            set
            {
                this.destination = value;
                this.NotifyPropertyChanged("Destination");
            }
        }

        public string Platform
        {
            get
            {
                return this.platform;
            }
            set
            {
                this.platform = value;
                this.NotifyPropertyChanged("Platform");
            }
        }

        public NrPrediction(ServiceItem stationItem)
        {
            this.Std = stationItem.std;
            this.Etd = stationItem.etd;
            this.Platform = stationItem.platform;
            this.Destination = stationItem.destination[0].locationName;
        }

        private void NotifyPropertyChanged(String info)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
