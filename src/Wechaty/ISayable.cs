using System.Threading.Tasks;
using Wechaty.User;

namespace Wechaty
{
    /// <summary>
    /// 可聊天
    /// </summary>
    public interface ISayable
    {
        /// <summary>
        /// 对话Id
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Wechaty 实例
        /// </summary>
        Wechaty WechatyInstance { get; }

        /// <summary>
        /// 发送聊天
        /// </summary>
        /// <param name="text">发送文本</param>
        /// <param name="replyTo">指定回复 @ 人员 </param>
        /// <returns></returns>
        Task<Message?> Say(string text, params Contact[]? replyTo);
    }
}
