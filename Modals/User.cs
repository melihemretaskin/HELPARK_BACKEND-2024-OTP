using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modals
{
    public class User
    {

        public int ID { get; set; }

    
        public string Name { get; set; }


        public string Surname { get; set; }


        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string AccessToken {  get; set; }


        
    }
}
