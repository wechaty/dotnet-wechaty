using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wechaty
{
    public partial class GrpcPuppet
    {
        #region Tag
        public override Task TagContactAdd(string tagId, string contactId)
        {
            throw new NotImplementedException();
        }

        public override Task TagContactDelete(string tagId)
        {
            throw new NotImplementedException();
        }

        public override Task<List<string>> TagContactList(string contactId)
        {
            throw new NotImplementedException();
        }

        public override Task<List<string>> TagContactList()
        {
            throw new NotImplementedException();
        }

        public override Task TagContactRemove(string tagId, string contactId)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
