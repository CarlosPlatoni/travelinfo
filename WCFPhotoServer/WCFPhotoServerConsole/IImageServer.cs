using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace WCFPhotoServerConsole
{
    // Define the service contract
    [ServiceContract]
    public interface IImageServer
    {
        [WebGet]
        Stream GetImage();

        [WebGet]
        string GetImageDetails();
    }
}
