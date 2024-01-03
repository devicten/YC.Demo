namespace YC.Demo1.Configs
{
    /// <summary>
    /// 標準物件基礎介面
    /// </summary>
    public interface IBaseClass
    {
        /// <summary> 物件名稱 </summary>
        string TAG { get; }
    }

    /// <summary>
    /// 標準物件
    /// </summary>
    public class BaseClass : IBaseClass
    {
        /// <summary> 物件名稱 </summary>
        public string TAG { get => this.GetType().Name; }
    }
}
