using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wechaty.User
{
    /// <summary>
    /// repository of <see cref="Favorite"/>
    /// </summary>
    public class FavoriteRepository
    {
        /// <summary>
        /// not implemented
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<Tag> List() => Array.Empty<Tag>();

        /// <summary>
        /// not implemented
        /// </summary>
        /// <returns></returns>
        public Task<IReadOnlyList<Tag>> Tags() => Task.FromResult((IReadOnlyList<Tag>)Array.Empty<Tag>());
    }
}
