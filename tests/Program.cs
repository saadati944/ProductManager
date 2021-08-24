using System;

namespace tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new DataLayer.Database();

            var br = new DataLayer.Repositories.BuyInvoicesRepository(db);
            var sr = new DataLayer.Repositories.SellInvoicesRepository(db);

            var bb = new Business.BuyInvoiceBusiness(db, sr, br);

            var im = bb.GetInvoiceModel(1);
            Console.WriteLine(im.GetPersianDate().ToString());

            Console.WriteLine("Press enter to continue ...");
            Console.ReadLine();
        }
    }


}