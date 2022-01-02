﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pokladnaInitial
{
    public class Product
    {
        public string Barcode { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class BoughtProduct
    {
        public string Barcode { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public float TotalPrice { get; set; }
    }
}
