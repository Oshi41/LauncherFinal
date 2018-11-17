using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LauncherFinal.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LauncherTests
{
    [TestClass]
    public class PathHelperTests
    {
        [TestMethod]
        public void GetJumpsToRoot()
        {
            var path =
                @"H:\Users\Tom\AppData\Roaming\.minecraft\libraries\net\minecraftforge\forge\1.10.2-12.18.3.2185";
            Assert.AreEqual(PathsHelper.GetJumpsToRoot(path), 10);

            path = "H:";
            Assert.AreEqual(PathsHelper.GetJumpsToRoot(path), 0);

            path = @"H:\10";
            Assert.AreEqual(PathsHelper.GetJumpsToRoot(path), 1);
        }

        [TestMethod]
        public void TestShortestName()
        {
            var paths = new List<string>
            {
                @"H:\Users\Tom\AppData\Roaming\.minecraft\libraries\net\minecraft\launchwrapper\1.12\launchwrapper-1.12.jar",
                @"H:\Users\Tom\AppData\Roaming\.minecraft\libraries\net\minecraft\launchwrapper\1.12\launchwrapper.jar",
                @"H:\Users\Tom\AppData\Roaming\.minecraft\libraries\net\minecraft\1.12\launchwrapper-1.12.jar",
                @"H:\Users\Tom\AppData\Roaming\\net\minecraft\launchwrapper\1.12\launchwrapper-1.12.jar",
                @"H:\launchwrapper-1.12.jar",
                @"H:\Users\Tom\AppData\Roaming\\net\minecraft\launchwrapper\1.12.jar",
            };

            Assert.IsTrue(string.Equals(PathsHelper.GetShortestName(paths), paths.LastOrDefault()));
        }

        [TestMethod]
        public void TestContainsInDir()
        {
            var paths = new List<string>
            {
                @"H:\Users\Tom\AppData\Roaming\.minecraft\mods\bdlib-1.12.4.25-mc1.10.2.jar",
                @"H:\Users\Tom\AppData\Roaming\.minecraft\mods\bdlib-1.12.4.25-mc1.10.2.jar",
                @"H:\Users\Tom\AppData\Roaming\.minecraft\mods\fghfgh\bdlib-1.12.4.25-mc1.10.2.jar",
                @"H:\Users\Tom\AppData\Roaming\.minecraft\mods\fghfgh\\fghfghfg\hfghfg\hfghbdlib-1.12.4.25-mc1.10.2.jar",
                @"H:\Users\Tom\AppData\Roaming\.minecraft\mods\bdlib-1.12.fghfgh4.25-mc1.10.2.jar",
                @"H:\.minecraft\mods\bdlib-1.12.fgh4.25-mc1.10.2.jar",

            };

            foreach (var path in paths)
            {
                Assert.IsTrue(PathsHelper.PlacedInFolder(path, "mods"));
            }
        }

        [TestMethod]
        public void TestNotContainsInDir()
        {
            var paths = new List<string>
            {
                @"H:\Users\Tom\AppData\Roaming\.minecraft\bdlib-1.12.4.25-mc1.10.2.jar",
                @"H:\Users\Tom\AppData\Roaming\.minecraft\bdlib-1.12.4.25-mc1.10.2.jar",
                @"H:\Users\Tom\AppData\Roaming\.minecraft\fghfgh\bdlib-1.12.4.25-mc1.10.2.jar",
                @"H:\Users\Tom\AppData\Roaming\.minecraft\fghfgh\\fghfghfg\hfghfg\hfghbdlib-1.12.4.25-mc1.10.2.jar",
                @"H:\Users\Tom\AppData\Roaming\.minecraft\bdlib-1.12.fghfgh4.25-mc1.10.2.jar",
                @"H:\.minecraft\bdlib-1.12.fgh4.25-mc1.10.2.jar",
                @"H:\Users\Tom\AppData\Roaming\.minecraft\MODS\bdlib-1.12.fghfgh4.25-mc1.10.2.jar",

            };

            foreach (var path in paths)
            {
                Assert.IsFalse(PathsHelper.PlacedInFolder(path, "mods"));
            }
        }

        [TestMethod]
        public void TestContainsIgnoreCaseInDir()
        {
            var paths = new List<string>
            {
                @"H:\Users\Tom\AppData\Roaming\.minecraft\MODS\bdlib-1.12.4.25-mc1.10.2.jar",
                @"H:\Users\Tom\AppData\Roaming\.minecraft\MODS\bdlib-1.12.4.25-mc1.10.2.jar",
                @"H:\Users\Tom\AppData\Roaming\.minecraft\mOdS\fghfgh\bdlib-1.12.4.25-mc1.10.2.jar",
                @"H:\Users\Tom\AppData\Roaming\.minecraft\modS\fghfgh\\fghfghfg\hfghfg\hfghbdlib-1.12.4.25-mc1.10.2.jar",
                @"H:\Users\Tom\AppData\Roaming\.minecraft\MoDS\bdlib-1.12.fghfgh4.25-mc1.10.2.jar",
                @"H:\.minecraft\mods\bdlib-1.12.fgh4.25-mc1.10.2.jar",

            };

            foreach (var path in paths)
            {
                Assert.IsTrue(PathsHelper.PlacedInFolder(path, "mods", StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
