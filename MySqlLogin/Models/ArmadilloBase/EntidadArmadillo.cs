using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MySqlLogin.Models
{
    public class EntidadArmadillo
    {
        [Key]
        public int ID_ENTIDAD { get; set; }

        public string RUT_ENTIDAD { get; set; }

        public string CLAVE_ENTIDAD { get; set; }

        public string USUARIO_ENTIDAD { get; set; }

        public string ENT_SALT { get; set; }

        public string ENT_ENC { get; set; }

        public bool ENT_ACTIVANDO { get; set; }

        public bool ENT_RESTABLECIMIENTO { get; set; }

        public string ENT_EMAIL { get; set; }

        public int ENT_TIPO { get; set; }

        public string ENT_REFERER { get; set; }
    }
}
