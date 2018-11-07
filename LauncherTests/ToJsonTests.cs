using System.Collections.Generic;
using Configurator.ViewModels;
using Core.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LauncherTests
{
    [TestClass]
    public class ToJsonTests
    {
        public void AuthViewModule_ToJson()
        {
            var auth = new AuthViewModule();
            var list = new List<JObject>();

            list.Add(auth.ToJson());

            auth.Checked = false;
            list.Add(auth.ToJson());

            auth.Checked = true;
            list.Add(auth.ToJson());

            auth.StrictUsage = true;
            list.Add(auth.ToJson());

            auth.Module = ModuleTypes.Custom;
            list.Add(auth.ToJson());

            auth.Module = ModuleTypes.Ely;
            list.Add(auth.ToJson());

            auth.Module = ModuleTypes.Default;
            list.Add(auth.ToJson());

            for (int i = 1; i < list.Count; i++)
            {
                var left = list[i - 1].ToString(Formatting.Indented);
                var right = list[i].ToString(Formatting.Indented);

                Assert.AreNotEqual(list[i - 1], list[i]);
            }
        }
    }
}
