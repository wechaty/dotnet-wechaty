using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wechaty.Schemas;

namespace Wechaty.User
{
    public class FriendshipRepository : Accessory<FriendshipRepository>
    {
        private readonly ILogger<Friendship> _loggerForFriendship;
        private readonly string? _name;

        public FriendshipRepository([DisallowNull] ILogger<Friendship> loggerForFriendship,
                                    [DisallowNull] Wechaty wechaty,
                                    [DisallowNull] Puppet puppet,
                                    [DisallowNull] ILogger<FriendshipRepository> logger,
                                    [AllowNull] string? name = null) : base(wechaty, puppet, logger, name)
        {
            _loggerForFriendship = loggerForFriendship;
            _name = name;
        }

        public Friendship Load(string id)
            => new Friendship(id, WechatyInstance, Puppet, _loggerForFriendship, _name);

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

        public async Task Delete([DisallowNull] Contact contact)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"static del({contact.Id})");
            }
            //Just as TypeScript implementation
            throw new NotImplementedException("to be implemented");
        }

        public Task<Friendship> FromJson(string payload) => FromJson(JsonConvert.DeserializeObject<FriendshipPayload>(payload));

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

        public override FriendshipRepository ToImplement() => this;
    }
}
