using Org.BouncyCastle.Math.EC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using bcMath = Org.BouncyCastle.Math;

namespace IBE
{
    public class tocka
    {
        public ECCurve Curve { get; }
        public bool IsCompressed { get; }
        public bool IsInfinity { get; }
        public ECFieldElement X { get; }
        public ECFieldElement Y { get; }

        public tocka(ECCurve curve, ECFieldElement x, ECFieldElement y, bool withCompression) : base(curve, x, y, withCompression)
        {
            Curve = curve;
            X = x;
            Y = y;
            IsCompressed = withCompression;
            IsInfinity = false;
        }

        public override ECPoint Add(ECPoint b)
        {
            return bcMath.EC.ECAlgorithms.ShamirsTrick(this, new bcMath.BigInteger("1"), b, new bcMath.BigInteger("1"));
        }

        public override byte[] GetEncoded()
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, this);
                return ms.ToArray();
            }
        }

        public override ECPoint Multiply(bcMath.BigInteger b)
        {
            if (b.CompareTo(new bcMath.BigInteger("0")) > 1)
            {
                // b je pozitivan
                return bcMath.EC.ECAlgorithms.ShamirsTrick(this, b.Subtract(new bcMath.BigInteger("1")),
                                                                       this, new bcMath.BigInteger("1"));
            }
            else
            {
                // b je nagativan
                return bcMath.EC.ECAlgorithms.ShamirsTrick(this, b.Add(new bcMath.BigInteger("1")),
                                                                       this, new bcMath.BigInteger("1").Negate());
            }
        }

        /// <summary>
        /// negira Y
        /// </summary>
        /// <returns>vraća točku zrcaljenu s obzirom na X-os</returns>
        public override ECPoint Negate()
        {
            return new tocka(Curve, X, Y.Negate(), IsCompressed);
        }

        public override ECPoint Subtract(ECPoint b)
        {
            return bcMath.EC.ECAlgorithms.ShamirsTrick(this, new bcMath.BigInteger("1"),
                                                             b, new bcMath.BigInteger("1"));
        }

        public override ECPoint Twice()
        {
            throw new NotImplementedException();
        }
    }
}