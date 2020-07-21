
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Wechaty.User
{
    public class ContactSelfRepository : ContactRepository<ContactSelfRepository, ContactSelf>
    {
        public ContactSelfRepository([DisallowNull] ILogger<ContactSelf> loggerForContactSelf,
                            [DisallowNull] Wechaty wechaty,
                            [DisallowNull] Puppet puppet,
                            [DisallowNull] ILogger<ContactSelfRepository> logger,
                            [AllowNull] string? name = null) : base(loggerForContactSelf, wechaty, puppet, logger, name)
        {
        }
        public override ContactSelfRepository ToImplement() => this;

        protected override ContactSelf New([DisallowNull] string id, [DisallowNull] Wechaty wechaty, [DisallowNull] Puppet puppet, [DisallowNull] ILogger<ContactSelf> logger, [AllowNull] string? name = null) => new ContactSelf(id, WechatyInstance, Puppet, logger, name);
    }
}
