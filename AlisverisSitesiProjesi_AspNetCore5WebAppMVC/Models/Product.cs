using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AlisverisSitesiProjesi_AspNetCore5WebAppMVC.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public string Color { get; set; }

        [Required]
        public string Size { get; set; }
        public string Image { get; set; }
        public bool IsHome { get; set; }        //Anasayfada olsun mu olmasın mı?
        public bool IsStock { get; set; }       //Stokta mı değil mi?


        public int KategoriID { get; set; }

        [ForeignKey("KategoriID")]
        public Category Category { get; set; }
    }
}
