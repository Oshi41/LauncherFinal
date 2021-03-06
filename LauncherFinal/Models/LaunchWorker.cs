﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using Core.Models;
using Core.Settings;
using LauncherFinal.Models.AuthModules;
using LauncherFinal.Models.LaunchWorkers;
using LauncherFinal.ViewModels.PopupViewModels;

namespace LauncherFinal.Models
{
    public class LaunchWorker
    {
        #region Fields

        private readonly AuthModuleFactory _factory = new AuthModuleFactory();
        private readonly ISettings _settings;
        private readonly IServer _server;
        private readonly ModuleTypes _module;
        private readonly string _login;
        private readonly SecureString _pass;
        private readonly string _baseFolder;
        private readonly HashCalculator _hashChecker = new HashCalculator();

        #endregion

        public LaunchWorker(ISettings settings,
            IServer server,
            ModuleTypes module,
            string login,
            SecureString pass)
        {
            _settings = settings;
            _server = server;
            _module = module;
            _login = login;
            _pass = pass;

            if (_server == null || _settings == null)
                throw new ArgumentNullException("Нет настроек, либо сервер не выбран");

            _baseFolder = Path.Combine(_settings.ClientFolder, server.Name);
        }

        #region Main launch

        public async Task<string> Launch(EventHandler<bool> onExitCallBack)
        {
            if (!IsFolderExists())
            {
                var donloadInfo = await SuccefullyDownloaded(DialogHostNames.WorkerDialogName);
                if (donloadInfo.Key)
                {
                    return "Ошибка в скачивании клиентских файлов";
                }

                if (!await SuccefullyUnzipped(DialogHostNames.WorkerDialogName,
                    donloadInfo.Value))
                {
                    return "Ошибка при распаковке файлов";
                }
            }

            if (!IsRightMD5())
            {
                var canReinstall = await MessageService.ShowDialog(
                    "Хэш-сумма папки не совпдает, требуется переустановка.\nПродолжить?",
                    false);

                if (canReinstall == true)
                {
                    Directory.Delete(_baseFolder, true);
                    // заного запускаю метод
                    return await Launch(onExitCallBack);
                }

                return "Хэш-сумма файлов не одинакова";
            }

            var tokenInfo = await GetAccessToken();
            if (!tokenInfo.Key)
            {
                return "Ошибка регистрации";
            }

            try
            {
                var launcher = new ForgeLaunchWorkerNew(_baseFolder,
                    _settings.JavaPath,
                    _login,
                    tokenInfo.Value,
                    _settings.OptimizeJava);

                launcher.RegularLaunch(onExitCallBack);

                Application.Current.MainWindow?.Hide();
            }
            catch (Exception e)
            {
                Trace.Write(e);
                return e.Message;
            }

            return null;
        }

        #endregion

        #region Methods

        bool IsFolderExists()
        {
            // папка существует и, вроде бы, не пустая
            return Directory.Exists(_baseFolder)
                   && Directory.GetFiles(_baseFolder).Any();
        }

        bool IsRightMD5()
        {
            foreach (var hash in _server.DirHashCheck)
            {
                var path = Path.GetFullPath(Path.Combine(_baseFolder, hash.Key));
                var calculatedHash = _hashChecker.CreateMd5ForFolder(path);
                if (!string.Equals(calculatedHash, hash.Value))
                {
                    return false;
                }
            }

            return true;
        }

        async Task<KeyValuePair<bool, List<string>>> SuccefullyDownloaded(string hostName)
        {
            var vm = new DownloadViewModel(hostName, _server.DownloadLink);
            var filePath = await vm.Start();

            return new KeyValuePair<bool, List<string>>(vm.IsError, filePath.Files);
        }

        async Task<bool> SuccefullyUnzipped(string hostName, List<string> zip)
        {
            foreach (var path in zip)
            {
                var compress = new CompressionViewModel(hostName, path, _baseFolder, true, true, true);
                if (!await compress.Extract())
                    return false;
            }

            return true;
        }

        async Task<KeyValuePair<bool, string>> GetAccessToken()
        {
            var module = _factory.GetByType(_module);
            return await module.GenerateToken(_login, _pass);
        }

        #endregion
    }
}
