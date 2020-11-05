using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wechaty
{
    public partial class GrpcPuppet
    {
        #region Contact

        public override Task<string> ContactAlias(string contactId)
        {
            throw new NotImplementedException();
        }

        public override Task ContactAlias(string ontactId, string? alias)
        {
            throw new NotImplementedException();
        }

        public override Task<FileBox> ContactAvatar(string contactId)
        {
            throw new NotImplementedException();
        }

        public override Task ContactAvatar(string contactId, FileBox file)
        {
            throw new NotImplementedException();
        }

        public override Task<List<string>> ContactList()
        {
            throw new NotImplementedException();
        }

        public override Task ContactSelfName(string name)
        {
            throw new NotImplementedException();
        }

        public override Task<string> ContactSelfQRCode()
        {
            throw new NotImplementedException();
        }

        public override Task ContactSelfSignature(string signature)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
