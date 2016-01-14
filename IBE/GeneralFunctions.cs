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

        public static string H2hash(BigInteger qid, int n)
        {
            // H2 je RiPEMD-120: točka iz polja -> niz bita mod 3 - n=3
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
            BigInteger c = hsh.DivideAndRemainder(new BigInteger(n.ToString(), 10))[1];

            return c.ToString();
        }

        internal static BigInteger Pair(FpPoint a, FpPoint b)
        {
            // TODO: UPARIVANJE
            throw new NotImplementedException();
        }
    }
}