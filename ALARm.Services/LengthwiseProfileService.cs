using ALARm.Core;
using Autofac;

using ALARm.DataAccess;
using System;
using System.Collections.Generic;

namespace ALARm.Services
{
    public class LengthwiseProfileService
    {
        static IContainer _container;
        static LengthwiseProfileService()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<LengthwiseProfileTxtRepository>().As<ILengthwiseProfileRepository>();
            _container = builder.Build();
        }

    public static List<LengthwiseProfile> GetLengthwiseProfile(string filename)
        {
            return _container.Resolve<ILengthwiseProfileRepository>().GetLengthwiseProfile(filename);
        }
    }
}
