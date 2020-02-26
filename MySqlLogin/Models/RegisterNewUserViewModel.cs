using System.ComponentModel.DataAnnotations;

namespace MySqlLogin.Models
{
    public class RegisterNewUserViewModel
    {

        public string RutEntidad { get; set; }

        [Required]
        [MinLength(6)]
        public string ClaveEntidad { get; set; }

        public string UsuarioEntidad { get; set; }

        public string EntSalt { get; set; }

        public string EntENC { get; set; }

        public bool EntActivando { get; set; }

        public bool EntRestableciendo { get; set; }

        public string EntEmail { get; set; }

        public int EntTipo { get; set; }

        public string EntReferer { get; set; }

        [Required]
        [Compare("ClaveEntidad")]
        public string ConfirmacionClaveEntidad { get; set; }


        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }
    }
}
