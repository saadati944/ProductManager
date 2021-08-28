using Framework.Interfaces;
using Framework.DataLayer.Models;
using BuyAndSell.DataLayer.Models;
using BuyAndSell.Interfaces;
using BasicData.Interfaces;
using BasicData.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BuyAndSell.Interfaces
{
    public interface IBuyInvoiceBusiness : IInvoiceBusiness
    {
        BuyInvoice FullLoadBuyInvoice(int number, SqlConnection connection = null, SqlTransaction transaction = null);
    }
}
