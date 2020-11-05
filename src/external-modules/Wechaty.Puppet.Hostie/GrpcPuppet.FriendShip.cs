using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wechaty
{
    public partial class GrpcPuppet
    {
        #region Friendship

        public override Task FriendshipAccept(string friendshipId)
        {
            throw new NotImplementedException();
        }

        public override Task FriendshipAdd(string contactId, string? hello)
        {
            throw new NotImplementedException();
        }

        public override Task<string?> FriendshipSearchPhone(string phone)
        {
            throw new NotImplementedException();
        }

        public override Task<string?> FriendshipSearchWeixin(string weixin)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
