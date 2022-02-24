using ALARm.Core;
using ALARm.Core.AdditionalParameteres;

using ALARm.DataAccess;
using Autofac;
using System;
using System.Collections.Generic;

namespace ALARm.Services
{
    public class MainParametersService
    {
        static IContainer _container;
        static MainParametersService()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MainParametersRepository>().As<IMainParametersRepository>();
            _container = builder.Build();
        }
        public static MainParameters GetMainParameters(int kilometer) {
            return _container.Resolve<IMainParametersRepository>().GetMainParameters(kilometer);
        }

        public static ShortRoughness GetAppParameters(int km)
        {
            return _container.Resolve<IMainParametersRepository>().GetAppParameters(km);
        }
		public static MainParameters GetMainParametersSKO(int kilometer)
        {
            return _container.Resolve<IMainParametersRepository>().GetMainParametersSKO(kilometer);
        }

        public static MainParameters DevPlan(int kilometer)
        {
            return _container.Resolve<IMainParametersRepository>().DevPlan(kilometer);
        }

        public static MainParameters GetMainParametersSKOvedom(int kilometer)
        {
            return _container.Resolve<IMainParametersRepository>().GetMainParametersSKOvedom(kilometer);
        }

        public static List<Sssp> GetMainParametersSKOvedomDB(int kilometer, long trip_id)
        {
            return _container.Resolve<IMainParametersRepository>().GetMainParametersSKOvedomDB(kilometer, trip_id);
        }
        public static List<Sssp> GetMainParametersSkoFP52(int kilometer, long trip_id)
        {
            return _container.Resolve<IMainParametersRepository>().GetMainParametersSkoFP52(kilometer, trip_id);
        }

        public static List<Sssp> GetMainParametersFromDB(long trip_id)
        {
            return _container.Resolve<IMainParametersRepository>().GetMainParametersFromDB(trip_id);
        }
        public static List<Sssp> GetMainParametersFromDBMeter(long trip_id)
        {
            return _container.Resolve<IMainParametersRepository>().GetMainParametersFromDBMeter(trip_id);
        }
    }
}
