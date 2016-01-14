using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using cryEC = Org.BouncyCastle.Crypto;

namespace IBE
{
    public class Setup
    {
        // definirati što se koristi
        /*
            E - krivulja M-221 - Montgomery oblik - y^2 = x^3+117050x^2+x - http://safecurves.cr.yp.to/equation.html
            q - p^n
            p - prost broj - za M-221 je 2^221 - 3 = 3369993333393829974333376885877453834204643052817571560137951281149
            Fq - polje nad kojim se računa - G2
            E(Fq) - krivulja nad poljem Fq - G1
            H1 - sha256(<string>) mod p -> x točka krivulje; y se izračuna
            H2 - ripemd-120 : točka iz polja -> niz bita mod 3 - n=3
            e - Weilovo uparivanje
            n - 3
            P - random točka sa krivulje (x1,y1) - početna točka
            Ppub - JAVNI KLJUČ - Ppub = sP
            s - MASTER TAJNI KLJUČ - random iz Zq i != 0
        */

        // random P iz E(Fq) - G1
        // za M-221 se preporuča: P = (4, 1630203008552496124843674615123983630541969261591546559209027208557)
        // BigInteger(<broj>,<baza>)
        private FpPoint P;

        // n
        public int n { get; }

        // q
        private BigInteger q;

        // p
        public BigInteger p { get; }

        // krivulja
        public FpCurve E { get; }

        // random s iz Zq*
        private int s = 0;

        // javni ključ
        private FpPoint Ppub;

        public Setup()
        {
            n = 3;

            do
            {
                Random r = new Random();
                s = r.Next(1, int.MaxValue - 1);
            } while (s == 0);

            // p i q
            p = new BigInteger("3369993333393829974333376885877453834204643052817571560137951281149", 10);
            q = p.Pow(n);

            // E - krivulja M - 221 - Montgomery oblik - y ^ 2 = x ^ 3 + 117050x ^ 2 + x
            BigInteger a = new BigInteger("117050", 10);
            BigInteger b = new BigInteger("1", 10);
            E = new FpCurve(q, a, b);

            // P
            BigInteger x1 = new BigInteger("4", 10);
            BigInteger y1 = new BigInteger("1630203008552496124843674615123983630541969261591546559209027208557", 10);
            FpFieldElement x = (FpFieldElement)E.FromBigInteger(x1); // new FpFieldElement(q, x1);
            FpFieldElement y = (FpFieldElement)E.FromBigInteger(y1); // new FpFieldElement(q, y1);

            P = new FpPoint(E, x, y);

            BigInteger k = new BigInteger(s.ToString(), 10);

            Ppub = (FpPoint)P.Multiply(k);
        }

        public FpPoint GetP()
        {
            return P;
        }

        public FpPoint GetPpub()
        {
            return Ppub;
        }

        public FpPoint Exctract(string ID)
        {
            //  y^2 = x^3 + 117050x^2 + x
            BigInteger x = GeneralFunctions.H1hash(ID, p);
            BigInteger y = x.Pow(3).Add(x.Pow(2).Multiply(new BigInteger("117050", 10))).Add(x).Pow(2).ModInverse(p);

            FpFieldElement x_Qid = new FpFieldElement(q, x);
            FpFieldElement y_Qid = new FpFieldElement(q, y);
            FpPoint Qid = new FpPoint(E, x_Qid, y_Qid);

            FpPoint d_id = (FpPoint)Qid.Multiply(new BigInteger(s.ToString(), 10));

            // privatni ključ
            return d_id;
        }
    }
}