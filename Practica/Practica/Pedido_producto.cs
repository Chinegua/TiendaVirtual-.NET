//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Practica
{
    using System;
    using System.Collections.Generic;
    
    public partial class Pedido_producto
    {
        public int Id { get; set; }
        public Nullable<int> Id_pedido { get; set; }
        public Nullable<int> Id_producto { get; set; }
    
        public virtual Pedido Pedido { get; set; }
        public virtual Producto Producto { get; set; }
    }
}
