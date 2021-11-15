using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductShop.DTO_S
{
    public class UserOutputDTO
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public ICollection<SoldProductDTO> SoldProducts { get; set; }
    }
}
