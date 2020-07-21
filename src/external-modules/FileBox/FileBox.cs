using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using MimeMapping;
using QRCoder;

namespace Wechaty
{
    public class FileBox
    {
        private static readonly IReadOnlyDictionary<string, object> EmptyMetadata
            = new Dictionary<string, object>();

        public FileBoxType BoxType { get; set; }
        public Optional<string> MimeType { get; set; }
        public string Name { get; set; }

        private Dictionary<string, object>? _metadata;

        private readonly string? _base64;
        private readonly string? _remoteUrl;
        private readonly string? _qrCode;

        private readonly byte[]? _buffer;
        private readonly string? _localPath;
        private readonly Stream? _stream;

        private readonly IDictionary<string, IEnumerable<string>>? _headers;

        public IReadOnlyDictionary<string, object> Metadata
        {
            get => _metadata ?? EmptyMetadata;
            set
            {
                if (_metadata != null)
                {
                    throw new InvalidOperationException("metadata can not be modified after set");
                }
                var metadata = new Dictionary<string, object>(value.Count);
                foreach (var item in value)
                {
                    metadata.Add(item.Key, item.Value);
                }
                _metadata = metadata;
            }
        }

        public FileBox(FileBoxOptions options)
        {
            Name = Path.GetFileName(options.Name);
            BoxType = options.Type;
            var mime = MimeUtility.GetMimeMapping(Name);
            MimeType = string.IsNullOrWhiteSpace(mime) ? default : new Optional<string>(mime);
            switch (options.Type)
            {
                case FileBoxType.Buffer:
                    var bufferOptions = (FileBoxOptionsBuffer)options;
                    if (bufferOptions.Buffer == null)
                    {
                        throw new ArgumentException("no buffer");
                    }
                    _buffer = bufferOptions.Buffer;
                    break;
                case FileBoxType.File:
                    var fileOptions = (FileBoxOptionsFile)options;
                    if (string.IsNullOrWhiteSpace(fileOptions.Path))
                    {
                        throw new ArgumentException("no path");
                    }
                    _localPath = fileOptions.Path;
                    break;
                case FileBoxType.Url:
                    var urlOptions = (FileBoxOptionsUrl)options;
                    if (string.IsNullOrWhiteSpace(urlOptions.Url))
                    {
                        throw new ArgumentException("no url");
                    }
                    _remoteUrl = urlOptions.Url;
                    if (urlOptions.Headers != null)
                    {
                        _headers = urlOptions.Headers;
                    }
                    break;
                case FileBoxType.Stream:
                    var streamOptions = (FileBoxOptionsStream)options;
                    if (streamOptions.Stream == null)
                    {
                        throw new ArgumentException("no stream");
                    }
                    _stream = streamOptions.Stream;
                    break;
                case FileBoxType.QRCode:
                    var qrCodeOptions = (FileBoxOptionsQRCode)options;
                    if (string.IsNullOrWhiteSpace(qrCodeOptions.QrCode))
                    {
                        throw new ArgumentException("no QR Code");
                    }
                    _qrCode = qrCodeOptions.QrCode;
                    break;
                case FileBoxType.Base64:
                    var base64Options = (FileBoxOptionsBase64)options;
                    if (string.IsNullOrWhiteSpace(base64Options.Base64))
                    {
                        throw new ArgumentException("no Base64 data");
                    }
                    _base64 = base64Options.Base64;
                    break;
                case FileBoxType.Unknown:
                default:
                    break;
            }
        }

        public async Task Ready()
        {
            if (BoxType == FileBoxType.Url)
            {
                await SyncRemoteName();
            }
        }

        protected async Task SyncRemoteName()
        {
            if (BoxType != FileBoxType.Url)
            {
                throw new InvalidOperationException("type is not Remote");
            }
            if (string.IsNullOrWhiteSpace(_remoteUrl))
            {
                throw new InvalidOperationException("no url");
            }
            var headers = await Http.HeadContentHeaders(_remoteUrl);
            if (!string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName))
            {
                Name = headers.ContentDisposition.FileName;
            }
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new InvalidOperationException("no name");
            }
            MimeType = headers.ContentType?.MediaType ?? MimeUtility.GetMimeMapping(Name) ?? "";
        }

        public override string ToString() => string.Concat("FileBox#", BoxType.ToString(), "<", Name, ">");

        public FileBoxJsonObject ToJson()
        {
            FileBoxJsonObject common;
            switch (BoxType)
            {
                case FileBoxType.Url:
                    if (string.IsNullOrWhiteSpace(_remoteUrl))
                    {
                        throw new InvalidOperationException("no url");
                    }
                    common = new FileBoxJsonObjectUrl
                    {
                        Headers = _headers,
                        RemoteUrl = _remoteUrl
                    };
                    break;
                case FileBoxType.QRCode:
                    if (string.IsNullOrWhiteSpace(_qrCode))
                    {
                        throw new InvalidOperationException("no qr code");
                    }
                    common = new FileBoxJsonObjectQRCode
                    {
                        QrCode = _qrCode
                    };
                    break;
                case FileBoxType.Base64:
                    if (string.IsNullOrWhiteSpace(_base64))
                    {
                        throw new InvalidOperationException("no base64 data");
                    }
                    common = new FileBoxJsonObjectBase64
                    {
                        Base64 = _base64
                    };
                    break;
                case FileBoxType.Buffer:
                case FileBoxType.File:
                case FileBoxType.Stream:
                case FileBoxType.Unknown:
                default:
                    throw new InvalidOperationException("FileBox.toJSON() can only work on limited FileBoxType(s). See: <https://github.com/huan/file-box/issues/25>");
            }
            common.Name = Name;
            common.Metadata = new Dictionary<string, object>(Metadata.Count);
            foreach (var item in Metadata)
            {
                common.Metadata.Add(item.Key, item.Value);
            }
            return common;
        }

        public async Task<Stream> ToStream()
        {
            switch (BoxType)
            {
                case FileBoxType.Buffer:
                    if (_buffer == null)
                    {
                        throw new InvalidOperationException("no buffer");
                    }
                    return new MemoryStream(_buffer);
                case FileBoxType.File:
                    if (string.IsNullOrWhiteSpace(_localPath))
                    {
                        throw new InvalidOperationException("no url(path)");
                    }
                    return File.OpenRead(_localPath);
                case FileBoxType.Url:
                    if (string.IsNullOrWhiteSpace(_remoteUrl))
                    {
                        throw new InvalidOperationException("no url");
                    }
                    return await Http.GetStream(_remoteUrl, _headers);
                case FileBoxType.Stream:
                    if (_stream == null)
                    {
                        throw new InvalidOperationException("no stream");
                    }
                    return _stream;
                case FileBoxType.QRCode:
                    if (string.IsNullOrWhiteSpace(_qrCode))
                    {
                        throw new InvalidOperationException("no QR Code");
                    }
                    //TODO:可能是错误的
                    var generator = new QRCodeGenerator();
                    var data = generator.CreateQrCode(_qrCode, QRCodeGenerator.ECCLevel.Q);
                    var bytes = data.GetRawData(QRCodeData.Compression.Uncompressed);
                    return new MemoryStream(bytes);
                case FileBoxType.Base64:
                    if (string.IsNullOrWhiteSpace(_base64))
                    {
                        throw new InvalidOperationException("no base64 data");
                    }
                    return new MemoryStream(Convert.FromBase64String(_base64));
                case FileBoxType.Unknown:
                default:
                    throw new InvalidOperationException("not supported FileBoxType: " + BoxType);
            }
        }

        public async Task ToFile(string? filePath, bool overwrite = false)
        {
            if (BoxType == FileBoxType.Url)
            {
                if (string.IsNullOrWhiteSpace(MimeType.Value) || string.IsNullOrWhiteSpace(Name))
                {
                    await SyncRemoteName();
                }
            }
            var fullFilePath = Path.GetFullPath(filePath ?? Name);

            var exist = File.Exists(fullFilePath);

            if (exist && !overwrite)
            {
                throw new InvalidOperationException($"FileBox.toFile({fullFilePath}): file exist. use FileBox.toFile({fullFilePath}, true) to force overwrite.");
            }

            using var writeStream = File.OpenWrite(fullFilePath);
            _ = await Pipe(writeStream);
        }

        public async Task<string> ToBase64()
        {
            if (BoxType == FileBoxType.Base64)
            {
                if (string.IsNullOrEmpty(_base64))
                {
                    throw new InvalidOperationException("no base64 data");
                }
                return _base64;
            }

            var buffer = await ToBuffer();
            return Convert.ToBase64String(buffer);
        }

        public async Task<byte[]> ToBuffer()
        {
            if (BoxType == FileBoxType.Buffer)
            {
                if (_buffer == null)
                {
                    throw new InvalidOperationException("no buffer!");
                }
                return _buffer;
            }

            using var stream = new MemoryStream();
            _ = await Pipe(stream);
            return stream.ToArray();
        }

        public async Task<string> ToQRCode()
        {
            if (BoxType == FileBoxType.QRCode)
            {
                if (string.IsNullOrEmpty(_qrCode))
                {
                    throw new InvalidOperationException("no QR Code!");
                }
                return _qrCode;
            }
            //TODO:可能是错误的
            var buffer = await ToBuffer();
            var generator = new QRCodeGenerator();
            var code = generator.CreateQrCode(buffer, QRCodeGenerator.ECCLevel.Q);
            var base64QRCode = new Base64QRCode(code);
            return base64QRCode.GetGraphic(1);
        }

        public async Task<Stream> Pipe(Stream destination)
        {
            var stream = await ToStream();
            await stream.CopyToAsync(destination);
            return destination;
        }

        public static FileBox FromUrl([DisallowNull] string url,
                                      [AllowNull] string? name = default,
                                      [AllowNull] IDictionary<string, IEnumerable<string>>? headers = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = new Uri(url).GetComponents(UriComponents.Path, UriFormat.UriEscaped);
            }
            var options = new FileBoxOptionsUrl
            {
                Headers = headers,
                Name = name,
                Url = url
            };
            return new FileBox(options);
        }

        public static FileBox FromFile([DisallowNull] string path, [AllowNull] string? name = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = Path.GetFileName(path);
            }
            var options = new FileBoxOptionsFile
            {
                Name = name,
                Path = path
            };
            return new FileBox(options);
        }

        public static FileBox FromStream([DisallowNull] Stream stream, [DisallowNull] string name)
        {
            var options = new FileBoxOptionsStream
            {
                Name = name,
                Stream = stream
            };
            return new FileBox(options);
        }

        public static FileBox FromBuffer([DisallowNull] byte[] buffer, [DisallowNull] string name)
        {
            var options = new FileBoxOptionsBuffer
            {
                Name = name,
                Buffer = buffer
            };
            return new FileBox(options);
        }

        public static FileBox FromBase64([DisallowNull] string base64, [DisallowNull] string name)
        {
            var options = new FileBoxOptionsBase64
            {
                Name = name,
                Base64 = base64
            };
            return new FileBox(options);
        }

        public static FileBox FromDataUrl([DisallowNull] string dataUrl, [DisallowNull] string name)
        {
            var dataList = dataUrl.Split(',');
#pragma warning disable IDE0056 // 使用索引运算符
            return FromBase64(dataList[dataList.Length - 1], name);
#pragma warning restore IDE0056 // 使用索引运算符
        }

        public static FileBox FromQRCode([DisallowNull] string qrCode)
        {
            var options = new FileBoxOptionsQRCode
            {
                Name = "qrcode.png",
                QrCode = qrCode
            };
            return new FileBox(options);
        }

        public static FileBox FromJson([DisallowNull] FileBoxJsonObject obj)
        {
            FileBox fileBox;
            switch (obj.BoxType)
            {
                case FileBoxType.Base64:
                    fileBox = FromBase64(((FileBoxJsonObjectBase64)obj).Base64, obj.Name);
                    break;
                case FileBoxType.Url:
                    fileBox = FromUrl(((FileBoxJsonObjectUrl)obj).RemoteUrl, obj.Name);
                    break;
                case FileBoxType.QRCode:
                    fileBox = FromQRCode(((FileBoxJsonObjectQRCode)obj).QrCode);
                    break;
                case FileBoxType.Unknown:
                case FileBoxType.Buffer:
                case FileBoxType.File:
                case FileBoxType.Stream:
                default:
                    throw new InvalidOperationException($"unknown filebox json object{{type}}: {JsonSerializer.Serialize(obj)}");
            }
            fileBox.Metadata = obj.Metadata.ToImmutableDictionary();
            return fileBox;
        }
    }
}
