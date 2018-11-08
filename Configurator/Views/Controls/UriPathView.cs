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

        protected UriPathView() 
            : base(null, null)
        {
            _action = s => _pinger.CheckPing(s);
            _canPing = s => _pinger.CanPing(s);

            ButtonToolTip = "Проверить адрес";
        }
    }
}
