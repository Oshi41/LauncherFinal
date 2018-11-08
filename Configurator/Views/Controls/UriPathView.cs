using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Configurator.Models;

namespace Configurator.Views.Controls
{
    class UriPathView : BaseUriView
    {
        private readonly Pinger _pinger = new Pinger();

        public UriPathView() 
            : base(null, null)
        {
            Action = s => _pinger.CheckPing(s);
            CanPing = s => _pinger.CanPing(s);

            ButtonHint = "Проверить адрес";
        }
    }
}
