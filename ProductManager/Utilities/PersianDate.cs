using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Utilities
{
    public class PersianDate
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }

        public PersianDate() { }

        public PersianDate(int year, int month, int day)
        {
            Year = year;
            Month = month;
            Day = day;
        }

        public override string ToString()
        {
            return String.Format("{0:0000}/{1:00}/{2:00}", Year, Month, Day);
        }

        public static string PersianDateStringFromDateTime(DateTime dt)
        {
            PersianCalendar pc = new PersianCalendar();
            return String.Format("{0:0000}/{1:00}/{2:00}", pc.GetYear(dt), pc.GetMonth(dt), pc.GetDayOfMonth(dt));
        }
        public static bool DateTimeFromPersianDateString(string persianDate, out DateTime dateTime)
        {
            dateTime = DateTime.Now;
            if (persianDate.Length != 10)
                return false;
            for (int i = 0; i < 10; i++)
            {
                if (i == 4 || i == 7)
                {
                    if (persianDate[i] != '/')
                        return false;
                }
                else if (!Char.IsDigit(persianDate[i]))
                    return false;
            }

            int year = int.Parse(persianDate.Substring(0, 4));
            int month = int.Parse(persianDate.Substring(5, 2));
            int day = int.Parse(persianDate.Substring(8, 2));

            PersianCalendar pc = new PersianCalendar();
            try
            {
                dateTime = pc.ToDateTime(year, month, day, 0, 0, 0, 0);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
