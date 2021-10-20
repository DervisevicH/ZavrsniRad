using System;
using System.Collections.Generic;

namespace TDSalon.Data
{
    public partial class KorisnickiNalozi
    {
        public KorisnickiNalozi()
        {
            AutorizacijskiTokeni = new HashSet<AutorizacijskiTokeni>();
            Kupci = new HashSet<Kupci>();
            Zaposlenici = new HashSet<Zaposlenici>();
        }

        public int KorisnickiNalogId { get; set; }
        public string Username { get; set; }
        public string PasswordSalt { get; set; }
        public string PasswordHash { get; set; }
        public bool? IsAktivan { get; set; }

        public virtual ICollection<AutorizacijskiTokeni> AutorizacijskiTokeni { get; set; }
        public virtual ICollection<Kupci> Kupci { get; set; }
        public virtual ICollection<Zaposlenici> Zaposlenici { get; set; }
    }
}
