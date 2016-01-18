using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using System;
using System.IO;

namespace IBE
{
    public class Setup
    {
        // definirati što se koristi
        /*
            E - krivulja secp256k1 - y^2 = x^3+0*x+7 - http://safecurves.cr.yp.to/equation.html
            q - p /// p^n
            p - prost broj - za secp256k1 je 2^256 - 2^32 - 977  = 115792089237316195423570985008687907853269984665640564039457584007908834671663
            Fq - polje nad kojim se računa
            E(Fq) - grupa u kojoj je krivulja E nad poljem Fq
            k - red grupe E(Fq) - 115792089237316195423570985008687907852837564279074904382605163141518161494337
            H1 - sha256(<string>) mod p -> x točka krivulje; y se izračuna
            H2 - ripemd-120 : točka iz polja -> niz bita mod p
            e - Weilovo uparivanje
            P - random točka sa krivulje (x1,y1) - početna točka
            Ppub - JAVNI KLJUČ - Ppub = sP
            s - MASTER TAJNI KLJUČ - random iz Zq i != 0
        */

        // random P iz E(Fq) - G1
        // za secp256k1 se preporuča: P = (55066263022277343669578718895168534326250603453777594175500187360389116729240,
        //                             32670510020758816978083085130507043184471273380659243275938904335757337482424)
        // BigInteger(<broj>,<baza>)
        private FpPoint P;

        // k
        public BigInteger k { get; }

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
            p = new BigInteger("115792089237316195423570985008687907853269984665640564039457584007908834671663", 10);
            //q = p.Pow(n);
            q = p;

            k = new BigInteger("115792089237316195423570985008687907852837564279074904382605163141518161494337", 10);

            // E - krivulja secp256k1 - y ^ 2 = x ^ 3 + 0*x + 7
            BigInteger a = new BigInteger("0", 10);
            BigInteger b = new BigInteger("7", 10);
            E = new FpCurve(q, a, b);

            // P
            BigInteger x1 = new BigInteger("55066263022277343669578718895168534326250603453777594175500187360389116729240", 10);
            BigInteger y1 = new BigInteger("32670510020758816978083085130507043184471273380659243275938904335757337482424", 10);
            FpFieldElement x = (FpFieldElement)E.FromBigInteger(x1); // new FpFieldElement(q, x1);
            FpFieldElement y = (FpFieldElement)E.FromBigInteger(y1); // new FpFieldElement(q, y1);

            P = new FpPoint(E, x, y);

            BigInteger mtp = new BigInteger(s.ToString(), 10);

            Ppub = (FpPoint)P.Multiply(mtp);

            File.WriteAllText("mk", s.ToString() + Environment.NewLine);
        }

        public FpPoint GetP()
        {
            return P;
        }

        public FpPoint GetPpub()
        {
            return Ppub;
        }

        public FpPoint Exctract(string ID, bool decrypt = false)
        {
            if (decrypt)
            {
                string sStr = File.ReadAllText("mk");
                s = int.Parse(sStr);
            }

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