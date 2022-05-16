using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALARm.Core.AdditionalParameteres;
using ALARm.Core.Report;

namespace ALARm.Core
{
    public interface IAdditionalParametersRepository
    {
        List<Gap> GetGaps(Int64 process_id, int direction, int kilometer);
        List<int> GetKilometers(Int64 process_id, int direction);
        CrossRailProfile GetCrossRailProfileFromText(int kilometer);
        ShortRoughness GetShortRoughnessFromText(int kilometer);
        List<Gap> GetGaps(Int64 trip_id, int kilometer);
		List<Gap> GetGap(long process_id, int direction);
        List<Gap> DirectName(long process_id, int direction);
        List<Heat> GetHeats(Int64 trip_id, int kilometer);
        Bitmap GetFrame(int v, Int64 file_id);
        List<VideoObject> GetObjectsByFrameNumber(int frame_Number, Int64 trip_id);
        List<Gap> GetGapsByFrameNumber(int frame_Number, Int64 id);
        Bitmap MatrixToTimage(int[,] matrix);
        List<RailFastener> GetRailFasteners(long tripId, int kilometer);
        List<int> GetKilometersByTripId(Int64 process_id);
		List<int> GetKilometersByTripId(long process_id, long trackId);
        CrossRailProfile vertIznos(int nkm);
        List<Gap> GetFusGap(long process_id, int direction);
        List<Bitmap> GetBitMap(long fileid, long ms);
        Dictionary<String,Object> getBitMaps(long fileId , long ms , int fnum, RepType RepType = RepType.Undefined);
        int[,] getFilter(long fileId, long ms);
        List<VideoObject> GetObjectsByFrameNumber(long fileId, long ms, int fnum, RepType RepType = RepType.Undefined);


        List<Gap> GetdefISGap(long id);
        List<Gap> RDGetGap(long process_id, int direction, int track_id);
        List<CrosProf> GetGaugeFromDB(int kilometer, long trip_id);


        List<CrosProf> GetGaugeFromDBkmmter(int kilometer,int meter, long trip_id);
        List<CrosProf> GetCrossRailProfileFromDBbyKm(int kilometer,  long trip_id);
        List<CrosProf> GetCrossRailProfileFromDBbyKm_forPPWEB(int kilometer, int index, int count, long trip_id);
    
        
        List<CrosProf> GetCrossRailProfileFromDBbyTripId(long trip_id);
        List<CrosProf> GetCrossRailProfileFromDB(Curve elem, long trip_id);
        List<Gap> RDGetShpal(long process_id, int direction, int km);
        List<Gap> GetMaech(long process_id, int direction);
        CrossRailProfile GetCrossRailProfileFromDBParse(List<CrosProf> dBcrossRailProfile);
        CarPosition GetCarPositionByFile(long fileId);
        ShortRoughness GetShortRoughnessFromDBParse(List<CrosProf> DBcrossRailProfile);
        List<CrosProf> GetCrossRailProfileDFPR3(long trip_id);
        List<Gap> Check_gap_state(long trip_id, int templ_id);
        string Insert_gap(long trip_id, int template_id, List<Digression> gaps);
        object Insert_bolt(long trip_id, int template_id, List<Digression> digressions);
        
        List<Gap> RDGetShpalGap(long trip_id, int direction, int kmetr, int epur);
        List<CrosProf> GetCrossRailProfileDFPR3Radius(long track_id, DateTime date_Vrem, int Km, int meter);
        List<CrosProf> GetCrossRailProfileDFPR3Radius(int kilometer, long trip_id);
        List<Digression> Check_bolt_state(long trip_id, int templ_id);
        List<Gap> Check_Sleep_gap_state(long trip_id, int templ_id);
        object Insert_badfastening(long trip_id, int template_id, List<RailFastener> badFasteners);
        List<Digression> Check_badfastening_state(long trip_id, int iD);
        object Insert_defshpal(long trip_id, int iD, List<Digression> digressions);
        List<Digression> Check_defshpal_state(long trip_id, int iD);
        List<Digression> Total(long trip_id, int iD);
        List<CrosProf> GetCrossRailProfileFromTrip(long trip_id);
        List<Digression> Check_Total_state_digression(long trip_id, int iD);
        List<Digression> Check_Total_state_fastening(long trip_id, int iD);
        List<Digression> Check_Total_state_rails(long trip_id, int iD);
        List<Digression> Check_sleepers_state(long trip_id, int iD);
        List<Digression> Check_Total_state_Jointless(long trip_id, int iD);
        object Insert_sleepers(long trip_id, int iD, List<Digression> digList);
        List<Digression> Check_Total_bolts(long trip_id, int iD);
        List<Digression> Check_Total_threat_id(long trip_id, int iD);
        List<Gap> Check_ListOgDerogations_state(long trip_id, int iD);
        object GetAddParam(long trip_id);
        object Insert_ListOgDerogations_state(long trip_id, int iD, List<Gap> GetMaech);
        List<Digression> GetImpulsesByKm(int kilometer);
        List<Digression> Check_deviationsinfastening_state(long trip_id, int iD);
        object Insert_deviationsinfastening(long trip_id, int iD, List<RailFastener> digList);
        List<Digression> Check_Total_balast(long trip_id, int iD);
        object Insert_deviationsinballast(long trip_id, int iD, List<RailFastener> digressions);
        List<Digression> GetFullGapsByNN(long km, long trip_id, string queru);
        void Insert_ViolPerpen(Kilometer km, List<RailsBrace> skreplenie, List<Digression> violPerpen);
        List<Digression> Check_ViolPerpen(long trip_id);
        List<CrosProf> GetCrossRailProfileFromDBbyCurve(Curve curve, long trip_id);
        List<Digression> GetFullGapsByNN(long km, long trip_id);

        List<Digression> Insert_additional_param_state_longwawes(List<Digression> impulses);
        List<Digression> Insert_additional_param_state_aslan(List<Digression> addDigressions, long trip_id);

        List<Digression> Insert_additional_param_state(List<Digression> addDigressions, int nkm);
        List<DataFlow> GetShortRough(long trip_id, int number);
    }
}
