using System.IO;
using System.ServiceModel;

namespace TravelInfo.Helpers
{
    [ServiceContract]
    public interface IImageServer
    {

        Stream GetImage(int width, int height);
    }
}
