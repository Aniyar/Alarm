using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALARm.Core;
using ALARm.DataAccess;

namespace ALARm.Services
{
    public class VideoObjectsService
    {
        static IContainer _container;
        static VideoObjectsService()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<VideoObjectsReposirory>().As<IVideoObjectsRepository>();
            _container = builder.Build();
        }
        public static List<VideoObject> GetObjectList(int tripId, int km, VideoObjectType type)
        {
            return _container.Resolve<IVideoObjectsRepository>().GetList(tripId, km, type);
        }

        
    }
}
