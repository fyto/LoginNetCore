using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MySqlLogin.Models
{
    public class Ciudad
    {
        [Key]
        public int CIU_ID { get; set; }

        public string CIU_NOMBRE { get; set; }

        public string CIU_CODIGO_PAIS { get; set; }

        public string CIU_PROVINCIA { get; set; }

        public int CIU_POBLACION { get; set; }
    }
}
