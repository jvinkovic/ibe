using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using System;

namespace IBE
{
    public class Decrypt
    {
        private FpPoint d_id;
        private BigInteger p;

        public Decrypt(FpPoint did, BigInteger prim)
        {
            d_id = did;
            p = prim;
        }

        public string GetMessage(Cypher c)
        {
            BigInteger eid = GeneralFunctions.Pair(d_id, c.U);

            char[] msg = c.V.ToCharArray();
            char[] m = new char[msg.Length];
            char[] hash = GeneralFunctions.H2hash(eid, p).ToCharArray();
            for (int i = 0; i < msg.Length; i++)
            {
                m[i] = (char)(msg[i] ^ hash[i % hash.Length]);
            }

            string message = new String(m);

            return message;
        }
    }
}