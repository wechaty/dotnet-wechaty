using System.Threading.Tasks;

namespace Wechaty
{
    /// <summary>
    /// 有 可接受 行为
    /// 比如：接受添加好友申请，接受邀请入群
    /// </summary>
    public interface IAcceptable
    {
        /// <summary>
        /// 确认接受
        /// </summary>
        /// <returns></returns>
        Task Accept();

        /// <summary>
        /// Weachty 实例
        /// </summary>
        Wechaty WechatyInstance { get; }
    }
}
