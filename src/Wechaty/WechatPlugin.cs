using System;

namespace Wechaty
{
    /// <summary>
    /// wechat plugin delegate
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    public delegate IDisposable WechatPlugin(Wechaty bot);
}
