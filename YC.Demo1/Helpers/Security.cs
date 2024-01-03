using System.Security.Cryptography;
using System.Text;
using YC.Demo1.Configs;

namespace YC.Demo1.Helpers
{/// <summary>
 /// SOIC.CORE.Security: 加解密物件
 /// </summary>
    public class Security : ConfigBase, IConfigBase
    {
        /// <summary>預設設定檔</summary>
        public dynamic DefaultConfig =>
        new
        {
            SERVER_KEY = "YC2024",
        };

        /// <summary>多執行緒使用保護:防止物件產生多個</summary>
        private static readonly object Mutex = new object();
        /// <summary>多執行緒使用保護:執行控制</summary>
        private static volatile Security _instance;
        /// <summary>多執行緒使用保護:私有化建構子</summary>
        private Security() : base() { }
        /// <summary>多執行緒使用保護:建立唯一物件</summary>
        public static Security Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Mutex)
                    {
                        if (_instance == null)
                        {
                            _instance = new Security();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary> 金鑰的結構 </summary>
        private struct DESKeyPack
        {
            /// <summary>Key</summary>
            public readonly byte[] Key;
            /// <summary>IV</summary>
            public readonly byte[] IV;
            /// <summary>金鑰的建構子，DES採用64位元的金鑰</summary>
            public DESKeyPack(byte[] data)
            {
                Key = new byte[8];  //DES採用64位元的金鑰，相當於8個byte
                Buffer.BlockCopy(data, 0, Key, 0, 8);

                IV = new byte[8];
                Buffer.BlockCopy(data, 8, IV, 0, 8);
            }
        }
        /// <summary>
        /// 依指定公鑰，加上系統私鑰，產生加密物件。
        /// </summary>
        /// <param name="keyString">string:指定公鑰</param>
        /// <returns>DESKeyPack:加密物件</returns>
        private DESKeyPack GenKeyPack(string keyString)
        {
            byte[] data = default(byte[]);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                //將使用者輸入的金鑰與程式的金鑰混在一起產生加密的金鑰
                data = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(keyString + LSYS.Config.Security.SERVER_KEY)).Clone() as byte[];
            }
            DESKeyPack dkp = new DESKeyPack(data);
            return dkp;
        }

        /// <summary> 加密 </summary>
        /// <param name="RawString">string:指定字串</param>
        /// <param name="KeyString">指定密碼</param>
        /// <returns>加密字串</returns>
        public string Encrypt(string RawString, string KeyString = null)
        {
            if (string.IsNullOrWhiteSpace(KeyString))
                KeyString = LSYS.Config.Security.SERVER_KEY;
            DEV.LOG("Encrypt(string RawString:" + RawString + ")");
            string Result = string.Empty;
            try
            {
                DESKeyPack dkp = GenKeyPack(KeyString);
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                using (ICryptoTransform trans = des.CreateEncryptor(dkp.Key, dkp.IV))
                {
                    MemoryStream ms = new MemoryStream();
                    using (CryptoStream cs = new CryptoStream(ms, trans, CryptoStreamMode.Write))
                    {
                        byte[] rawData = Encoding.UTF8.GetBytes(RawString);
                        cs.Write(rawData, 0, rawData.Length);
                        cs.FlushFinalBlock();
                        Result = Convert.ToBase64String(ms.ToArray());
                    }
                }
                return Result;
            }
            catch (Exception E)
            {
                DEV.ERROR(E);
                return Result;
            }
        }

        /// <summary> 解密 </summary>
        /// <param name="EncryptString">指定加密字串</param>
        /// <param name="KeyString">指定密碼</param>
        /// <returns>解密字串</returns>
        public string Decrypt(string EncryptString, string KeyString = null)
        {
            if (string.IsNullOrWhiteSpace(KeyString))
                KeyString = LSYS.Config.Security.SERVER_KEY;
            DEV.LOG("Decrypt(string EncryptString:" + EncryptString + ", string KeyString:" + KeyString + ")");

            string Result = string.Empty;
            try
            {
                DESKeyPack dkp = GenKeyPack(KeyString);
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                using (ICryptoTransform trans = des.CreateDecryptor(dkp.Key, dkp.IV))
                {
                    MemoryStream ms = new MemoryStream();
                    using (CryptoStream cs = new CryptoStream(ms, trans, CryptoStreamMode.Write))
                    {
                        byte[] rawData = Convert.FromBase64String(EncryptString);// Encoding.UTF8.GetString(Convert.FromBase64String(EncryptString));
                        cs.Write(rawData, 0, rawData.Length);
                        cs.FlushFinalBlock();
                        Result = Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
                return Result;
            }
            catch (Exception E)
            {
                DEV.ERROR(E);
                return Result;
            }
        }
    }
}
