using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Command;
using TravelInfo.Helpers;

namespace TravelInfo.ViewModel
{
    public class HoldingScreenViewModel
    {
        private RelayCommand tappedCommand;

        public RelayCommand TappedCommand
        {
            get
            {
                if (this.tappedCommand == null)
                {
                    this.tappedCommand = new RelayCommand(this.Tapped);
                }

                return this.tappedCommand;
            }
        }

        private void Tapped()
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame?.Navigate(typeof (TravelInfoPage), null);
            App application = TravelInfo.App.Current as App;
            if (application != null)
            {
                Scheduler scheduler = application.scheduler;
                scheduler.StartTravelTimer();
            }
        }
    }
}
