namespace YC.Demo1.Helpers
{
    /// <summary>
    /// 系統終控台
    /// </summary>
    public static class LSYS
    {
        /// <summary>全域設定檔</summary>
        public static dynamic Config = YC.Demo1.Configs.CFG.Instance.xml;
        /// <summary>>全域安全物件</summary>
        public static Security Security = YC.Demo1.Helpers.Security.Instance;
        /// <summary>全域資料庫物件</summary>
        //public static SOIC.Database.DB DB = default(SOIC.Database.DB);
    }
}
