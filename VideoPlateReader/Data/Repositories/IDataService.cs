using System.Collections.Generic;
using System.Threading.Tasks;

namespace VideoPlateReader.Data.Repositories
{
    public interface IDataService
    {
        Task<IEnumerable<MatchedImageLog>> GetAllLogReader(string path);

        Task<MatchedImageLog> GetByIdAsync(string id, string path);
    }
}