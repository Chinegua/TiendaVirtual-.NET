using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Practica;
using Practica.Models;

namespace Practica.Controllers
{
    public class ProductoesController : Controller
    {
        private practica1Entities3 db = new practica1Entities3();

        // GET: Productoes
        public ActionResult Index()
        {
            return View(db.Producto.ToList());
        }

        // GET: Productoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Producto.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // GET: Productoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Productoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nombre,Descripcion,Precio,Imagen,Cantidad")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                db.Producto.Add(producto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(producto);
        }

        // GET: Productoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Producto.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // POST: Productoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nombre,Descripcion,Precio,Imagen,Cantidad")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                db.Entry(producto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(producto);
        }

        // GET: Productoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Producto.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // POST: Productoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Producto producto = db.Producto.Find(id);
            db.Producto.Remove(producto);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult AddCarrito(int id,CarritoCompra cc)
        {
            TempData["items"] = cc.Count() ;
            Producto p = db.Producto.Find(id);
            if (p.Cantidad > 0)
            {
                cc.Add(p);
            }
           
            return RedirectToAction("Index");


        }
        public ActionResult Carrito( CarritoCompra cc)
        {
            var suma = 0;
            return View(cc);

        }

        public ActionResult DeleteCarrito(int id, CarritoCompra cc)
        {
            Producto p = cc.Find(x => x.Id == id);
            cc.Remove(p);
            return RedirectToAction("Carrito");
        }
        [HttpPost]
        public ActionResult ProcesarCarrito(CarritoCompra cc)
        {
            /*Pedido pedido = new Pedido();
            db.Pedido.Add(pedido);
            foreach (Producto producto in cc)
            {
                Pedido_producto pedidoProducto = new Pedido_producto();
                pedidoProducto.Id_pedido = pedido.Id;
                pedidoProducto.Id_producto = producto.Id;
                db.Pedido_producto.Add(pedidoProducto);

            }
            db.SaveChanges();*/
            return RedirectToAction("Index");
        }
        [ChildActionOnly]
        public ActionResult Child()
        {
            var model = new Usuario();
            return PartialView("ProcesarCarrito",model);
        }
        [HttpPost]
        public ActionResult Child(CarritoCompra cc)
        {
            Usuario usuario = new Usuario();
            NameValueCollection nvc = Request.Form;
            var DNI = nvc["DNI"];
            var user = (from u in db.Usuario
                             where u.DNI.Equals(DNI)
                             select u).FirstOrDefault();

            if (user == null ) {
                usuario.DNI = nvc["DNI"];
                usuario.Nombre = nvc["Nombre"];
                usuario.Direccion = nvc["Direccion"];
                usuario.Email = nvc["Email"];
                db.Usuario.Add(usuario);
                db.SaveChanges();
            }

            var usuarioID = (from u in db.Usuario
                            where u.DNI.Equals(DNI)
                            select u).First();
            Pedido pedido = new Pedido();
            pedido.Usuario = usuarioID.Id;
            Random r = new Random();
            pedido.Nombre =( r.Next(0, 100000000).ToString());
            db.Pedido.Add(pedido);

            foreach (Producto producto in cc)
            {
                Pedido_producto pedidoProducto = new Pedido_producto();
                pedidoProducto.Id_pedido = pedido.Id;
                pedidoProducto.Id_producto = producto.Id;
                db.Pedido_producto.Add(pedidoProducto);
                var query = (from p in db.Producto
                                 where p.Id.Equals(producto.Id)
                                 select p).First();
                query.Cantidad = query.Cantidad - 1;
                
            }
            db.SaveChanges();
            var pedidoID = (from u in db.Usuario
                             where u.DNI.Equals(DNI)
                             join ped in db.Pedido on u.Id equals ped.Usuario
                             orderby ped.Id descending
                            select ped).First();
            SendMessage(pedidoID.Nombre, usuarioID.Email);

            cc.Clear();




            return RedirectToAction("Index");
        }

        public void SendMessage (string pedido,string mail)
        {
            MailMessage email = new MailMessage();
            email.To.Add(new MailAddress(mail));
            email.From = new MailAddress("bananacomputershop@gmail.com");
            email.Subject = "SU PEDIDO SE ESTA PREPARANDO ( " + DateTime.Now.ToString("dd / MMM / yyy hh:mm:ss") + " ) ";
            email.Body = "Tu pedido nº " + pedido + " está siendo preparado, puedes consultar si estado desde nuestra web. " ;
            email.IsBodyHtml = true ;
            email.Priority = MailPriority.Normal;

            SmtpClient smtp = new SmtpClient();
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(email.From.Address, "banana1234");
            smtp.Host = "smtp.gmail.com";

            string output = null;
            smtp.Send(email);

        }



    }
}
