using ALARm.Core.AdditionalParameteres;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core
{
    public interface IMainParametersRepository
    {
        MainParameters GetMainParameters(int km);
        ShortRoughness GetAppParameters(int km);
		MainParameters GetMainParametersSKO(int kilometer);
        MainParameters DevPlan(int kilometer);
        MainParameters GetMainParametersSKOvedom(int kilometer);
        List<Sssp> GetMainParametersSKOvedomDB(int kilometer, long trip_id);
        List<Sssp> GetMainParametersSkoFP52(int kilometer, long trip_id);
        List<Sssp> GetMainParametersFromDB(long trip_id);
        List<Sssp> GetMainParametersFromDBMeter(long trip_id);
    }
}
