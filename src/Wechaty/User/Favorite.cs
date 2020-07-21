using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Wechaty.User
{
    public class Favorite : Accessory<Favorite>
    {
        public Favorite([DisallowNull] Wechaty wechaty,
                        [DisallowNull] Puppet puppet,
                        [DisallowNull] ILogger<Favorite> logger,
                        [AllowNull] string? name = null) : base(wechaty, puppet, logger, name)
        {
        }

        public override Favorite ToImplement() => this;
    }
}
