using System.Threading.Tasks;
using Wechaty.Schemas;

namespace Wechaty.User
{
#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable
    public class MiniProgramRepository
#pragma warning restore CA1052 // Static holder types should be Static or NotInheritable
    {
#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        public static async Task<MiniProgram> Create()
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        {
            // TODO: get appid and username from wechat
            // the same implement as ts 😁
            var payload = new MiniProgramPayload
            {
                Appid = "todo",
                Description = "todo",
                PagePath = "todo",
                ThumbKey = "todo",
                ThumbUrl = "todo",
                Title = "todo",
                Username = "todo"
            };

            return new MiniProgram(payload);
        }

    }
}
