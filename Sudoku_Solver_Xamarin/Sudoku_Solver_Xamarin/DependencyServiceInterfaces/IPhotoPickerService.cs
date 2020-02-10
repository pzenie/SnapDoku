using System.IO;
using System.Threading.Tasks;

namespace SnapDoku_Xamarin.DependencyServiceInterfaces
{
    public interface IPhotoPickerService
    {
        Task<Stream> GetImageStreamAsync();
    }
}
