using System;
using System.Collections.Generic;
using TravelInfo.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight.Threading;
using System.Threading;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight.Command;
using TravelInfo.Helpers;

namespace TravelInfo.ViewModel
{
    public class TravelInfoViewModel : INotifyPropertyChanged, IPlatzNavigate
    {
        private DateTime updateTime194;
        private DateTime updateTimeSyd;
        private DateTime updateTimeCw;
        private DateTime updateTimeLb;
        private DateTime updateTimeGp;
        private DateTime updateTimeTflLineStatus;

        private TravelInfoModel model { get; set; }

        private Timer clockTimer;
        private RelayCommand tappedCommand;

        public ObservableCollection<TflPrediction> Bus194 { get; set; }

        public ObservableCollection<NrPrediction> SydTrains { get; set; }

        public ObservableCollection<TflPrediction> CwTubeTrains { get; set; }

        public ObservableCollection<TflPrediction> LbTubeTrains { get; set; }

        public ObservableCollection<TflPrediction> GpTubeTrains { get; set; }

        public ObservableCollection<TflLine> TflStatus { get; set; }

        public RelayCommand TappedCommand
        {
            get
            {
                return this.tappedCommand ?? (this.tappedCommand = new RelayCommand(this.Tapped));
            }
        } 

        public void Tapped()
        {
            App application = Application.Current as App;
            if (application != null)
            {
                Scheduler scheduler = application.scheduler;
                scheduler.KillTravelTimer();
            }
        }

        public string TflBusHeader => "Bus 194 " + this.updateTime194.ToLocalTime().ToString("t");

        public string SydTrainHeader => "London Trains " + this.updateTimeSyd.ToLocalTime().ToString("t");

        public string CwTubeHeader => "Canada Water Tubes " + this.updateTimeCw.ToLocalTime().ToString("t");

        public string LbTubeHeader => "London Bridge Tubes " + this.updateTimeLb.ToLocalTime().ToString("t");

        public string GpTubeHeader => "Green Park Tubes " + this.updateTimeGp.ToLocalTime().ToString("t");

        public string TflLineStatusHeader => "Tube Status " + this.updateTimeTflLineStatus.ToLocalTime().ToString("t");

        public string ClockTime => DateTime.Now.ToString("HH:mm:ss");

        public TravelInfoViewModel()
        {
            this.model = new TravelInfoModel();
            this.SydTrains = new ObservableCollection<NrPrediction>();
            this.Bus194 = new ObservableCollection<TflPrediction>();
            this.CwTubeTrains = new ObservableCollection<TflPrediction>();
            this.LbTubeTrains = new ObservableCollection<TflPrediction>();
            this.GpTubeTrains = new ObservableCollection<TflPrediction>();
            this.TflStatus = new ObservableCollection<TflLine>();
            this.clockTimer = new Timer(this.ClockTick, null, 1000, 1000);
        }

        private void ClockTick(object state)
        {
            this.OnPropertyChanged("ClockTime");
        }

        private void ModelOnPreditionChanged(CallbackDetail callbackdetail)
        {
            // Taken from stops.csv
            if (callbackdetail.StopPointId == "490009750P")
            {
                this.ModelOn194Changed(callbackdetail.TflPredictions, callbackdetail.Timestamp);
            }
            // taken from railreferences.csv
            else if (callbackdetail.StopPointId == "SYD")
            {
                this.ModelNationalRailSYDChanged(callbackdetail.NrPredictions, callbackdetail.Timestamp);
            }
            else if (callbackdetail.StopPointId == "940GZZLUCWR")
            {
                this.ModelOnCWTubeChanged(callbackdetail.TflPredictions, callbackdetail.Timestamp);
            }
            else if (callbackdetail.StopPointId == "940GZZLULNB")
            {
                this.ModelOnLBTubeChanged(callbackdetail.TflPredictions, callbackdetail.Timestamp);
            }
            else if (callbackdetail.StopPointId == "940GZZLUGPK")
            {
                this.ModelOnGPTubeChanged(callbackdetail.TflPredictions, callbackdetail.Timestamp);
            }
            else if (callbackdetail.ResultType == eResultType.TFLStatus)
            {
                this.ModelOnTflStatusChanged(callbackdetail.TflStatuses, callbackdetail.Timestamp);
            }
        }

        private async void ModelOnTflStatusChanged(List<TflLine> lines, DateTime timestamp)
        {
            await DispatcherHelper.RunAsync(() => this.updateTimeTflLineStatus = timestamp);
            this.OnPropertyChanged(nameof(this.TflLineStatusHeader));
            await DispatcherHelper.RunAsync(() => this.TflStatus.Clear());
         
            lines = lines.OrderBy(x => x.Name).ToList();
            foreach (var line in lines)
            {
                await DispatcherHelper.RunAsync(() => this.TflStatus.Add(line));
            }
        }

        private async void ModelOnGPTubeChanged(List<TflPrediction> tubes, DateTime timestamp)
        {
            await DispatcherHelper.RunAsync(() => this.updateTimeGp = timestamp);
            this.OnPropertyChanged(nameof(this.GpTubeHeader));
            await DispatcherHelper.RunAsync(() => this.GpTubeTrains.Clear());
         
            tubes = tubes.Where(x => x.Direction == "outbound" && x.LineId == "victoria").OrderBy(x => x.TimeToStation).Take(3).ToList();
            foreach (var tube in tubes)
            {
                await DispatcherHelper.RunAsync(() => this.GpTubeTrains.Add(tube));
            }
        }

        private async void ModelOnLBTubeChanged(List<TflPrediction> tubes, DateTime timestamp)
        {
            await DispatcherHelper.RunAsync(() => this.updateTimeLb = timestamp);
            this.OnPropertyChanged(nameof(this.LbTubeHeader));
            await DispatcherHelper.RunAsync(() => this.LbTubeTrains.Clear());

            tubes = tubes.Where(x => x.Direction == "inbound" && x.LineId=="jubilee").OrderBy(x => x.TimeToStation).Take(3).ToList();
            foreach (var tube in tubes)
            {
                await DispatcherHelper.RunAsync(() => this.LbTubeTrains.Add(tube));
            }
        }

        private async void ModelOnCWTubeChanged(List<TflPrediction> tubes, DateTime timestamp)
        {
            await DispatcherHelper.RunAsync(() => this.updateTimeCw = timestamp);
            this.OnPropertyChanged(nameof(this.CwTubeHeader));
            await DispatcherHelper.RunAsync(() => this.CwTubeTrains.Clear());

            tubes = tubes.Where(x => x.Direction == "inbound").OrderBy(x => x.TimeToStation).Take(3).ToList();
            foreach (var tube in tubes)
            {
                await DispatcherHelper.RunAsync(() => this.CwTubeTrains.Add(tube));
            }
        }

        private async void ModelOn194Changed(List<TflPrediction> buses, DateTime timestamp)
        {
            await DispatcherHelper.RunAsync(() => this.updateTime194 = timestamp);
            this.OnPropertyChanged(nameof(this.TflBusHeader));
            await DispatcherHelper.RunAsync(() => this.Bus194.Clear());

            buses = buses.Where(x => x.LineName == "194").OrderBy(x=> x.TimeToStation).Take(4).ToList();
            foreach (var bus in buses)
            {
                await DispatcherHelper.RunAsync(() => this.Bus194.Add(bus));
            }
        }

        private async void ModelNationalRailSYDChanged(List<NrPrediction> trains, DateTime timestamp)
        {
            await DispatcherHelper.RunAsync(() => this.updateTimeSyd = timestamp);
            this.OnPropertyChanged(nameof(this.SydTrainHeader));
            await DispatcherHelper.RunAsync(() => this.SydTrains.Clear());
            foreach (var train in trains.Where(x=>x.Platform=="1").OrderBy(x=>x.Std).Take(10))
            {
                await DispatcherHelper.RunAsync(() => this.SydTrains.Add(train));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                DispatcherHelper.RunAsync(() => PropertyChanged(this, new PropertyChangedEventArgs(propertyName)));
            }
        }

        public void NavigatedTo()
        {
            this.model.Changed += this.ModelOnPreditionChanged;
            this.model.StartTimers();
        }

        public void NavigatingFrom()
        {
            this.model.Changed -= this.ModelOnPreditionChanged;
            this.model.KillTimers();
        }

        public void NavigatedFrom()
        {
            throw new NotImplementedException();
        }
    }
}
