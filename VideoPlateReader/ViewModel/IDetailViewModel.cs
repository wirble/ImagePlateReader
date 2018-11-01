using System.Threading.Tasks;

namespace VideoPlateReader.ViewModel
{
    public interface IDetailViewModel
    {
        Task LoadAsync(string id, string path);
    }
}