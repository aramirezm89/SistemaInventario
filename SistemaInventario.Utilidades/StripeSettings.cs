using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaInventario.Utilidades
{
    //clase con las claves de stripe para los pagos en la aplicacion
    public class StripeSettings
    {
        public string SecretKey { get; set; }
        public string PublishableKey { get; set; }
    }
}
