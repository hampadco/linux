using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class DateUtility
    {
        public static readonly long TimeOffset = (long)((DateTimeOffset)DateTime.Now).Offset.TotalMilliseconds;
        public static string GetPersianDate(DateTime dt)
        {
            return ToPersianDate(dt);
        }
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
        public static DateTime ShamsiToMilady(string PersianDate)
        {
            try
            {
                PersianCalendar PersianCalendar = new PersianCalendar();
                char[] Seperate = new char[] { '/', ' ', ':' };


                string[] ShamsiDate = PersianDate.Split(Seperate);

                int hour = 0;
                int minute = 0;
                int second = 0;
                int PersianMonth = Convert.ToInt32(ShamsiDate[1]);
                int PersianDay = Convert.ToInt32(ShamsiDate[2]);
                Int32 PersianYear = Convert.ToInt32(ShamsiDate[0]);
                if (ShamsiDate.Length > 3)
                {
                    hour = Convert.ToInt32(ShamsiDate[3]);
                    minute = Convert.ToInt32(ShamsiDate[4]);
                    second = Convert.ToInt32(ShamsiDate[5]);
                }
                DateTime DateTime = PersianCalendar.ToDateTime(PersianYear, PersianMonth, PersianDay, hour, minute, second, 0);
                return DateTime;
            }
            catch
            {
                throw new Exception("تاریخ را صحیح وارد نمایید");
            }
        }
        public static string MiladyToShamsiDateOnly(DateTime Date)
        {
            PersianCalendar PersianCalander = new PersianCalendar();
            const int HourZone = 3;//////////Tehran
            const int MinuteZone = 30;////////Tehran Time Zone
            Date = Date.AddHours(HourZone);
            Date = Date.AddMinutes(MinuteZone);
            int Year = PersianCalander.GetYear(Date);
            int Month = PersianCalander.GetMonth(Date);
            int Day = PersianCalander.GetDayOfMonth(Date);
            int Hour = PersianCalander.GetHour(Date);
            int Minute = PersianCalander.GetMinute(Date);
            int Second = PersianCalander.GetSecond(Date);
            return Year.ToString() + "/" + (Month < 10 ? 0 + Month.ToString() : Month.ToString()) + "/" + (Day < 10 ? "0" + Day.ToString() : Day.ToString());
        }
        public static DateTime ShamsiToMiladi(string shamsi)
        {
            PersianCalendar p = new PersianCalendar();

            if (shamsi.IndexOf('/') == -1)
                return p.ToDateTime(Convert.ToInt32(shamsi.Substring(0, 4)), Convert.ToInt32(shamsi.Substring(4, 2)), Convert.ToInt32(shamsi.Substring(6, 2)), 0, 0, 0, 0);
            else
            {
                var split = shamsi.Split('/');
                return p.ToDateTime(Convert.ToInt32(split[0]), Convert.ToInt32(split[1]), Convert.ToInt32(split[2]), 0, 0, 0, 0);
            }
        }
        public static string GetPersainDay(int dayOfWeek)
        {
            Dictionary<string, string[]> DayOfWeeks = new Dictionary<string, string[]>();
            DayOfWeeks.Add("en", new string[] { "Saturday", "Sunday", "Monday", "Tuesday", "Thursday", "Wednesday", "Friday" });
            DayOfWeeks.Add("fa", new string[] { "یک شنبه", "دوشنبه", "سه شنبه", "چهارشنبه", "پنج شنبه", "جمعه", "شنبه" });
            return DayOfWeeks["fa"][dayOfWeek];
        }
        public static string GetPersainMonth(int month)
        {
            Dictionary<string, string[]> DayOfWeeks = new Dictionary<string, string[]>();
            DayOfWeeks.Add("fa", new string[] { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" });
            return DayOfWeeks["fa"][(int)month - 1];
        }
        public static string GetPersianDate()
        {
            PersianCalendar PersianCalendar1 = new PersianCalendar();
            var dt = DateTime.Now;
            return string.Format(@"{0} {1} {2} {3}",
                         GetPersainDay((int)PersianCalendar1.GetDayOfWeek(dt)),
                         PersianCalendar1.GetDayOfMonth(dt),
                         GetPersainMonth((int)PersianCalendar1.GetMonth(dt)),
                         PersianCalendar1.GetYear(dt));
        }
        public static string GetEnglishDate()
        {
            return DateTime.Now.ToString("yyyy/MM/dd MMMM");
        }
        public static string ToPersianDate(DateTime date)
        {
            PersianCalendar calander = new PersianCalendar();

            return string.Format("{0}/{1}/{2}", calander.GetYear(date).ToString("####"),
               calander.GetMonth(date).ToString().Length < 2 ? "0" + calander.GetMonth(date).ToString() : calander.GetMonth(date).ToString(),
               calander.GetDayOfMonth(date).ToString().Length < 2 ? "0" + calander.GetDayOfMonth(date).ToString() : calander.GetDayOfMonth(date).ToString());
        }
        public static string GetPersianYear(DateTime date)
        {
            PersianCalendar calander = new PersianCalendar();

            return string.Format("{0}", calander.GetYear(date).ToString("####"));
        }
        public static string ToPersianDateTimeString(DateTime date)
        {
            PersianCalendar calander = new PersianCalendar();

            return string.Format("{0}/{1}/{2} {3}:{4}:{5}", calander.GetYear(date).ToString("####"),
               calander.GetMonth(date).ToString().Length < 2 ? "0" + calander.GetMonth(date).ToString() : calander.GetMonth(date).ToString(),
               calander.GetDayOfMonth(date).ToString().Length < 2 ? "0" + calander.GetDayOfMonth(date).ToString() : calander.GetDayOfMonth(date).ToString(),
               date.Hour.ToString().Length < 2 ? "0" + date.Hour.ToString() : date.Hour.ToString(),
               date.Minute.ToString().Length < 2 ? "0" + date.Minute.ToString() : date.Minute.ToString(),
               date.Second.ToString().Length < 2 ? "0" + date.Second.ToString() : date.Second.ToString());
        }
        public static string GetShamsiMonthName(DateTime Date)
        {
            var month = GetAllShamsiMonth().FirstOrDefault(p => p.Key == MiladyToShamsiGetMonth(Date));
            return month.Value;
        }
        public static Dictionary<int, string> GetAllShamsiMonth()
        {
            return new Dictionary<int, string> {{1, "فروردین"},{2, "اردیبهشت"},{3, "خرداد"},
                                           {4,  "تیر"},{5, "مرداد"},{6, "شهریور"},
                                           {7,  "مهر"},{8, "آبان"},{9, "آذر"},
                                           {10, "دی"},{11, "بهمن"},{12, "اسفند"}};
        }
        public static int MiladyToShamsiGetMonth(DateTime Date)
        {
            PersianCalendar p = new PersianCalendar();
            const int HourZone = 3;//////////Tehran
            const int MinuteZone = 30;////////Tehran Time Zone
            Date = Date.AddHours(HourZone);
            Date = Date.AddMinutes(MinuteZone);
            int Month = p.GetMonth(Date);
            return Month;
        }
        public static string GetMiladiWithoutCalture(DateTime dt)
        {
            return dt.Year + "/" + dt.Month + "/" + dt.Day + " " + dt.TimeOfDay.ToString();
        }
        public static string GetFirstEachMonth(DateTime dt)
        {
            PersianCalendar PersianCalander1 = new PersianCalendar();
            int Month = PersianCalander1.GetMonth(dt);
            int ShamsiYear = PersianCalander1.GetYear(dt);
            switch (Month)
            {
                case 1:
                    return ShamsiYear + "/01/01";
                case 2:
                    return ShamsiYear + "/02/01";
                case 3:
                    return ShamsiYear + "/03/01";
                case 4:
                    return ShamsiYear + "/04/01";
                case 5:
                    return ShamsiYear + "/05/01";
                case 6:
                    return ShamsiYear + "/06/01";
                case 7:
                    return ShamsiYear + "/07/01";
                case 8:
                    return ShamsiYear + "/08/01";
                case 9:
                    return ShamsiYear + "/09/01";
                case 10:
                    return ShamsiYear + "/10/01";
                case 11:
                    return ShamsiYear + "/11/01";
                case 12:
                    return ShamsiYear + "/12/01";
                default:
                    return ShamsiYear + "/12/01";
            }

        }
        public static Tuple<DateTime, DateTime> GetWithMonth(int ShamsiYear, int Month)
        {
            PersianCalendar PersianCalander1 = new PersianCalendar();
            string Start = $"{ShamsiYear}/{Month}/01";
            string End = $"{ShamsiYear}/{Month}/{PersianCalander1.GetDaysInMonth(ShamsiYear, Month)}";
            return new Tuple<DateTime, DateTime>(ShamsiToMiladi(Start), ShamsiToMiladi(End));
        }
        public static string GetFriendlyNameDateString(DateTime dt)
        {
            string result = "";
            var year = GetPersianYear(dt);
            var monthName = GetPersainMonth(MiladyToShamsiGetMonth(dt));
            result = $"{monthName} ماه سال {year}";
            return result;
        }
        public static void GetPersianDate(DateTime dt, out int shamsiYear, out int shamsiMonth, out int shamsiDay)
        {
            var persianDate = DateUtility.ToPersianDate(dt).Split('/');
            shamsiYear = Convert.ToInt32(persianDate[0]);
            shamsiMonth = Convert.ToInt32(persianDate[1]);
            shamsiDay = Convert.ToInt32(persianDate[2]);
        }
    }
}
