using System;

namespace tests
{
    abstract class c1 { }
    abstract class c2 : c1 { }
    class c3 : c2 { }
    class Program
    {
        static void Main(string[] args)
        {
            //var db = new DataLayer.Database();
            //var t = new DataLayer.Models.Party();
            //t.Name = "testing the party";
            //db.Save(t);
            //Console.WriteLine("Press enter to continue ...");
            //Console.ReadLine();
            //db.Delete(t);

            //var c = db.GetConnection();
            //SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Items;", c);
            //DataTable tbl = new DataTable();
            //adapter.Fill(tbl);
            //foreach(DataRow r in tbl.Rows)
            //{
            //    Console.WriteLine(String.Join("", ((byte[])r.ItemArray.Last()).Select(x=>x.ToString())));
            //}
            //c.Close();
            c1 gl = new c3();

            Console.WriteLine(gl is c1);
            Console.WriteLine(gl is c2);
            Console.WriteLine(gl is c3);

            Console.WriteLine("Press enter to continue ...");
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
