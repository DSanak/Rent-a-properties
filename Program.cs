using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WynajemObiektow
{
    enum TypDzialanosci
    {
        Rozrywka,
        Odziez,
        Gastronomia,
        AGD,
        ArtykulySpozywcze,
        DomIWnetrze
    }

    class Obiekt
    {
        public int Id { get; set; }
        public string Nazwa { get; set; }
        public string NazwaTechniczna { get; set; }
        public IList<Obiekt> Obiekty { get; set; }
        public virtual decimal ObliczCeneWynajmu() => 0;

        public decimal ObliczCalkowitaCeneWynajmu ()
            {
            var ccw = ObliczCeneWynajmu();
            
            if(Obiekty is{ }) 
               ccw += Obiekty.Sum(x => x.ObliczCalkowitaCeneWynajmu());

            return ccw;
                
            }

        public void Kolekcja(ref List<Obiekt> collectionofobject) //ZMIANA STRUKTURY NASZEJ GALERI NA PŁASKA, 
                                                                   //ZEBY DOBRZE JĄ ODPYTYWAĆ
        {
            collectionofobject.Add(this);


            if (Obiekty is { })
            {
                foreach (var obiekt in Obiekty)
                {
                    obiekt.Kolekcja(ref collectionofobject);
                }
            }
            

        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"ID: {Id}, Nazwa: {Nazwa}");

            foreach (var obiekt in Obiekty ?? new List<Obiekt>())
            {
                sb.AppendLine(obiekt.ToString());
            }

            return sb.ToString();
        }
    };


    class ObiektZPowierzchniaCalkowita : Obiekt
    {
        public double PowierzchniaCalkowita { get; set; }
    }

    class ObiektDoWynajecia : ObiektZPowierzchniaCalkowita
    {
        public TypDzialanosci TypDzialanosci { get; set; }
        public decimal CenaWynajmu { get; set; }
        public DateTime DataPoczatkuWynajmu { get; set; }
        public DateTime? DataZakonczeniaWynajmu { get; set; }

        protected bool CzyWynajmuje()
        {
            return DataPoczatkuWynajmu <= DateTime.Now && (!DataZakonczeniaWynajmu.HasValue || DataZakonczeniaWynajmu >= DateTime.Now);
        }
        public override decimal ObliczCeneWynajmu() =>
            CzyWynajmuje() ? CenaWynajmu : 0;



    }

    class Pomieszczenie : ObiektDoWynajecia
    {
        public double PowierzchniaWynajmu { get; set; }
        public override decimal ObliczCeneWynajmu() =>
            CzyWynajmuje() ? (decimal)PowierzchniaWynajmu * CenaWynajmu : 0;
    }

    class Parking : Obiekt
    {
        public int LiczbaMiejscParkingowych { get; set; }
    }





    class Program
    {
        static void Main(string[] args)
        {
            var galeria = new Obiekt
            {
                Id = 1,
                Nazwa = "ATH-Sfera",
                NazwaTechniczna = "ATHS",
                Obiekty = new List<Obiekt>
                {
                    new ObiektZPowierzchniaCalkowita//Poziom 1
                    {
                        Id=1,
                        Nazwa="Poziom 1",
                        NazwaTechniczna="P1",
                        Obiekty =new List<Obiekt>
                        {
                            new ObiektZPowierzchniaCalkowita //Korytarz na P1
                            {
                                Id=100,
                                Nazwa="Korytarz na Poziomie 1",
                                NazwaTechniczna="K1",
                                PowierzchniaCalkowita=500,
                                Obiekty= new List<Obiekt>
                                {
                                    new Pomieszczenie
                                    {
                                        Id=1000,
                                        Nazwa="Cinema City",
                                        NazwaTechniczna="Kino1",
                                        PowierzchniaCalkowita=250,
                                        PowierzchniaWynajmu=200,
                                        CenaWynajmu=1200,
                                        DataPoczatkuWynajmu=new DateTime(2019,7,1),
                                        TypDzialanosci=TypDzialanosci.AGD
                                        
                                    }, 
                                    new Pomieszczenie
                                    {
                                        Id=1001,
                                        Nazwa="Sklep 1",
                                        NazwaTechniczna="SHOP_1",
                                        PowierzchniaCalkowita=120,
                                        PowierzchniaWynajmu=100,
                                        CenaWynajmu=1500,
                                        DataPoczatkuWynajmu=new DateTime(2019,7,1),
                                        DataZakonczeniaWynajmu=new DateTime(2021,9,1),
                                        TypDzialanosci=TypDzialanosci.Gastronomia
                                    },
                                    new ObiektDoWynajecia
                                    {
                                        Id=1002,
                                        Nazwa="Stand 1",
                                        NazwaTechniczna="Stand_1",
                                        PowierzchniaCalkowita=20,
                                        CenaWynajmu=3500,
                                        DataPoczatkuWynajmu=new DateTime(2020,10,1),
                                       TypDzialanosci=TypDzialanosci.Gastronomia
                                    }
                                }
                            }
                            
                        }
                    },
                    
                }//50 MINUTA
                
            };

            Console.WriteLine(galeria);

            Console.WriteLine(galeria.ObliczCalkowitaCeneWynajmu());

            var colletion = new List<Obiekt>();
            galeria.Kolekcja(ref colletion);

           var pc= colletion.
                OfType<ObiektDoWynajecia>()
                .Where(x => x.TypDzialanosci == TypDzialanosci.Gastronomia)
                .Sum(x => x.PowierzchniaCalkowita);

            Console.WriteLine(pc);

            //colletion.Where(x => x is ObiektDoWynajecia obiekt && obiekt.TypDzialanosci == TypDzialanosci.Gastronomia);
        }
    }
};










//WYNAJEM OBIEKTOW GOTOWE
/*
namespace WynajemObiektow
{
    enum TypDzialalnosci
    {​​
    Rozrywka,
        Odziez,
        Gastronomia,
        AGD,
        ArtykulySpozywcze,
        DomIWnetrze
    }​​
class Obiekt
    {​​
    public int Id {​​ get; set; }​​
    public string Nazwa {​​ get; set; }​​
    public string NazwaTechniczna {​​ get; set; }​​
    public IList<Obiekt> Obiekty {​​ get; set; }​​
    public virtual decimal ObliczCeneWynajmu() => 0;
        public decimal ObliczCalkowitaCeneWynajmu()
        {​​
        return ObliczCeneWynajmu() +
               (Obiekty ?? new List<Obiekt>()).Sum(x => x.ObliczCalkowitaCeneWynajmu());
            // var ccw = ObliczCeneWynajmu();
            //
            // if (Obiekty is {​​ }​​)
            //     ccw += Obiekty.Sum(x => x.ObliczCalkowitaCeneWynajmu());
            //
            // return ccw;
        }​​
}​​
class ObiektZPowierzchniaCalkowita : Obiekt
    {​​
    public double PowierzchniaCalkowita {​​ get; set; }​​
}​​
class ObiektOoWynajęcia : ObiektZPowierzchniaCalkowita
    {​​
    public TypDzialalnosci TypDzialalnosci {​​ get; set; }​​
    public decimal CenaWynajmu {​​ get; set; }​​
    public DateTime DataPoczatkuWynajmu {​​ get; set; }​​
    public DateTime DataZakonczeniaWynajmu {​​ get; set; }​​
    public override decimal ObliczCeneWynajmu() =>
        CenaWynajmu;
    }​​
class Pomieszczenie : ObiektOoWynajęcia
    {​​
    public double PowierzchniaWynajmu {​​ get; set; }​​
    public override decimal ObliczCeneWynajmu() =>
        (decimal)PowierzchniaWynajmu * CenaWynajmu;
    }​​
class Parking : Obiekt
    {​​
    public int LiczbaMiejscParkingowych {​​ get; set; }​​
}​​
class Program
    {​​    static void Main(string[] args)
        {​​
        var galeria = new Obiekt
        {​​
            Id = 1,
            Nazwa = "ATH Sfera",
            NazwaTechniczna = "ATHS",
            Obiekty = new List<Obiekt>
            {​​
                new ObiektZPowierzchniaCalkowita
                {​​
                    Id = 10,
                    Nazwa = "Poziom 1",
                    NazwaTechniczna = "L1",
                    PowierzchniaCalkowita = 1200,
                    Obiekty = new List<Obiekt>
                    {​​
                        new ObiektZPowierzchniaCalkowita
                        {​​
                            Id = 100,
                            Nazwa = "Korytarz 1",
                            NazwaTechniczna = "K1",
                            PowierzchniaCalkowita = 500,
                        }​​
                    }​​
                }​​,
                new Parking
                {​​
                    Id = 1000,
                    Nazwa = "Parking",
                    NazwaTechniczna = "P",
                    LiczbaMiejscParkingowych = 2000
                }​​
            }​​
        }​​;
            Console.WriteLine(galeria.ObliczCalkowitaCeneWynajmu());
        }​​
} 
}
*/
