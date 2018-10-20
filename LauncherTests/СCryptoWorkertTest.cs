using System.Security.Cryptography;
using LauncherFinal.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LauncherTests
{
    [TestClass]
    public class CryptoWorkertTest
    {
        [TestMethod]
        public void TestCryptoWorker_ReturnEquals()
        {
            var text = "Once upon a time there was a little boy...";
            var pass = "12345678";
            var worker = new CryptoWorker();
            var salt = worker.GetUniqueSalt();
            var encrypted = worker.Encrypt<RijndaelManaged>(text, pass, salt);

            Assert.AreEqual(salt, worker.GetUniqueSalt());

            Assert.AreEqual(text, worker.Decrypt<RijndaelManaged>(encrypted, pass, salt));
        }

        [TestMethod]
        public void TestCryptoWorker_ReturnEqualsSalt()
        {
            var worker = new CryptoWorker();

            var salt = worker.GetUniqueSalt();
            var copy = worker.GetUniqueSalt();

            Assert.AreEqual(salt, copy);
        }

        [TestMethod]
        public void TestCryptoWorker_ReturnDifferentSalt()
        {
            var worker = new CryptoWorker();

            var salt = worker.GetRandomSalt();
            var copy = worker.GetRandomSalt();

            Assert.AreNotEqual(salt, copy);
        }
    }
}
