﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDSalon.Data;

namespace TDSalon.Web.Helper
{
    public static class NarudzbaExtension
    {
        public static int GenerisiBrojNarudzbe(Narudzbe posljednjaNarudzba) {

            var datum = System.DateTime.Today.Date.ToString("ddMMyyyy");
            if (posljednjaNarudzba != null)
            {
                var brojZadnjeNarudzbe = posljednjaNarudzba.BrojNarudzbe.ToString();
                var posljednjiBroj = brojZadnjeNarudzbe.Substring(7, brojZadnjeNarudzbe.Length);
                int number;
                bool isParsable = Int32.TryParse(posljednjiBroj, out number);

                if (isParsable)
                { number++;
                    var stringNumber = number.ToString();
                    var newNumber = datum + stringNumber;
                    int generisaniBroj;
                    bool isBroj = Int32.TryParse(newNumber, out generisaniBroj);

                    if (isBroj)
                    {
                        return generisaniBroj;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                    return 0;
            }
            else
            {

                return Int32.Parse(datum)+1;
            }
            return 0;
        }

    }
}