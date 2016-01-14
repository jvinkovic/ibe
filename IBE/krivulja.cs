using Org.BouncyCastle.Math.EC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bcMath = Org.BouncyCastle.Math;

namespace IBE
{
    public class krivulja : ECCurve
    {
        /*
        public ECFieldElement A { get; }
        public ECFieldElement B { get; }
        public abstract int FieldSize { get; }
        public abstract ECPoint Infinity { get; }
        */

        public override int FieldSize
        {
            get
            {
                return FieldSize;
            }
        }

        public override ECPoint Infinity
        {
            get
            {
                return Infinity;
            }
        }

        public override ECPoint CreatePoint(bcMath.BigInteger x, bcMath.BigInteger y, bool withCompression)
        {
            throw new NotImplementedException();
        }

        public override ECPoint DecodePoint(byte[] encoded)
        {
            throw new NotImplementedException();
        }

        public override ECFieldElement FromBigInteger(bcMath.BigInteger x)
        {
            throw new NotImplementedException();
        }
    }
}