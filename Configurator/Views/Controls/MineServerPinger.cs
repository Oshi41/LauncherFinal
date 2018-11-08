using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;

namespace Configurator.Views.Controls
{
    class MineServerPinger : BaseUriView
    {
        private bool _isPinging;

        protected MineServerPinger() 
            : base(null, null)
        {
            _action = async s =>
            {
                try
                {
                    _isPinging = true;

                    var stat = new ServerStat(s);
                    await stat.GetInfoAsync();
                    return stat.ServerUp;
                }
                catch (Exception e)
                {
                    Trace.Write(e);
                    return false;
                }
                finally
                {
                    _isPinging = false;
                }
            };

            _canPing = s => !_isPinging && !string.IsNullOrWhiteSpace(s);

            ButtonToolTip = "Проверить связь с сервером";
        }
    }
}
