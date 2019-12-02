using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayDB
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new EntitiesContext())
            {
                foreach (var item in db.Bicycles)
                {
                    Console.WriteLine(item);
                }
                Console.ReadLine();
            }
        }
    }
}
