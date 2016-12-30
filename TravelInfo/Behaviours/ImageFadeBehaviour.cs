using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Xaml.Interactivity;
using Microsoft.Toolkit.Uwp.UI.Animations;

namespace TravelInfo.ViewModel
{
    public class ImageFadeBehaviour : Behavior<Image>
    {
        public static readonly DependencyProperty UriImageProperty = DependencyProperty.Register(
            "UriImage", typeof(Uri), typeof(ImageFadeBehaviour), new PropertyMetadata(new Uri(string.Format("http://lotso:9050/Service/GetImage")), new PropertyChangedCallback(OnUriChangedCallBack)));

        public static Uri GetUriImage(DependencyObject obj)
        {
            return (Uri)obj.GetValue(UriImageProperty);
        }

        public static void SetUriImage(DependencyObject obj, Uri value)
        {
            obj.SetValue(UriImageProperty, value);
        }
        
        private static void OnUriChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BitmapImage bi = new BitmapImage();
            Uri uri = e.NewValue as Uri;
            bi.UriSource = uri;
            Image image = sender as Image;
            SetImage(image, bi);            
        }

        private static async void SetImage(Image sender, BitmapImage bi)
        {
            await sender.Fade(0, 1500).StartAsync();
            sender.Opacity = 0;
            sender.Source = bi;
            await sender.Fade(1, 1500).StartAsync();
        }
    }
}
