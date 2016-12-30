using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using WCFPhotoServerConsole;

namespace RawImageService
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = "http://" + Environment.MachineName + ":9050/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(IImageServer), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Service is running");
            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();

        }
    }
}
