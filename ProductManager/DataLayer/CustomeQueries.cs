namespace Framework.DataLayer
{
    public static class CustomeQueries
    {
        //TODO: move each query to its own layer
        public static string ProductsList = @"SELECT Items.Id AS 'شماره', Items.Name AS 'نام', Items.Description AS 'توضیحات', MeasurementUnits.Name AS 'واحد اندازه گیری', Items.Price AS 'قیمت هر واحد' FROM Items JOIN MeasurementUnits ON MeasurementUnits.Id = Items.MeasurementUnitRef";
        //public static string BuyingInvoices = @"SELECT Invoices.id AS 'شماره فاکتور', Date AS 'تاریخ', TotalPrice AS 'قیمت کل', (SELECT COUNT(*) FROM InvoiceItems WHERE InvoiceRef = Invoices.Id) AS 'انواع موارد خریداری شده', (SELECT SUM(InvoiceItems.Quantity) FROM InvoiceItems WHERE InvoiceRef = Invoices.Id) AS 'تعداد کل موارد خریداری شده' FROM Invoices WHERE Buying = 1";
        //public static string SellingInvoices = @"SELECT Invoices.id AS 'شماره فاکتور', Date AS 'تاریخ', TotalPrice AS 'قیمت کل', (SELECT COUNT(*) FROM InvoiceItems WHERE InvoiceRef = Invoices.Id) AS 'انواع موارد فروخته شده', (SELECT SUM(InvoiceItems.Quantity) FROM InvoiceItems WHERE InvoiceRef = Invoices.Id) AS 'تعداد کل موارد فروخته شده' FROM Invoices WHERE Buying = 0";
        public const string ItemQuantity = @"DECLARE @buy AS int = ISNULL((SELECT SUM(Quantity) FROM BuyInvoiceItems WHERE ItemRef=@ItemId), 0); DECLARE @sell AS int = ISNULL((SELECT SUM(Quantity) FROM SellInvoiceItems WHERE ItemRef=@ItemId), 0); SELECT @buy AS Buy, @sell AS Sell, @Buy - @Sell AS Quantity;"; // input param : ItemId as int
        public const string MaxBuyInvoiceNumber = @"SELECT MAX(Number) AS MaxNum From BuyInvoices;";
        public const string MaxSellInvoiceNumber = @"SELECT MAX(Number) AS MaxNum From SellInvoices;";
        public const string BuyInvoiceTotalPrice = @"SELECT SUM(TotalPrice) AS TotalPrice From BuyInvoices;";
        public const string SellInvoiceTotalPrice = @"SELECT SUM(TotalPrice) AS TotalPrice From SellInvoices;";
        public const string GenerateInvoiceVersion = @"DECLARE @Ver AS INT = ISNULL((SELECT TOP 1 Version FROM InvoiceLocks WHERE InvoiceNumber=@InvoiceNumber AND InvoiceType=@InvoiceType), -1) IF @Ver = -1 BEGIN INSERT INTO InvoiceLocks(InvoiceNumber, InvoiceType, Version) VALUES(@InvoiceNumber, @InvoiceType, 0);"
                                                    + "SELECT 0; END ELSE BEGIN UPDATE InvoiceLocks SET Version=@Ver+1 WHERE InvoiceNumber=@InvoiceNumber AND InvoiceType=@InvoiceType; SELECT @Ver+1; END"; // inputs : @InvoiceNumber, @InvoiceType
        public const string GetInvoiceVersion = @"SELECT ISNULL((SELECT TOP 1 Version FROM InvoiceLocks WHERE InvoiceNumber=@InvoiceNumber AND InvoiceType=@InvoiceType), -1)"; // inputs : @InvoiceNumber, @InvoiceType
    }
}
