using System;
using System.Collections.Generic;
using ALARm.Core;
using ALARm.Core.AdditionalParameteres;
using ALARm.Core.Report;
using ALARm.DataAccess;
using Autofac;
using IContainer = Autofac.IContainer;

namespace ALARm.Services
{
    public class AdditionalParametersService
    {
        static readonly IContainer Container;
        static AdditionalParametersService()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<AdditionalParametersRepository>().As<IAdditionalParametersRepository>();
            Container = builder.Build();
        }

        public static List<Gap> GetGaps(long process_id, int direction, int kilometer)
        {
            return Container.Resolve<IAdditionalParametersRepository>().GetGaps(process_id, direction, kilometer);
        }

        public static List<int> GetKilometers(long process_id, int direction)
        {
            return Container.Resolve<IAdditionalParametersRepository>().GetKilometers(process_id, direction);
        }

        public static CrossRailProfile GetCrossRailProfileFromText(int kilometer)
        {
            return Container.Resolve<IAdditionalParametersRepository>().GetCrossRailProfileFromText(kilometer);
        }
        public static List<CrosProf> GetCrossRailProfileFromDBbyKm(int kilometer, long trip_id)
        {
            return Container.Resolve<IAdditionalParametersRepository>().GetCrossRailProfileFromDBbyKm(kilometer, trip_id);
        }
        public static List<CrosProf> GetCrossRailProfileFromDBbyKm_forPPWEB(int kilometer, int index, int count, long trip_id)
        {
            return Container.Resolve<IAdditionalParametersRepository>().GetCrossRailProfileFromDBbyKm_forPPWEB(kilometer, index,count, trip_id);
        }
        public static List<CrosProf> GetCrossRailProfileFromTrip(long trip_id)
        {
            return Container.Resolve<IAdditionalParametersRepository>().GetCrossRailProfileFromTrip(trip_id);
        }
        public static List<CrosProf> GetCrossRailProfileFromDBbyTripId(long trip_id)
        {
            return Container.Resolve<IAdditionalParametersRepository>().GetCrossRailProfileFromDBbyTripId(trip_id);
        }
        public static List<CrosProf> GetGaugeFromDB(int kilometer, long trip_id)
        {
            return Container.Resolve<IAdditionalParametersRepository>().GetGaugeFromDB(kilometer, trip_id);
        }

        public static List<CrosProf> GetGaugeFromDBkmmter(int kilometer, int meter, long trip_id)
        {
            return Container.Resolve<IAdditionalParametersRepository>().GetGaugeFromDBkmmter(kilometer, meter, trip_id);
        }




        public static List<CrosProf> GetCrossRailProfileFromDB(Curve elem, long trip_id)
        {
            return Container.Resolve<IAdditionalParametersRepository>().GetCrossRailProfileFromDB(elem, trip_id);
        }
        public static CrossRailProfile GetCrossRailProfileFromDBParse(List<CrosProf> DBcrossRailProfile)
        {
            return Container.Resolve<IAdditionalParametersRepository>().GetCrossRailProfileFromDBParse(DBcrossRailProfile);
        }


        public static List<Gap> Check_gap_state(long trip_id, int templ_id)
        {
            return Container.Resolve<IAdditionalParametersRepository>().Check_gap_state(trip_id, templ_id);
        }

      
        public static ShortRoughness GetShortRoughnessFromDBParse(List<CrosProf> DBcrossRailProfile)
        {
            return Container.Resolve<IAdditionalParametersRepository>().GetShortRoughnessFromDBParse(DBcrossRailProfile);
        }

        public static List<Digression> Check_bolt_state(long trip_id, int templ_id)
        {
            return Container.Resolve<IAdditionalParametersRepository>().Check_bolt_state(trip_id, templ_id);
        }

       

       

        public static ShortRoughness GetShortRoughnessFromText(int kilometer)
        {
            return Container.Resolve<IAdditionalParametersRepository>().GetShortRoughnessFromText(kilometer);
        }

        public static List<RailFastener> GetRailFasteners(long tripId, int kilometer)
        {
            return Container.Resolve<IAdditionalParametersRepository>().GetRailFasteners(tripId,kilometer);
        }

        public static List<Gap> DirectName(long process_id, int direction)
        {
            return Container.Resolve<IAdditionalParametersRepository>().DirectName(process_id, direction);
        }

       
        public static List<Gap> RDGetShpalGap(long trip_id, int direction, int kmetr, int epur)
        {
            return Container.Resolve<IAdditionalParametersRepository>().RDGetShpalGap(trip_id, direction, kmetr, epur);
        }

        public static object GetAddParam(long trip_id)
        {
            return Container.Resolve<IAdditionalParametersRepository>().GetAddParam(trip_id);
        }

        public static List<Gap> GetGap(Int64 process_id, int direction)
        {
            return Container.Resolve<IAdditionalParametersRepository>().GetGap(process_id, direction);
        }
		public static List<Gap> GetFusGap(long process_id, int direction)
        {
            return Container.Resolve<IAdditionalParametersRepository>().GetFusGap(process_id, direction);
        }

        public static List<CrosProf> GetCrossRailProfileDFPR3(long trip_id)
        {
            return Container.Resolve<IAdditionalParametersRepository>().GetCrossRailProfileDFPR3(trip_id);
        }

       

        public static List<int> GetKilometersByTripId(Int64 process_id)
        {
            return Container.Resolve<IAdditionalParametersRepository>().GetKilometersByTripId(process_id);
        }
		public static List<int> GetKilometersByTripId(long process_id, long trackId)
        {
            return Container.Resolve<IAdditionalParametersRepository>().GetKilometersByTripId(process_id, trackId);
        }

       

        public static List<Gap> RDGetShpal(long process_id, int direction, int km)
        {
            return Container.Resolve<IAdditionalParametersRepository>().RDGetShpal(process_id, direction, km);
        }

        

        public static List<Gap> GetMaech(Int64 process_id, int direction)
        {
            return Container.Resolve<IAdditionalParametersRepository>().GetMaech(process_id, direction);
        }


        public static List<Gap> RDGetGap(long process_id, int direction, int track_id)
        {
            return Container.Resolve<IAdditionalParametersRepository>().RDGetGap(process_id, direction, track_id);

        }

        public static CrossRailProfile vertIznos(int nkm)
        {
            return Container.Resolve<IAdditionalParametersRepository>().vertIznos(nkm);
        }

        public static List<Gap> GetdefISGap(long id)
        {
            return Container.Resolve<IAdditionalParametersRepository>().GetdefISGap(id);
        }

      

        public static object Insert_bolt(long trip_id, int template_id, List<Digression> digressions)
        {
            return Container.Resolve<IAdditionalParametersRepository>().Insert_bolt(trip_id, template_id, digressions);
        }
        public static List<Gap> Check_Sleep_gap_state(long trip_id, int templ_id)
        {
            return Container.Resolve<IAdditionalParametersRepository>().Check_Sleep_gap_state(trip_id, templ_id);
        }

        public static List<CrosProf> GetCrossRailProfileDFPR3Radius(long track_id, DateTime date_Vrem, int Km, int meter)
        {
            return Container.Resolve<IAdditionalParametersRepository>().GetCrossRailProfileDFPR3Radius(track_id, date_Vrem,  Km, meter);
        }

        public static List<Digression> GetFullGapsByNN(long km, long trip_id)
        {
            return Container.Resolve<IAdditionalParametersRepository>().GetFullGapsByNN(km, trip_id);
        }
        public static List<Digression> GetFullGapsByNN(long km, long trip_id, string queru)
        {
            return Container.Resolve<IAdditionalParametersRepository>().GetFullGapsByNN(km, trip_id, queru);
        }

        public static object Insert_badfastening(long trip_id, int iD, List<RailFastener> badFasteners)
        {
            return Container.Resolve<IAdditionalParametersRepository>().Insert_badfastening(trip_id, iD, badFasteners);
        }
        public static List<Digression> Check_defshpal_state(long trip_id, int iD)
        {
            return Container.Resolve<IAdditionalParametersRepository>().Check_defshpal_state(trip_id, iD);
        }

        public static object Insert_defshpal(long trip_id, int iD, List<Digression> digressions)
        {
            return Container.Resolve<IAdditionalParametersRepository>().Insert_defshpal(trip_id, iD, digressions);
        }
        public static List<Digression> Total(long trip_id, int iD)
        {
            return Container.Resolve<IAdditionalParametersRepository>().Total(trip_id, iD);
        }
        public static List<Digression> Check_Total_state_digression(long trip_id, int iD)
        {
            return Container.Resolve<IAdditionalParametersRepository>().Check_Total_state_digression(trip_id, iD);
        }

        public static List<Digression> Check_badfastening_state(long trip_id, int templ_id)
        {
            return Container.Resolve<IAdditionalParametersRepository>().Check_badfastening_state(trip_id, templ_id);
        }
        public static List<Digression> Check_Total_state_fastening(long trip_id, int templ_id)
        {
            return Container.Resolve<IAdditionalParametersRepository>().Check_Total_state_fastening(trip_id, templ_id);
        }

       
        public static List<Digression> Check_sleepers_state(long trip_id, int iD)
        {
            return Container.Resolve<IAdditionalParametersRepository>().Check_sleepers_state(trip_id, iD);
        }

     

        public static List<Digression> Check_Total_state_Jointless(long trip_id, int templ_id)
        {
            return Container.Resolve<IAdditionalParametersRepository>().Check_Total_state_Jointless(trip_id, templ_id);
        }
        public static object Check_Total_state_rails(long trip_id, int iD)
        {
            return Container.Resolve<IAdditionalParametersRepository>().Check_Total_state_rails(trip_id, iD);
        }
        public static List<Digression> Check_Total_bolts(long trip_id, int iD)
        {
            return Container.Resolve<IAdditionalParametersRepository>().Check_Total_bolts(trip_id, iD);
        }
        public static List<Digression> Check_Total_threat_id(long trip_id, int iD)
        {
            return Container.Resolve<IAdditionalParametersRepository>().Check_Total_threat_id(trip_id, iD);
        }
        public static List<Gap> Check_ListOgDerogations_state(long trip_id, int iD)
        {
            return Container.Resolve<IAdditionalParametersRepository>().Check_ListOgDerogations_state(trip_id, iD);
        }
        public static object Insert_ListOgDerogations_state(long trip_id, int iD, List<Gap> GetMaech)
        {
            return Container.Resolve<IAdditionalParametersRepository>().Insert_ListOgDerogations_state(trip_id, iD, GetMaech);
        }

        public static List<Digression> GetImpulsesByKm(int kilometer)
        {
            return Container.Resolve<IAdditionalParametersRepository>().GetImpulsesByKm(kilometer);
        }


        public static object Insert_sleepers(long trip_id, int iD, List<Digression> digList)
        {
            return Container.Resolve<IAdditionalParametersRepository>().Insert_sleepers(trip_id, iD, digList);
        }
        public static void Insert_ViolPerpen(Kilometer km, List<RailsBrace> skreplenie, List<Digression> violPerpen)
        {
            Container.Resolve<IAdditionalParametersRepository>().Insert_ViolPerpen(km, skreplenie, violPerpen);
        }

        public static object Insert_deviationsinfastening(long trip_id, int iD, List<RailFastener> digList)
        {
            return Container.Resolve<IAdditionalParametersRepository>().Insert_deviationsinfastening(trip_id, iD, digList);
        }

        public static List<Digression> Check_Total_balast(long trip_id, int iD)
        {
            return Container.Resolve<IAdditionalParametersRepository>().Check_Total_balast(trip_id, iD);
        }
        public static object Insert_deviationsinballast(long trip_id, int iD, List<RailFastener> digressions)
        {
            return Container.Resolve<IAdditionalParametersRepository>().Insert_deviationsinballast(trip_id, iD, digressions);
        }
        public static string Insert_gap(long trip_id, int template_id, List<Digression> gaps)
        {
            return Container.Resolve<IAdditionalParametersRepository>().Insert_gap(trip_id, template_id, gaps);
        }
        public static List<Digression> Check_deviationsinfastening_state(long trip_id, int iD)
        {
            return Container.Resolve<IAdditionalParametersRepository>().Check_deviationsinfastening_state(trip_id, iD);
        }
        public static List<Digression> Check_ViolPerpen(long trip_id)
        {
            return Container.Resolve<IAdditionalParametersRepository>().Check_ViolPerpen(trip_id);
        }

        public static List<CrosProf> GetCrossRailProfileFromDBbyCurve(Curve curve, long trip_id)
        {
            return Container.Resolve<IAdditionalParametersRepository>().GetCrossRailProfileFromDBbyCurve(curve, trip_id);
        }
        public static List<Digression> Insert_additional_param_state_longwawes(List<Digression> impulses)
        {
            return Container.Resolve<IAdditionalParametersRepository>().Insert_additional_param_state_longwawes(impulses);
        }
        public static List<Digression> Insert_additional_param_state_aslan(List<Digression> addDigressions, long trip_id)
        {
            return Container.Resolve<IAdditionalParametersRepository>().Insert_additional_param_state_aslan(addDigressions, trip_id);
        }
        public static List<Digression> Insert_additional_param_state(List<Digression> addDigressions,int nkm)
        {
            return Container.Resolve<IAdditionalParametersRepository>().Insert_additional_param_state(addDigressions,  nkm);
        }

        public static List<DataFlow> GetShortRough(long trip_id, int number)
        {
            return Container.Resolve<IAdditionalParametersRepository>().GetShortRough(trip_id, number);
        }


    }
}
