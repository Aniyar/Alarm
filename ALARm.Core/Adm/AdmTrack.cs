namespace ALARm.Core
{
    public class AdmTrack : AdmUnit
    {
        private new string Chief_fullname { get; set; }
        public string Border { get; set; }
        //принадлежность
        public string Belong { get; set; }
        public string Direction { get; set; } = "";
        public string Identity {
            get {
                if (Parent_Id == -1)
                    return "Станционный";
                else
                    return "Главный";
            }
        }
        public bool Accept { get; set; }
    }
}