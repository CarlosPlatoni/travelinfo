using System;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Threading;

namespace TravelInfo.Helpers
{
    public class Scheduler
    {
        private Timer schedulertimer;
        private DateTime? TravelTimerStart = null;

        public void Initialise()
        {
            this.schedulertimer = new Timer(timercallback, null, 1000, 10000);
        }

        public void KillTravelTimer()
        {
            this.TravelTimerStart = null;
        }

        public void StartTravelTimer()
        {
            this.TravelTimerStart = DateTime.Now;
        }

        private async void timercallback(object state)
        {
            await DispatcherHelper.RunAsync(() =>
            {
                Frame rootFrame = Window.Current.Content as Frame;
                if (rootFrame == null)
                {
                    return;
                }

                if (this.TravelTimerStart.HasValue)
                {
                    if ((DateTime.Now - this.TravelTimerStart.Value).TotalMinutes < 1)
                    {
                        if (rootFrame.CurrentSourcePageType.Name != "TravelInfoPage")
                        {
                            rootFrame.Navigate(typeof(TravelInfoPage));                            
                        }

                        return;
                    }

                    this.TravelTimerStart = null;
                }

                // Between 06:15 and 08:30 show TravelInfo Monday to Friday
                // Between 23:50 and 06:15 show Holdingscreen
                // 08:30 to 23:50 show PhotoFrame
                if (DateTime.Now.TimeOfDay >= new TimeSpan(6, 20, 0) && DateTime.Now.TimeOfDay < new TimeSpan(9, 0, 0))
                {
                    if (rootFrame.CurrentSourcePageType.Name != "TravelInfoPage")
                    {
                        rootFrame.Navigate(typeof(TravelInfoPage));
                        return;
                    }

                    return;
                }

                if (DateTime.Now.TimeOfDay > new TimeSpan(0, 0, 0) && DateTime.Now.TimeOfDay < new TimeSpan(6, 20, 0))
                {
                    if (rootFrame.CurrentSourcePageType.Name != "HoldingScreenPage")
                    {
                        rootFrame.Navigate(typeof(HoldingScreenPage));
                        return;
                    }

                    return;
                }

                if (rootFrame.CurrentSourcePageType.Name != "PhotoFramePage")
                {
                    rootFrame.Navigate(typeof(PhotoFramePage));
                    return;
                }
           }
        );
        }
    }
}
