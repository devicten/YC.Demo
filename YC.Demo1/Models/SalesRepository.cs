using Dapper;
using System.Data;
using YC.Demo1.Helpers;

namespace YC.Demo1.Models
{
    public interface ISalesRepository
    {
        public Task<(bool IsSuccess, List<Sales> ListSales, List<Stores> ListStores, List<Titles> ListTitles)> GetSales();
        public Task<(bool IsSuccess, int NumOfRowsAffected)> PutSales(PutSales data);
        public Task<(bool IsSuccess, int NumOfRowsAffected)> PatchSales(PutSales data);
        public Task<(bool IsSuccess, int NumOfRowsAffected)> DeleteSales(DeleteSales data);
    }
    public class SalesRepository : ISalesRepository
    {
        private readonly DBHelper _context;
        public SalesRepository(DBHelper context)
        {
            _context = context;
        }
        public async Task<(bool IsSuccess, List<Sales> ListSales, List<Stores> ListStores, List<Titles> ListTitles)> GetSales()
        {
            using (var connection = _context.CreateConnection())
            {
                SqlMapper.GridReader results = await connection.QueryMultipleAsync(
                    @"SELECT sales.[stor_id]
      ,stores.[stor_name]
      ,stores.[stor_address]
      ,sales.[ord_num]
      ,FORMAT(sales.[ord_date],'yyyy-MM-dd') ord_date
      ,sales.[qty]
      ,sales.[payterms]
      ,sales.[title_id]
      ,titles.[title]
      ,titles.[type]
      ,titles.[pub_id]
      ,titles.[price]
      ,titles.[advance]
      ,titles.[royalty]
      ,titles.[ytd_sales]
      ,titles.[notes]
      ,FORMAT(titles.[pubdate],'yyyy-MM-dd') pubdate
      ,pub_info.[logo]
      ,pub_info.[pr_info]
  FROM [pubs].[dbo].[sales] sales
  LEFT JOIN [pubs].[dbo].[stores] stores ON sales.stor_id = stores.stor_id
  LEFT JOIN [pubs].[dbo].[titles] titles ON sales.title_id = titles.title_id
  LEFT JOIN [pubs].[dbo].[pub_info] pub_info ON pub_info.[pub_id] = titles.[pub_id]

  SELECT stores.[stor_id]
      ,stores.[stor_name]
      ,stores.[stor_address]
  FROM [pubs].[dbo].[stores] stores

  SELECT titles.[title_id]
      ,titles.[title]
      ,titles.[type]
      ,titles.[pub_id]
      ,titles.[price]
      ,titles.[advance]
      ,titles.[royalty]
      ,titles.[ytd_sales]
      ,titles.[notes]
      ,FORMAT(titles.[pubdate],'yyyy-MM-dd') pubdate
      ,pub_info.[logo]
      ,pub_info.[pr_info]
  FROM [pubs].[dbo].[titles] titles 
  LEFT JOIN [pubs].[dbo].[pub_info] pub_info ON pub_info.[pub_id] = titles.[pub_id]
  
                ",
                                new {  });
                var ListSales = results.Read<Sales>().ToList();
                var ListStores = results.Read<Stores>().ToList();
                var ListTitles = results.Read<Titles>().ToList();
                return (IsSuccess: true, ListSales: ListSales, ListStores: ListStores, ListTitles: ListTitles);
            }
        }
        public async Task<(bool IsSuccess, int NumOfRowsAffected)> PutSales(PutSales data)
        {
            using (var connection = _context.CreateConnection())
            {
                var NumOfRowsAffected = await connection.ExecuteAsync(
                    @"INSERT INTO [dbo].[sales] ([stor_id] ,[ord_num] ,[ord_date] ,[qty] ,[payterms] ,[title_id]) VALUES  (@StorId ,@OrdNum ,@OrdDate ,@Qty ,@Payterms ,@TitleId)",
                    new List<object>() {
                        new { 
                            StorId = data.stor_id,
                            OrdNum = data.ord_num,
                            OrdDate = data.ord_date,
                            Qty = data.qty,
                            Payterms = data.payterms,
                            TitleId = data.title_id
                        }
                    });
                return (IsSuccess: true, NumOfRowsAffected: NumOfRowsAffected);
            }
        }
        public async Task<(bool IsSuccess, int NumOfRowsAffected)> PatchSales(PutSales data)
        {
            using (var connection = _context.CreateConnection())
            {
                var NumOfRowsAffected = await connection.ExecuteAsync(
                    @"UPDATE [dbo].[sales] SET [ord_date] = @OrdDate, [qty] = @Qty, [payterms] = @Payterms ,[title_id] = @TitleId WHERE [stor_id] = @StorId AND [ord_num] = @OrdNum",
                    new List<object>() {
                        new {
                            StorId = data.stor_id,
                            OrdNum = data.ord_num,
                            OrdDate = data.ord_date,
                            Qty = data.qty,
                            Payterms = data.payterms,
                            TitleId = data.title_id
                        }
                    });
                return (IsSuccess: true, NumOfRowsAffected: NumOfRowsAffected);
            }
        }
        public async Task<(bool IsSuccess, int NumOfRowsAffected)> DeleteSales(DeleteSales data)
        {
            using (var connection = _context.CreateConnection())
            {
                var NumOfRowsAffected = await connection.ExecuteAsync(
                    @"DELETE [dbo].[sales] WHERE [stor_id] = @StorId AND [ord_num] = @OrdNum",
                    new List<object>() {
                        new {
                            StorId = data.stor_id,
                            OrdNum = data.ord_num
                        }
                    });
                return (IsSuccess: true, NumOfRowsAffected: NumOfRowsAffected);
            }
        }
    }
}
