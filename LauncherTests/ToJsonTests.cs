using System.Collections.Generic;
using System.Collections.ObjectModel;
using Configurator.ViewModels;
using Core.Json;
using Core.Settings;
using LauncherFinal.Helper;
using LauncherFinal.Models.Settings;
using LauncherFinal.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServerViewModel = Configurator.ViewModels.ServerViewModel;

namespace LauncherTests
{
    [TestClass]
    public class ToJsonTests
    {
        [TestMethod]
        public void AuthViewModule_ToJson()
        {
            var auth = new AuthViewModule();
            var list = new List<JObject> {auth.ToJson()};

            auth.StrictUsage = true;
            list.Add(auth.ToJson());

            auth.Module = ModuleTypes.Custom;
            list.Add(auth.ToJson());

            auth.Module = ModuleTypes.Ely;
            list.Add(auth.ToJson());

            auth.Module = ModuleTypes.Default;
            list.Add(auth.ToJson());

            auth.Uri = "dfgdfg";
            list.Add(auth.ToJson());

            for (int i = 1; i < list.Count; i++)
            {
                var left = list[i - 1].ToString(Formatting.Indented);
                var right = list[i].ToString(Formatting.Indented);

                Assert.AreNotEqual(list[i - 1], list[i]);
            }
        }

        [TestMethod]
        public void ServerViewModel_ToJson()
        {
            var server = new Configurator.ViewModels.ServerViewModel();
            var list = new List<JObject> {server.ToJson()};

            server.ClientUries.Add("sdfgdgdfg");
            list.Add(server.ToJson());

            server.Address = "dsfgdfg";
            list.Add(server.ToJson());

            server.Name = "dfgdfg";
            list.Add(server.ToJson());

            server = new Configurator.ViewModels.ServerViewModel(new Dictionary<string, string>
            {
                {"1", "2"}
            });
            list.Add(server.ToJson());

            for (int i = 1; i < list.Count; i++)
            {
                var left = list[i - 1].ToString(Formatting.Indented);
                var right = list[i].ToString(Formatting.Indented);

                Assert.AreNotEqual(list[i - 1], list[i]);
            }
        }

        [TestMethod]
        public void ProjectViewModel_ToJson()
        {
            var conf = new ProjectViewModel();
            var list = new List<JObject> {conf.ToJson()};

            var server = new ServerViewModel(new Dictionary<string, string> {{"1", "2"}})
            {
                Address = "adasdasdasd",
                Name = "dgfhdgfj"
            };
            server.ClientUries.Add("sfghdtghdgfh");
            conf.Servers.Add(server);

            list.Add(conf.ToJson());

            conf.ProjectSite = "dsfgd";
            list.Add(conf.ToJson());

            conf = new ProjectViewModel(new AuthViewModule());
            list.Add(conf.ToJson());

            for (int i = 1; i < list.Count; i++)
            {
                var left = list[i - 1].ToString(Formatting.Indented);
                var right = list[i].ToString(Formatting.Indented);

                Assert.AreNotEqual(list[i - 1], list[i]);
            }
        }



        [TestMethod]
        public void AuthViewModule_ToJson_FromJson()
        {
            var auth = new AuthViewModule
            {
                Module = ModuleTypes.Ely,
                StrictUsage = true,
                Uri = "123123"
            };

            var text = auth.ToJson().ToString(Formatting.None);

            var model = JsonConvert.DeserializeObject<AuthModuleSettings>(text);

            var copy = JsonConvert.SerializeObject(model);

            Assert.AreEqual(text, copy);
        }

        [TestMethod]
        public void ServerViewModel_ToJson_FromJson()
        {
            var server = new Configurator.ViewModels.ServerViewModel(new Dictionary<string, string>
            {
                {"1", "2"}
            })
            {
                Address = "827r09er8wt",
                Name = "dfghfdhsfghbsdykjfyu",
            };
            server.ClientUries.Add("1111111111111111111");
            server.ClientUries.Add("222222222222222222");

            var text = server.ToJson().ToString(Formatting.None);

            var model = JsonConvert.DeserializeObject<Server>(text);

            var copy = JsonConvert.SerializeObject(model);

            Assert.AreEqual(text, copy);

        }

        [TestMethod]
        public void ProjectViewModel_ToJson_FromJson()
        {
            var conf = new ProjectViewModel(new AuthViewModule())
            {
                Servers = new ObservableCollection<ServerViewModel>
                {
                    new ServerViewModel(new Dictionary<string, string>
                    {
                        {"1", "2"}
                    })
                    {
                        Address = "827r09er8wt",
                        Name = "dfghfdhsfghbsdykjfyu",
                    },

                    new ServerViewModel(new Dictionary<string, string>
                    {
                        {"111111111", "222222222"}
                    })
                    {
                        Address = "address",
                        Name = "name",
                    },
                },
                ProjectSite = "dfgdfgdfg",
            };

            conf.Servers.ForEach(x => x.ClientUries.Add("uri"));

            var text = conf.ToJson().ToString(Formatting.None);

            var model = JsonConvert.DeserializeObject<ProjectConfig>(text);

            var copy = JsonConvert.SerializeObject(model);

            Assert.AreEqual(text, copy);
        }

        [TestMethod]
        public void DynamicProxyTest()
        {
            var project = new ProjectConfig
            {
                AuthModuleSettings = new AuthModuleSettings
                {
                    Type = ModuleTypes.Default,
                    AuthUri = "123",
                    StrictUsage = true
                }
            };

            var json = JsonConvert.SerializeObject(project, Formatting.Indented);

            var jsonObj = JObject.Parse(json);
            var imp = jsonObj.ToProxy<IProjectConfig>();
            
        }
    }
}
