using System;

namespace Wechaty
{
    public interface IWechatPlugin
    {
        IDisposable Install(Wechaty bot);
    }
}
