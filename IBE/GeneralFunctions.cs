using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using System;
using System.Security.Cryptography;
using System.Text;

namespace IBE
{
    public static class GeneralFunctions
    {
        public static BigInteger H1hash(string ID, BigInteger p)
        {
            // H1 je sha256(ID) mod p
            SHA256Managed crypt = new SHA256Managed();
            StringBuilder hash = new StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(ID), 0, Encoding.UTF8.GetByteCount(ID));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }

            BigInteger hsh = new BigInteger(hash.ToString(), 16);
            // hash mod p
            BigInteger x = hsh.DivideAndRemainder(p)[1];

            return x;
        }

        public static string H2hash(BigInteger qid, BigInteger p)
        {
            // H2 je RiPEMD-120: točka iz polja -> niz bita mod p
            string tocka = qid.ToString();

            RIPEMD160Managed crypt = new RIPEMD160Managed();
            StringBuilder hash = new StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(tocka), 0, Encoding.UTF8.GetByteCount(tocka));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }

            BigInteger hsh = new BigInteger(hash.ToString(), 16);
            // hash mod n
            BigInteger c = hsh.DivideAndRemainder(p)[1];

            return c.ToString();
        }

        internal static BigInteger Pair(FpPoint Q, FpPoint P, BigInteger m, BigInteger p)
        {
            // TODO: napravi jebeno uparivanje!!!! - nešto zeza

            BigInteger pq = Miller(P, Q, m, p);
            BigInteger qp = Miller(Q, P, m, p);

            int parity = m.Mod(new BigInteger("2", 10)).IntValue;

            BigInteger rez = new BigInteger(Math.Pow(-1, parity).ToString(), 10).Multiply(pq.Divide(qp)).Mod(p);

            return rez;
        }

        /// <summary>
        /// millerov agoritam
        /// </summary>
        /// <param name="a">točka</param>
        /// <param name="b">točka</param>
        /// <param name="m">red grupe</param>
        /// <param name="p">red polja, prim</param>
        /// <returns></returns>
        private static BigInteger Miller(FpPoint P, FpPoint Q, BigInteger m, BigInteger prim)
        {
            // Millerov algoritam

            string mBin = m.ToString(2);

            BigInteger t1 = new BigInteger("1", 10);
            BigInteger t2 = new BigInteger("1", 10);
            FpPoint V = P;

            for (int i = 0; i < m.BitLength; i++)
            {
                V = (FpPoint)V.Twice();
                t1 = t1.ModPow(new BigInteger("2", 10), prim).Multiply(MLF(V, V, Q));
                if (mBin[i] == '1')
                {
                    t1 = t1.Multiply(MLF(V, P, Q));
                    V = (FpPoint)V.Add(P);
                }
            }
            return t1;
        }

        // Miller "function"
        private static BigInteger MLF(FpPoint P, FpPoint R, FpPoint Q)
        {
            if (!P.Equals(R))
            {
                if (P.X.Equals(R.X))
                {
                    return Q.X.Subtract(P.X).ToBigInteger();
                }
                else {
                    BigInteger l = R.Y.Subtract(P.Y).Divide((R.X.Subtract(P.X))).ToBigInteger();

                    return Q.Y.Subtract(P.Y).ToBigInteger().Subtract(l.Multiply((Q.X.Subtract(P.X).ToBigInteger())));
                }
            }
            else
            {
                // z*y^2=d*x^3+a*x+b -> derivacija po x -> 3*x^2+a
                BigInteger brojnik = new BigInteger("3", 10).Multiply(P.X.ToBigInteger().Pow(2));
                // z*y^2=d*x^3+a*x+b -> derivacija po y -> 2*y
                BigInteger nazivnik = new BigInteger("2", 10).Multiply(P.Y.ToBigInteger());

                if (nazivnik.ToString(10) == "0")
                {
                    return (Q.X.ToBigInteger().Subtract(P.X.ToBigInteger()));
                }
                else
                {
                    double koef = (double)(brojnik.IntValue / nazivnik.IntValue);

                    double rez = Q.Y.Subtract(P.Y).ToBigInteger().IntValue - koef * (Q.X.Subtract(P.X).ToBigInteger().IntValue);

                    return new BigInteger(rez.ToString(), 10);
                }
            }
        }
    }
}