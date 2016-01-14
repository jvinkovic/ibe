using Org.BouncyCastle.Math.EC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            Encrypt e = new Encrypt("ime.prezime@mail.hr", setup.GetP(), setup.GetPpub(), setup.p, setup.n, setup.E);

            string poruka = "porati posluku";
            Cypher c = e.GetCypher(poruka);

            Console.Out.WriteLine("poruka: \"" + poruka + "\"");
            Console.Out.WriteLine("sifrat: \"" + c.V + "\"");

            Decrypt d = new Decrypt(d_id, setup.n);
            string msg = d.GetMessage(c);

            Console.Out.WriteLine("decoded: \"" + msg + "\"");

            Console.ReadKey();
        }
    }
}