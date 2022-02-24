using System;
using System.Collections.Generic;
using System.IO;

namespace ALARm.Core
{
    public interface IExportImportRepository
    {
        string ExportQueryReturnString(string text);
        bool Execute(string text);
        long ImportQueryReturnLong(string text);
        List<long> ImportQueryReturnListLong(string text);
    }
}
