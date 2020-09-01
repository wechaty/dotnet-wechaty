using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Wechaty.User
{
    /// <summary>
    /// repository of <see cref="Contact"/>
    /// </summary>
    public class ContactRepository : ContactRepository<ContactRepository, Contact>
    {
        /// <summary>
        /// init <see cref="ContactRepository"/>
        /// </summary>
        /// <param name="loggerForContact"></param>
        /// <param name="wechaty"></param>
        /// <param name="logger"></param>
        /// <param name="name"></param>
        public ContactRepository([DisallowNull] ILogger<Contact> loggerForContact,
                                 [DisallowNull] Wechaty wechaty,
                                 [DisallowNull] ILogger<ContactRepository> logger,
                                 [AllowNull] string? name = null) : base(loggerForContact, wechaty, logger, name)
        {
        }

        ///<inheritdoc/>
        public override ContactRepository ToImplement => this;

        ///<inheritdoc/>
        protected override Contact New([DisallowNull] string id,
                                       [DisallowNull] Wechaty wechaty,
                                       [DisallowNull] ILogger<Contact> logger,
                                       [AllowNull] string? name = null) => new Contact(id, WechatyInstance, logger, name);
    }
}
