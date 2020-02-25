using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySqlLogin.Models
{
    public class Entidad : IdentityUser
    {
        //public int Id { get; set; }
        public string RutEntidad { get; set; }
        public string ClaveEntidad { get; set; }
        public string UsuarioEntidad { get; set; }
        public string EntSalt { get; set; }
        public string EntENC { get; set; }
        public bool EntActivando { get; set; }
        public bool EntRestableciendo { get; set; }
        public string EntEmail { get; set; }
        public int EntTipo { get; set; }
        public string EntReferer { get; set; }
    }
}
