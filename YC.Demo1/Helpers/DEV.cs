using System.Diagnostics;

namespace YC.Demo1.Helpers
{/// <summary>
 /// SOIC.DEV: 設定事件紀錄器
 /// </summary>
    public class DEV
    {
        /// <summary>事件工具</summary>
        //private static LoggerServiceClient logger = null;

        /// <summary>紀錄器名字/專案名稱</summary>
        public static string LogName
        {
            get
            {
                StackTrace st = new StackTrace();
                int f = st.FrameCount > 2 ? 2 : 0;
                return st.GetFrame(f).GetMethod().ReflectedType.Namespace;
            }
        }
        /// <summary>程式執行位置</summary>
        private static string TAG
        {
            get
            {
                StackTrace st = new StackTrace();
                int f = st.FrameCount > 2 ? 2 : 0;
                string M = st.GetFrame(f).GetMethod().Name;
                string NS = st.GetFrame(f).GetMethod().ReflectedType.FullName;
                return string.Format("{0}{1}", NS, M);
            }
        }

        //private static void ConnectWCFLogger()
        //{
        //    if (logger != null)
        //        return;

        //    //Specify the binding to be used for the client.
        //    //BasicHttpBinding binding = new BasicHttpBinding();
        //    //Specify the address to be used for the client.
        //    EndpointAddress address = new EndpointAddress("http://localhost:8733/Logger");
        //    //Binding
        //    BasicHttpBinding basicHttpBinding = new BasicHttpBinding()//(BasicHttpSecurityMode.TransportWithMessageCredential)
        //    {
        //        OpenTimeout = TimeSpan.FromMinutes(1),
        //        CloseTimeout = TimeSpan.FromMinutes(1),
        //        SendTimeout = TimeSpan.FromMinutes(10),
        //        ReceiveTimeout = TimeSpan.FromMinutes(10),
        //        MaxReceivedMessageSize = 2147483647,
        //        MaxBufferPoolSize = 2147483647,
        //        BypassProxyOnLocal = false,
        //        AllowCookies = false,
        //        MessageEncoding = WSMessageEncoding.Text,
        //        //TextEncoding            = Encoding.Unicode,
        //        UseDefaultWebProxy = true,
        //        HostNameComparisonMode = HostNameComparisonMode.StrongWildcard,
        //        ReaderQuotas = new XmlDictionaryReaderQuotas()
        //        {
        //            MaxArrayLength = 2147483647,
        //            MaxBytesPerRead = 2147483647,
        //            MaxStringContentLength = 2147483647,
        //            MaxNameTableCharCount = 2147483647,
        //            MaxDepth = 2147483647
        //        }
        //    };


        //    //Create the logger
        //    logger = new LoggerServiceClient(basicHttpBinding, address);

        //    //Sets the MaxItemsInObjectGraph, so that client can receive large objects
        //    foreach (var operation in logger.Endpoint.Contract.Operations)
        //    {
        //        DataContractSerializerOperationBehavior operationBehavior = operation.Behaviors.Find<DataContractSerializerOperationBehavior>();
        //        //If DataContractSerializerOperationBehavior is not present in the Behavior, then add
        //        if (operationBehavior == null)
        //        {
        //            operationBehavior = new DataContractSerializerOperationBehavior(operation);
        //            operation.Behaviors.Add(operationBehavior);
        //        }
        //        //IMPORTANT: As 'operationBehavior' is a reference, changing anything here will automatically update the value in list, so no need to add this behavior to behaviorlist
        //        operationBehavior.MaxItemsInObjectGraph = 2147483647;
        //    }
        //}


        /// <summary>
        /// SOIC.DEV.VaildateSuccess: 
        /// 紀錄事件「稽核成功」
        /// </summary>
        /// <param name="Message">string:訊息</param>
        public static void VaildateSuccess(string Message)
        {

            if (string.IsNullOrWhiteSpace(TAG))
                throw new ArgumentNullException("TAG", "TAG 參數不可為空白!");

            if (string.IsNullOrWhiteSpace(Message))
                throw new ArgumentNullException("Message", "Message 參數不可為空白!");

            try
            {
                //連線WCFLogger
                //ConnectWCFLogger();
                //顯示到 Visual Stuido 即時除錯視窗
                System.Diagnostics.Trace.WriteLine(Message);
                //寫入系統Event Log
                //CORE.LOG.Instance.WRITE(TAG, EventLogEntryType.SuccessAudit, -1, Message);
                //logger.WRITE(LogName, TAG, EventLogEntryType.SuccessAudit, -1, Message, null, null);
                //EventLog.WriteEntry(EVENT_Source, Message, EventLogEntryType.SuccessAudit, 3000);
            }
            catch
            {
                //LogService服務未啟動
            }
        }

        /// <summary>
        /// SOIC.DEV.VaildateFailure: 
        /// 紀錄事件「稽核失敗」
        /// </summary>
        /// <param name="Message">string:訊息</param>
        public static void VaildateFailure(string Message)
        {
            if (string.IsNullOrWhiteSpace(TAG))
                throw new ArgumentNullException("TAG", "TAG 參數不可為空白!");
            if (string.IsNullOrWhiteSpace(Message))
                throw new ArgumentNullException("Message", "Message 參數不可為空白!");

            try
            {
                //連線WCFLogger
                //ConnectWCFLogger();
                //顯示到 Visual Stuido 即時除錯視窗
                System.Diagnostics.Trace.WriteLine(Message);
                //寫入系統Event Log
                //logger.WRITE(LogName, TAG, EventLogEntryType.FailureAudit, -1, Message, null, null);
                //EventLog.WriteEntry(EVENT_Source, Message, EventLogEntryType.FailureAudit, 4000);
            }
            catch
            {
                //LogService服務未啟動
            }
        }

        /// <summary>
        /// SOIC.DEV.LOG: 
        /// 紀錄事件「資訊」
        /// </summary>
        /// <param name="Message">string:訊息</param>
        /// <param name="EvnetID">int:[Optional]事件編號</param>
        public static void LOG(string Message, int EvnetID = 1000)
        {
            if (string.IsNullOrWhiteSpace(TAG))
                throw new ArgumentNullException("TAG", "TAG 參數不可為空白!");
            if (string.IsNullOrWhiteSpace(Message))
                throw new ArgumentNullException("Message", "Message 參數不可為空白!");

            try
            {
                //連線WCFLogger
                //ConnectWCFLogger();
                //顯示到 Visual Stuido 即時除錯視窗
                System.Diagnostics.Trace.WriteLine(Message);
                //寫入系統Event Log
                //logger.WRITE(LogName, TAG, EventLogEntryType.Information, -1, Message, null, null);
                //EventLog.WriteEntry(EVENT_Source, Message, EventLogEntryType.Information, EvnetID);
            }
            catch (Exception e)
            {
                //LogService服務未啟動
                e.DUMP();
            }
        }

        /// <summary>
        /// SOIC.DEV.WARNNING: 
        /// 紀錄事件「警告」
        /// </summary>
        /// <param name="Message">string:[Optional]訊息</param>
        /// <param name="E">Exception:[Optional]例外</param>
        public static void WARNNING(string Message = null, Exception E = null)
        {
            if (string.IsNullOrWhiteSpace(TAG))
                throw new ArgumentNullException("TAG", "TAG 參數不可為空白!");
            if (string.IsNullOrWhiteSpace(Message) && E == null)
                throw new ArgumentNullException("Message 或 E 參數其中之一必須有值!");

            try
            {
                //連線WCFLogger
                //ConnectWCFLogger();
                //顯示到 Visual Stuido 即時除錯視窗
                System.Diagnostics.Trace.WriteLine(Message);

                //寫入系統Event Log
                //if (E == null)
                //    logger.WRITE(LogName, TAG, EventLogEntryType.Warning, -1, Message, null, null);
                //else
                //    logger.WRITE(LogName, TAG, EventLogEntryType.Warning, -1, Message, E.Message, E.StackTrace);
            }
            catch
            {
                //LogService服務未啟動
            }
        }

        /// <summary>
        /// SOIC.DEV.`: 
        /// 紀錄事件「錯誤」
        /// </summary>
        /// <param name="E">Exception:例外</param>
        public static void ERROR(Exception E)
        {
            if (string.IsNullOrWhiteSpace(TAG))
                throw new ArgumentNullException("TAG", "TAG 參數不可為空白!");
            if (E == null)
                throw new ArgumentNullException("E", "E 參數不可為空白!");

            try
            {
                //連線WCFLogger
                //ConnectWCFLogger();
                //顯示到 Visual Stuido 即時除錯視窗
                System.Diagnostics.Trace.WriteLine(TAG + " >> " + E.Message);
                System.Diagnostics.Trace.WriteLine(E.StackTrace);

                ////寫入系統Event Log
                //logger.WRITE(LogName, TAG, EventLogEntryType.Error, -1, null, E.Message, E.StackTrace);
            }
            catch (Exception e)
            {
                //LogService服務未啟動
                e.DUMP();
            }
        }

    }
}
