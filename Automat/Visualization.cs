using System.Drawing;
using Math = System.Math;
using System.Linq;

namespace Serpen.Uni.Automat {
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
        static readonly Font FONT = new Font(SystemFonts.DefaultFont.FontFamily, 14);

        static readonly Pen PEN = Pens.Black;

        /// <summary>Calculates the position of state Q</summary>
        static int qPos(int q) => (int)IMAGE_SPACE + q * STATE_DIAMETER + q * IMAGE_SPACE;
        static int qPos(uint q) => (int)IMAGE_SPACE + (int)q * STATE_DIAMETER + (int)q * IMAGE_SPACE;

        class AlreadyDrawnClass : System.Collections.Generic.Dictionary<System.Tuple<int, int>, float> {
            public void AddOrInc(int q1, int q2, float Height) {
                float val;
                var t = new System.Tuple<int, int>(q1, q2);
                if (base.TryGetValue(t, out val)) {
                    base[t] += Height;
                } else {
                    base.Add(t, Height);
                }
            }

            public float this[int q1, int q2] {
                get {
                    float val;
                    if (base.TryGetValue(new System.Tuple<int, int>(q1, q2), out val))
                        return val;
                    else
                        return 0;
                }
            }
        }
        public static Bitmap DrawAutomat(IAutomat A) {

            var alreadyDrawn = new AlreadyDrawnClass();

            int lastCurveHeigth = 3 * CURVE_BONUS;

            int bmpwidth = Math.Max(500, (int)(IMAGE_SPACE + (A.StatesCount) * (STATE_DIAMETER + IMAGE_SPACE)));
            var bmp = new Bitmap(bmpwidth, (int)(bmpwidth / 1.5));

            Graphics g = Graphics.FromImage(bmp);
            Font headFont = new Font(SystemFonts.DefaultFont.FontFamily, Math.Max(14, bmpwidth / 60));

            int vCenter = (int)(bmp.Height / 2.6);

            g.FillRectangle(Brushes.White, 0, 0, bmp.Width, bmp.Height);

            g.DrawString(A.Name, headFont, Brushes.Black, 20, 20);

            //Draw full declaration to bottom
            var toString = A.ToString();
            try {
                toString = toString.Replace(A.Name, "").Trim();
            } catch { }
            var toStringSize = g.MeasureString(toString, FONT);
            if (toStringSize.Width < bmpwidth)
                g.DrawString(toString, FONT, Brushes.Black, 20, bmp.Height - toStringSize.Height * 2);
            else {
                int toStringSplits = (int)Math.Ceiling(bmpwidth / toStringSize.Width) + 1;
                int toStringSplitFactor = toString.Length / toStringSplits;
                for (int i = 0; i < toStringSplits; i++) {
                    g.DrawString(toString.Substring(i * toStringSplitFactor, toStringSplitFactor), FONT, Brushes.Black, 20, bmp.Height - toStringSplits * toStringSize.Height + i * toStringSize.Height);
                }
            }


            for (uint iq = 0; iq < A.StatesCount; iq++) {
                var strSize = g.MeasureString(A.States[iq], FONT);
                g.DrawString(A.States[iq], FONT, Brushes.Black, qPos(iq) + STATE_DIAMETER / 2 - strSize.Width / 2, vCenter - strSize.Height / 2);

                var ellipsePen = PEN;
                if (A.IsAcceptedState(iq))
                    ellipsePen = PEN_DOUBLE;
                g.DrawEllipse(ellipsePen, qPos(iq), vCenter - STATE_DIAMETER / 2, STATE_DIAMETER, STATE_DIAMETER);
            }

            //draw Start Arrow
            {
                int lineX = qPos(A.StartState) - IMAGE_SPACE / 2;
                int lineX2 = qPos(A.StartState);
                g.DrawLine(PEN_START_ARROW, lineX, vCenter, lineX2, vCenter);
            }

            //draw Transform Lines, depends which automat, because of inconsistent transform inheritance and nondeterministic transform 
            foreach (var vtl in A.VisualizationLines())
                DrawTransformLine(ref g, vtl.Item1, vtl.Item2, vtl.Item3, vCenter, ref lastCurveHeigth, ref alreadyDrawn);

            //convert to 8Bit, less is not good
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            Bitmap newBitmap = new Bitmap(bmp.Width, bmp.Height, bmpData.Stride, System.Drawing.Imaging.PixelFormat.Format8bppIndexed, bmpData.Scan0);
            return newBitmap;
        }

        static void DrawTransformLine(ref Graphics g, int qStart, int qEnd, string desc, int vCenter, ref int lastCurveHeigth, ref AlreadyDrawnClass alreadyDrawn) {
            var descSize = g.MeasureString(desc, FONT);

            var penArrow = PEN_ARROW;
            var alreadyDrawnIndex = alreadyDrawn[qStart, qEnd];

            if (qStart == qEnd - 1) { //state left to right
                int lineX = qPos(qStart) + STATE_DIAMETER;
                int lineX2 = qPos(qEnd);

                g.DrawLine(penArrow, lineX, vCenter, lineX2, vCenter);
                g.DrawString(desc, FONT, Brushes.Black, (lineX + lineX2) / 2 - descSize.Width / 2 + 10, vCenter - 5 - descSize.Height - alreadyDrawnIndex);

            } else if (qStart == qEnd + 1) { //state right to left
                int lineX = qPos(qStart);
                int lineX2 = qPos(qEnd) + STATE_DIAMETER;

                g.DrawLine(penArrow, lineX, vCenter, lineX2, vCenter);
                g.DrawString(desc, FONT, Brushes.Black, (lineX + lineX2) / 2 - descSize.Width / 2 - 10, vCenter + 5 + alreadyDrawnIndex);

            } else if (qStart == qEnd) { //loop above
                int lineX = qPos(qStart) + STATE_DIAMETER / 2 - LOOP_WIDTH / 4;
                int lineY = vCenter - STATE_DIAMETER / 2 - LOOP_WIDTH;

                g.DrawEllipse(PEN_ARROW, lineX, lineY, LOOP_WIDTH / 2, LOOP_WIDTH);
                g.DrawString(desc, FONT, Brushes.Black, lineX + LOOP_WIDTH / 4 - descSize.Width / 2, lineY - descSize.Height + alreadyDrawnIndex);

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
                        new Point(lineX, vCenter+STATE_DIAMETER/2),
                        new Point(lineM, vCenter+STATE_DIAMETER/2+thisCurveHeigth),
                        new Point(lineX2, vCenter+STATE_DIAMETER/2)};

                    g.DrawCurve(penArrow, ps);
                    g.DrawString(desc, FONT, Brushes.Black, lineM - descSize.Width / 2,
                        vCenter + STATE_DIAMETER / 2 + thisCurveHeigth);
                } else {
                    thisCurveHeigth = lastCurveHeigth - CURVE_BONUS;
                    g.DrawString(desc, FONT, Brushes.Black, lineM - descSize.Width / 2,
                        vCenter + STATE_DIAMETER / 2 + thisCurveHeigth + alreadyDrawnIndex);
                }
            }
            alreadyDrawn.AddOrInc(qStart, qEnd, descSize.Height);
        }
    }
}