using System.Dynamic;
using YC.Demo1.Helpers;

namespace YC.Demo1.Configs
{
    /// <summary>需要寫入Config的Class需寫入介面。</summary>
    public interface IConfigBase : IBaseClass
    {
        /// <summary>必須宣告設定以及期預設值</summary>
        object DefaultConfig { get; }
    }
    /// <summary>需要寫入Config的Class需繼承的父層。</summary>
    public class ConfigBase : BaseClass
    {
        /// <summary>Config建構子</summary>
        public ConfigBase()
        {
            this.ConfigInit();
        }

        /// <summary>Config 初始化</summary>
        public void ConfigInit()
        {
            string MESSAGE = string.Empty;
            try
            {
                var self = this as IConfigBase;
                if (CFG.Instance == null)
                    return;
                CFG.Instance.CHECK(self.TAG, self.DefaultConfig);
                CFG.Instance.SubClassCount++;
            }
            catch
            {

            }
        }
    }
    public class CFG : BaseClass
    {  /// <summary>Config版本</summary>
        private readonly string version = "0.20240103001";
        /// <summary>Config預設參數</summary>
        public object DefaultConfig => new
        {
            version
        };

        /// <summary>多少的設定檔</summary>
        public int SubClassCount = 0;

        /// <summary>多執行緒使用保護:防止物件產生多個</summary>
        private static readonly object Mutex = new object();
        /// <summary>多執行緒使用保護:執行控制</summary>
        private static volatile CFG _instance;
        /// <summary>多執行緒使用保護:私有化建構子</summary>
        private CFG() : base() { }
        /// <summary>多執行緒使用保護:建立唯一物件</summary>
        public static CFG Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Mutex)
                    {
                        if (_instance == null)
                        {
                            _instance = new CFG();
                            _instance.INIT();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>Config 父層物件</summary>
        public dynamic xml = null;
        /// <summary>Config 檔案名稱</summary>
        public string CoreConfigPath
        {
            get
            {
                return AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "sys.config";
            }
        }

        /// <summary>BaseClass 初始</summary>
        public void INIT()
        {
            string MESSAGE = string.Empty;
            try
            {
                ExpandoObject newxml = default(ExpandoObject);
                if (File.Exists(CoreConfigPath) == true)
                {
                    string LoadedConfigData = File.ReadAllText(this.CoreConfigPath);

                    if (JSON.Deserialize<ExpandoObject>(LoadedConfigData, out newxml, out MESSAGE) == false)
                        throw new FormatException(MESSAGE);
                }
                else
                {
                    if (JSON.Deserialize<ExpandoObject>(@"{}", out newxml, out MESSAGE) == false)
                        throw new FormatException(MESSAGE);
                }
                if (newxml.ExistProperty(this.TAG) == false)
                    newxml.AddProperty(this.TAG, DefaultConfig);
                this.xml = newxml;
                this.SAVE();
                this.KEEPWATCH();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>BaseClass 結束</summary>
        public void EXIT()
        {
            watcher.Dispose();
            watcher = null;
        }

        /// <summary>
        ///檢查該Config存在與否
        /// </summary>
        /// <param name="ClassName"></param>
        /// <param name="v"></param>
        public void CHECK(string ClassName, object v)
        {
            ExpandoObject newxml = this.xml as ExpandoObject;
            if (newxml.ExistProperty(ClassName) == false)
                newxml.AddProperty(ClassName, v);
            this.xml = newxml;
            this.SAVE();
        }

        /// <summary>Config 寫入檔案</summary>
        public void SAVE()
        {
            try
            {
                string MESSAGE = string.Empty;
                string xmlstring = string.Empty;
                if (JSON.Serialize(this.xml, out xmlstring, out MESSAGE, Newtonsoft.Json.Formatting.Indented) == false)
                    throw new FormatException(MESSAGE);
                File.WriteAllText(this.CoreConfigPath, xmlstring);
            }
            catch (Exception E)
            {
                DEV.ERROR(E);
            }
        }

        /// <summary>Config 讀取檔案</summary>
        public void LOAD()
        {
            try
            {
                string MESSAGE = string.Empty;
                ExpandoObject newxml = default(ExpandoObject);
                if (File.Exists(CoreConfigPath) == true)
                {
                    string LoadedConfigData = File.ReadAllText(this.CoreConfigPath);

                    if (JSON.Deserialize<ExpandoObject>(LoadedConfigData, out newxml, out MESSAGE) == false)
                        throw new FormatException(MESSAGE);
                }
                newxml.AddProperty(this.TAG, this.DefaultConfig);
                LSYS.Config = this.xml = null;
                LSYS.Config = this.xml = newxml;
            }
            catch (Exception E)
            {
                DEV.ERROR(E);
            }
        }

        private FileSystemWatcher watcher = null;
        /// <summary>Config 變動監控</summary>
        public void KEEPWATCH()
        {
            if (watcher != null)
                return;
            // Create a new FileSystemWatcher and set its properties.
            watcher = new FileSystemWatcher()
            {
                Path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = "soic.config"
            };

            // Add event handlers.
            watcher.Changed += ConfigOnChanged;
            watcher.EnableRaisingEvents = true;
            //watcher.Created += ConfigOnChanged;
            //watcher.Deleted += ConfigOnChanged;
            //watcher.Renamed += OnRenamed;
        }
        private int CountChangedTimes = 0;
        private bool IsFirstRun = true;
        private const int FirstRunChangedTimes = 6;
        private const int NormallyChangedTimes = 2;
        private void ConfigOnChanged(object source, FileSystemEventArgs e)
        {
            CountChangedTimes++;
            if (CountChangedTimes < (IsFirstRun == true ? FirstRunChangedTimes : NormallyChangedTimes))
                return;
            if (IsFirstRun == true)
            {
                IsFirstRun = false;
                return;
            }
            DEV.LOG($"ConfigOnChanged(object {source.ToJSON()}, FileSystemEventArgs {e.ToJSON()})");
            this.LOAD();
            //Console.Clear();
            //(LSYS.Config as object).DUMP();
            //Console.WriteLine($"CountChangedTime={CountChangedTimes}");
            //CountChangedTimes = 0;
        }
    }
}
