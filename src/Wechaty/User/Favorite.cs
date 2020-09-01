using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Wechaty.User
{
    /// <summary>
    /// favorite
    /// </summary>
    public class Favorite : Accessory<Favorite>
    {
        /// <summary>
        /// init <see cref="Favorite"/>
        /// </summary>
        /// <param name="wechaty"></param>
        /// <param name="logger"></param>
        /// <param name="name"></param>
        public Favorite([DisallowNull] Wechaty wechaty,
                        [DisallowNull] ILogger<Favorite> logger,
                        [AllowNull] string? name = null) : base(wechaty, logger, name)
        {
        }

        ///<inheritdoc/>
        public override Favorite ToImplement => this;
    }
}
