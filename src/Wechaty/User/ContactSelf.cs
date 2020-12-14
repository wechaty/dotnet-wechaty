using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wechaty.Module.Common;
using Wechaty.Module.FileBox;

namespace Wechaty.User
{
    public class ContactSelf : Contact
    {
        public ContactSelf([DisallowNull] string id,
                           [DisallowNull] Wechaty wechaty,
                           [DisallowNull] ILogger<ContactSelf> logger,
                           [AllowNull] string? name = null) : base(id, wechaty, logger, name)
        {
        }

        public Task Avatar([DisallowNull] FileBox file)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"avatar({file.Name})");
            }
            if (Id != Puppet.SelfId)
            {
                throw new InvalidOperationException("set avatar only available for user self");
            }
            return Puppet.ContactAvatar(Id, file);
        }

        public async Task<string> QrCode()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace("qrcode()");
            }
            string puppetId;
            try
            {
                puppetId = Puppet!.SelfId!;
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException("Can not get qrcode, user might be either not logged in or already logged out", exception);
            }
            if (Id != puppetId)
            {
                throw new InvalidOperationException("only can get qrcode for the login userself");
            }
            var qrCodeValue = await Puppet.ContactSelfQRCode();
            return qrCodeValue.GuardQrCodeValue();
        }

        public async Task SetName(string name)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"name({Name})");
            }
            try
            {
                var puppetId = Puppet!.SelfId;
                if (Id != puppetId)
                {
                    throw new InvalidOperationException("only can set name for user self");
                }
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException("Can not set name for user self, user might be either not logged in or already logged out", exception);
            }
            await Puppet.ContactSelfName(name);
            await Sync();
        }

        public async Task SetSignature(string signature)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"signature({signature})");
            }
            try
            {
                var puppetId = Puppet!.SelfId;
                if (Id != puppetId)
                {
                    throw new InvalidOperationException("only can change signature for user self");
                }
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException("Can not set signature for user self, user might be either not logged in or already logged out", exception);
            }
            await Puppet.ContactSelfSignature(signature);
            await Sync();
        }
    }
}
