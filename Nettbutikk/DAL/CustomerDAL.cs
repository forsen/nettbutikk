﻿using Nettbutikk.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Nettbutikk.DAL
{
    public class CustomerDAL : DAL.ICustomerDAL
    {
        public List<Customer> getAll()
        {
            var db = new DatabaseContext();
            var customers = db.Customers.ToList();
            var list = new List<Customer>();
            foreach(var item in customers){
                list.Add(new Customer()
                {
                    id = item.Id,
                    firstname = item.Firstname,
                    lastname = item.Lastname,
                    address = item.Address,
                    email = item.Email,
                    postalcode = item.PostalareasId.ToString(), 
                    postalarea = db.Postalareas.Find(item.PostalareasId).Postalarea,
                    phonenumber = item.Phonenumber
                    
                });
            }
            return list; 
             
        }

        public Customer getCustomer(int id)
        {
            var db = new DatabaseContext();
            Customers cust = (Customers)db.Customers.FirstOrDefault(c => c.Id == id);
            Customer customer = new Customer();

            customer.id = cust.Id;
            customer.firstname = cust.Firstname;
            customer.lastname = cust.Lastname;
            customer.phonenumber = cust.Phonenumber;
            customer.email = cust.Email;
            customer.address = cust.Address;
            customer.hashpassword = cust.Password;
            customer.postalcode = cust.PostalareasId.ToString();
            

            return customer;
        }

        
        public bool add(Customer inCustomer, byte[] hashedPassword)
        {
                return true;
        }

        public Customer findCustomer(String email)
        {
            return null;
        }

        public Customer findUser(String email)
        {
            var db = new DatabaseContext();
            Customers userFound = db.Customers.FirstOrDefault(u => u.Email == email);
            Customer c = new Customer();
            c.id = userFound.Id;
            c.email = userFound.Email;
            c.hashpassword = userFound.Password;
            c.admin = true;
            return c;

        }

        public bool validate(String email, byte[] hashedPassword)
        {
            var db = new DatabaseContext();
            Customers userFound = db.Customers.FirstOrDefault(u => u.Email == email);

            if (userFound != null)
            {
                if (email.Equals(userFound.Email) && Enumerable.SequenceEqual(hashedPassword, userFound.Password))
                {
                    if (userFound.Admin == true)
                    {
                        return true;
                    }
                }
            }
            return false;   
        }

        public bool update(int id, Customer updateUser)
        {
                return false;
        }

        public bool updatePw(int id, byte[] newPassword)
        {
            return false;

        }

        public bool checkEmail(string email, int? id)
        {
     
         return false;
        }
    }
}

