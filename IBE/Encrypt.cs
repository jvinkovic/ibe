using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBE
{
    public class Encrypt
    {
        private string ID;
        private BigInteger prim;
        private FpPoint P;
        private FpPoint Ppub;
        private int n;
        private FpCurve E;

        public Encrypt(string id, FpPoint tocka, FpPoint Ppublic, BigInteger prost, int num, FpCurve curve)
        {
            ID = id;
            P = tocka;
            Ppub = Ppublic;
            prim = prost;
            n = num;
            E = curve;
        }

        public Cypher GetCypher(string message)
        {
            BigInteger x = GeneralFunctions.H1hash(ID, prim);
            BigInteger y = x.Pow(3).Add(x.Pow(2).Multiply(new BigInteger("117050", 10))).Add(x).Pow(2).ModInverse(prim);
            FpFieldElement x_Qid = new FpFieldElement(E.Q, x);
            FpFieldElement y_Qid = new FpFieldElement(E.Q, y);
            FpPoint Qid = new FpPoint(E, x_Qid, y_Qid);

            int r = 0;
            do
            {
                Random rnd = new Random();
                r = rnd.Next(1, int.MaxValue - 1);
            } while (r == 0);

            FpPoint rP = (FpPoint)P.Multiply(new BigInteger(r.ToString(), 10));

            BigInteger gid = GeneralFunctions.Pair(Qid, Ppub);

            char[] M = message.ToCharArray();
            char[] cArray = new char[M.Length];
            char[] hash = GeneralFunctions.H2hash(gid, n).ToCharArray();
            for (int i = 0; i < message.Length; i++)
            {
                cArray[i] = (char)(M[i] ^ hash[i % n]);
            }

            string c = cArray.ToString();

            return new Cypher { U = rP, V = c };
        }
    }
}