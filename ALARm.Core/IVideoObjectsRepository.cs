using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core
{
    public interface IVideoObjectsRepository
    {
        List<VideoObject> GetList(long tripId, int km, VideoObjectType type);
    }
}
