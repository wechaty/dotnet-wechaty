using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wechaty.User
{
    public class FavoriteRepository
    {
        public IReadOnlyList<Tag> List() => Array.Empty<Tag>();
        public Task<IReadOnlyList<Tag>> Tags() => Task.FromResult((IReadOnlyList<Tag>)Array.Empty<Tag>());
    }
}
