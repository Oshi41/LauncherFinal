using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.Json
{
    public static class Extensions
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public static string ToStringIgnoreNull(this JObject value)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented, Settings);
        }
    }
}
