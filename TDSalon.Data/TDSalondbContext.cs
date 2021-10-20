using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TDSalon.Data
{
    public partial class TDSalondbContext : DbContext
    {
        public TDSalondbContext()
        {
        }

        public TDSalondbContext(DbContextOptions<TDSalondbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Akcije> Akcije { get; set; }
        public virtual DbSet<AkcijeProizvodi> AkcijeProizvodi { get; set; }
        public virtual DbSet<AutorizacijskiTokeni> AutorizacijskiTokeni { get; set; }
        public virtual DbSet<Dimenzije> Dimenzije { get; set; }
        public virtual DbSet<Dobavljaci> Dobavljaci { get; set; }
        public virtual DbSet<Favoriti> Favoriti { get; set; }
        public virtual DbSet<FavoritiStavke> FavoritiStavke { get; set; }
        public virtual DbSet<Gradovi> Gradovi { get; set; }
        public virtual DbSet<JediniceMjere> JediniceMjere { get; set; }
        public virtual DbSet<Kantoni> Kantoni { get; set; }
        public virtual DbSet<Kategorije> Kategorije { get; set; }
        public virtual DbSet<Komentari> Komentari { get; set; }
        public virtual DbSet<KorisnickiNalozi> KorisnickiNalozi { get; set; }
        public virtual DbSet<KorpaProizvodi> KorpaProizvodi { get; set; }
        public virtual DbSet<Korpe> Korpe { get; set; }
        public virtual DbSet<Kupci> Kupci { get; set; }
        public virtual DbSet<NarudzbaStavke> NarudzbaStavke { get; set; }
        public virtual DbSet<Narudzbe> Narudzbe { get; set; }
        public virtual DbSet<Notifikacije> Notifikacije { get; set; }
        public virtual DbSet<Obavijesti> Obavijesti { get; set; }
        public virtual DbSet<Odgovori> Odgovori { get; set; }
        public virtual DbSet<Pitanja> Pitanja { get; set; }
        public virtual DbSet<Proizvodi> Proizvodi { get; set; }
        public virtual DbSet<ProizvodiDetalji> ProizvodiDetalji { get; set; }
        public virtual DbSet<Slike> Slike { get; set; }
        public virtual DbSet<Zaposlenici> Zaposlenici { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.;Database=TDSalondb;Trusted_Connection=True; MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Akcije>(entity =>
            {
                entity.HasKey(e => e.AkcijaId)
                    .HasName("PK__Akcije__3499D6332376EB0A");

                entity.Property(e => e.AkcijaId).HasColumnName("AkcijaID");

                entity.Property(e => e.DatumDo).HasColumnType("datetime");

                entity.Property(e => e.DatumOd).HasColumnType("datetime");

                entity.Property(e => e.Naziv).HasMaxLength(50);

                entity.Property(e => e.Postotak).HasColumnType("decimal(5, 2)");
            });

            modelBuilder.Entity<AkcijeProizvodi>(entity =>
            {
                entity.HasKey(e => e.AkcijaProizvodiId)
                    .HasName("PK__AkcijePr__FD29B5F2074094C3");

                entity.Property(e => e.AkcijaProizvodiId).HasColumnName("AkcijaProizvodiID");

                entity.Property(e => e.AkcijaId).HasColumnName("AkcijaID");

                entity.Property(e => e.AkcijskaCijena).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Cijena).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ProizvodId).HasColumnName("ProizvodID");

                entity.HasOne(d => d.Akcija)
                    .WithMany(p => p.AkcijeProizvodi)
                    .HasForeignKey(d => d.AkcijaId)
                    .HasConstraintName("FK__AkcijePro__Akcij__160F4887");

                entity.HasOne(d => d.Proizvod)
                    .WithMany(p => p.AkcijeProizvodi)
                    .HasForeignKey(d => d.ProizvodId)
                    .HasConstraintName("FK__AkcijePro__Proiz__17036CC0");
            });

            modelBuilder.Entity<AutorizacijskiTokeni>(entity =>
            {
                entity.HasKey(e => e.TokenId)
                    .HasName("PK__Autoriza__658FEE8A9E7D4037");

                entity.Property(e => e.TokenId).HasColumnName("TokenID");

                entity.Property(e => e.KorisnickiNalogId).HasColumnName("KorisnickiNalogID");

                entity.Property(e => e.Vrijeme).HasColumnType("datetime");

                entity.HasOne(d => d.KorisnickiNalog)
                    .WithMany(p => p.AutorizacijskiTokeni)
                    .HasForeignKey(d => d.KorisnickiNalogId)
                    .HasConstraintName("FK__Autorizac__Koris__6EF57B66");
            });

            modelBuilder.Entity<Dimenzije>(entity =>
            {
                entity.HasKey(e => e.DimenzijaId)
                    .HasName("PK__Dimenzij__A371632F545D94DA");

                entity.Property(e => e.DimenzijaId).HasColumnName("DimenzijaID");

                entity.Property(e => e.Debljina).HasMaxLength(20);

                entity.Property(e => e.Duzina).HasMaxLength(20);

                entity.Property(e => e.KategorijaId).HasColumnName("KategorijaID");

                entity.Property(e => e.Sirina).HasMaxLength(20);

                entity.HasOne(d => d.Kategorija)
                    .WithMany(p => p.Dimenzije)
                    .HasForeignKey(d => d.KategorijaId)
                    .HasConstraintName("FK__Dimenzije__Kateg__35BCFE0A");
            });

            modelBuilder.Entity<Dobavljaci>(entity =>
            {
                entity.HasKey(e => e.DobavljacId)
                    .HasName("PK__Dobavlja__DE87BE1996C2E46C");

                entity.Property(e => e.DobavljacId).HasColumnName("DobavljacID");

                entity.Property(e => e.KontaktOsoba).HasMaxLength(50);

                entity.Property(e => e.NazivFirme).HasMaxLength(50);

                entity.Property(e => e.Telefon).HasMaxLength(30);
            });

            modelBuilder.Entity<Favoriti>(entity =>
            {
                entity.HasKey(e => e.FavoritId)
                    .HasName("PK__Favoriti__C32DB22C3F7B69FC");

                entity.Property(e => e.FavoritId).HasColumnName("FavoritID");

                entity.Property(e => e.KupacId).HasColumnName("KupacID");

                entity.HasOne(d => d.Kupac)
                    .WithMany(p => p.Favoriti)
                    .HasForeignKey(d => d.KupacId)
                    .HasConstraintName("FK__Favoriti__KupacI__4AB81AF0");
            });

            modelBuilder.Entity<FavoritiStavke>(entity =>
            {
                entity.HasKey(e => e.FavoritStavkaId)
                    .HasName("PK__Favoriti__78D1E31956F1B397");

                entity.Property(e => e.FavoritStavkaId).HasColumnName("FavoritStavkaID");

                entity.Property(e => e.DatumDodavanja).HasColumnType("datetime");

                entity.Property(e => e.FavoritId).HasColumnName("FavoritID");

                entity.Property(e => e.ProizvodId).HasColumnName("ProizvodID");

                entity.HasOne(d => d.Favorit)
                    .WithMany(p => p.FavoritiStavke)
                    .HasForeignKey(d => d.FavoritId)
                    .HasConstraintName("FK__FavoritiS__Favor__4D94879B");

                entity.HasOne(d => d.Proizvod)
                    .WithMany(p => p.FavoritiStavke)
                    .HasForeignKey(d => d.ProizvodId)
                    .HasConstraintName("FK__FavoritiS__Proiz__4E88ABD4");
            });

            modelBuilder.Entity<Gradovi>(entity =>
            {
                entity.HasKey(e => e.GradId)
                    .HasName("PK__Gradovi__B0F3C9846CB12916");

                entity.Property(e => e.GradId).HasColumnName("GradID");

                entity.Property(e => e.KantonId).HasColumnName("KantonID");

                entity.Property(e => e.Naziv).HasMaxLength(50);

                entity.HasOne(d => d.Kanton)
                    .WithMany(p => p.Gradovi)
                    .HasForeignKey(d => d.KantonId)
                    .HasConstraintName("FK__Gradovi__KantonI__2B3F6F97");
            });

            modelBuilder.Entity<JediniceMjere>(entity =>
            {
                entity.HasKey(e => e.JedinicaMjereId)
                    .HasName("PK__Jedinice__F73C302E6E03B02C");

                entity.Property(e => e.JedinicaMjereId).HasColumnName("JedinicaMjereID");

                entity.Property(e => e.Naziv).HasMaxLength(30);
            });

            modelBuilder.Entity<Kantoni>(entity =>
            {
                entity.HasKey(e => e.KantonId)
                    .HasName("PK__Kantoni__F1D12B416CE46894");

                entity.Property(e => e.KantonId).HasColumnName("KantonID");

                entity.Property(e => e.Naziv).HasMaxLength(50);
            });

            modelBuilder.Entity<Kategorije>(entity =>
            {
                entity.HasKey(e => e.KategorijaId)
                    .HasName("PK__Kategori__6C3B8FCE9DABE6C0");

                entity.Property(e => e.KategorijaId).HasColumnName("KategorijaID");

                entity.Property(e => e.Naziv).HasMaxLength(50);
            });

            modelBuilder.Entity<Komentari>(entity =>
            {
                entity.HasKey(e => e.KomentarId)
                    .HasName("PK__Komentar__C0C304BC4146DD33");

                entity.Property(e => e.KomentarId).HasColumnName("KomentarID");

                entity.Property(e => e.Datum).HasColumnType("datetime");

                entity.Property(e => e.KupacId).HasColumnName("KupacID");

                entity.Property(e => e.ProizvodDetaljiId).HasColumnName("ProizvodDetaljiID");

                entity.HasOne(d => d.Kupac)
                    .WithMany(p => p.Komentari)
                    .HasForeignKey(d => d.KupacId)
                    .HasConstraintName("FK__Komentari__Kupac__5165187F");

                entity.HasOne(d => d.ProizvodDetalji)
                    .WithMany(p => p.Komentari)
                    .HasForeignKey(d => d.ProizvodDetaljiId)
                    .HasConstraintName("FK__Komentari__Proiz__52593CB8");
            });

            modelBuilder.Entity<KorisnickiNalozi>(entity =>
            {
                entity.HasKey(e => e.KorisnickiNalogId)
                    .HasName("PK__Korisnic__3FA446327A5C59DC");

                entity.Property(e => e.KorisnickiNalogId).HasColumnName("KorisnickiNalogID");

                entity.Property(e => e.Username).HasMaxLength(50);
            });

            modelBuilder.Entity<KorpaProizvodi>(entity =>
            {
                entity.HasKey(e => e.KorpaProizvodId)
                    .HasName("PK__KorpaPro__EE228A4CC46626F1");

                entity.Property(e => e.KorpaProizvodId).HasColumnName("KorpaProizvodID");

                entity.Property(e => e.Cijena).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Kolicina).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.KorpaId).HasColumnName("KorpaID");

                entity.Property(e => e.ProizvodId).HasColumnName("ProizvodID");

                entity.HasOne(d => d.Korpa)
                    .WithMany(p => p.KorpaProizvodi)
                    .HasForeignKey(d => d.KorpaId)
                    .HasConstraintName("FK__KorpaProi__Korpa__6477ECF3");

                entity.HasOne(d => d.Proizvod)
                    .WithMany(p => p.KorpaProizvodi)
                    .HasForeignKey(d => d.ProizvodId)
                    .HasConstraintName("FK__KorpaProi__Proiz__6383C8BA");
            });

            modelBuilder.Entity<Korpe>(entity =>
            {
                entity.HasKey(e => e.KorpaId)
                    .HasName("PK__Korpe__C298DFB33423AE48");

                entity.HasIndex(e => e.KupacId)
                    .HasName("UQ__Korpe__A9593C7AE0253B0F")
                    .IsUnique();

                entity.Property(e => e.KorpaId).HasColumnName("KorpaID");

                entity.Property(e => e.DatumModifikacije).HasColumnType("datetime");

                entity.Property(e => e.KupacId).HasColumnName("KupacID");

                entity.Property(e => e.Ukupno).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Kupac)
                    .WithOne(p => p.Korpe)
                    .HasForeignKey<Korpe>(d => d.KupacId)
                    .HasConstraintName("FK__Korpe__KupacID__02084FDA");
            });

            modelBuilder.Entity<Kupci>(entity =>
            {
                entity.HasKey(e => e.KupacId)
                    .HasName("PK__Kupci__A9593C7B73ED109E");

                entity.Property(e => e.KupacId).HasColumnName("KupacID");

                entity.Property(e => e.Adresa).HasMaxLength(50);

                entity.Property(e => e.DatumRegistracije).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.GradId).HasColumnName("GradID");

                entity.Property(e => e.Ime).HasMaxLength(50);

                entity.Property(e => e.KorisnickiNalogId).HasColumnName("KorisnickiNalogID");

                entity.Property(e => e.Prezime).HasMaxLength(50);

                entity.Property(e => e.Spol).HasMaxLength(15);

                entity.Property(e => e.Telefon).HasMaxLength(20);

                entity.HasOne(d => d.Grad)
                    .WithMany(p => p.Kupci)
                    .HasForeignKey(d => d.GradId)
                    .HasConstraintName("FK__Kupci__GradID__2F10007B");

                entity.HasOne(d => d.KorisnickiNalog)
                    .WithMany(p => p.Kupci)
                    .HasForeignKey(d => d.KorisnickiNalogId)
                    .HasConstraintName("FK__Kupci__Korisnick__2E1BDC42");
            });

            modelBuilder.Entity<NarudzbaStavke>(entity =>
            {
                entity.HasKey(e => e.StavkaId)
                    .HasName("PK__Narudzba__ACCAA8F3BE5D6F31");

                entity.Property(e => e.StavkaId).HasColumnName("StavkaID");

                entity.Property(e => e.Cijena).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Kolicina).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.NarudzbaId).HasColumnName("NarudzbaID");

                entity.Property(e => e.Popust).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.ProizvodId).HasColumnName("ProizvodID");

                entity.HasOne(d => d.Narudzba)
                    .WithMany(p => p.NarudzbaStavke)
                    .HasForeignKey(d => d.NarudzbaId)
                    .HasConstraintName("FK__NarudzbaS__Narud__6B24EA82");

                entity.HasOne(d => d.Proizvod)
                    .WithMany(p => p.NarudzbaStavke)
                    .HasForeignKey(d => d.ProizvodId)
                    .HasConstraintName("FK__NarudzbaS__Proiz__6C190EBB");
            });

            modelBuilder.Entity<Narudzbe>(entity =>
            {
                entity.HasKey(e => e.NarudzbaId)
                    .HasName("PK__Narudzbe__FBEC135777D1287F");

                entity.Property(e => e.NarudzbaId).HasColumnName("NarudzbaID");

                entity.Property(e => e.Datum).HasColumnType("datetime");

                entity.Property(e => e.Komentar).HasMaxLength(255);

                entity.Property(e => e.KupacId).HasColumnName("KupacID");

                entity.Property(e => e.StatusNarudzbe).HasMaxLength(255);

                entity.Property(e => e.TroskoviDostave).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Ukupno).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ZaposlenikId).HasColumnName("ZaposlenikID");

                entity.HasOne(d => d.Kupac)
                    .WithMany(p => p.Narudzbe)
                    .HasForeignKey(d => d.KupacId)
                    .HasConstraintName("FK__Narudzbe__KupacI__6754599E");

                entity.HasOne(d => d.Zaposlenik)
                    .WithMany(p => p.Narudzbe)
                    .HasForeignKey(d => d.ZaposlenikId)
                    .HasConstraintName("FK__Narudzbe__Zaposl__68487DD7");
            });

            modelBuilder.Entity<Notifikacije>(entity =>
            {
                entity.HasKey(e => e.NotifikacijaId)
                    .HasName("PK__Notifika__595D01C3E9A76E8D");

                entity.Property(e => e.NotifikacijaId).HasColumnName("NotifikacijaID");

                entity.Property(e => e.DatumKreiranja).HasColumnType("datetime");

                entity.Property(e => e.KupacId).HasColumnName("KupacID");

                entity.Property(e => e.Sadrzaj).HasMaxLength(255);

                entity.Property(e => e.TipNotifikacije).HasMaxLength(255);

                entity.Property(e => e.ZaposlenikId).HasColumnName("ZaposlenikID");

                entity.HasOne(d => d.Kupac)
                    .WithMany(p => p.Notifikacije)
                    .HasForeignKey(d => d.KupacId)
                    .HasConstraintName("FK__Notifikac__Kupac__282DF8C2");

                entity.HasOne(d => d.Zaposlenik)
                    .WithMany(p => p.Notifikacije)
                    .HasForeignKey(d => d.ZaposlenikId)
                    .HasConstraintName("FK__Notifikac__Zapos__2739D489");
            });

            modelBuilder.Entity<Obavijesti>(entity =>
            {
                entity.HasKey(e => e.ObavijestId)
                    .HasName("PK__Obavijes__99D330C053843AFC");

                entity.Property(e => e.ObavijestId).HasColumnName("ObavijestID");

                entity.Property(e => e.DatumObjave).HasColumnType("datetime");

                entity.Property(e => e.SlikaUrl).HasColumnName("SlikaURL");

                entity.Property(e => e.ZaposlenikId).HasColumnName("ZaposlenikID");

                entity.HasOne(d => d.Zaposlenik)
                    .WithMany(p => p.Obavijesti)
                    .HasForeignKey(d => d.ZaposlenikId)
                    .HasConstraintName("FK__Obavijest__Zapos__71D1E811");
            });

            modelBuilder.Entity<Odgovori>(entity =>
            {
                entity.HasKey(e => e.OdgovorId)
                    .HasName("PK__Odgovori__23380104D0D424FD");

                entity.Property(e => e.OdgovorId).HasColumnName("OdgovorID");

                entity.Property(e => e.PitanjeId).HasColumnName("PitanjeID");

                entity.Property(e => e.ZaposlenikId).HasColumnName("ZaposlenikID");

                entity.HasOne(d => d.Pitanje)
                    .WithMany(p => p.Odgovori)
                    .HasForeignKey(d => d.PitanjeId)
                    .HasConstraintName("FK__Odgovori__Pitanj__59FA5E80");

                entity.HasOne(d => d.Zaposlenik)
                    .WithMany(p => p.Odgovori)
                    .HasForeignKey(d => d.ZaposlenikId)
                    .HasConstraintName("FK__Odgovori__Zaposl__59063A47");
            });

            modelBuilder.Entity<Pitanja>(entity =>
            {
                entity.HasKey(e => e.PitanjeId)
                    .HasName("PK__Pitanja__1B924A4EAEEF8C3D");

                entity.Property(e => e.PitanjeId).HasColumnName("PitanjeID");

                entity.Property(e => e.Datum).HasColumnType("datetime");

                entity.Property(e => e.KupacId).HasColumnName("KupacID");

                entity.Property(e => e.ProizvodId).HasColumnName("ProizvodID");

                entity.HasOne(d => d.Kupac)
                    .WithMany(p => p.Pitanja)
                    .HasForeignKey(d => d.KupacId)
                    .HasConstraintName("FK__Pitanja__KupacID__5535A963");

                entity.HasOne(d => d.Proizvod)
                    .WithMany(p => p.Pitanja)
                    .HasForeignKey(d => d.ProizvodId)
                    .HasConstraintName("FK__Pitanja__Proizvo__5629CD9C");
            });

            modelBuilder.Entity<Proizvodi>(entity =>
            {
                entity.HasKey(e => e.ProizvodId)
                    .HasName("PK__Proizvod__21A8BE18763C9C6E");

                entity.Property(e => e.ProizvodId).HasColumnName("ProizvodID");

                entity.Property(e => e.Cijena).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.DimenzijaId).HasColumnName("DimenzijaID");

                entity.Property(e => e.Prodato).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ProizvodDetaljiId).HasColumnName("ProizvodDetaljiID");

                entity.Property(e => e.Sifra).HasMaxLength(100);

                entity.Property(e => e.Stanje).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Dimenzija)
                    .WithMany(p => p.Proizvodi)
                    .HasForeignKey(d => d.DimenzijaId)
                    .HasConstraintName("FK__Proizvodi__Dimen__4222D4EF");

                entity.HasOne(d => d.ProizvodDetalji)
                    .WithMany(p => p.Proizvodi)
                    .HasForeignKey(d => d.ProizvodDetaljiId)
                    .HasConstraintName("FK__Proizvodi__Proiz__412EB0B6");
            });

            modelBuilder.Entity<ProizvodiDetalji>(entity =>
            {
                entity.HasKey(e => e.ProizvodDetaljiId)
                    .HasName("PK__Proizvod__596E80F494EC6DA4");

                entity.Property(e => e.ProizvodDetaljiId).HasColumnName("ProizvodDetaljiID");

                entity.Property(e => e.DatumIzmjene).HasColumnType("datetime");

                entity.Property(e => e.DobavljacId).HasColumnName("DobavljacID");

                entity.Property(e => e.JedinicaMjereId).HasColumnName("JedinicaMjereID");

                entity.Property(e => e.KategorijaId).HasColumnName("KategorijaID");

                entity.Property(e => e.NapravljenoU).HasMaxLength(50);

                entity.HasOne(d => d.Dobavljac)
                    .WithMany(p => p.ProizvodiDetalji)
                    .HasForeignKey(d => d.DobavljacId)
                    .HasConstraintName("FK__Proizvodi__Dobav__3D5E1FD2");

                entity.HasOne(d => d.JedinicaMjere)
                    .WithMany(p => p.ProizvodiDetalji)
                    .HasForeignKey(d => d.JedinicaMjereId)
                    .HasConstraintName("FK__Proizvodi__Jedin__3C69FB99");

                entity.HasOne(d => d.Kategorija)
                    .WithMany(p => p.ProizvodiDetalji)
                    .HasForeignKey(d => d.KategorijaId)
                    .HasConstraintName("FK__Proizvodi__Kateg__3E52440B");
            });

            modelBuilder.Entity<Slike>(entity =>
            {
                entity.HasKey(e => e.SlikaId)
                    .HasName("PK__Slike__FFAE2D4695020B06");

                entity.Property(e => e.SlikaId).HasColumnName("SlikaID");

                entity.Property(e => e.ProizvodDetaljiId).HasColumnName("ProizvodDetaljiID");

                entity.Property(e => e.SlikaUrl).HasColumnName("SlikaURL");

                entity.HasOne(d => d.ProizvodDetalji)
                    .WithMany(p => p.Slike)
                    .HasForeignKey(d => d.ProizvodDetaljiId)
                    .HasConstraintName("FK__Slike__ProizvodD__47DBAE45");
            });

            modelBuilder.Entity<Zaposlenici>(entity =>
            {
                entity.HasKey(e => e.ZaposlenikId)
                    .HasName("PK__Zaposlen__3F8CBE1571ADCBBD");

                entity.Property(e => e.ZaposlenikId).HasColumnName("ZaposlenikID");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.Ime).HasMaxLength(50);

                entity.Property(e => e.KorisnickiNalogId).HasColumnName("KorisnickiNalogID");

                entity.Property(e => e.Prezime).HasMaxLength(50);

                entity.Property(e => e.Telefon).HasMaxLength(20);

                entity.HasOne(d => d.KorisnickiNalog)
                    .WithMany(p => p.Zaposlenici)
                    .HasForeignKey(d => d.KorisnickiNalogId)
                    .HasConstraintName("FK__Zaposleni__Koris__267ABA7A");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
