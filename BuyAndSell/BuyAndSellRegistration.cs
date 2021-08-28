using Framework;
using BuyAndSell.Interfaces;
using BuyAndSell.Repositories;
using BuyAndSell.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuyAndSell
{
    public class BuyAndSellRegistration : Framework.Interfaces.IRegistration
    {
        public BuyAndSellRegistration()
        {
            InstanceScanner.Register<IBuyInvoiceBusiness, BuyInvoiceBusiness>();
            InstanceScanner.Register<ISellInvoiceBusiness, SellInvoiceBusiness>();

            InstanceScanner.RegisterSingleton<IBuyInvoicesRepository, BuyInvoicesRepository>();
            InstanceScanner.RegisterSingleton<ISellInvoicesRepository, SellInvoicesRepository>();
        }
    }
}
