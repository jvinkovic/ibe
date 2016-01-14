using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBE
{
    public class Decrypt
    {
        private FpPoint d_id;
        private int n;

        public Decrypt(FpPoint did, int nen)
        {
            d_id = did;
            n = nen;
        }

        public string GetMessage(Cypher c)
        {
            BigInteger eid = GeneralFunctions.Pair(d_id, c.U);

            char[] msg = c.V.ToCharArray();
            char[] m = new char[msg.Length];
            char[] hash = GeneralFunctions.H2hash(eid, n).ToCharArray();
            for (int i = 0; i < msg.Length; i++)
            {
                m[i] = (char)(msg[i] ^ hash[i % n]);
            }

            string message = m.ToString();

            return message;
        }
    }
}