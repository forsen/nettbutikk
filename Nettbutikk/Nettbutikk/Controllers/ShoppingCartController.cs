﻿using Nettbutikk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nettbutikk.Controllers
{
    public class ShoppingCartController : Controller
    {
        // GET: ShoppingCart
        public ActionResult viewShoppingCart()
        {
            ShoppingCart cart = getCart();
            if (cart != null)
                return View(cart);
            else
                return RedirectToAction("Frontpage", "Main");

        }

        

        //TODO: Få metoden til å fungere
        public ActionResult removeItem(int quantity, int itemnumber)
        {
            var db = new DBProduct();
            Product p = db.get(itemnumber);
            ShoppingCart cart = getCart();
            if (cart == null)
                return Json(false);
            ShoppingCartItem item = new ShoppingCartItem(p, quantity);
            List<ShoppingCartItem> list = cart.shoppingCartItems;
            cart.sum += p.price * quantity;
            list.Remove(item);

            return RedirectToAction("viewShoppingCart");
        }

        [HttpPost]
        public ActionResult addToCart(int quantity, int itemnumber)
        {
            var db = new DBProduct();
            Product p = db.get(itemnumber);
            ShoppingCart cart = getCart();
            if (cart == null)
                return Json(false);  
            ShoppingCartItem item = new ShoppingCartItem(p, quantity);
            List<ShoppingCartItem> list = cart.shoppingCartItems;
            cart.sum += p.price * quantity;
            list.Add(item);


            //TODO hva skal returneres hvor her?
            return RedirectToAction("viewShoppingCart");
        }
 
        private ShoppingCart getCart()
        {
            if (Session["loggedInUser"] != null)
            {
                Customer c = (Customer)Session["loggedInUser"];
                ShoppingCart cart = c.shoppingcart;
                return cart;
            }
            return null;
        }

        

        public ActionResult checkout()
        {
            ViewBag.Empty = false; 
            ShoppingCart cart = getCart();
            if (cart == null)
            {
                ViewBag.Empty = true;
                return View();
            }

            var orderDB = new DBOrder();
            orderDB.checkout(cart);
            
            var returnCustomer= (Customer) Session["LoggedInUser"];
            returnCustomer.shoppingcart = new ShoppingCart(returnCustomer.id);
            Session["LoggedInUser"] = returnCustomer; 

            return View(new BillingDocument()
            {
                customer = returnCustomer,
                shoppingcart = cart,
                order = new Order()
                {
                    customerid = returnCustomer.id,
                    orderdate = DateTime.Now
                },
                sum = cart.sum, 
                exmva = cart.sum * 0.8,
                mva = cart.sum * 0.2
            }); 

        }
    
public  int quantity { get; set; }}
}