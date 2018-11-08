using System;
using System.Collections.Generic;
using System.Linq;
using Core.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.Json
{
    public class SettingsSerializer
    {
        readonly JsonSerializer _serializer = JsonSerializer.CreateDefault();

        public JObject WriteServer(string name, string address, string clientUri, Dictionary<string, string> hashes)
        {
            IServer server;

            var obj = new JObject();
            var writer = obj.CreateWriter();

            writer.WritePropertyName(nameof(server.Address));
            writer.WriteValue(address);

            writer.WritePropertyName(nameof(server.Name));
            writer.WriteValue(name);

            writer.WritePropertyName(nameof(server.DownloadLink));
            writer.WriteValue(clientUri);

            if (hashes?.Any() == true)
            {
                writer.WritePropertyName(nameof(server.DirHashCheck));
                _serializer.Serialize(writer, hashes);
            }

            writer.Close();
            return obj;
        }

        public JObject WriteAuth(string uri, ModuleTypes type, bool strict)
        {
            IAuthModuleSettings set;
            var obj = new JObject();
            var writer = obj.CreateWriter();

            writer.WritePropertyName(nameof(set.AuthUri));
            writer.WriteValue(uri);

            writer.WritePropertyName(nameof(set.StrictUsage));
            writer.WriteValue(strict);

            writer.WritePropertyName(nameof(set.Type));
            writer.WriteValue(type);

            writer.Close();
            return obj;
        }

        public JObject WriteProjectConfig(List<JObject> servers, JObject auth, string link)
        {
            IProjectConfig conf;
            var obj = new JObject();
            var writer = obj.CreateWriter();

            writer.WritePropertyName(nameof(conf.ProjectSite));
            writer.WriteValue(link);

            if (servers?.Any() == true)
            {
                writer.WritePropertyName(nameof(conf.Servers));
                writer.WriteStartArray();
                foreach (var server in servers)
                {
                    _serializer.Serialize(writer, server);
                }

                writer.WriteEndArray();
            }

            if (auth != null && auth.HasValues)
            {
                writer.WritePropertyName(nameof(conf.AuthModuleSettings));
                _serializer.Serialize(writer, auth);
            }

            writer.Close();
            return obj;

        }

        public JObject WriteUpdateConfig(string uri, Version version)
        {
            IUpdateConfig conf;
            var obj = new JObject();
            var writer = obj.CreateWriter();

            writer.WritePropertyName(nameof(conf.ExeUrl));
            writer.WriteValue(uri);

            writer.WritePropertyName(nameof(conf.Version));
            writer.WriteValue(version?.ToString());

            writer.Close();
            return obj;
        }

        public JObject WriteSettings(JObject updateSettings, string updateLink, JObject projectSettings, string projLink)
        {
            ISettings conf;
            var obj = new JObject();
            var writer = obj.CreateWriter();

            writer.WritePropertyName(nameof(conf.ProjectConfigUrl));
            writer.WriteValue(projLink);

            writer.WritePropertyName(nameof(conf.UpdateConfigUrl));
            writer.WriteValue(updateLink);

            writer.WritePropertyName(nameof(conf.ProjectConfig));
            _serializer.Serialize(writer, projectSettings);

            writer.WritePropertyName(nameof(conf.UpdateConfig));
            _serializer.Serialize(writer, updateSettings);

            writer.Close();
            return obj;
        }
    }
}
