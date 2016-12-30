using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using TravelInfo.Helpers;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TravelInfo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TravelInfoPage : Page
    {
        public TravelInfoPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            IPlatzNavigate model = this.DataContext as IPlatzNavigate;
            model?.NavigatingFrom();
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            IPlatzNavigate model = this.DataContext as IPlatzNavigate;
            model?.NavigatedTo();
            base.OnNavigatedTo(e);
        }
    }
}
