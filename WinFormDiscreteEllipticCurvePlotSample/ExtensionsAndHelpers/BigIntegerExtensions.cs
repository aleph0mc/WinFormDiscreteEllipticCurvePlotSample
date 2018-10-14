using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace EllipticCurves.ExtensionsAndHelpers
{
    public class EcModPoint
    {
        public BigInteger x { get; set; }
        public BigInteger y { get; set; }
    }


    // Miller-Rabin primality test as an extension method on the BigInteger type.
    // https://rosettacode.org/wiki/Miller%E2%80%93Rabin_primality_test#C.23
    public static class BigIntegerExtensions
    {
        private static BigInteger Q;
        private static int S;
        private static BigInteger Z;

        private static BigInteger quadraticNonResidue(BigInteger P)
        {
            BigInteger z = 0;
            for (BigInteger i = 0; i < P; i++)
            {
                BigInteger check = BigInteger.ModPow(i, (P - 1) / 2, P);
                if (check == (P - 1) || -1 == check)
                {
                    z = i;
                    break;
                }
            }

            return z;
        }

        /// <summary>
        /// Finds Q (odd number) and S such that p-1 = Q2^S and a non quadratic residue Z
        /// </summary>
        /// <param name="P"></param>
        private static void computeQSZ(BigInteger P)
        {
            // 
            BigInteger pMinus1 = P - 1;
            int j = 1;
            bool found = false;
            BigInteger res = 0;
            BigInteger pow2 = 0;
            do
            {
                pow2 = BigInteger.Pow(2, j);
                res = pMinus1 / pow2;
                if (res % 2 != 0)
                {
                    found = true;
                    S = j;
                    Q = res;
                }

                j++;
            } while ((P < pow2) || !found);

            Z = quadraticNonResidue(P);
        }

        public static bool IsProbablePrime(this BigInteger Source, int Certainty)
        {
            if (Source == 2 || Source == 3)
                return true;
            if (Source < 2 || Source % 2 == 0)
                return false;

            BigInteger d = Source - 1;
            int s = 0;

            while (d % 2 == 0)
            {
                d /= 2;
                s += 1;
            }

            // There is no built-in method for generating random BigInteger values.
            // Instead, random BigIntegers are constructed from randomly generated
            // byte arrays of the same length as the source.
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[Source.ToByteArray().LongLength];
            BigInteger a;

            for (int i = 0; i < Certainty; i++)
            {
                do
                {
                    // This may raise an exception in Mono 2.10.8 and earlier.
                    // http://bugzilla.xamarin.com/show_bug.cgi?id=2761
                    rng.GetBytes(bytes);
                    a = new BigInteger(bytes);
                }
                while (a < 2 || a >= Source - 2);

                BigInteger x = BigInteger.ModPow(a, d, Source);
                if (x == 1 || x == Source - 1)
                    continue;

                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, Source);
                    if (x == 1)
                        return false;
                    if (x == Source - 1)
                        break;
                }

                if (x != Source - 1)
                    return false;
            }

            return true;
        }

        public static BigInteger BigPow(this BigInteger Source, BigInteger Power)
        {
            BigInteger result = 1;

            var intMaxIterations = Power / int.MaxValue;

            for (BigInteger i = 0; i < intMaxIterations; i++)
            {
                result *= BigInteger.Pow(Source, int.MaxValue);
            }

            var remainder = (int)(Power % int.MaxValue);

            if (remainder > 0) result *= BigInteger.Pow(Source, remainder);

            return result;
        }

        /// <summary>
        /// Finds the inverse mod p
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="P"></param>
        /// <returns></returns>
        public static BigInteger ModInv(this BigInteger Source, BigInteger P)
        {
            BigInteger t = 0;
            BigInteger r = P;
            BigInteger newt = 1;
            BigInteger newr = Source;

            while (0 != newr)
            {
                BigInteger prov;
                BigInteger quot = r / newr;
                prov = newt;
                newt = t - quot * prov;
                t = prov;
                prov = newr;
                newr = r - quot * prov;
                r = prov;
            }
            if (r > 1) // not invertible
                throw new Exception("Not invertible");

            // returns t or t-P if t < 0
            return (t + P) % P;
        }

        /// <summary>
        /// Finds an integer square root r.
        /// The other is -r.
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static BigInteger Sqrt(this BigInteger Source)
        {
            if (0 == Source) { return 0; }  // Avoid zero divide
            BigInteger n = (Source / 2) + 1;       // Initial estimate, never low
            BigInteger n1 = (n + (Source / n)) / 2;
            while (n1 < n)
            {
                n = n1;
                n1 = (n + (Source / n)) / 2;
            }
            return n;
        }

        /// <summary>
        /// Finds the square root r mod p using the Tonelli-Shanks algorithm.
        /// The other one is -r or p - r.
        /// </summary>
        /// <param name=""></param>
        /// <param name="P"></param>
        /// <param name="CalcParamsQSZ">true</param>
        /// <returns></returns>
        public static BigInteger ModSqrt(this BigInteger Source, BigInteger P, bool CalcParamsQSZ = true)
        {
            // Check whether the number is a quadratic residue (Euler's criterion)
            var check = BigInteger.ModPow(Source, (P - 1) / 2, P);
            if (1 != check)
                throw new Exception($"{Source} is not a quadratic residue");

            if (CalcParamsQSZ)
            {
                CalcParamsQSZ = true;
                computeQSZ(P);
            }

            BigInteger res = 0;
            BigInteger pow2 = 0;

            // compute square root mod p - Tonelli-Shanks algorithm
            var m = S;
            var c = BigInteger.ModPow(Z, Q, P);
            var t = BigInteger.ModPow(Source, Q, P);
            var r = BigInteger.ModPow(Source, (Q + 1) / 2, P);

            while ((t != 0) && (t != 1))
            {
                int k = 0;
                BigInteger tmp = 0;
                while ((k < m) && (tmp != 1))
                {
                    ++k;
                    pow2 = BigInteger.Pow(2, k);
                    tmp = BigInteger.ModPow(t, pow2, P);
                }
                var b1 = BigInteger.ModPow(c, BigInteger.Pow(2, m - k - 1), P);
                m = k;
                c = BigInteger.ModPow(b1, 2, P);
                t = (t * c) % P;
                r = (r * b1) % P;
            }

            return r;
        }

        public static bool Coprime(this BigInteger Source, BigInteger N)
        {
            return 1 == BigInteger.GreatestCommonDivisor(Source, N);
        }

        /// <summary>
        /// Converts a <see cref="BigInteger"/> to a binary string.
        /// </summary>
        /// <param name="Source">A <see cref="BigInteger"/>.</param>
        /// <returns>
        /// A <see cref="System.String"/> containing a binary
        /// representation of the supplied <see cref="BigInteger"/>.
        /// </returns>
        public static string ToBinaryString(this BigInteger Source)
        {
            var bytes = Source.ToByteArray();
            var idx = bytes.Length - 1;

            // Create a StringBuilder having appropriate capacity.
            var base2 = new StringBuilder(bytes.Length * 8);

            // Convert first byte to binary.
            var binary = Convert.ToString(bytes[idx], 2);

            // Ensure leading zero exists if value is positive.
            if (binary[0] != '0' && Source.Sign == 1)
            {
                base2.Append('0');
            }

            // Append binary string to StringBuilder.
            base2.Append(binary);

            // Convert remaining bytes adding leading zeros.
            for (idx--; idx >= 0; idx--)
            {
                base2.Append(Convert.ToString(bytes[idx], 2).PadLeft(8, '0'));
            }

            return base2.ToString();
        }

        /// <summary>
        /// Converts a <see cref="BigInteger"/> to a hexadecimal string.
        /// </summary>
        /// <param name="Source">A <see cref="BigInteger"/>.</param>
        /// <returns>
        /// A <see cref="System.String"/> containing a hexadecimal
        /// representation of the supplied <see cref="BigInteger"/>.
        /// </returns>
        public static string ToHexadecimalString(this BigInteger Source)
        {
            return Source.ToString("X");
        }

        /// <summary>
        /// Converts a <see cref="BigInteger"/> to a octal string.
        /// </summary>
        /// <param name="Source">A <see cref="BigInteger"/>.</param>
        /// <returns>
        /// A <see cref="System.String"/> containing an octal
        /// representation of the supplied <see cref="BigInteger"/>.
        /// </returns>
        public static string ToOctalString(this BigInteger Source)
        {
            var bytes = Source.ToByteArray();
            var idx = bytes.Length - 1;

            // Create a StringBuilder having appropriate capacity.
            var base8 = new StringBuilder(((bytes.Length / 3) + 1) * 8);

            // Calculate how many bytes are extra when byte array is split
            // into three-byte (24-bit) chunks.
            var extra = bytes.Length % 3;

            // If no bytes are extra, use three bytes for first chunk.
            if (extra == 0)
            {
                extra = 3;
            }

            // Convert first chunk (24-bits) to integer value.
            int int24 = 0;
            for (; extra != 0; extra--)
            {
                int24 <<= 8;
                int24 += bytes[idx--];
            }

            // Convert 24-bit integer to octal without adding leading zeros.
            var octal = Convert.ToString(int24, 8);

            // Ensure leading zero exists if value is positive.
            if (octal[0] != '0' && Source.Sign == 1)
            {
                base8.Append('0');
            }

            // Append first converted chunk to StringBuilder.
            base8.Append(octal);

            // Convert remaining 24-bit chunks, adding leading zeros.
            for (; idx >= 0; idx -= 3)
            {
                int24 = (bytes[idx] << 16) + (bytes[idx - 1] << 8) + bytes[idx - 2];
                base8.Append(Convert.ToString(int24, 8).PadLeft(8, '0'));
            }

            return base8.ToString();
        }


        #region ELLIPTIC CURVE KEY PAIR GENERATOR

        private static EcModPoint pointAdd(EcModPoint pointP, EcModPoint pointQ, BigInteger a, BigInteger p)
        {
            // compute m = (y1 - y0) / (x1 - x0) (mod p)
            var x0 = pointP.x;
            var y0 = pointP.y;

            var x1 = pointQ.x;
            var y1 = pointQ.y;

            // compute m mod p
            BigInteger dy = (y1 - y0) % p;
            BigInteger dx = (x1 - x0) % p;
            dy = (dy + p) % p;
            dx = (dx + p) % p;

            BigInteger inv_dx = dx.ModInv(p);
            BigInteger m = BigInteger.Multiply(dy, inv_dx) % p;
            m = (m + p) % p;

            var x = (m * m - x0 - x1) % p;
            x = (x + p) % p;
            var y = (m * (x0 - x) - y0) % p;  //(-m * m * m + 3 * m * x0 - y0) % p;
            y = (y + p) % p;

            EcModPoint ret = new EcModPoint { x = x, y = y };
            return ret;

        }

        private static EcModPoint pointDouble(EcModPoint point, BigInteger a, BigInteger p)
        {
            // compute m = dy/dx = dy * dx^-1 (mod p)
            var x0 = point.x;
            var y0 = point.y;

            BigInteger dy = (3 * x0 * x0 + a) % p;
            BigInteger dx = (2 * y0) % p;
            dy = (dy + p) % p;
            dx = (dx + p) % p;

            // compute inverse of dx mod p
            BigInteger inv_dx = dx.ModInv(p);

            // compute m mod p
            BigInteger m = BigInteger.Multiply(dy, inv_dx) % p;
            m = (m + p) % p;

            var x = (m * m - 2 * x0) % p;
            x = (x + p) % p;
            var y = (m * (x0 - x) - y0) % p;  //(-m * m * m + 3 * m * x0 - y0) % p;
            y = (y + p) % p;

            EcModPoint ret = new EcModPoint { x = x, y = y };
            return ret;
        }

        private static EcModPoint computeECPointMultiply(BigInteger a, EcModPoint g, BigInteger p, BigInteger sk)
        {
            var bitString = sk.ToBinaryString();
            EcModPoint q = g;
            foreach (var bit in bitString)
            {
                q = pointDouble(q, a, p);
                if (bit == '1')
                    q = pointAdd(q, g, a, p);
            }

            return q;

        }

        private static EcModPoint computePointSum(BigInteger a, EcModPoint pointP, EcModPoint pointQ, BigInteger p)
        {
            BigInteger x2 = 0;
            BigInteger y2 = 0;
            BigInteger m = 0;
            EcModPoint ret = null;

            if (null == pointQ) // P1 = P0, P2 = P0 + P1 = 2P0
                ret = pointDouble(pointP, a, p);
            else // P2 = P0 + P1
                ret = pointAdd(pointP, pointQ, a, p);

            return ret;
        }

        /// <summary>
        /// Returns the key pair
        /// </summary>
        /// <param name="P"></param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="G"></param>
        /// <param name="N"></param>
        /// <param name="SecretKey"></param>
        /// <returns></returns>
        public static EcModPoint ECKeyPairGenerator(BigInteger P, BigInteger A, BigInteger B, EcModPoint G, BigInteger N, BigInteger SecretKey)
        {
            // If private key Sk is not in the range 0 < Sk < N (order of G) then error
            if (SecretKey < 1 || SecretKey >= N)
                throw new Exception("Private key is not in the range.");

            // Check the discriminant for the eliptic curve y^2 = x^3 + a*x + b: -16(4a^3 + 27b^2) != 0
            var delta = 4 * A * A * A + 27 * B * B;
            if (0 == delta)
                throw new Exception("Delta cannot be zero.");

            return computeECPointMultiply(A, G, P, SecretKey);
        }

        /// <summary>
        /// Return point Q coords representing public key pair (see specs for secp256k1 parameters)
        /// </summary>
        /// <returns></returns>
        public static EcModPoint SecP256k1KeyPairGenerator(BigInteger Sk)
        {
            // Big prime 
            var p = BigInteger.Parse("115792089237316195423570985008687907853269984665640564039457584007908834671663");
            // Elliptic curve equation: y^2 = x^3 + a*x + b => y^2 = x^3 + 7
            var a = 0;
            var b = 7;
            // Generator coordinates
            EcModPoint G = new EcModPoint
            {
                x = BigInteger.Parse("55066263022277343669578718895168534326250603453777594175500187360389116729240"),
                y = BigInteger.Parse("32670510020758816978083085130507043184471273380659243275938904335757337482424")
            };
            // Order of G
            var ordG = BigInteger.Parse("115792089237316195423570985008687907852837564279074904382605163141518161494337");

            return ECKeyPairGenerator(p, a, b, G, ordG, Sk);
        }

        /// <summary>
        /// Returns the final point via summation. This method is very slow. Do not use big value for Sk.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="G"></param>
        /// <param name="Sk"></param>
        /// <returns></returns>
        public static EcModPoint EcSlowKeyPairGeneratorByClassicPointSum(BigInteger P, BigInteger A, EcModPoint G, int Sk)
        {
            int cnt = 2;
            bool inf = false;
            EcModPoint qMod = null;
            while (cnt <= Sk && !inf)
            {
                qMod = computePointSum(A, G, qMod, P);
                inf = G.x == qMod.x;
                ++cnt;
            }

            return qMod;
        }

        /// <summary>
        /// Returns the list of points by addition (P + Q). This method is very slow. Do not use big value for Sk.
        /// </summary>
        /// <param name="P"></param>
        /// <param name="A"></param>
        /// <param name="G"></param>
        /// <param name="Sk"></param>
        /// <returns></returns>
        public static List<EcModPoint> EcSlowPointListByAddition(BigInteger P, BigInteger A, EcModPoint G, int Sk)
        {
            var ret = new List<EcModPoint>();
            ret.Add(G);
            int cnt = 2;
            bool inf = false;
            EcModPoint qMod = null;
            while (cnt <= Sk && !inf)
            {
                qMod = computePointSum(A, G, qMod, P);
                inf = G.x == qMod.x;
                ret.Add(qMod);
                ++cnt;
            }

            return ret;
        }

        #endregion
    }
}
