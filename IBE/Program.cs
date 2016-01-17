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

            Encrypt e = new Encrypt("ime.prezime@mail.hr", setup.GetP(), setup.GetPpub(), setup.p, setup.E, setup.k);

            string poruka = "moram porati posluku";
            Cypher c = e.GetCypher(poruka);

            Console.Out.WriteLine("poruka: \"" + poruka + "\"");
            Console.Out.WriteLine("sifrat: \"" + c.V + "\"");
            Console.Out.WriteLine("tocka: \"(" + c.U.X.ToBigInteger().ToString(16) + " ,\n\t" + c.U.Y.ToBigInteger().ToString(16) + "\"");

            Decrypt d = new Decrypt(d_id, setup.p, setup.k);
            string msg = d.GetMessage(c);

            Console.Out.WriteLine("decoded: \"" + msg + "\"");

            Console.ReadKey();
        }
    }
}