﻿using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace LauncherFinal.Models
{
    public class CryptoWorker
    {
        private int _prevSalt;

        private static readonly string DeviceName = "Win32_Processor";
        private static readonly string[] Identifiers = {
            "UniqueId",
            "ProcessorId",
            "Name",
            "Manufacturer"
        };

        public CryptoWorker()
        {
            _prevSalt = (int) DateTime.Now.ToBinary();
        }

        /// <summary>
        ///     Шифруем пароль
        /// </summary>
        /// <typeparam name="T">
        ///     Любое симметричное шифрование,
        ///     я предпочитаю <see cref="AesManaged" />>
        /// </typeparam>
        /// <param name="value">Шифруемые данные</param>
        /// <param name="password">Ключ шифрования</param>
        /// <param name="salt">Зерно шифрования</param>
        /// <returns></returns>
        public string Encrypt<T>(string value, string password, string salt)
            where T : SymmetricAlgorithm, new()
        {
            if (string.IsNullOrEmpty(value)
                || string.IsNullOrEmpty(password)
                || string.IsNullOrEmpty(salt))
            {
                return string.Empty;
            }

            DeriveBytes rgb = new Rfc2898DeriveBytes(password, Encoding.Unicode.GetBytes(salt));

            SymmetricAlgorithm algorithm = new T();

            var rgbKey = rgb.GetBytes(algorithm.KeySize >> 3);
            var rgbIV = rgb.GetBytes(algorithm.BlockSize >> 3);

            var transform = algorithm.CreateEncryptor(rgbKey, rgbIV);

            using (var buffer = new MemoryStream())
            {
                using (var stream = new CryptoStream(buffer, transform, CryptoStreamMode.Write))
                {
                    using (var writer = new StreamWriter(stream, Encoding.Unicode))
                    {
                        writer.Write(value);
                    }
                }

                return Convert.ToBase64String(buffer.ToArray());
            }
        }

        /// <summary>
        ///     Дешифруем пароль
        /// </summary>
        /// <typeparam name="T">
        ///     Любое симметричное шифрование,
        ///     я предпочитаю <see cref="AesManaged" />>
        /// </typeparam>
        /// <param name="text">Шифруемые данные</param>
        /// <param name="password">Ключ шифрования</param>
        /// <param name="salt">Зерно шифрования</param>
        /// <returns></returns>
        public string Decrypt<T>(string text, string password, string salt)
            where T : SymmetricAlgorithm, new()
        {
            if (string.IsNullOrEmpty(text)
                || string.IsNullOrEmpty(password)
                || string.IsNullOrEmpty(salt))
            {
                return string.Empty;
            }

            DeriveBytes rgb = new Rfc2898DeriveBytes(password, Encoding.Unicode.GetBytes(salt));

            SymmetricAlgorithm algorithm = new T();

            var rgbKey = rgb.GetBytes(algorithm.KeySize >> 3);
            var rgbIV = rgb.GetBytes(algorithm.BlockSize >> 3);

            var transform = algorithm.CreateDecryptor(rgbKey, rgbIV);

            using (var buffer = new MemoryStream(Convert.FromBase64String(text)))
            {
                using (var stream = new CryptoStream(buffer, transform, CryptoStreamMode.Read))
                {
                    using (var reader = new StreamReader(stream, Encoding.Unicode))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        /// <summary>
        ///     Генерирует случайное зерно для шифрования
        /// </summary>
        /// <returns></returns>
        public string GetRandomSalt(int length = 32)
        {
            // используем созданныйсозданное ранее зерно
            var rand = new Random(_prevSalt);
            var buffer = new byte[length];
            rand.NextBytes(buffer);

            // генерируем новое зерно
            _prevSalt = rand.Next(int.MinValue, int.MaxValue);

            return Convert.ToBase64String(buffer);
        }

        /// <summary>
        /// Уникальная характеристика для компа
        /// </summary>
        /// <returns></returns>
        public string GetUniqueSalt()
        {
            return GetNextUniqueSalt(null);
        }

        /// <summary>
        /// Возвращает уник. характеристику компа, исключая передаваемые значения
        /// </summary>
        /// <param name="toSkip"></param>
        /// <returns></returns>
        public string GetNextUniqueSalt(params string[] toSkip)
        {
            var mbs = new ManagementClass(DeviceName);
            var mbsList = mbs.GetInstances();

            foreach (var mo in mbsList)
            {
                foreach (var identifier in Identifiers)
                {
                    var unique = mo[identifier]?.ToString();
                    if (unique != null)
                    {
                        if (toSkip?.Contains(unique) == true)
                            continue;

                        return unique;
                    }
                }
            }

            return string.Empty;
        }
    }
}
