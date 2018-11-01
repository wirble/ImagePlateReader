using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlateReader.Data.Repositories
{
    public class DataService : IDataService
    {
        //need to create the datacontext.....!!!
        public async Task<IEnumerable<MatchedImageLog>> GetAllLogReader(string path)
        {
            //TODO: maybe load data from a real database
            return await Task.Run(()=>MatchedImageLogReader.Read(path));
        }

        public async Task<MatchedImageLog> GetByIdAsync(string id,string path)
        {
            var log = await Task.Run(() => MatchedImageLogReader.Read(path));
            return log.Single(l => l.Id == id);
        }
    }
}
