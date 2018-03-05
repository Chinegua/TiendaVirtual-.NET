using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Practica.Models.Binder
{
    public class CarritoModelBinder : IModelBinder
    {
        private string id_carrito_session = "ID_session_car";

        public object BindModel(ControllerContext mbctx, ModelBindingContext bctx)
        {
            CarritoCompra cc = (CarritoCompra)mbctx.HttpContext.Session[id_carrito_session];

            if (cc == null)
            {
                cc = new CarritoCompra();
                mbctx.HttpContext.Session[id_carrito_session] = cc;
            }

            return cc;
        }
    }
}