namespace YC.Demo1.Helpers
{
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;
    using System.Threading;
    using System.Xml.Serialization;


    /// <summary>擴充工具</summary>
    public class Extensions { }

    /// <summary>Directory物件擴充</summary>
    public static class DirectoryExtensions
    {
        [SuppressUnmanagedCodeSecurityAttribute]
        internal static class SafeNativeMethods
        {
            internal static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            internal struct WIN32_FIND_DATA
            {
                public uint dwFileAttributes;
                public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
                public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
                public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
                public uint nFileSizeHigh;
                public uint nFileSizeLow;
                public uint dwReserved0;
                public uint dwReserved1;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
                public string cFileName;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
                public string cAlternateFileName;
            }

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
            [return: MarshalAs(UnmanagedType.SysInt)]
            internal static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            internal static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);

            [DllImport("kernel32.dll")]
            internal static extern bool FindClose(IntPtr hFindFile);
        }

        /// <summary>
        /// 確定目錄是否為空
        /// </summary>
        /// <param name="path">目標路徑</param>
        /// <returns>是否為空</returns>
        public static bool CheckDirectoryEmpty(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(path);
            }

            if (Directory.Exists(path))
            {
                if (path.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    path += "*";
                else
                    path += Path.DirectorySeparatorChar + "*";

                var findHandle = SafeNativeMethods.FindFirstFile(path, out SafeNativeMethods.WIN32_FIND_DATA findData);

                if (findHandle != SafeNativeMethods.INVALID_HANDLE_VALUE)
                {
                    try
                    {
                        bool empty = true;
                        do
                        {
                            if (findData.cFileName != "." && findData.cFileName != "..")
                                empty = false;
                        } while (empty && SafeNativeMethods.FindNextFile(findHandle, out findData));

                        return empty;
                    }
                    finally
                    {
                        SafeNativeMethods.FindClose(findHandle);
                    }
                }

                throw new Exception("Failed to get directory first file",
                    Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
            }
            throw new DirectoryNotFoundException();
        }

        /// <summary>
        /// 取得所有子目錄
        /// </summary>
        /// <param name="path">目標路徑</param>
        /// <returns>是否為空</returns>
        public static IEnumerable<string> ReadSubfoldersAlternate(string path)
        {
            return Directory.EnumerateDirectories(path);
        }

        /// <summary>
        /// 是否存在子目錄
        /// </summary>
        /// <param name="path">目標路徑</param>
        /// <returns>是否存在</returns>
        public static bool HasSubfoldersAlternate(string path)
        {
            IEnumerable<string> subfolders = Directory.EnumerateDirectories(path);
            return subfolders != null && subfolders.Any();
        }

        /// <summary>
        /// (呼叫kernel32.dll)是否存在子目錄或檔案
        /// </summary>
        /// <param name="path">目標路徑</param>
        /// <returns>是否存在子目錄或檔案</returns>
        public static bool HasSubfoldersOrFilesAlternate(string path)
        {
            IEnumerable<string> subfolders = Directory.EnumerateDirectories(path);
            IEnumerable<string> subfiles = Directory.EnumerateFiles(path);
            return (subfolders != null && subfolders.Any())
                    || (subfiles != null && subfiles.Any());
        }
    }

    /// <summary>動態物件擴充</summary>
    public static class DynamicHelper
    {
        /// <summary>
        /// 轉成ExpandObject
        /// </summary>
        /// <param name="obj">目標物件</param>
        /// <returns>轉換結果</returns>
        public static ExpandoObject ToExpandObject(this object obj)
        {
            //Get Properties Using Reflections
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            PropertyInfo[] properties = obj.GetType().GetProperties(flags);

            //Add Them to a new Expando
            ExpandoObject expando = new ExpandoObject();
            foreach (PropertyInfo property in properties)
            {
                AddProperty(expando, property.Name, property.GetValue(obj, null));
            }
            return expando;
        }

        /// <summary>
        /// ExpandObject加入屬性
        /// </summary>
        /// <param name="expando">目標物件</param>
        /// <param name="propertyName">屬性名稱</param>
        /// <param name="propertyValue">屬性內容</param>
        public static void AddProperty(this ExpandoObject expando, string propertyName, dynamic propertyValue)
        {
            var expandoDict = expando as IDictionary<string, dynamic>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }

        /// <summary>
        /// ExpandObject確認屬性
        /// </summary>
        /// <param name="expando">目標物件</param>
        /// <param name="propertyName">屬性名稱</param>
        /// <returns>存在與否</returns>
        public static bool ExistProperty(this ExpandoObject expando, string propertyName)
        {
            var expandoDict = expando as IDictionary<string, dynamic>;
            return expandoDict.ContainsKey(propertyName);
        }
    }

    /// <summary>
    /// SOIC.CORE.StringExtensions 字串擴充
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 檢查字串是否為JSON格式(不返回錯誤)
        /// </summary>
        /// <param name="Input">擴充對象:string</param>
        /// <returns>bool:是否為JSON</returns>
        public static bool IsJson(this string Input)
        {
            try
            {
                //new JavaScriptSerializer().DeserializeObject(Input);
                return JSON.Deserialize<object>(Input, out _, out _);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 檢查字串是否為JSON格式(返回錯誤)
        /// </summary>
        /// <param name="Input">擴充對象:string</param>
        /// <param name="ERROR_MESSAGE">string:錯誤訊息</param>
        /// <returns>bool:是否為JSON</returns>
        public static bool IsJson(this string Input, out string ERROR_MESSAGE)
        {
            ERROR_MESSAGE = string.Empty;
            try
            {
                //new JavaScriptSerializer().DeserializeObject(Input);
                return JSON.Deserialize<object>(Input, out _, out ERROR_MESSAGE);
            }
            catch (Exception E)
            {
                ERROR_MESSAGE = E.Message;
                return false;
            }
        }
    }

    /// <summary>
    /// SOIC.CORE.ObjectExtensions 物件相關擴充
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// 傾印物件
        /// </summary>
        /// <param name="obj">目標物件</param>
        public static void DUMP(this object obj)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
        }

        /// <summary>
        /// 取得物件所有屬性名稱
        /// </summary>
        /// <param name="pObject">目標物件</param>
        /// <returns>屬性名稱陣列</returns>
        public static List<string> GetPropertiesNameOfClass(this object pObject)
        {
            List<string> propertyList = new List<string>();
            if (pObject != null)
            {
                foreach (var prop in pObject.GetType().GetProperties())
                {
                    propertyList.Add(prop.Name);
                }
            }
            return propertyList;
        }

        /// <summary>
        /// 序列化物件
        /// </summary>
        /// <param name="obj">擴充對象:object</param>
        /// <returns>string:物件序列化字串</returns>
        public static string Serialize(this object obj)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
                using (StringWriter textWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(textWriter, obj);
                    return textWriter.ToString();
                }
            }
            catch { return obj.GetType().FullName; }
        }

        /// <summary>
        /// JSON化物件
        /// </summary>
        /// <param name="obj">擴充對象:object</param>
        /// <returns>string:JSON字串</returns>
        public static string ToJSON(this object obj)
        {
            try
            {
                string JsonString = string.Empty;
                string ERROR_MESSAGE = string.Empty;
                if (JSON.Serialize(obj, out JsonString, out ERROR_MESSAGE) == false)
                    throw new Exception(ERROR_MESSAGE);
                return JsonString;
            }
            catch (Exception) { throw; }
        }
    }

    /// <summary>
    /// SOIC.CORE.DataTableExtension DataTable相關擴充
    /// </summary>
    public static class DataTableExtension
    {
        /* DataTable To List<ApprovalMenu>
         * 單例型轉換
         *   _LIST_APPROVAL_MENU = oDS.Tables[1].AsEnumerable().Select(m => new ApprovalMenu()
         *   {
         *       MENU_ID = m.Field<int>("MENU_ID"),
         *       MENU_TYPE = m.Field<int>("MENU_TYPE"),
         *       MENU_NAME = m.Field<string>("MENU_NAME"),
         *       MENU_URL = m.Field<string>("MENU_URL"),
         *       MENU_SORT = m.Field<int>("MENU_SORT")
         *   }).ToList();
        */
        /// <summary>
        /// 泛用型轉換 DataTable To List
        /// </summary>
        /// <typeparam name="T">擴充對像泛型:T</typeparam>
        /// <param name="dt">擴充對象:DataTable</param>
        /// <returns>結果陣列</returns>
        public static List<T> ToList<T>(this DataTable dt) where T : new()
        {
            List<T> Temp = new List<T>();
            try
            {
                var columnNames = dt.Columns.Cast<DataColumn>()
                .Select(c => c.ColumnName)
                .ToList();
                return dt.AsEnumerable().Select(row =>
                {
                    T model = new T();
                    model.GetType().GetProperties();
                    var ps = model.GetType().GetFields();
                    foreach (var property in ps)
                    {
                        if (columnNames.Contains(property.Name))
                        {
                            var value = row[property.Name] == DBNull.Value ? null : Convert.ChangeType(row[property.Name], property.FieldType);
                            property.SetValue(model, value);
                        }
                    }
                    return model;
                }).ToList();
            }
            catch (Exception) { throw; }
        }
        /// <summary>
        /// 將DataTable轉成List Dictionary
        /// </summary>
        /// <param name="dt">目標物件</param>
        /// <returns>結果陣列</returns>
        public static List<Dictionary<string, dynamic>> ToListDictionary(this DataTable dt)
        {
            List<Dictionary<string, dynamic>> Temp = new List<Dictionary<string, dynamic>>();
            try
            {
                return dt.AsEnumerable().Select(row =>
                {
                    Dictionary<string, dynamic> model = new Dictionary<string, dynamic>();
                    foreach (DataColumn oDC in dt.Columns)
                    {
                        if (row[oDC.ColumnName] is DBNull)
                            model.Add(oDC.ColumnName, string.Empty);
                        else
                            model.Add(oDC.ColumnName, Convert.ChangeType(row[oDC.ColumnName], oDC.DataType));
                    }
                    return model;
                }).ToList();
            }
            catch (Exception) { throw; }
        }
    }

    /// <summary>
    /// SOIC.CORE.ListExtensions List相關擴充
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// 轉成DataTable
        /// </summary>
        /// <typeparam name="T">擴充對像泛型:T</typeparam>
        /// <param name="items">擴充對象:List</param>
        /// <returns>DataTable</returns>
        public static DataTable ToDataTable<T>(this List<T> items)
        {
            try
            {
                DataTable dataTable = new DataTable(typeof(T).Name);

                //Get all the properties
                PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in Props)
                {
                    //Defining type of data column gives proper data table 
                    var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                    //Setting column names as Property names
                    dataTable.Columns.Add(prop.Name, type);
                }
                foreach (T item in items)
                {
                    var values = new object[Props.Length];
                    for (int i = 0; i < Props.Length; i++)
                    {
                        //inserting property values to datatable rows
                        values[i] = Props[i].GetValue(item, null);
                    }
                    dataTable.Rows.Add(values);
                }
                //put a breakpoint here and check datatable
                return dataTable;
            }
            catch (Exception) { throw; }
        }
    }

    /// <summary>
    /// SOIC.CORE.DateTimeExtensions 時間相關擴充
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>時間格式轉換</summary>
        /// <param name="datetimes">擴充對象:string</param>
        /// <param name="input_format">string:[Optional]原字串內時間格式</param>
        /// <param name="output_format">string:[Optional]輸出字串指定時間格式</param>
        /// <returns>string:轉換結果</returns>
        public static string ConvertStringTime(this string datetimes, string input_format = "yyyyMMddHHmmss", string output_format = "yyyy/MM/dd HH:mm:ss")
        {
            string result = "";
            try
            {
                result = DateTime.ParseExact(datetimes, input_format, new System.Globalization.CultureInfo("zh-TW", true)).ToString(output_format);
                return result;
            }
            catch (Exception) { throw; }
        }

        /// <summary>    
        /// 將C# DateTime時間格式轉換為Unix時間戳格式
        /// </summary>    
        /// <param name="time">擴充對象:DateTime</param>    
        /// <returns>long</returns>    
        public static long ConvertDateTimeToLong(this DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            return (DateTime.Now.Ticks - startTime.Ticks);
        }

        /// <summary>
        /// Unix時間戳轉換為C#時間格式 
        /// </summary>
        /// <param name="TimeStamp">擴充對象:string</param>
        /// <returns>DateTime</returns>
        public static DateTime ConvertStringToDateTime(this string TimeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(TimeStamp + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
    }

}
