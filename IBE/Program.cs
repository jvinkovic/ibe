using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using System;
using System.IO;

namespace IBE
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string id = "ime.prezime@mail.hr";
            string poruka = "moram porati posluku";
            Cypher sifrat;

            if (args.Length < 2)
            {
                test();
                Console.WriteLine("\n");
                upute();
                return;
            }

            // namjesti postavke prvo
            Setup setup = new Setup();

            if (args[0] == "-f")
            {
                string put = args[1];
                if (!File.Exists(put))
                {
                    poruka = File.ReadAllText(put);
                    if (args.Length != 3)
                    {
                        upute();
                        return;
                    }

                    id = args[args.Length - 1];

                    encode(poruka, id, setup);
                }
                else
                {
                    Console.WriteLine("File does not exists!\n");
                    upute();
                    return;
                }
            }

            string sif;
            string xs;
            string ys;

            if (args[1] == "-d")
            {
                if (args[1] == "-f" && args.Length == 6)
                {
                    string put = args[2];
                    sif = File.ReadAllText(put);

                    id = args[args.Length - 1];
                    xs = args[3];
                    ys = args[4];
                }
                else if (args.Length > 6 || args.Length != 5)
                {
                    upute();
                    return;
                }
                else
                {
                    sif = args[1];
                    xs = args[2];
                    ys = args[3];
                    id = args[args.Length - 1];
                }

                BigInteger x1 = new BigInteger(xs, 10);
                BigInteger y1 = new BigInteger(ys, 10);

                FpFieldElement x = (FpFieldElement)setup.E.FromBigInteger(x1);
                FpFieldElement y = (FpFieldElement)setup.E.FromBigInteger(y1);

                FpPoint point = new FpPoint(setup.E, x, y);

                sifrat = new Cypher { U = point, V = sif };

                decode(sifrat, id, setup);
            }
            else
            {
                poruka = "";
                for (int i = 1; i < args.Length - 2; i++)
                {
                    poruka += args[i] + " ";
                }
                poruka += args[args.Length - 2];

                id = args[args.Length - 1];

                encode(poruka, id, setup);
            }

            Console.ReadKey();
        }

        private static void decode(Cypher cypher, string id, Setup setup)
        {
            // tajni ključ
            FpPoint d_id = setup.Exctract(id, true);

            Decrypt d = new Decrypt(d_id, setup.p, setup.k);
            string msg = d.GetMessage(cypher);

            Console.Out.WriteLine("decoded: \"" + msg + "\"");
        }

        private static void encode(string poruka, string id, Setup setup)
        {
            Encrypt e = new Encrypt(id, setup.GetP(), setup.GetPpub(), setup.p, setup.E, setup.k);

            Cypher c = e.GetCypher(poruka);

            Console.Out.WriteLine("message: \"" + poruka + "\"");
            Console.Out.WriteLine("cypher: \"" + c.V + "\"");
            Console.Out.WriteLine("point: \"(" + c.U.X.ToBigInteger().ToString(16) + " ,\n\t" + c.U.Y.ToBigInteger().ToString(16) + "\"");
        }

        private static void test()
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

        private static void upute()
        {
            Console.WriteLine("Usage:\nIBE [-d -f] [message | path | cypher] [pointX pointY] ID");
            Console.WriteLine("To encode: IBE [-f path] message ID");
            Console.WriteLine("\t-f - if you are trying to encode file");
            Console.WriteLine("\tpath - path to file");
            Console.WriteLine("\tmessage - string to encode");
            Console.WriteLine("\tID - identity to encode by");
            Console.WriteLine("Examples:\n\tIBE this is secret message secret.mail@gmail.com");
            Console.WriteLine("\tIBE -f C:\\secret secret.mail@gmail.com");
            Console.WriteLine();

            Console.WriteLine("To decode: IBE -d [-f path] cypher pointX pointY ID");
            Console.WriteLine("\tpointX pointY - first and second point parameters of cypher");
            Console.WriteLine("Example:\nIBE -d $OH$&)=9-*/=BDO adc70903da987d fed1ad07d6a8d6 secret.mail@gmail.com");

            Console.ReadKey();
        }
    }
}