using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class Login
    {
        #region Attributs
        private long idLogin;
        private string? password;
        private Seller? user;
        #endregion

        #region Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Idlogin { get { return idLogin; } set { idLogin = value; } }
        public string? Password { get { return password; } set { password = value; } }
        public Seller? User { get { return user; } set { user = value; } }
        #endregion

        public Login() { }
    }
}
