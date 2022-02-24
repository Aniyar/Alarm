using System;
using ALARm.Core;
using ALARm.DataAccess;
using Autofac;
using System.IO;
using System.Collections.Generic;

namespace ALARm.Services
{
    public class ExportImportService
    {
        static readonly IContainer Container;
        static ExportImportService()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ExportImportRepository>().As<IExportImportRepository>();
            Container = builder.Build();
        }

        public static string ExportQueryReturnString(string text)
        {
            return Container.Resolve<IExportImportRepository>().ExportQueryReturnString(text);
        }

        public static bool Execute(string text)
        {
            return Container.Resolve<IExportImportRepository>().Execute(text);
        }

        public static long ImportQueryReturnLong(string text)
        {
            return Container.Resolve<IExportImportRepository>().ImportQueryReturnLong(text);
        }

        public static List<long> ImportQueryReturnListLong(string text)
        {
            return Container.Resolve<IExportImportRepository>().ImportQueryReturnListLong(text);
        }
    }
}
