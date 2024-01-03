namespace YC.Demo1.Models
{
    public class Sales
    {
        public string stor_id { get; set; } = null;
        public string stor_name { get; set; } = null;
        public string stor_address { get; set; } = null;
        public string ord_num { get; set; } = null;
        public string ord_date { get; set; } = null;
        public int qty { get; set; } = -1;
        public string payterms { get; set; } = null;
        public string title_id { get; set; } = null;
        public string title { get; set; } = null;
        public string type { get; set; } = null;
        public string pub_id { get; set; } = null;
        public float price { get; set; } = -1;
        public float advance { get; set; } = -1;
        public int royalty { get; set; } = -1;
        public int ytd_sales { get; set; } = -1;
        public string notes { get; set; } = null;
        public string pubdate { get; set; } = null;
        public byte[] logo { get; set; } = null;
        public string pr_info { get; set; } = null;
    }

    public class PutSales
    {
        public string title_id { get; set; } = null;
        public string stor_id { get; set; } = null;
        public string ord_num { get; set; } = null;
        public string ord_date { get; set; } = null;
        public int qty { get; set; } = -1;
        public string payterms { get; set; } = null;
    }

    public class DeleteSales
    {
        public string stor_id { get; set; } = null;
        public string ord_num { get; set; } = null;
    }

}