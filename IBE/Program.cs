using Org.BouncyCastle.Math.EC;
using System;

namespace IBE
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Out.WriteLine("Start!");

            // namjesti postavke prvo
            Setup setup = new Setup();

            // tajni ključ
            FpPoint d_id = setup.Exctract("ime.prezime@mail.hr");

            Encrypt e = new Encrypt("ime.prezime@mail.hr", setup.GetP(), setup.GetPpub(), setup.p, setup.E);

            string poruka = "moram porati posluku";
            Cypher c = e.GetCypher(poruka);

            Console.Out.WriteLine("poruka: \"" + poruka + "\"");
            Console.Out.WriteLine("sifrat: \"" + c.V + "\"");

            Decrypt d = new Decrypt(d_id, setup.p);
            string msg = d.GetMessage(c);

            Console.Out.WriteLine("decoded: \"" + msg + "\"");

            Console.ReadKey();
        }
    }
}