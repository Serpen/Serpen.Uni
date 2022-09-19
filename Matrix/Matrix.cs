using System;

namespace Serpen.Uni {
    public class IntMatrix : System.ICloneable {

        int[,] M;

        IntMatrix() { }

        public object Clone() {
            return FromArray(M);
        }

        public IntMatrix(uint rows, uint cols) {
            M = new int[rows, cols];
        }

        public static IntMatrix FromArray(int[,] array) {
            return new IntMatrix {
                M = array
            };
        }

        public static IntMatrix FromArray(int[] array, uint cols) {
            var newM = new IntMatrix((uint)array.GetLength(0) / cols, cols);
            for (uint r = 0; r < array.GetLength(0); r++)
                for (uint c = 0; c < array.GetLength(0); c++)
                    newM[r, c] = array[r * cols + r];
            return newM;

        }

        public static IntMatrix FromArray(int[][] array) {
            var newM = new IntMatrix((uint)array.GetLength(0), (uint)array[0].GetLength(0));
            for (int r = 0; r < array.GetLength(0); r++) {
                for (int c = 0; c < array.GetLength(1); c++) {
                    newM.M[r, c] = array[r][c];
                }
            }
            return newM;
        }

        public static IntMatrix E(uint i, uint j, uint rowsAndCols) {
            var newM = new IntMatrix(rowsAndCols, rowsAndCols);
            newM[i, j] = 1;
            return newM;
        }

        public static IntMatrix P(uint i, uint j, uint rowsAndCols) {
            return EinheitsMatrix(rowsAndCols)
                - E(i, j, rowsAndCols)
                - E(i, i, rowsAndCols)
                + E(i, j, rowsAndCols)
                - E(j, j, rowsAndCols)
                + E(j, i, rowsAndCols);
        }

        public static IntMatrix D(uint i, int r, uint rowsAndCols)
            => EinheitsMatrix(rowsAndCols) + ((r - 1) * E(i, i, rowsAndCols)); //Di(r) = Im + (r − 1)Eii.

        public static IntMatrix T(uint i, uint j, int s, uint rowsAndCols)
            => EinheitsMatrix(rowsAndCols) + (s * E(i, j, rowsAndCols)); //Tij(s) = Im + sEij

        /*
        public int[][] ToRaggedArray() {
            int[][] array = new int[Rows];
            for (int r = 0; r < array.GetLength(0); r++) {
                array[r] = new int[Cols];
                for (int c = 0; c < array.GetLength(1); c++) {
                    array[r][c] = m[r,c];
                }
            }
        }
        */
        public int this[uint r, uint c] {
            get {
                return M[r, c];
            }
            internal set {
                M[r, c] = value;
            }
        }

        public uint Rows => (uint)M.GetLength(0);
        public uint Cols => (uint)M.GetLength(1);

        public override String ToString() {
            int maxLen = 0;
            String rowString;
            var totalString = new System.Text.StringBuilder(); ;

            for (int r = 0; r < M.GetLength(0); r++) {
                for (int c = 0; c < Cols; c++) {
                    int len = (int)Math.Floor(Math.Log10(M[r, c]));
                    if (len > maxLen)
                        maxLen = len;
                }
            }

            for (int r = 0; r < Rows; r++) {
                rowString = "";
                for (int c = 0; c < Cols; c++) {
                    rowString += M[r, c].ToString().PadLeft(maxLen) + ",";
                }
                totalString.Append(rowString + Environment.NewLine);
            }
            return totalString.ToString();

        }

        public static IntMatrix operator +(IntMatrix a, IntMatrix b) {
            if (a.Rows != b.Rows ||
                a.Cols != b.Cols)
                throw new System.NotSupportedException();
            else {
                IntMatrix newM = new IntMatrix(a.Rows, a.Cols);
                for (int r = 0; r < a.Rows; r++)
                    for (int c = 0; c < a.Cols; c++)
                        newM.M[r, c] = a.M[r, c] + b.M[r, c];
                return newM;
            }
        }
        public static IntMatrix operator -(IntMatrix a, IntMatrix b) {
            if (a.Rows != b.Rows ||
                a.Cols != b.Cols)
                throw new System.NotSupportedException();
            else {
                IntMatrix newM = new IntMatrix(a.Rows, a.Cols);
                for (int r = 0; r < a.Rows; r++)
                    for (int c = 0; c < a.Cols; c++)
                        newM.M[r, c] = a.M[r, c] - b.M[r, c];
                return newM;
            }
        }

        public static bool operator ==(IntMatrix a, IntMatrix b) {
            if (a.Rows != b.Rows ||
                a.Cols != b.Cols)
                return false;
            else {
                for (int r = 0; r < a.Cols; r++)
                    for (int c = 0; c < a.Cols; c++)
                        if (a.M[r, c] != b.M[r, c])
                            return false;
                return true;
            }
        }

        public static bool operator !=(IntMatrix a, IntMatrix b) => !(a == b);

        public override bool Equals(object obj) =>
            obj is IntMatrix m && this == m;

        public static IntMatrix operator -(IntMatrix M) => -1 * M;

        public static IntMatrix operator *(IntMatrix a, IntMatrix b) {

            if (a.Cols != b.Rows)
                throw new System.NotSupportedException();
            else {
                IntMatrix newM = new IntMatrix(a.Rows, b.Cols);
                for (int r = 0; r < a.Rows; r++)
                    for (int c = 0; c < b.Cols; c++)
                        for (int x = 0; x < a.Cols; x++)
                            newM.M[r, c] += a.M[r, x] * b.M[x, c]; //

                return newM;
            }
        }

        public static IntMatrix EinheitsMatrix(uint i) {
            var newM = new IntMatrix(i, i);
            for (uint j = 0; j < i; j++)
                newM[j, j] = 1;
            return newM;
        }
        public static IntMatrix operator *(int factor, IntMatrix a) {
            IntMatrix newM = new IntMatrix(a.Rows, a.Cols);
            for (uint r = 0; r < a.Rows; r++)
                for (uint c = 0; c < a.Cols; c++)
                    newM.M[r, c] = a[r, c] * factor;

            return newM;
        }
        public static IntMatrix operator *(IntMatrix a, int factor) => factor * a;

        public int Eigenwert() {
            throw new System.NotImplementedException();
        }

        public IntMatrix SwapRows(uint r1, uint r2) {
            var newM = IntMatrix.FromArray(M);

            for (uint c = 0; c < newM.Cols; c++)
                (newM[r1, c], newM[r2, c]) = (newM[r1, c], newM[r2, c]);
            return newM;
        }

        public IntMatrix MultiplyRow(uint r, int factor) {
            var newM = IntMatrix.FromArray(M);
            for (uint c = 0; c < newM.Cols; c++)
                newM[r, c] *= factor;
            return newM;
        }

        public IntMatrix AddRows(uint rSrc, uint rDst, int factor) {
            var newM = IntMatrix.FromArray(M);
            for (uint c = 0; c < newM.Cols; c++)
                newM[rDst, c] += factor * newM[rSrc, c];
            return newM;
        }

        public bool isTNF() {
            throw new System.NotImplementedException();
        }

        public override int GetHashCode() => this.M.GetHashCode();
    }
}