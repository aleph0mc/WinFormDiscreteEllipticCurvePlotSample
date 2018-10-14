using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
using EllipticCurves.ExtensionsAndHelpers;
using System.Security.Cryptography;

namespace WinFormDiscreteEllipticCurvePlotSample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);

            //computeBitcoinHash();
        }

        private static BigInteger randomBigInteger(int BitLength)
        {
            Random rnd = new Random();
            Byte[] bytes = new Byte[BitLength * 8];
            rnd.NextBytes(bytes);

            return new BigInteger(bytes);
        }

        // CPU mining sample code
        private static void computeBitcoinHash()
        {
            var difficulty = BigInteger.Parse("2121212");
            var target = (BigInteger.Parse("65536") << 208) / difficulty;
            var count = BigInteger.One << 32;
            BigInteger hashVal = 0;
            int coinvbase_nonce = 0;
            bool found = false;
            while (!found)
            {
                var header = randomBigInteger(150);
                for (int header_nonce = 0; header_nonce < count; header_nonce++)
                {
                    hashVal = PasswordManager.Hash(PasswordManager.Hash(header + header_nonce, SHA256.Create()), SHA256.Create());
                    if (hashVal < target)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    ++coinvbase_nonce;
            }
            MessageBox.Show($"nonce: {coinvbase_nonce.ToString()} - Hash: {hashVal.ToHexadecimalString()}");
        }

        private void Calc(int secretKey)
        {
            const double scale_f = 1e50;
            BigInteger p = BigInteger.Parse("115792089237316195423570985008687907853269984665640564039457584007908834671663");
            float scale_p = (float)((double)p / scale_f);
            // Elliptic curve: y^2 = x^3 + a*x + b (mod p): y^2 = x^3 + 7
            BigInteger a = 0;
            BigInteger b = 7;
            // P = G
            var G = new EcModPoint
            {
                x = BigInteger.Parse("55066263022277343669578718895168534326250603453777594175500187360389116729240"),
                y = BigInteger.Parse("32670510020758816978083085130507043184471273380659243275938904335757337482424")
            };


            var lstPoints = BigIntegerExtensions.EcSlowPointListByAddition(p, a, G, secretKey);

            var g = pbEcDescrete.CreateGraphics();
            g.FillRectangle(Brushes.White, new RectangleF(0, 0, pbEcDescrete.Width, pbEcDescrete.Height));

            foreach (var ecPoint in lstPoints)
            {
                var x = (float)((double)ecPoint.x / scale_f);
                var y = (float)((double)ecPoint.y / scale_f);
                var scale_x = pbEcDescrete.Width / scale_p;
                var scale_y = pbEcDescrete.Height / scale_p;
                var x1 = scale_x * x;
                var y1 = scale_y * y;

                g.FillRectangle(Brushes.Blue, new RectangleF(x1, y1, 3F, 3F));
            }
        }

        //private void Form1_Paint(object sender, PaintEventArgs e)
        //{
        //    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        //    e.Graphics.TranslateTransform(-25, this.Height);
        //    e.Graphics.DrawRectangle(Pens.Red, new Rectangle(0, 0, 90, 90));
        //    e.Graphics.ScaleTransform(1, -1);
        //    foreach (var item in pointList)
        //    {
        //        e.Graphics.FillRectangle(Brushes.Blue, new RectangleF(item.X, item.Y, 1.5F, 1.5F));
        //    }
        //}

        private void btnGen_Click(object sender, EventArgs e)
        {
            int loops = 0;

            if (!int.TryParse(txtLoops.Text, out loops))
            {
                MessageBox.Show("Insert a correct value for Loop count.");
                return;
            }

            if (loops > 10000)
            {
                MessageBox.Show("Loop count must be 10000 maximum.");
                return;
            }

            Calc(loops);
        }
    }
}
