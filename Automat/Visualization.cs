using System.Drawing;
using Math = System.Math;

namespace Serpen.Uni.Automat {

    public class VisualizationTuple : System.Tuple<int, int, string> {
        public VisualizationTuple(int q, int q2, string desc) : base(q, q2, desc) { }
        public VisualizationTuple(uint q, uint q2, string desc) : base((int)q, (int)q2, desc) { }
    }

    public static class Visualization {

        const int STATE_DIAMETER = 45;
        const int IMAGE_SPACE = 75;
        const int ARROW_WIDTH = 6;
        const int LOOP_WIDTH = 40;
        const int CURVE_BONUS = 11;

        static readonly Pen PEN_DOUBLE = new Pen(Color.Black) {
            Width = 5,
            CompoundArray = new float[] { 0.0f, 0.4f, 0.6f, 1f }
        };

        static Pen PEN_ARROW {
            get {
                var tmpen = new Pen(Color.Black);
                var hPath = new System.Drawing.Drawing2D.GraphicsPath();
                hPath.AddLine(new Point(0, 0), new Point(-ARROW_WIDTH, -ARROW_WIDTH));
                hPath.AddLine(new Point(0, 0), new Point(ARROW_WIDTH, -ARROW_WIDTH));
                var HookCap = new System.Drawing.Drawing2D.CustomLineCap(null, hPath);
                tmpen.CustomEndCap = HookCap;
                return tmpen;
            }
        }

        static Pen PEN_START_ARROW {
            get {
                const int WIDTH = 2;
                var tmpen = new Pen(Color.Black, WIDTH);
                var hPath = new System.Drawing.Drawing2D.GraphicsPath();
                hPath.AddLine(new Point(0, 0), new Point(-ARROW_WIDTH / WIDTH, -ARROW_WIDTH / WIDTH));
                hPath.AddLine(new Point(0, 0), new Point(ARROW_WIDTH / WIDTH, -ARROW_WIDTH / WIDTH));
                var HookCap = new System.Drawing.Drawing2D.CustomLineCap(null, hPath);
                tmpen.CustomEndCap = HookCap;
                return tmpen;
            }
        }
        static readonly Font CourierNewFont = new Font("Courier New", 14);

        static readonly Pen PEN = Pens.Black;

        /// <summary>Calculates the position of state Q</summary>
        static int qPos(int q) => (int)IMAGE_SPACE + q * STATE_DIAMETER + q * IMAGE_SPACE;
        static int qPos(uint q) => (int)IMAGE_SPACE + (int)q * STATE_DIAMETER + (int)q * IMAGE_SPACE;

        class AlreadyDrawnClass : System.Collections.Generic.Dictionary<System.Tuple<int, int>, float> {
            public void AddOrInc(int q1, int q2, float Height) {
                var t = new System.Tuple<int, int>(q1, q2);
                if (base.TryGetValue(t, out _)) {
                    base[t] += Height;
                } else {
                    base.Add(t, Height);
                }
            }

            public float this[int q1, int q2] {
                get {
                    if (base.TryGetValue(new System.Tuple<int, int>(q1, q2), out float val))
                        return val;
                    else
                        return 0;
                }
            }
        }
        public static Bitmap DrawAutomat(IAutomat A, uint? highlightState = null) {

            var alreadyDrawn = new AlreadyDrawnClass();

            int lastCurveHeigth = 3 * CURVE_BONUS;

            float relFactor = 1;

            var vls = A.VisualizationLines();

            if (vls.Length > A.StatesCount*3) {
                relFactor *= vls.Length/(A.StatesCount*3);
                Utils.DebugMessage($"corrected bmpRelFactor to {relFactor} by transform count", A, Uni.Utils.eDebugLogLevel.Verbose);
            } else if (A.Alphabet.Length > A.StatesCount*4) {
                relFactor *= A.Alphabet.Length/(A.StatesCount*4);
                Utils.DebugMessage($"corrected bmpRelFactor to {relFactor} by alphabet count", A, Uni.Utils.eDebugLogLevel.Verbose);
            }

            int bmpwidth = Math.Max(500, (int)(IMAGE_SPACE + (A.StatesCount) * (STATE_DIAMETER + IMAGE_SPACE)*relFactor));
            var bmp = new Bitmap(bmpwidth, (int)(bmpwidth / 1.5));

            Graphics g = Graphics.FromImage(bmp);
            Font headFont = new Font(SystemFonts.DefaultFont.FontFamily, Math.Max(14, bmpwidth / 60));

            int vCenter = (int)(bmp.Height / 2.6);

            g.FillRectangle(Brushes.White, 0, 0, bmp.Width, bmp.Height);

            g.DrawString(A.Name, headFont, Brushes.Black, 20, 20);

            //Draw full declaration to bottom
            var FullDesc = A.ToString().Replace(A.Name, "").Trim();
            // try {
            //     FullDesc = FullDesc.Replace(A.Name, "").Trim();
            // } catch { }

            const int MARGIN = 10;

            var FullDescSize = g.MeasureString(FullDesc, CourierNewFont);
            if (FullDescSize.Width < bmpwidth - MARGIN*2)
                g.DrawString(FullDesc, CourierNewFont, Brushes.Black, 15f, bmp.Height - FullDescSize.Height * 1.5f);
            else {
                int DescPartCount = (int)Math.Ceiling(FullDescSize.Width / (bmpwidth - MARGIN*2));
                int toStringSplitFactor = (int)Math.Ceiling((float)FullDesc.Length / DescPartCount);
                for (int i = 0; i < DescPartCount; i++) {
                    string rowString = FullDesc.Substring(i * toStringSplitFactor, Math.Min(toStringSplitFactor, FullDesc.Length-i*toStringSplitFactor));
                    g.DrawString(rowString, CourierNewFont, Brushes.Black, 15, 
                        bmp.Height - (DescPartCount - i + 1) * (FullDescSize.Height));
                }
            }


            // draw States
            for (uint iq = 0; iq < A.StatesCount; iq++) {
                var strSize = g.MeasureString(A.States[iq], CourierNewFont);
                g.DrawString(A.States[iq], CourierNewFont, Brushes.Black, qPos(iq) + STATE_DIAMETER / 2 - strSize.Width / 2, vCenter - strSize.Height / 2);

                var ellipsePen = PEN;
                if (A.IsAcceptedState(iq))
                    ellipsePen = PEN_DOUBLE;
                if (iq == highlightState)
                    ellipsePen = Pens.Red;
                
                g.DrawEllipse(ellipsePen, qPos(iq), vCenter - STATE_DIAMETER / 2, STATE_DIAMETER, STATE_DIAMETER);
            }

            // draw Start Arrow
            {
                int lineX = qPos(A.StartState) - IMAGE_SPACE / 2;
                int lineX2 = qPos(A.StartState);
                g.DrawLine(PEN_START_ARROW, lineX, vCenter, lineX2, vCenter);
            }

            // draw Transform Lines, depends which automat, because of inconsistent transform inheritance and nondeterministic transform 
            foreach (var vtl in vls)
                DrawTransformLine(ref g, vtl.Item1, vtl.Item2, vtl.Item3, vCenter, ref lastCurveHeigth, ref alreadyDrawn);

            return bmp;
        }

        static void DrawTransformLine(ref Graphics g, int qStart, int qEnd, string desc, int vCenter, ref int lastCurveHeigth, ref AlreadyDrawnClass alreadyDrawn) {
            var descSize = g.MeasureString(desc, CourierNewFont);

            var penArrow = PEN_ARROW;
            var alreadyDrawnIndex = alreadyDrawn[qStart, qEnd];

            if (qStart == qEnd - 1) { //state left to right
                int lineX = qPos(qStart) + STATE_DIAMETER;
                int lineX2 = qPos(qEnd);

                g.DrawLine(penArrow, lineX, vCenter, lineX2, vCenter);
                g.DrawString(desc, CourierNewFont, Brushes.Black, (lineX + lineX2) / 2 - descSize.Width / 2 + 10, vCenter - 5 - descSize.Height - alreadyDrawnIndex);

            } else if (qStart == qEnd + 1) { //state right to left
                int lineX = qPos(qStart);
                int lineX2 = qPos(qEnd) + STATE_DIAMETER;

                g.DrawLine(penArrow, lineX, vCenter, lineX2, vCenter);
                g.DrawString(desc, CourierNewFont, Brushes.Black, (lineX + lineX2) / 2 - descSize.Width / 2 - 10, vCenter + 5 + alreadyDrawnIndex);

            } else if (qStart == qEnd) { //loop above
                int lineX = qPos(qStart) + STATE_DIAMETER / 2 - LOOP_WIDTH / 4;
                int lineY = vCenter - STATE_DIAMETER / 2 - LOOP_WIDTH;

                g.DrawEllipse(PEN_ARROW, lineX, lineY, LOOP_WIDTH / 2, LOOP_WIDTH);
                g.DrawString(desc, CourierNewFont, Brushes.Black, lineX + LOOP_WIDTH / 4 - descSize.Width / 2, lineY - descSize.Height + alreadyDrawnIndex);

            } else { //other connections below
                int lineX = qPos(qStart) + STATE_DIAMETER / 2;
                int lineX2 = qPos(qEnd) + STATE_DIAMETER / 2;
                var lineM = (lineX + lineX2) / 2;
                int thisCurveHeigth;

                if (qStart > qEnd)
                    penArrow.Color = Color.DarkGray;

                if (alreadyDrawnIndex == 0) {
                    thisCurveHeigth = lastCurveHeigth;
                    lastCurveHeigth += CURVE_BONUS;
                    var ps = new Point[] {
                        new Point(lineX, vCenter+STATE_DIAMETER / 2),
                        new Point(lineM, vCenter+STATE_DIAMETER/2+thisCurveHeigth),
                        new Point(lineX2, vCenter+STATE_DIAMETER/2)};

                    g.DrawCurve(penArrow, ps);
                    g.DrawString(desc, CourierNewFont, Brushes.Black, lineM - descSize.Width / 2,
                        vCenter + STATE_DIAMETER / 2 + thisCurveHeigth);
                } else {
                    thisCurveHeigth = lastCurveHeigth - CURVE_BONUS;
                    g.DrawString(desc, CourierNewFont, Brushes.Black, lineM - descSize.Width / 2,
                        vCenter + STATE_DIAMETER / 2 + thisCurveHeigth + alreadyDrawnIndex);
                }
            }
            alreadyDrawn.AddOrInc(qStart, qEnd, descSize.Height);
        }
    }
}