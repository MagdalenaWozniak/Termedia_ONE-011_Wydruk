using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Wydruk
{
    public class Formaty
    {
        public string Nazwa { get; set; }
        public int Format { get; set; }
        public int Wydruk { get; set; }
        

        public Formaty(string Nazwa, int Format, int Wydruk)
        {
            this.Nazwa = Nazwa;
            this.Format = Format;
            this.Wydruk = Wydruk;
        }
    }
}
