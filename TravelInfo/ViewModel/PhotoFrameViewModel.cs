using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using TravelInfo.Helpers;

namespace TravelInfo.ViewModel
{
    public class PhotoFrameViewModel : INotifyPropertyChanged, IPlatzNavigate
    {
        private RelayCommand tappedCommand;
        private Uri uri;
        private Timer photochangetimer;
        private bool isbusy;
        private bool pictureVisible;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                DispatcherHelper.RunAsync(() => PropertyChanged(this, new PropertyChangedEventArgs(propertyName)));
            }
        }

        public bool PictureVisible
        {
            get
            {
                return this.pictureVisible;
            }

            set
            {
                this.pictureVisible = value; 
                this.OnPropertyChanged();
            }
        }

        public Uri PictureUri
        {
            get
            {
                return this.uri;

            }
            set
            {
                this.uri = value;
                this.OnPropertyChanged();
            }
        }

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

        private async void GetFile(object state)
        {
            await LoadImage();
        }

        public async Task LoadImage()
        {
            Guid guid = Guid.NewGuid();
            string uniquestring = guid.ToString().Replace("-", "");

            if (this.isbusy)
            {
                Debug.WriteLine("Busy");
                return;
            }

            this.isbusy = true;

            Uri uri = new Uri(string.Format("http://lotso:9050/Service/GetImage?query={0}", uniquestring));

            try
            {
                this.PictureUri = uri;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load the image: {0}", ex.Message);
            }
            finally
            {
                isbusy = false;
            }

        }

        private void Tapped()
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame?.Navigate(typeof(TravelInfoPage), null);
            App application = TravelInfo.App.Current as App;
            if (application != null)
            {
                Scheduler scheduler = application.scheduler;
                scheduler.StartTravelTimer();
            }
        }

        public void NavigatedTo()
        {
            this.photochangetimer = new Timer(this.GetFile, null, 0, 12000);
        }

        public void NavigatingFrom()
        {
            this.photochangetimer = null;
        }

        public void NavigatedFrom()
        {
            throw new System.NotImplementedException();
        }
    }
}
