using FEALiTE2D.CrossSections;
using FEALiTE2D.Elements;
using FEALiTE2D.Materials;
using FEALiTE2D.Loads;
using FEALiTE2D.Tests.Helper;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace FEALiTE2D.Tests.Loads
{
    public class FramePointLoadTest
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
            n2 = new Node2D(10 * 12, 20 * 12, "n2");
            material = new GenericIsotropicMaterial() { E = 29e3, U = 0.2, Alpha = 1e-5, Gama = 7.85, MaterialType = MaterialType.Steel, Label = "Steel" };
            section = new Generic2DSection(28.2, 28.2, 28.2, 833, 833, 0, 0, 0, material);

            e1 = new FrameElement2D(n1, n2, "f1") { CrossSection = section };
            e1.Initialize();

        }

        // confirmed with robot
        [Test]
        public void FramePointLoadTestPointLoad()
        {
            var pl = new FEALiTE2D.Loads.FramePointLoad(0, 90, 0, e1.Length / 2, new LoadCase("dead", LoadCaseType.Dead), LoadDirection.Global);
            var f = pl.GetGlobalFixedEndForces(e1);
            var fExpected = new double[] { 0, 45, 1350, 0, 45, -1350 };

            for (int i = 0; i < f.Length; i++)
            {
                Assert.AreEqual(f[i], fExpected[i], 1e-8);
            }


            pl = new FEALiTE2D.Loads.FramePointLoad(90*Math.Sin(63.43494882*Math.PI/180), 90*Math.Cos(63.43494882 * Math.PI / 180), 0, e1.Length / 2, new LoadCase("dead", LoadCaseType.Dead), LoadDirection.Local);
            f = pl.GetGlobalFixedEndForces(e1);
            fExpected = new double[] { 0, 45, 1350, 0, 45, -1350 };

            for (int i = 0; i < f.Length; i++)
            {
                Assert.AreEqual(f[i], fExpected[i], 1e-6);
            }
        }


        // confirmed with robot
        [Test]
        public void FramePointLoadTestPointAndMomentLoad()
        {
            Console.WriteLine(e1.LocalCoordinateSystemMatrix.PrintDenseMatrix());

            var pl = new FEALiTE2D.Loads.FramePointLoad(50, -90, 20, e1.Length / 2, new LoadCase("dead", LoadCaseType.Dead), LoadDirection.Global);
            var f = pl.GetGlobalFixedEndForces(e1);
            var fExpected = new double[] { 25.1, -45.05, -2855, 24.9, -44.95, 2845 };

            for (int i = 0; i < f.Length; i++)
            {
                Assert.AreEqual(f[i], fExpected[i], 1e-8);
            }

        }
    }
}
