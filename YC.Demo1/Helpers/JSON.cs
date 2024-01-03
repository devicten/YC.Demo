using YC.Demo1.Configs;

namespace YC.Demo1.Helpers
{
    public class JSON : BaseClass
    {
        /// <summary>多執行緒使用保護:私有化建構子</summary>
        private JSON() : base() { }

        /// <summary> 將指定物件轉為JSON字串，且格式化。 </summary>
        /// <param name="JsonObject">指定JSON物件</param>
        /// <param name="JsonString">返回JSON字串</param>
        /// <param name="ERROR_MESSAGE">返回錯誤訊息</param>
        /// <param name="format">是否格式化</param>
        /// <returns>成功與否</returns>
        public static bool Serialize(object JsonObject, out string JsonString, out string ERROR_MESSAGE, Newtonsoft.Json.Formatting format = Newtonsoft.Json.Formatting.None)
        {
            //DEV.LOG("Serialize(object JsonObject:" + JsonObject.Serialize() + ", out string ERROR_MESSAGE)");

            JsonString = string.Empty;
            ERROR_MESSAGE = string.Empty;
            try
            {
                JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(JsonObject, format);
                //JavaScriptSerializer serializer = new JavaScriptSerializer();
                //JsonString = serializer.Serialize(JsonObject);
                return true;
            }
            catch (Exception E)
            {
                ERROR_MESSAGE = E.Message;
                //DEV.ERROR(E);
                return false;
            }
        }

        /// <summary> 將指定JSON字串反轉為指定型別 </summary>
        /// <typeparam name="T">指定型別</typeparam>
        /// <param name="JsonString">指定JSON字串</param>
        /// <param name="JsonObject">返回指定型別物件</param>
        /// <param name="ERROR_MESSAGE">返回錯誤訊息</param>
        /// <returns>成功與否</returns>
        public static bool Deserialize<T>(string JsonString, out T JsonObject, out string ERROR_MESSAGE)
        {
            //DEV.LOG("Deserialize<T>(string JsonString:" + JsonString + ", out string ERROR_MESSAGE)");

            ERROR_MESSAGE = string.Empty;
            JsonObject = default(T);
            try
            {
                JsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(JsonString);
                //JavaScriptSerializer jss = new JavaScriptSerializer();
                //JsonObject = jss.Deserialize<T>(JsonString);
                return true;
            }
            catch (Exception E)
            {
                ERROR_MESSAGE = E.Message;
                //DEV.ERROR(E);
                return false;
            }
        }
    }
}
