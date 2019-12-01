using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Payment
    {
        #region Attributs
        private long idPayment;
        private string paymentType;
        private float amount;
        #endregion

        #region Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdPayment
        {
            get { return idPayment; }
            set { idPayment = value; }
        }

        public string PaymentType
        {
            get { return paymentType; }
            set { paymentType = value; }
        }

        public float Amount
        {
            get { return amount; }
            set { amount = value; }
        }
        #endregion

        #region Constructors
        public Payment()
        {
        }

        //public Payment(string paymentType, float amount)
        //{
        //    PaymentType = paymentType;
        //    Amount = amount;
        //}
        #endregion
    }
}
