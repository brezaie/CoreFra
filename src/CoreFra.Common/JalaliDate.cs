using System;
using System.Globalization;

namespace CoreFra.Common
{
    public class JalaliDate : PersianCalendar
    {
        #region Public Methods

        public string AddDate(string shamsiDate, int gap)
        {
            DateTime M;
            M = AddDays(Convert.ToDateTime(ConvertJalaliToMiladi(shamsiDate)), gap);
            return GetJalaliDate(M, "/");
        }

        public int Gap(string beginDate, string endDate)
        {
            DateTime StartDate, FinishDate;

            StartDate = Convert.ToDateTime(ConvertJalaliToMiladi(beginDate));
            FinishDate = Convert.ToDateTime(ConvertJalaliToMiladi(endDate));

            TimeSpan Diff = FinishDate.Subtract(StartDate);

            return Diff.Days;
        }

        public static int GetLJalaliDiffMonths(DateTime start, DateTime end)
        {
            try
            {
                var diff = 0;
                var s = GetJalaliDate(start);
                var e = GetJalaliDate(end);
                string[] Arr;
                Arr = s.Split('/');
                int startYear = Convert.ToInt16(Arr[0]);
                int startMonth = Convert.ToInt16(Arr[1]);
                Arr = e.Split('/');
                int endYear = Convert.ToInt16(Arr[0]);
                int endMonth = Convert.ToInt16(Arr[1]);
                if (endYear - startYear > 0)
                {
                    diff = (12 - startMonth) + endMonth + (12 * (int)(endYear - startYear));
                }
                else
                {
                    diff = endMonth - startMonth;
                }
                return diff;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool IsValidJalaliDate(String shamsiDate)
        {
            if (string.IsNullOrEmpty(shamsiDate))
                return false;
            string[] Arr;
            Arr = shamsiDate.Split('/');
            int Year = Convert.ToInt16(Arr[0]);
            int Month = Convert.ToInt16(Arr[1]);
            int Day = Convert.ToInt16(Arr[2]);
            bool Result = false;
            //Check Year
            if ((Year < 0) || (Year > 2000))
            {
                //Result = false;
                return false;
            }
            //Check Month
            if ((Month < 0) || (Month > 12))
            {
                //Result = false;
                return false;
            }
            //Check Day
            if ((Day < 0) || (Day > 31))
            {
                // Result = false;
                return false;
            }
            //Check Valid Day With Month
            if (Month < 7)
            {
                //Result = true;
                return true;
            }
            if ((Month < 12) && (Month > 6))
            {
                if (Day > 30)
                {
                    //Result = false;
                    return false;
                }
                else
                {
                    // Result = true;
                    return true;
                }
            }
            if (Month == 12)
            {
                if (Day > 29)
                {
                    //Result = false;
                    return false;
                }
                else
                {
                    // Result = true;
                    return true;
                }
            }
            return Result;
        }

        #endregion

        #region Private Methods

        private string GetStringJalaliDate(DateTime miladyDate)
        {
            String pdate = PersianWeekDayNames[(int)miladyDate.DayOfWeek];
            pdate += "، " + GetDayOfMonth(miladyDate);
            pdate += " " + PersianMonthNames[GetMonth(miladyDate)];
            pdate += " سال " + GetYear(miladyDate);
            return pdate;
        }

        #endregion

        private static readonly String[] PersianMonthNames = new[]
                                                             {
                                                                 "", "فروردين", "ارديبهشت", "خرداد", "تير", "امرداد",
                                                                 "شهريور", "مهر", "آبان", "آذر", "دي", "بهمن", "اسفند"
                                                             };

        internal static readonly String[] PersianWeekDayNames = new[]
                                                               {
                                                                   "يکشنبه", "دوشنبه", "سه شنبه", "چهارشنبه", "پنجشنبه",
                                                                   "جمعه", "شنبه", ""
                                                               };

        public static double DeffrenceTime;

        #region Compare

        public static int Compare(string pt1, string pt2)
        {
            //JalaliDate objDate = new JalaliDate();
            DateTime t1 = Convert.ToDateTime(ConvertJalaliToMiladi(pt1));
            DateTime t2 = Convert.ToDateTime(ConvertJalaliToMiladi(pt2));
            if (t1.Ticks > t2.Ticks)
                return 1;
            if (t1.Ticks < t2.Ticks)
                return -1;

            return 0;
        }

        #endregion

        #region ConvertJalaliToMiladi

        public static DateTime ConvertJalaliToMiladi(string shamsiDate)
        {
            if (string.IsNullOrEmpty(shamsiDate))
            {
                return new DateTime();
            }
            string[] Arr;
            Arr = shamsiDate.Split('/');

            int Year = Convert.ToInt16(Arr[0]);
            int Month = Convert.ToInt16(Arr[1]);
            int Day = Convert.ToInt16(Arr[2]);
            if ((Year > 10) && (Year < 99))
            {
                Year = Year + 1300;
            }
            GregorianCalendar sss = new GregorianCalendar();
            int year = sss.GetYear(new DateTime(Year, Month, Day, new PersianCalendar()));
            int month = sss.GetMonth(new DateTime(Year, Month, Day, new PersianCalendar()));
            int day = sss.GetDayOfMonth(new DateTime(Year, Month, Day, new PersianCalendar()));


            string final_str =
                year + "/" +
                (month < 10 ? "0" + month : month.ToString()) + "/" +
                (day < 10 ? "0" + day : day.ToString());

            DateTime m = Convert.ToDateTime(final_str);

            return m;
        }

        #endregion

        #region GetJalaliDateString

        public static string GetJalaliDateString(string miladyDate)
        {
            if (string.IsNullOrEmpty(miladyDate))
            {
                return string.Empty;
            }
            return GetJalaliDate(Convert.ToDateTime(miladyDate), "/", "rtl");
        }

        #endregion

        #region GetJalaliDateTime

        public static string GetJalaliDateTime(DateTime date)
        {
            JalaliDate obj = new JalaliDate();
            int Day, Month, Year, minute, hour;
            Day = obj.GetDayOfMonth(date);
            Month = obj.GetMonth(date);
            Year = obj.GetYear(date);
            hour = obj.GetHour(date);
            minute = obj.GetMinute(date);
            return string.Format("{0}/{1}/{2} {3}:{4}", Year, Month, Day, hour, minute);
        }

        #endregion

        #region GetPersianFormat

        public static string GetPersianFormat(DateTime mDate)
        {
            JalaliDate obj = new JalaliDate();
            int Day, Year;
            Day = obj.GetDayOfMonth(mDate);
            string Month = PersianMonthNames[obj.GetMonth(mDate)];
            Year = obj.GetYear(mDate);
            return string.Format("{2}    {1}  {0}", Year, Month, Day);
        }

        #endregion

        #region ToPersianNumberString
        public static string ToPersianNumberString(int number, bool isOrdinal)
        {
            return PersianNumberExtension.GetNumberToPersianString(number.ToString());
        }
        //public static String ToPersianNumberString(int number, bool isOrdinal)
        //{
        //    if (number < 1 || 19999 < number) return number.ToString();
        //    if (number == 1 && isOrdinal) return "اول";
        //    String[,] units = new[,]
        //                      {
        //                          {"", "يک", "دو", "سه", "چهار", "پنج", "شـش", "هفت", "هشت", "نه", null, null, null, null, null, null, null, null, null, null, null},
        //                          {"", "ده", "بيست", "سي", "چهل", "پنجاه", "شصت", "هفتاد", "هشتاد", "نود", null, null, null, null, null, null, null, null, null, null, null},
        //                          {"", "صد", "دويست", "سيصد", "چهارصد", "پانصد", "ششصد", "هفتصد", "هشتصد", "نه‌صد", null, null, null, null, null, null, null, null, null, null, null},
        //                          {
        //                              "", "يک هزار", "دوهزار", "سه‌هزار", "چهارهزار", "پنج‌هزار", "شش‌هزار", "هفت‌هزار",
        //                              "هشت‌هزار","نه‌هزار","ده هزار","یازده هزار","دوازده هزار","سیزده هزار","چهارده هزار","پانزده هزار","شانزده هزار",
        //                              "هفده هزار","هجده هزار","نوزده هزار","بیست هزار"
        //                          }
        //                      };
        //    String ordinalSuffix = "م";
        //    if (number % 100 == 30) ordinalSuffix = "‌ام";
        //    String str = "";
        //    for (int i = 3; i >= 0; i--)
        //    {
        //        int p = (int)Math.Pow(10, i);
        //        String s = units[i, (number % (10 * p) - number % p) / p];
        //        if (i == 3 && number.ToString().Length <= 6 && number.ToString().Length >=5)
        //        {
        //            s = units[i, number/p];
        //        }
        //        if (str != "" && s != "") str += " و ";
        //        if (10 < number && number < 20)
        //        {
        //            str +=
        //                new[] { "يازده", "دوازده", "سيزده", "چهارده", "پانزده", "شانزده", "هفده", "هجده", "نوزده" }[
        //                    number - 11];
        //            break;
        //        }
        //        else
        //        {
        //            if (s == "سه" && isOrdinal) s = "سو";
        //            str += s;
        //        }
        //        number %= p;
        //    }
        //    if (isOrdinal)
        //    {
        //        str += ordinalSuffix;
        //    }
        //    return str;
        //}

        #endregion

        #region AddMonths

        public new static DateTime AddMonths(DateTime miladi, int period)
        {
            string str = GetJalaliDate(miladi);
            return AddMonths(str, period);
        }

        public static DateTime AddMonths2(string jalali, int period)
        {
            jalali = jalali.Replace(@"‏", string.Empty);
            DateTime dateTime = ConvertJalaliToMiladi(jalali);
            return dateTime.AddDays(30 * period);
        }

        public static DateTime AddMonths(string jalali, int period)
        {
            jalali = jalali.Replace(@"‏", string.Empty);
            string[] parts = jalali.Split('/');
            if (parts.Length == 3)
            {
                int month = Convert.ToInt32(parts[1]);
                int newMonth = month + period;
                int newYear = Convert.ToInt32(parts[0]);
                if (newMonth <= 0)
                {
                    newYear--;
                    newMonth += 12;
                }
                else if (newMonth > 12)
                {
                    newYear++;
                    newMonth = ((newMonth % 12) == 0) ? 12 : (newMonth % 12);
                }
                if (newMonth > 6 && newMonth < 12 && parts[2] == "31")
                {
                    parts[2] = "30";
                }
                else if (newMonth == 12 && Convert.ToInt32(parts[2]) >= 30)
                {
                    parts[2] = "29";
                }
                string temp = newYear + "/" + newMonth + "/" + parts[2];
                //if(IsValidJalaliDate(temp))
                //{
                //    return ConvertJalaliToMiladi(temp);
                //}
                //else
                //{

                //}

                return ConvertJalaliToMiladi(newYear + "/" + newMonth + "/" + parts[2]);
            }
            return DateTime.Now;
        }

        #endregion

        #region GetDefferntDate

        public static DateTime GetDefferntDate(DateTime beginDate)
        {
            return GetDefferntDate(beginDate, DateTime.Now);
        }

        public static DateTime GetDefferntDate(DateTime beginDate, DateTime endDate)
        {
            long beginTicket = beginDate.Ticks;
            long endTicket = endDate.Ticks;
            long resultTicket = endTicket - beginTicket;
            if (resultTicket > 0)
            {
                DateTime resultDate = new DateTime(resultTicket);
                return resultDate;
            }
            else
            {
                return beginDate;
            }
        }

        #endregion

        #region GetJalaliDate

        #region GetJalaliDate (DateTime)

        public static string GetJalaliDate(DateTime MiladyDate)
        {
            return ("‏" + GetJalaliDate(MiladyDate, "/", "rtl") + "‏").RemoveNoise();
        }

        #endregion

        #region GetJalaliDate (DateTime,string)

        public static string GetJalaliDate(DateTime MiladyDate, string Seprator)
        {
            return GetJalaliDate(MiladyDate, Seprator, "rtl");
        }

        #endregion

        #region GetJalaliDate (string,bool)

        public static string GetJalaliDate(String MiladyDate, bool SmallDate)
        {
            DateTime Date = Convert.ToDateTime(MiladyDate);
            Date = Date.AddHours(DeffrenceTime);
            if (SmallDate)
            {
                return GetJalaliDate(Date, "/");
            }
            else
            {
                JalaliDate obj = new JalaliDate();

                return obj.GetStringJalaliDate(Date);
            }
        }

        #endregion

        #region GetJalaliDate (DateTime,string,string)

        public static string GetJalaliDate(DateTime MiladyDate, string Seprator, string Direction)
        {
            int Day, Month, Year;
            MiladyDate = MiladyDate.AddHours(DeffrenceTime);
            JalaliDate obj = new JalaliDate();

            Day = obj.GetDayOfMonth(MiladyDate);

            Month = obj.GetMonth(MiladyDate);
            Year = obj.GetYear(MiladyDate);
            if (Direction == "ltr")
            {
                return
                    (Day < 10 ? "0" + Day : Day.ToString()) + Seprator +
                    (Month < 10 ? "0" + Month : Month.ToString()) + Seprator + Year;
            }
            else
            {
                return
                    Year + Seprator + (Month < 10 ? "0" + Month : Month.ToString()) + Seprator +
                    (Day < 10 ? "0" + Day : Day.ToString());
            }
        }

        #endregion

        #region GetJalaliDate (DateTime,string,string,bool)

        public static string GetJalaliDate(DateTime MiladyDate, string Seprator, string Direction, bool IsIntelligent)
        {
            int Day, Month, Year;
            MiladyDate = MiladyDate.AddHours(DeffrenceTime);
            JalaliDate obj = new JalaliDate();

            Day = obj.GetDayOfMonth(MiladyDate);
            Month = obj.GetMonth(MiladyDate);
            Year = obj.GetYear(MiladyDate);
            if (IsIntelligent)
            {
                TimeSpan difference = DateTime.Now - MiladyDate;
                if (difference.Days == 0)
                {
                    return "امروز";
                }
                if (difference.Days == 1)
                {
                    return "دیروز";
                }
            }
            if (Direction == "ltr")
            {
                //return Year + Seprator + (Month<10 ? "0" +Month.ToString():Month.ToString()) + Seprator +(Day<10 ? "0" +Day.ToString():Day.ToString());
                return
                    (Day < 10 ? "0" + Day : Day.ToString()) + Seprator +
                    (Month < 10 ? "0" + Month : Month.ToString()) + Seprator + Year;
            }
            else
            {
                return
                    Year + Seprator + (Month < 10 ? "0" + Month : Month.ToString()) + Seprator +
                    (Day < 10 ? "0" + Day : Day.ToString());
            }
        }

        #endregion

        #endregion

        //	public string GetJalaliDate(String MiladyDate)
        //		{
        //			return GetJalaliDate(MiladyDate,false);
        //		}
    }
}