namespace Serpen.Uni.Automat.Turing {
    public sealed class TuringTransformMultiTrack : TransformBase<TuringTransformSingleBand.TuringKey, TuringTransformSingleBand.TuringVal> {

        public TuringTransformMultiTrack(string[] stateTracks, string[] bandTracks, char[] bandSymbols) {
            StateTracks = stateTracks;
            BandTracks = bandTracks;
            BandSymbols = bandSymbols;
        }

        readonly string[] StateTracks;
        internal readonly string[] BandTracks;
        readonly char[] BandSymbols;

        public override string ToString() {
            var sw = new System.Text.StringBuilder();
            foreach (var item in this) {
                sw.Append($"({item.Key.ToString()})=>");
                sw.Append($"({item.Value.ToString()}); ");
                // sw.Append("); ");
            }
            return sw.ToString();
        }

        public void AddByStateStoreAndTracks(string q, string c, string qNext, string c2, TMDirection dir) {
            base.Add(
                new TuringTransformSingleBand.TuringKey(Utils.ArrayIndex(StateTracks, q), BandSymbols[Utils.ArrayIndex(BandTracks, c)]),
                new TuringTransformSingleBand.TuringVal(Utils.ArrayIndex(StateTracks, qNext), BandSymbols[Utils.ArrayIndex(BandTracks, c2)], dir)
            );
        }
        public void AddByTracks(uint q, string c, uint qNext, string c2, TMDirection dir) {
            base.Add(
                new TuringTransformSingleBand.TuringKey(q, BandSymbols[Utils.ArrayIndex(BandTracks, c)]),
                new TuringTransformSingleBand.TuringVal(qNext, BandSymbols[Utils.ArrayIndex(BandTracks, c2)], dir)
            );
        }
    }
}