using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Wechaty.User
{

    public class ContactRepository : ContactRepository<ContactRepository, Contact>
    {
        public ContactRepository([DisallowNull] ILogger<Contact> loggerForContact,
                                 [DisallowNull] Wechaty wechaty,
                                 [DisallowNull] Puppet puppet,
                                 [DisallowNull] ILogger<ContactRepository> logger,
                                 [AllowNull] string? name = null) : base(loggerForContact, wechaty, puppet, logger, name)
        {
        }

        public override ContactRepository ToImplement() => this;

        protected override Contact New([DisallowNull] string id,
                                       [DisallowNull] Wechaty wechaty,
                                       [DisallowNull] Puppet puppet,
                                       [DisallowNull] ILogger<Contact> logger,
                                       [AllowNull] string? name = null) => new Contact(id, WechatyInstance, Puppet, logger, name);
    }
}
