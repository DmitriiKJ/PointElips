using System;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;

namespace PointElips
{
    public class Program
    {
        static string curveName = "secp256r1";
        static X9ECParameters curveParameters = SecNamedCurves.GetByName(curveName);
        static ECCurve curve = curveParameters.Curve;

        static void Main(string[] args)
        {
            SecureRandom random = new SecureRandom();
            byte[] randomBytes1 = new byte[32];
            random.NextBytes(randomBytes1);

            byte[] randomBytes2 = new byte[32];
            random.NextBytes(randomBytes2);

            BigInteger k = new BigInteger(randomBytes1).Abs();
            BigInteger d = new BigInteger(randomBytes2).Abs();

            ECPoint G = BasePointGGet();
            ECPoint H1 = ScalarMult(d, G);
            ECPoint H2 = ScalarMult(k, H1);

            ECPoint H3 = ScalarMult(k, G);
            ECPoint H4 = ScalarMult(d, H3);
            bool result = P1EgualP2(H2, H4);

            Console.WriteLine("===========================");

            Console.WriteLine("H2 coordinates:");
            PrintECPoint(H2);

            Console.WriteLine("===========================");

            Console.WriteLine("H4 coordinates:");
            PrintECPoint(H4);

            Console.WriteLine("===========================");

            Console.WriteLine($"Is H2 valid: {IsOnCurveCheck(H2)}");
            Console.WriteLine($"Is H4 valid: {IsOnCurveCheck(H4)}");

            Console.WriteLine(result ? "True" : "False");
        }

        static ECPoint BasePointGGet() => curveParameters.G;

        static ECPoint ECPointGen(BigInteger x, BigInteger y) => curve.CreatePoint(x, y);

        static bool IsOnCurveCheck(ECPoint p) => p.IsValid();

        static ECPoint AddECPoints(ECPoint a, ECPoint b)
        {
            if (a.XCoord == b.XCoord && a.YCoord == b.YCoord)
            {
                ECPoint res = DoubleECPoints(a);
                return res;
            }
            ECPoint result = a.Add(b);
            return result;
        }

        static ECPoint DoubleECPoints(ECPoint p1) => p1.Twice();

        static ECPoint ScalarMult(BigInteger n, ECPoint p1)
        {
            if (n.Equals(BigInteger.Zero))
            {
                return curve.Infinity;
            }

            return p1.Multiply(n).Normalize();
        }
        static string ECPointToString(ECPoint p1)
        {
            if (p1.IsInfinity) return "\nX: 0\nY: 0";
            BigInteger x = p1.XCoord.ToBigInteger();
            BigInteger y = p1.YCoord.ToBigInteger();

            return $"\nX: {x}\nY: {y}";
        }

        static void PrintECPoint(ECPoint p1)
        {
            Console.WriteLine(ECPointToString(p1));
        }

        static bool P1EgualP2(ECPoint p1, ECPoint p2)
        {
            if (p1.IsInfinity && p2.IsInfinity)
            {
                return true;
            }
            else if (p1.IsInfinity || p2.IsInfinity)
            {
                return false;
            }

            return p1.XCoord.ToBigInteger().Equals(p2.XCoord.ToBigInteger()) && p1.YCoord.ToBigInteger().Equals(p2.YCoord.ToBigInteger());
        }
    }
}
