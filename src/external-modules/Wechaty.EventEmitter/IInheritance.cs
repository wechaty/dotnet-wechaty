
namespace Wechaty.EventEmitter
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TImplement"></typeparam>
    /// <typeparam name="TSupper"></typeparam>
    public interface IInheritance<TImplement, TSupper>
        where TImplement : TSupper, IInheritance<TImplement, TSupper>
    {
        /// <summary>
        /// implement of <see cref="IInheritance{TImplement, TSupper}"/>, for get `this`
        /// </summary>
        TImplement ToImplement { get; }
    }
}
