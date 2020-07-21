
namespace Wechaty
{
    /// <summary>
    /// 事件发射器
    /// </summary>
    public abstract class EventEmitter : EventEmitter<EventEmitter>
    {
        public sealed override EventEmitter ToImplement() => this;
    }
}
