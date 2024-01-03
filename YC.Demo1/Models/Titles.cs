namespace YC.Demo1.Models
{
    public class Titles
    {
        public string title_id { get; set; } = null;
        public string title { get; set; } = null;
        public string type { get; set; } = null;
        public string pub_id { get; set; } = null;
        public float price { get; set; } = -1;
        public float advance { get; set; } = -1;
        public int royalty { get; set; } = -1;
        public int ytd_sales { get; set; } = -1;
        public string notes { get; set; } = null;
        public DateTime? pubdate { get; set; } = null;
        public byte[] logo { get; set; } = null;
        public string pr_info { get; set; } = null;
    }
}
