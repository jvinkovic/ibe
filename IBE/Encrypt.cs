using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using System;

namespace IBE
{
    public class Encrypt
    {
        private string ID;
        private BigInteger prim;
        private FpPoint P;
        private FpPoint Ppub;
        private FpCurve E;

        public Encrypt(string id, FpPoint tocka, FpPoint Ppublic, BigInteger prost, FpCurve curve)
        {
            ID = id;
            P = tocka;
            Ppub = Ppublic;
            prim = prost;
            E = curve;
        }

        public Cypher GetCypher(string message)
        {
            BigInteger x = GeneralFunctions.H1hash(ID, prim);
            BigInteger y = x.Pow(3).Add(new BigInteger("7", 10)).Pow(2).ModInverse(prim);
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
            gid = gid.ModPow(new BigInteger(r.ToString(), 10), prim);

            char[] M = message.ToCharArray();
            char[] cArray = new char[M.Length];
            char[] hash = GeneralFunctions.H2hash(gid, prim).ToCharArray();
            for (int i = 0; i < message.Length; i++)
            {
                cArray[i] = (char)(M[i] ^ hash[i % hash.Length]);
            }

            string c = new String(cArray);

            return new Cypher { U = rP, V = c };
        }
    }
}