﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMDataManager.Library.Models
{
    public class InventoryDBModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public DateTime PurchaseDate { get; set; }
    }
}
