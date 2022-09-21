using System;
using System.Collections.Generic;
using System.Text;

namespace consumer.Domain
{
    public class Order
    {
        public int OrderNumber { get; set; }
        public string ItemName { get; set; }
        public float Price { get; set; }
    }
}
