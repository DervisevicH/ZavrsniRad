using AutoMapper;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TDSalon.Data;
using TDSalon.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TDSalon.Web.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProizvodiUrediVM, Proizvodi>()
                .ForMember(x => x.Stanje, opt => opt.MapFrom(s=>s.Kolicina)).ReverseMap();
            CreateMap<ProizvodiUrediVM, ProizvodiDetalji>().ReverseMap();
            CreateMap<Akcije, AkcijeVM>()
                .ForMember(x => x.DatumOd, opt => opt.MapFrom(s => s.DatumOd.Value.ToString("dd/MM/yyyy")))
                .ForMember(x => x.DatumDo, opt => opt.MapFrom(s => s.DatumDo.Value.ToString("dd/MM/yyyy")));
            CreateMap<Akcije, AkcijeUrediVM>();
            CreateMap<AkcijeProizvodi, AkcijeProizvodiVM>()
                .ForMember(x => x.Naziv, opt => opt.MapFrom(s => s.Proizvod.ProizvodDetalji.Naziv))
                .ForMember(x => x.Sifra, opt => opt.MapFrom(s => s.Proizvod.Sifra));
            CreateMap<AkcijeProizvodiVM, AkcijeProizvodi>();
            CreateMap<Narudzbe, NarudzbeIndexVM>()
                .ForMember(x => x.Kupac, opt => opt.MapFrom(x => x.Kupac.Ime + " " + x.Kupac.Prezime));
            CreateMap<Narudzbe, NarudzbaDetaljiVM>()
                .ForMember(x => x.Kupac, opt => opt.MapFrom(x => x.Kupac.Ime + " " + x.Kupac.Prezime))
                .ForMember(x => x.Adresa, opt => opt.MapFrom(x => x.Kupac.Adresa))
                .ForMember(x => x.Status, opt => opt.MapFrom(x => x.StatusNarudzbe))
                .ForMember(x => x.Telefon, opt => opt.MapFrom(x => x.Kupac.Telefon));
            CreateMap<Dobavljaci, DobavljaciVM>().ReverseMap();
            CreateMap<Pitanja, PitanjaVM>()
                .ForMember(x => x.ImePrezime, opt => opt.MapFrom(s => s.Kupac.KorisnickiNalog.Username))
                .ForMember(x => x.ProizvodNaziv, opt => opt.MapFrom(s => s.Proizvod.ProizvodDetalji.Naziv));
            CreateMap<Proizvodi, SelectListItem>()
                .ForMember(x => x.Text, opt => opt.MapFrom(s => s.ProizvodDetalji.Naziv))
                .ForMember(x => x.Value, opt => opt.MapFrom(s => s.ProizvodId.ToString()));
            CreateMap<Kupci, KupciVM>()
                .ForMember(x => x.ImePrezime, opt => opt.MapFrom(x => x.Ime + " " + x.Prezime))
                .ForMember(x => x.Grad, opt => opt.MapFrom(x => x.Grad.Naziv))
                .ForMember(x => x.IsAktivan, opt => opt.MapFrom(x => x.KorisnickiNalog.IsAktivan));
            CreateMap<ObavijestiDodajVM, Obavijesti>().ReverseMap();
            CreateMap<Obavijesti, ObavijestiVM>().ReverseMap();
            CreateMap<Obavijesti, ObavijestiVM>().ReverseMap();
            CreateMap<Odgovori, OdgovoriDodajVM>().ReverseMap();
            CreateMap<Zaposlenici, ZaposlenikVM>().ReverseMap();
            CreateMap<Kupci, ProfilVM>().ReverseMap();
            CreateMap<Kupci, KupciDodajInfoVM>().ReverseMap();            
        }
    }
}
