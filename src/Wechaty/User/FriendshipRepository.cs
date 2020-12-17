using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wechaty.Module.Puppet.Schemas;

namespace Wechaty.User
{
    /// <summary>
    /// repository of <see cref="Friendship"/>
    /// </summary>
    public class FriendshipRepository : Accessory<FriendshipRepository>
    {
        private readonly ILogger<Friendship> _loggerForFriendship;
        private readonly string? _name;

        /// <summary>
        /// init <see cref="FriendshipRepository"/>
        /// </summary>
        /// <param name="loggerForFriendship"></param>
        /// <param name="wechaty"></param>
        /// <param name="logger"></param>
        /// <param name="name"></param>
        public FriendshipRepository([DisallowNull] ILogger<Friendship> loggerForFriendship,
                                    [DisallowNull] Wechaty wechaty,
                                    [DisallowNull] ILogger<FriendshipRepository> logger,
                                    [AllowNull] string? name = null) : base(wechaty, logger, name)
        {
            _loggerForFriendship = loggerForFriendship;
            _name = name;
        }

        /// <summary>
        /// load <see cref="Friendship"/> by <paramref name="id"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Friendship Load(string id)
            => new Friendship(id, WechatyInstance, _loggerForFriendship, _name);

        /// <summary>
        /// search <see cref="Contact"/> by <paramref name="query"/>
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<Contact?> Search([DisallowNull] FriendshipSearchCondition query)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"search{JsonConvert.SerializeObject(query)}");
            }
            var contactId = await Puppet.FriendshipSearch(query);
            if (string.IsNullOrWhiteSpace(contactId))
            {
                return null;
            }
            var contact = WechatyInstance.Contact.Load(contactId);
            await contact.Ready();
            return contact;
        }

        /// <summary>
        /// use <see cref="Add(Contact, string)"/> instead
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="hello"></param>
        /// <returns></returns>
        [Obsolete("use Add instead")]
        public Task Send([DisallowNull] Contact contact, [DisallowNull] string hello)
        {
            Logger.LogWarning("static send() DEPRECATED， use add() instead.");
            return Add(contact, hello);
        }

        /// <summary>
        /// Send a Friend Request to a `contact` with message `hello`.
        ///
        /// The best practice is to send friend request once per minute.
        /// Remeber not to do this too frequently, or your account may be blocked.
        /// </summary>
        /// <param name="contact">Send friend request to contact</param>
        /// <param name="hello">The friend request content</param>
        /// <returns></returns>
        public Task Add([DisallowNull] Contact contact, [DisallowNull] string hello)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"static add({contact.Id}, {hello})");
            }
            return Puppet.FriendshipAdd(contact.Id, hello);
        }

        /// <summary>
        /// delete <paramref name="contact"/> from friendship
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public async Task Delete([DisallowNull] Contact contact)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"static del({contact.Id})");
            }
            //Just as TypeScript implementation
            throw new NotImplementedException("to be implemented");
        }

        /// <summary>
        /// from json
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public Task<Friendship> FromJson(string payload) => FromJson(JsonConvert.DeserializeObject<FriendshipPayload>(payload));

        /// <summary>
        /// from json
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public async Task<Friendship> FromJson(FriendshipPayload payload)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"fromJSON{JsonConvert.SerializeObject(payload)}");
            }
            await Puppet.SetFriendshipPayload(payload.Id, payload);
            var instance = WechatyInstance.Friendship.Load(payload.Id);
            await instance.Ready();
            return instance;
        }

        ///<inheritdoc/>
        public override FriendshipRepository ToImplement => this;
    }
}
