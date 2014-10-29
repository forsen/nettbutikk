﻿using Nettbutikk.BLL;
using Nettbutikk.admin.Models;
using Nettbutikk.Model; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using PagedList;


namespace Nettbutikk.admin.Controllers
{
    public class CustomerController : Controller
    {
        private ICustomerBLL _customerbll;

        public CustomerController()
        {
            _customerbll = new CustomerBLL();
        }

        public CustomerController(CustomerBLL stud)
        {
            _customerbll = stud;
        }

        public ActionResult ListCustomers(int? page, int? itemsPerPage, string sortOrder, string currentFilter, string searchString)
        {
            if (!isAdmin())
                return RedirectToAction("LogIn", "Main");
            ViewBag.CurrentSort = sortOrder;
            ViewBag.IDSortParm = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewBag.FirstNameSortParm = sortOrder == "FName" ? "fname_desc" : "FName";
            ViewBag.LastNameSortParm = sortOrder == "LName" ? "lname_desc" : "LName";
            ViewBag.AddressSortParm = sortOrder == "Address" ? "address_desc" : "Address";
            ViewBag.EMailSortParm = sortOrder == "EMail" ? "email_desc" : "EMail";
            ViewBag.PostAreaSortParm = sortOrder == "PArea" ? "parea_desc" : "PArea";
            ViewBag.PostCodeSortParm = sortOrder == "PCode" ? "pcode_desc" : "PCode";

            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentFilter = searchString;

            List<Customer> allCustomers;

            if (!String.IsNullOrEmpty(searchString))
            {
                allCustomers = _customerbll.getResult(searchString); 
            }
            else
                allCustomers = _customerbll.getAll(); 
            
            switch(sortOrder)
            {
                case "id_desc":
                    allCustomers = allCustomers.OrderByDescending(s => s.id).ToList();
                    break;
                case "FName":
                    allCustomers = allCustomers.OrderBy(s => s.firstname).ToList();
                    break;
                case "fname_desc":
                    allCustomers = allCustomers.OrderByDescending(s => s.firstname).ToList();
                    break;
                case "LName":
                    allCustomers = allCustomers.OrderBy(s => s.lastname).ToList();
                    break;
                case "lname_desc":
                    allCustomers = allCustomers.OrderByDescending(s=> s.lastname).ToList();
                    break;
                case "Address":
                    allCustomers = allCustomers.OrderBy(s => s.address).ToList();
                    break;
                case "address_desc":
                    allCustomers = allCustomers.OrderByDescending(s => s.address).ToList();
                    break;
                case "EMail":
                    allCustomers = allCustomers.OrderBy(s=>s.email).ToList();
                    break;
                case "email_desc":
                    allCustomers = allCustomers.OrderByDescending(s => s.email).ToList();
                    break;
                case "PArea":
                    allCustomers = allCustomers.OrderBy(s => s.postalarea).ToList();
                    break;
                case "parea_desc":
                    allCustomers = allCustomers.OrderByDescending(s => s.postalarea).ToList();
                    break;
                case "PCode":
                    allCustomers = allCustomers.OrderBy(s => s.postalcode).ToList();
                    break;
                case "pcode_desc":
                    allCustomers = allCustomers.OrderByDescending(s => s.postalcode).ToList(); 
                    break;
                default:
                    allCustomers = allCustomers.OrderBy(s => s.id).ToList(); 
                    break; 
            }

            ViewBag.CurrentItemsPerPage = itemsPerPage;

            List<UserInfo> list = new List<UserInfo>();
            foreach(var item in allCustomers)
            {
                list.Add(
                    new UserInfo()
                    {
                        id = item.id,
                        firstname = item.firstname,
                        lastname = item.lastname,
                        email = item.email,
                        address = item.address,
                        phonenumber = item.phonenumber,
                        password = item.password,
                        hashpassword = item.hashpassword,
                        postalarea = item.postalarea,
                        postalcode = item.postalcode,
                        admin = item.admin

                        
                    });
                

             
            }
            return View(list.ToPagedList(pageNumber: page ?? 1, pageSize: itemsPerPage ?? 15));
        }

        // GET: Customer
        public ActionResult Index()
        {
            if (!isAdmin())
                return RedirectToAction("LogIn", "Main"); 
            return View();
        }
        private bool isAdmin(){
            if (Session == null)
            {
                Debug.WriteLine("her?");
                return false;
            }
            var user = (Customer) Session["loggedInUser"];
            return (user == null)?false:user.admin; 
        }

        [HttpGet]
        public ActionResult  makeAdmin(int id)
        {
            Customer c = (Customer)Session["loggedInUser"];
            var b =  _customerbll.makeAdmin(id, c.id);
            return RedirectToAction("CustomerDetails", new { id = id});
        }

        [HttpGet]
        public ActionResult revokeAdmin(int id)
        {
            Customer c = (Customer)Session["loggedInUser"];
            var b = _customerbll.revokeAdmin(id,c.id);
            return RedirectToAction("CustomerDetails", new { id = id });
        }

        public ActionResult CustomerDetails(int id)
        {
            if (!isAdmin())
            {
                return RedirectToAction("LogIn", "Main");
            }
            Customer customerDetails = _customerbll.findCustomer(id);

            CustomerDetail custinfo = new CustomerDetail()
            {
                id = customerDetails.id,
                firstname = customerDetails.firstname,
                lastname = customerDetails.lastname,
                email = customerDetails.email,
                address = customerDetails.address,
                postalarea = customerDetails.postalarea,
                postalcode = customerDetails.postalcode,
                phonenumber = customerDetails.phonenumber,
                admin = customerDetails.admin
            };
            return View(custinfo);
        }

        public ActionResult ListCustomerOrders(int id)
        {
             if (!isAdmin())
                return RedirectToAction("LogIn", "Main");

            IOrderBLL _orderbll = new OrderBLL();
           List<Order> orders =  _customerbll.getAllOrdersbyCust(id);
           
            List<OrderViewModel> list = new List<OrderViewModel>();

             foreach (var item in orders)
             {
                list.Add(new OrderViewModel()
                {
                    id = item.id,
                    orderdate = item.orderdate,
                    customerid = item.customerid,
                    customer = item.customer,
                    quantity = _orderbll.getNumItems(item),
                    sum = _orderbll.getSum(item)
                });
            }
           
            return View(list);
        }

        public ActionResult ListCustomersOrderLines(int id)
        {
            if (!isAdmin())
                return RedirectToAction("Main", "Main");
            IOrderBLL _orderbll = new OrderBLL();
            List<OrderLineViewModel> list = new List<OrderLineViewModel>();

            List<Order> allOrders = _orderbll.getAllOrders(id);
            int linje = 1;
            foreach (var item in allOrders)
            {

                foreach (var olItem in item.orderLine)
                {

                    list.Add(new OrderLineViewModel()
                    {
                        id = linje,
                        customer = item.customer,
                        orderdate = item.orderdate,
                        orderId = item.id,
                        product = olItem.product,
                        quantity = olItem.quantity,
                        orderlineSum = (olItem.quantity * olItem.product.price),
                        customerid = item.customerid

                    });
                    linje++;
                }
            }
            return View(list);

        }

        public ActionResult AddCustomer()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddCustomer(Customer c)
        {
            Customer a =(Customer) Session["loggedInUser"];
            _customerbll.addCustomer(c, a.id);
            return RedirectToAction("ListCustomers");
        }

        public ActionResult delete(int id)
        {
            Customer a = (Customer)Session["loggedInUser"];
            var b =_customerbll.delete(id, a.id); 
            return RedirectToAction("ListCustomers");
 
        }
    }
}