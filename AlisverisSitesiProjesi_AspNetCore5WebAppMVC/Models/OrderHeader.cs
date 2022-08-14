using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AlisverisSitesiProjesi_AspNetCore5WebAppMVC.Models
{
    public class OrderHeader
    {
        [Key]
        public int OrderHeaderID { get; set; }


        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        public DateTime OrderDate { get; set; }
        public double OrderTotal { get; set; }
        public int OrderStatus { get; set; }

        [Required]
        public int Name { get; set; }

        [Required]
        public int Surname { get; set; }

        [Required]
        public int PhoneNumber { get; set; }

        [Required]
        public int Adres { get; set; }

        [Required]
        public int MyProSemtperty { get; set; }

        [Required]
        public int Sehir { get; set; }

        [Required]
        public int PostaKodu { get; set; }

    }
}
