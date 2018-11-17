using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LauncherFinal.Models;
using LauncherFinal.Models.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LauncherTests
{
    [TestClass]
    public class LauncherTests
    {
        [TestMethod]
        public void Launch()
        {
            var minecraft = @"H:\Users\Tom\AppData\Roaming\.minecraft";
            var token = "null";
            var settings = Settings.CreateDefault();
            settings.Login = "Oshi";

            var launcher = new ForgeLaunchWorker(minecraft, token, settings.JavaPath, settings.Login, false);

            var bat = launcher.GetCmdArgs() + "\npause";
            var batPath = Path.Combine(minecraft, "start.bat");
            File.WriteAllText(batPath, bat);

            var info = new ProcessStartInfo(batPath);
            var proc = new Process{ StartInfo = info};

            var semaphore = new AutoResetEvent(false);

            proc.Exited += (sender, args) =>
            {
                Assert.IsTrue(proc.ExitCode == 0);
                semaphore.Set();
            };

            proc.Start();

            semaphore.WaitOne();
        }

        [TestMethod]
        public void LaunchTestNew()
        {
            var minecraft = @"H:\Users\Tom\AppData\Roaming\.minecraft";
            var token = "null";
            var settings = Settings.CreateDefault();
            settings.Login = "Oshi";

            var launcher = new ForgeLaunchWorkerNew(minecraft, settings.JavaPath, settings.Login, token, false);

            var bat = launcher.GetCmdArgs() + "\npause";
            var batPath = Path.Combine(minecraft, "start.bat");
            File.WriteAllText(batPath, bat);

            var info = new ProcessStartInfo(batPath);
            var proc = new Process { StartInfo = info };

            var semaphore = new AutoResetEvent(false);

            proc.Exited += (sender, args) =>
            {
                Assert.IsTrue(proc.ExitCode == 0);
                semaphore.Set();
            };

            proc.Start();

            semaphore.WaitOne();
        }
    }
}
