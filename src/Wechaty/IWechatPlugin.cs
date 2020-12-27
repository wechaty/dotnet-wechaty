using System;
using System.Threading.Tasks;

namespace Wechaty
{
    public interface IWechatPlugin
    {
        //IDisposable Install(Wechaty bot);

        Task Execute(Wechaty bot);
        

    }
}
