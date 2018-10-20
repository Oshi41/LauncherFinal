using System;
using System.IO;
using LauncherFinal.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LauncherTests
{
    [TestClass]
    public class HashTest
    {
        [TestMethod]
        public void TestHash_EqualsHash()
        {
            var hashChecker = new HashChecker();
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(path);
            var file = Path.Combine(path, "file.txt");
            File.WriteAllText(file, "123456");

            var hash = hashChecker.CreateMd5ForFolder(path);

            var hash2 = hashChecker.CreateMd5ForFolder(path);

            Assert.AreEqual(hash2, hash);

            Directory.Delete(path, true);
        }

        [TestMethod]
        public void TestHash_NotEqualsHash()
        {
            var hashChecker = new HashChecker();
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(path);
            var file = Path.Combine(path, "file.txt");
            File.WriteAllText(file, "123456");

            var hash = hashChecker.CreateMd5ForFolder(path);

            File.WriteAllText(file, "123456,");

            var hash2 = hashChecker.CreateMd5ForFolder(path);

            Assert.AreNotEqual(hash2, hash);

            Directory.Delete(path, true);
        }
    }
}
