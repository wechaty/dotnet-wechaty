using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wechaty.Schemas;


namespace Wechaty.User
{
    public abstract class ContactRepository<TContactRepository, TContact> : Accessory<TContactRepository>
        where TContactRepository : ContactRepository<TContactRepository, TContact>
        where TContact : Contact
    {
        private readonly ILogger<TContact> _loggerForContact;
        private readonly string? _name;

        protected ContactRepository([DisallowNull] ILogger<TContact> loggerForContact,
                                    [DisallowNull] Wechaty wechaty,
                                    [DisallowNull] Puppet puppet,
                                    [DisallowNull] ILogger<TContactRepository> logger,
                                    [AllowNull] string? name = null) : base(wechaty, puppet, logger, name)
        {
            _loggerForContact = loggerForContact;
            _name = name;
        }

        protected ConcurrentDictionary<string, TContact>? Pool { get; set; }

        public TContact Load(string id)
        {
            if (Pool == null)
            {
                if (Logger.IsEnabled(LogLevel.Trace))
                {
                    Logger.LogTrace($"load({id}) init pool");
                }
                Pool = new ConcurrentDictionary<string, TContact>();
            }

            return Pool.GetOrAdd(id, key => New(key, WechatyInstance, Puppet, _loggerForContact, _name));
        }

        public async Task<TContact?> Find([DisallowNull] ContactQueryFilter query)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"find({JsonConvert.SerializeObject(query)})");
            }
            var contactList = await FindAll(query);

            if (contactList.Count > 1)
            {
                Logger.LogWarning($"find() got more than one({contactList.Count}) result");
            }
            var index = 0;
            foreach (var contact in contactList)
            {
                var valid = await Puppet.ContactValidate(contact.Id);
                if (valid)
                {
                    if (Logger.IsEnabled(LogLevel.Trace))
                    {
                        Logger.LogTrace($"find() confirm contact[{index}] with id={contact.Id} is valid result, return it.");
                    }
                }
                else
                {
                    if (Logger.IsEnabled(LogLevel.Trace))
                    {
                        Logger.LogTrace($"find() confirm contact[{index}] with id={contact.Id} is INVALID result, try next");
                    }
                }
                if (valid)
                {
                    return contact;
                }
                index++;
            }
            Logger.LogWarning($"find() got {contactList.Count} contacts but no one is valid.");
            return null;
        }

        public async Task<IReadOnlyList<TContact>> FindAll([AllowNull] ContactQueryFilter? query = default)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"findAll({JsonConvert.SerializeObject(query)})");
            }
            try
            {
                var contactIdList = await Puppet.ContactSearch(query);
                var contactList = contactIdList.Select(id => Load(id))
                    .ToList();

                const int BATCH_SIZE = 16;
                var batchIndex = 0;
                var invalid = new ConcurrentDictionary<string, bool>();
                while (batchIndex * BATCH_SIZE < contactList.Count)
                {
                    await Task.WhenAll(contactList.Skip(batchIndex * BATCH_SIZE)
                          .Take(BATCH_SIZE)
                          .Select(async contact =>
                          {
                              try
                              {
                                  await contact.Ready();
                              }
                              catch (Exception exception)
                              {
                                  invalid.TryAdd(contact.Id, true);
                                  Logger.LogError(exception, "findAll() contact.ready() failed.");
                              }
                          }));
                    batchIndex++;
                }
                return contactList.Where(c => invalid.ContainsKey(c.Id))
                    .ToImmutableList();
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, "this.puppet.contactFindAll() rejected");
                return Array.Empty<TContact>();
            }
        }

        [Obsolete("TODO")]
#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        public async Task Delete(TContact contact)
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        {
            //TODO: just as TypeScript.
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"static delete({contact.Id})");
            }
        }

        /// <summary>
        /// Get tags for all contact
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyList<Tag>> Tags()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"static tags() for {this}");
            }
            try
            {
                var tagIdList = await Puppet.TagContactList();
                return tagIdList.Select(id => WechatyInstance.Tag.Load(id))
                    .ToImmutableList();
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, "static tags() failed.");
                return Array.Empty<Tag>();
            }
        }

        protected abstract TContact New([DisallowNull] string id,
            [DisallowNull] Wechaty wechaty,
            [DisallowNull] Puppet puppet,
            [DisallowNull] ILogger<TContact> logger,
            [AllowNull] string? name = default);
    }
}
