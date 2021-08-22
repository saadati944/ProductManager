using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace tests
{
    class Program
    {
        static void Main(string[] args)
        {
            //var y = Tappe.container.Create<Tappe.Data.Repositories.MeasurementUnitsRepository>();
            // var x = Tappe.cccontainer.Create<Tappe.Data.Repositories.ItemsRepository>();
            //y.Update();
            // Console.WriteLine(x.GetItemPrice(2, new DateTime(2020, 01, 04)));

            //x.BeginTransaction();
            //Console.WriteLine(x.CommitTransaction());
            //var c = new Tappe.Data.Models.Category { CategoryID = 3 };
            //x.Load(c);

            //foreach (Tappe.Data.Models.Category c in x.categories)
            //{
            //    c.Name = c.Name.ToLower();
            //    x.Save(c);
            //}


            //x.Save(new Tappe.Data.Models.Attribute { Name = "weight", Description = "test" });

            //new Tappe.Data.Models.Attribute { Id = 2 }.Delete();
            //new Tappe.Data.Models.Category { Id = 2 }.Delete();

            //new Tappe.Data.Models.User { FirstName = "محمد", LastName = "", Age = 30, Gender = true, Password = "1234" }.Save();
            //var user = new Tappe.Data.Models.User { Id = 1 };
            //user.Load();
            //user.Gender = true;
            //System.IO.File.WriteAllText(@"C:\Users\AliSaa\Desktop\user.txt", user.ToString());
            //user.Save();

            // new Tappe.Data.Models.Product { CreatorRef = 1, Name = "‍پفک", Description = "پفک چیتوز موتوری", Price = 7000 }.Save();

            //var prd = new Tappe.Data.Models.Invoice { Id = 1 };
            //prd.Load();
            //prd.Quantity = 250;
            //prd.Save();

            //new Tappe.Data.Models.Invoice { ProductRef = 1, Date = DateTime.Now, Buying = false, Quantity = 100 }.Save();
            //new Tappe.Data.Models.Party { Name = "اصغر فرهادی" }.Save();
            //new Tappe.Data.Models.Party { Name = "احمد مهدوی" }.Save();

            //Tappe.Data.Models.SellInvoice si = new Tappe.Data.Models.SellInvoice();
            //si.Number = 1;
            //si.UserRef = 2;
            //si.PartyRef = 2;
            //si.Date = DateTime.Now;
            //si.Save();

            //var ss = new Tappe.Data.Models.SellInvoice { Id = 1 };
            //ss.Load();
            //foreach (string z in ss.GetValues())
            //    Console.WriteLine(z);

            //var inv = new Tappe.Business.ItemsBusiness();
            //foreach(var x in inv.Items)
            //{
            //    Console.WriteLine(x);
            //}


            Console.ReadLine();
        }
        //private static void show(IEnumerable<Tappe.Data.Models.Model> models)
        //{
        //    var list = models.ToList();
        //    if (list.Count == 0)
        //        return;
        //    Console.WriteLine(list.First().TableName());
        //    foreach (var x in list)
        //        Console.WriteLine(x.Id + " :  "+x);
        //}
    }
}
