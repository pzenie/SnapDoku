using System.IO;
using System.Threading.Tasks;

namespace Sudoku_Solver_Xamarin.DependencyServiceInterfaces
{
    public interface IPhotoPickerService
    {
        Task<Stream> GetImageStreamAsync();
    }
}
