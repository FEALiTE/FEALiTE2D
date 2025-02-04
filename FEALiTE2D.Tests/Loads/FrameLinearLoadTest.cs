using FEALiTE2D.CrossSections;
using FEALiTE2D.Elements;
using FEALiTE2D.Materials;
using FEALiTE2D.Loads;
using FEALiTE2D.Helper;
using FEALiTE2D.Tests.Helper;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using MathNet.Numerics.Integration;

namespace FEALiTE2D.Tests.Loads
{
    public class FrameLinearLoadTest
    {
        Node2D n1;
        Node2D n2;
        FrameElement2D e1;
        IMaterial material;
        IFrame2DSection section;

        [SetUp]
        public void Setup()
        {
            n1 = new Node2D(0, 0, "n1");
            n2 = new Node2D(10, 0, "n2");
            material = new GenericIsotropicMaterial() { E = 21e6, U = 0.2, Alpha = 1e-5, Gama = 7.85, MaterialType = MaterialType.Steel, Label = "Steel" };
            section = new Generic2DSection(0.3 * 0.3, 0.3 * 0.3, 0.3 * 0.3, 0.3 * 0.3 * 0.3 * 0.3 / 12, 0.3 * 0.3 * 0.3 * 0.3 / 12, 0, 0, 0, material);

            e1 = new FrameElement2D(n1, n2, "f1") { CrossSection = section };
            e1.Initialize();

        }

        // confirmed with robot
        [Test]
        public void FrameUniformLoadStraightTest()
        {
            Console.WriteLine(e1.LocalCoordinateSystemMatrix.PrintDenseMatrix());

            var pl = new FEALiTE2D.Loads.FrameUniformLoad(11, 7, LoadDirection.Local, new LoadCase("dead", LoadCaseType.Dead), 3, 2);
            var f = pl.GetGlobalFixedEndForces(e1);
            var fexpected = new[] { 24.75, 15.1025, 36.42916666, 30.25, 19.8975, -42.904166666 };

            for (int i = 0; i < 6; i++)
            {
                Assert.AreEqual(f[i], fexpected[i], 1e-8);
            }
        }

        // confirmed with robot
        [Test]
        public void FrameUniformLoadLocalStraightTest2()
        {
            e1.EndNode = new Node2D(10, 10, "n2");
            e1.Initialize();
            Console.WriteLine(e1.LocalCoordinateSystemMatrix.PrintDenseMatrix());

            var pl = new FEALiTE2D.Loads.FrameUniformLoad(11, 7, LoadDirection.Local, new LoadCase("dead", LoadCaseType.Dead), 3, 2);
            var f = pl.GetGlobalFixedEndForces(e1);
            var fexpected = new[] { 12.476367613, 53.60458482541, 92.1875294514, 13.381496759, 62.7558048677, -101.4205092616 };

            for (int i = 0; i < 6; i++)
            {
                Assert.AreEqual(f[i], fexpected[i], 1e-8);
            }

            var pl_1 = pl.GetLoadValueAt(e1, 0);
            Assert.IsNull(pl_1);
            pl_1 = pl.GetLoadValueAt(e1, 1);
            Assert.IsNull(pl_1);
            pl_1 = pl.GetLoadValueAt(e1, 14);
            Assert.IsNull(pl_1);
            pl_1 = pl.GetLoadValueAt(e1, 13);
            Assert.IsNull(pl_1);
            pl_1 = pl.GetLoadValueAt(e1, 10);
            Assert.IsNotNull(pl_1);
            Assert.AreEqual((pl_1 as FrameUniformLoad).Wx, 11);
            Assert.AreEqual((pl_1 as FrameUniformLoad).Wy, 7);
        }

        // confirmed with robot
        [Test]
        public void FrameUniformLoadGlobalStraightTest3()
        {
            e1.EndNode = new Node2D(10, 10, "n2");
            e1.Initialize();
            Console.WriteLine(e1.LocalCoordinateSystemMatrix.PrintDenseMatrix());

            var pl = new FEALiTE2D.Loads.FrameUniformLoad(11, 7, LoadDirection.Global, new LoadCase("dead", LoadCaseType.Dead), 3, 2);
            var f = pl.GetGlobalFixedEndForces(e1);
            var fexpected = new[] { 46.539755217801385, 29.921445911116606, -37.249386980526339, 54.023736643239047, 34.073503455000022, 40.980074200163081 };

            for (int i = 0; i < 6; i++)
            {
                Assert.AreEqual(f[i], fexpected[i], 1e-8);
            }

            var pl_1 = pl.GetLoadValueAt(e1, 0);
            Assert.IsNull(pl_1);
            pl_1 = pl.GetLoadValueAt(e1, 1);
            Assert.IsNull(pl_1);
            pl_1 = pl.GetLoadValueAt(e1, 14);
            Assert.IsNull(pl_1);
            pl_1 = pl.GetLoadValueAt(e1, 13);
            Assert.IsNull(pl_1);
            pl_1 = pl.GetLoadValueAt(e1, 10);
            Assert.IsNotNull(pl_1);
            Assert.AreEqual((pl_1 as FrameUniformLoad).Wx, 7 * Math.Cos(45 * Math.PI / 180) + 11 * Math.Cos(45 * Math.PI / 180), 1e-5);
            Assert.AreEqual((pl_1 as FrameUniformLoad).Wy, 7 * Math.Sin(45 * Math.PI / 180) - 11 * Math.Sin(45 * Math.PI / 180), 1e-5);
        }

        // confirmed with robot
        [Test]
        public void FrameTrapezoidalLoadStraightTest()
        {
            var pl = new FEALiTE2D.Loads.FrameTrapezoidalLoad(0, 0, -10, -20, LoadDirection.Local, new LoadCase(), 2, 1.5);
            var f = pl.GetGlobalFixedEndForces(e1);
            var fexpected = new[] { 0, -40.7899375, -93.517395833, 0, -56.7100625, 113.5346875 };

            for (int i = 0; i < 6; i++)
            {
                Assert.AreEqual(f[i], fexpected[i], 1e-8);
            }

            var pl_1 = pl.GetLoadValueAt(e1, 0);
            Assert.IsNull(pl_1);
            pl_1 = pl.GetLoadValueAt(e1, 1);
            Assert.IsNull(pl_1);
            pl_1 = pl.GetLoadValueAt(e1, 1.5);
            Assert.IsNull(pl_1);
            pl_1 = pl.GetLoadValueAt(e1, 8.6);
            Assert.IsNull(pl_1);
            pl_1 = pl.GetLoadValueAt(e1, 5);
            Assert.IsNotNull(pl_1);
            Assert.AreEqual((pl_1 as FrameTrapezoidalLoad).Wx1, 0);
            Assert.AreEqual((pl_1 as FrameTrapezoidalLoad).Wy1, -14.615384615384617, 1e-5);
        }
    }
}
