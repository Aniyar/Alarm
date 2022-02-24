namespace ALARm.Core
{
    public class StCurve : MainTrackObject
    {
        public double Radius { get; set; }
        public double Wear { get; set; }
        public int Width { get; set; }
        public int Transition_1 { get; set; }
        public int Transition_2 { get; set; }
        public double FirstTransitionEnd { get; set; }
        public double SecondTransitionStart { get; set; }

    }
}
