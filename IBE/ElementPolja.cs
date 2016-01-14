using Org.BouncyCastle.Math.EC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bcMath = Org.BouncyCastle.Math;

namespace IBE
{
    internal class ElementPolja : ECFieldElement
    {
        /*
        public abstract string FieldName { get; }
        */

        // jer je veće od int
        public bcMath.BigInteger Size { get; }

        public override string FieldName
        {
            get
            {
                return FieldName;
            }
        }

        public bcMath.BigInteger value { get; }

        public override int FieldSize
        {
            get
            {
                return Size.IntValue;
            }
        }

        public override ECFieldElement Add(ECFieldElement b)
        {
            bcMath.BigInteger[] DiR = value.Add(((ElementPolja)b).value).DivideAndRemainder(Size);
            return null;
        }

        public override ECFieldElement Divide(ECFieldElement b)
        {
            throw new NotImplementedException();
        }

        public override ECFieldElement Invert()
        {
            throw new NotImplementedException();
        }

        public override ECFieldElement Multiply(ECFieldElement b)
        {
            throw new NotImplementedException();
        }

        public override ECFieldElement Negate()
        {
            throw new NotImplementedException();
        }

        public override ECFieldElement Sqrt()
        {
            throw new NotImplementedException();
        }

        public override ECFieldElement Square()
        {
            throw new NotImplementedException();
        }

        public override ECFieldElement Subtract(ECFieldElement b)
        {
            throw new NotImplementedException();
        }

        public override bcMath.BigInteger ToBigInteger()
        {
            throw new NotImplementedException();
        }
    }
}