using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDSalon.Data;

namespace TDSalon.Web.Helper
{
    public static class NarudzbaExtension
    {
        public static int GenerisiBrojNarudzbe(Narudzbe posljednjaNarudzba) {
            int nmb = posljednjaNarudzba.BrojNarudzbe.Value+1;

            //var datum = System.DateTime.Today.Date.ToString("ddMMyyyy");
            //if (posljednjaNarudzba != null)
            //{
            //    var brojZadnjeNarudzbe = posljednjaNarudzba.BrojNarudzbe.ToString();
            //    var posljednjiBroj = brojZadnjeNarudzbe.Substring(8);
            //    int number;
            //    bool isParsable = Int32.TryParse(posljednjiBroj, out number);

            //    if (isParsable)
            //    { number++;
            //        var stringNumber = number.ToString();
            //        var newNumber = datum + stringNumber;
            //        int generisaniBroj;
            //        bool isBroj = Int32.TryParse(newNumber, out generisaniBroj);

            //        if (isBroj)
            //        {
            //            return generisaniBroj;
            //        }
            //        else
            //        {
            //            return 0;
            //        }
            //    }
            //    else
            //        return 0;
            //}
            //else
            //{
            //    DateTime.Now.toi
            //    return Int32.Parse(datum)+1;
            //}
            return nmb;
        }

    }
}
