using System;
using System.IO;
using System.Web;

using Newtonsoft.Json;

namespace Scoop
{
    public class CookieTokenEncoder
    {
        private readonly byte[] _keyHash;
        private readonly bool _urlEncode;

        public CookieTokenEncoder(bool encrypt, string key, bool urlEncode)
        {
            _keyHash = GetKeyHash(key, encrypt);
            _urlEncode = urlEncode;
        }

        public string Serialize(CookieToken token)
        {
            return token == null ? null : Serialize(token, _keyHash, _urlEncode);
        }

        public static string Serialize(CookieToken token, bool encrypt, string secret, bool encodeToken)
        {
            if (token == null)
                return null;

            var keyHash = GetKeyHash(secret, encrypt);

            return Serialize(token, keyHash, encodeToken);
        }

        private static string Serialize(CookieToken token, byte[] keyHash, bool encodeToken)
        {
            try
            {
                var result = JsonConvert.SerializeObject(token, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                if (keyHash != null)
                    result = EncodeBase64(result, keyHash);

                return encodeToken ? HttpUtility.UrlEncode(result) : result;
            }
            catch
            {
                /* ignored */
            }

            return null;
        }

        public CookieToken Deserialize(string data)
        {
            if (data == null)
                return null;

            return Deserialize(data, _keyHash, _urlEncode);
        }

        public static CookieToken Deserialize(string data, bool encrypt, string secret, bool decodeToken)
        {
            if (data == null)
                return null;

            var keyHash = GetKeyHash(secret, encrypt);

            return Deserialize(data, keyHash, decodeToken);
        }

        private static CookieToken Deserialize(string data, byte[] keyHash, bool decodeToken)
        {
            try
            {
                var json = decodeToken ? HttpUtility.UrlDecode(data) : data;

                if (keyHash != null)
                    json = DecodeBase64(json, keyHash);

                return JsonConvert.DeserializeObject<CookieToken>(json);
            }
            catch
            {
                /* ignored */
            }

            return null;
        }

        private static byte[] GetKeyHash(string key, bool encrypt) => encrypt && key != null ? EncryptionHelper.ComputeHashSha256(key, null) : null;

        private static string DecodeBase64(string encrypted, byte[] keyHash)
        {
            var decoded = Convert.FromBase64String(encrypted);

            return (string)EncryptionHelper.Decode(decoded, 0, decoded.Length, keyHash, stream =>
            {
                using (var reader = new StreamReader(stream))
                {
                    string decrypted = reader.ReadToEnd();

                    return decrypted;
                }
            });
        }

        private static string EncodeBase64(string data, byte[] keyHash)
        {
            var encrypted = EncryptionHelper.Encode(keyHash, stream =>
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(data);
                }
            });

            var encoded = Convert.ToBase64String(encrypted);

            return encoded;
        }
    }
}