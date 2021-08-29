using Framework.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace BuyAndSell.DataLayer.Models
{
    public abstract class Invoice : VersionableModel
    {
        public const string NumberColumnName = "Number";
        public const string PartyRefColumnName = "PartyRef";
        public const string UserRefColumnName = "UserRef";
        public const string DateColumnName = "Date";
        public const string TotalPriceColumnName = "TotalPrice";

        public enum InvoiceType
        {
            Selling,
            Buying
        }
        public IEnumerable<InvoiceItem> InvoiceItems { get; set; }

        public int Number { get; set; }
        public int PartyRef { get; set; }
        public Party Party { get; set; }
        public int UserRef { get; set; }
        public User User { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalPrice { get; set; }


        public new bool Included
        {
            get
            {
                return Party != null && User != null;
            }
        }
        public override void Include()
        {
            User = new User { Id = UserRef };
            Party = new Party { Id = PartyRef };

            Load(User);
            Load(Party);
        }

        public override void MapToModel(DataRow row)
        {
            base.MapToModel(row);
            Number = GetField(row, NumberColumnName, Number);
            PartyRef = GetField(row, PartyRefColumnName, PartyRef);
            UserRef = GetField(row, UserRefColumnName, UserRef);
            Date = GetField(row, DateColumnName, Date);
            TotalPrice = GetField(row, TotalPriceColumnName, TotalPrice);
        }

        public void SetPersianDate(int year, int month, int day, int hour = 0, int minute = 0, int second = 0, int millisecond = 0)
        {
            PersianCalendar pc = new PersianCalendar();
            Date = pc.ToDateTime(year, month, day, hour, minute, second, millisecond);
        }
        public PersianDate GetPersianDate()
        {
            PersianCalendar pc = new PersianCalendar();
            return new PersianDate
            {
                Year = pc.GetYear(Date),
                Month = pc.GetMonth(Date),
                Day = pc.GetDayOfMonth(Date),
            };
        }

        public void SetPersianDate(PersianDate pdate)
        {
            PersianCalendar pc = new PersianCalendar();
            Date = pc.ToDateTime(pdate.Year, pdate.Month, pdate.Day, 0, 0, 0, 0);
        }

        public override string[] Columns()
        {
            return new string[] { NumberColumnName, PartyRefColumnName, UserRefColumnName, DateColumnName, TotalPriceColumnName };
        }

        public override string[] GetValues()
        {
            return new string[] { Number.ToString(), PartyRef.ToString(), UserRef.ToString(), Date.ToString("yyyy-MM-dd"), TotalPrice.ToString() };
        }

        public override string TableName()
        {
            return GetInvoiceType() == InvoiceType.Selling ? "SellInvoices" : "BuyInvoices";
        }

        public abstract InvoiceType GetInvoiceType();
    }
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
