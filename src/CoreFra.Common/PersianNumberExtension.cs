using System;

namespace CoreFra.Common
{
    public static class PersianNumberExtension
    {
        public static string GetNumberToPersianString(string TXT)
        {
            string RET = " ", STRVA = " ";
            string[] MainStr = StrToInt(TXT);
            int Q = 0;
            for (int i = MainStr.Length - 1; i >= 0; i--)
            {
                STRVA = " ";
                if (RET != " " && RET != null && i != 1)
                    STRVA = " و ";
                RET = ConvertStr(GetCountStr(MainStr[i]), Q) + STRVA + RET;


                Q++;
            }
            if (RET == " " || RET == null || RET == "  ")
                RET = "صفر";
            return RET;
        }

        private static string[] StrToInt(string STR)
        {
            STR = GetCountStr(STR);
            string[] RET = new string[STR.Length / 3];
            int Q = 0;
            for (int I = 0; I < STR.Length; I += 3)
            {
                RET[Q] = STR.Substring(I, 3);
                Q++;
            }
            return RET;
        }

        private static string GetCountStr(string STR)
        {
            string RET = STR;
            int LEN = (STR.Length / 3 + 1) * 3 - STR.Length;
            if (LEN < 3)
            {
                for (int i = 0; i < LEN; i++)
                {
                    RET = "0" + RET;
                }
            }
            if (RET == "")
                return "000";
            return RET;
        }

        private static string ConvertStr(string INT, int Count)
        {
            string RET = "";
            //یک صد
            if (Count == 0)
            {
                if (INT.Substring(1, 1) == "1" && INT.Substring(2, 1) != "0")
                {
                    RET = GetNumber(3, Convert.ToInt32(INT.Substring(0, 1)), " ") + GetNumber(1, Convert.ToInt32(INT.Substring(2, 1)), "");
                }
                else
                {
                    string STR = GetNumber(0, Convert.ToInt32(INT.Substring(2, 1)), "");
                    RET = GetNumber(3, Convert.ToInt32(INT.Substring(0, 1)), GetNumber(2, Convert.ToInt32(INT.Substring(1, 1)), "") + STR) + GetNumber(2, Convert.ToInt32(INT.Substring(1, 1)), STR) + GetNumber(0, Convert.ToInt32(INT.Substring(2, 1)), "");
                }
            }
            //هزار
            else if (Count == 1)
            {
                RET = ConvertStr(INT, 0);
                RET += " هزار";
            }
            //میلیون
            else if (Count == 2)
            {
                RET = ConvertStr(INT, 0);
                RET += " میلیون";
            }
            //میلیارد
            else if (Count == 3)
            {
                RET = ConvertStr(INT, 0);
                RET += " میلیارد";
            }
            //میلیارد
            else if (Count == 4)
            {
                RET = ConvertStr(INT, 0);
                RET += " تیلیارد";
            }
            //میلیارد
            else if (Count == 5)
            {
                RET = ConvertStr(INT, 0);
                RET += " بیلیارد";
            }
            else
            {
                RET = ConvertStr(INT, 0);
                RET += Count.ToString();
            }
            return RET;
        }

        private static string GetNumber(int Count, int Number, string VA)
        {
            string RET = "";

            if (VA != "" && VA != null)
            {
                VA = " و ";
            }
            if (Count == 0 || Count == 1)
            {
                bool IsDah = Convert.ToBoolean(Count);
                string[] MySTR = new string[10];
                MySTR[1] = IsDah ? "یازده" : "یک" + VA;
                MySTR[2] = IsDah ? "دوازده" : "دو" + VA;
                MySTR[3] = IsDah ? "سیزده" : "سه" + VA;
                MySTR[4] = IsDah ? "چهارده" : "چهار" + VA;
                MySTR[5] = IsDah ? "پانزده" : "پنج" + VA;
                MySTR[6] = IsDah ? "شانزده" : "شش" + VA;
                MySTR[7] = IsDah ? "هفده" : "هفت" + VA;
                MySTR[8] = IsDah ? "هجده" : "هشت" + VA;
                MySTR[9] = IsDah ? "نوزده" : "نه" + VA;
                return MySTR[Number];
            }
            else if (Count == 2)
            {
                string[] MySTR = new string[10];
                MySTR[1] = "ده";
                MySTR[2] = "بیست" + VA;
                MySTR[3] = "سی" + VA;
                MySTR[4] = "چهل" + VA;
                MySTR[5] = "پنجاه" + VA;
                MySTR[6] = "شصت" + VA;
                MySTR[7] = "هفتاد" + VA;
                MySTR[8] = "هشتاد" + VA;
                MySTR[9] = "نود" + VA;
                return MySTR[Number];
            }
            else if (Count == 3)
            {
                string[] MySTR = new string[10];
                MySTR[1] = "یکصد" + VA;
                MySTR[2] = "دویست" + VA;
                MySTR[3] = "سیصد" + VA;
                MySTR[4] = "چهارصد" + VA;
                MySTR[5] = "پانصد" + VA;
                MySTR[6] = "ششصد" + VA;
                MySTR[7] = "هفتصد" + VA;
                MySTR[8] = "هشتصد" + VA;
                MySTR[9] = "نهصد" + VA;
                return MySTR[Number];
            }
            return RET;
        }

        public static string PersianNumberToLatin(this string number)
        {
            return number.Replace("۰", "0").Replace("۱", "1").Replace("۲", "2").Replace("۳", "3").Replace("۴", "4").Replace("۵", "5").Replace("۶", "6").Replace("۷", "7").Replace("۸", "8").Replace("۹", "9");
        }
    }
}