using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;

namespace Domain.Models
{
    public class OrderFileInput
    {
        [Name("Documento")]
        public string Document { get; set; }
        [Name("Razão Social")]
        public string CorporateName { get; set; }
        [Name("CEP")]
        public string ZipCode { get; set; } 
        [Name("Produto")]
        public string Product { get; set; }
        [Name("Número do pedido")]
        public string OrderNumber { get; set; }
        [Name("Data")]
        public DateTime DateOrdered { get; set; }
    }
}