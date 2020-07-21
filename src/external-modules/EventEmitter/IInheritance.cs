
namespace Wechaty
{
    public interface IInheritance<TImplement, TSupper>
        where TImplement : TSupper, IInheritance<TImplement, TSupper>
    {
        TImplement ToImplement();
    }
}
