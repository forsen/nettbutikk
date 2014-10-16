﻿using Nettbutikk.Model;
using System;
using System.Collections.Generic;
namespace Nettbutikk.DAL
{
    public interface IProductDAL
    {
        Product get(int id);
        List<Product> getAll();
        List<Product> getAll(int? id, int? sort);
        List<Product> getAll(int? id, string sc, int? sort);
        List<string> getAutoComplete(string term);
    }
}
