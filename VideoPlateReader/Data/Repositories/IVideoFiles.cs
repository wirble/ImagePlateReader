using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlateReader.Data.Repositories
{
    public interface IVideoFiles
    {
        IEnumerable<FileProp> GetImageList(string path);
        IEnumerable<FileProp> GetImageListWithDateTime(string path, DateTime dt);
    }
}
