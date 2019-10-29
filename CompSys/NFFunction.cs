namespace Serpen.Uni.CompSys {
    public class NFFunction {
        public enum eNFForm { KF, DF }

        public enum ValType { Positive, Negative, Unset }

        ValType[,] Array;
        public readonly eNFForm Form;

        public NFFunction(ValType[,] array, eNFForm form) {
            Array = array;
        }

        public bool Invoke(params bool[] values) {
            bool totalresult;
            if (Form == eNFForm.DF)
                totalresult = false;
            else //if (Form == eNFForm.KF)
                totalresult = true;

            for (int i = 0; i < Array.GetLength(0); i++) {
                bool rowresult;
                if (Form == eNFForm.DF)
                    rowresult = true;
                else //if (Form == eNFForm.KF)
                    rowresult = false;

                for (int j = 0; j < Array.GetLength(1); j++) {
                    if (Form == eNFForm.DF) {
                        if (Array[i, j] == ValType.Positive)
                            rowresult &= values[i];
                        else if (Array[i, j] == ValType.Negative)
                            rowresult &= !values[i];
                        else if (Array[i, j] == ValType.Unset) {
                            // nothing to do!
                        }
                    } else { //if (Form == eNFForm.KF)
                        if (Array[i, j] == ValType.Positive)
                            rowresult |= values[i];
                        else if (Array[i, j] == ValType.Negative)
                            rowresult |= !values[i];
                        else if (Array[i, j] == ValType.Unset) {
                            // nothing to do!
                        }
                    }
                } //next j
                if (Form == eNFForm.DF)
                    totalresult |= rowresult;
                else //if (Form == eNFForm.KF)
                    totalresult &= rowresult ;
            } //next i
            return totalresult;
        } //end Invoke

        public static NFFunction Sample {
            get {
                return null;
            }
        }
    }
}