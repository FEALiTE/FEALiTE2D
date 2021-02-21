using FEALiTE2D.CrossSections;
using FEALiTE2D.Elements;
using FEALiTE2D.Loads;
using FEALiTE2D.Materials;
using FEALiTE2D.Meshing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace FEALiTE2D.Tests.Meshing
{
    public class LinearMesherTest
    {
        [Test]
        public void TestLinearMesher()
        {
            FEALiTE2D.Structure.Structure structure = new FEALiTE2D.Structure.Structure();
            IMaterial material = new GenericIsotropicMaterial() { E = 30E6, U = 0.2, Label = "Steel", Alpha = 0.000012, Gama = 39885, MaterialType = MaterialType.Steel };
            IFrame2DSection section = new Generic2DSection(0.075, 0.075, 0.075, 0.000480, 0.000480, 0.000480 * 2, 0.1, 0.1, material);

            structure.LinearMesher = new LinearMesher(10, 1);
            Node2D n1 = new Node2D(0, 0, "n1");
            Node2D n2 = new Node2D(0, 10, "n2");
            FrameElement2D e1 = new FrameElement2D(n1, n2, "e1") { CrossSection = section };

            structure.AddElement(e1, true);

            LoadCase loadCase = new LoadCase("live", LoadCaseType.Live);
            e1.Loads.Add(new FramePointLoad(0, 0, 7.5, e1.Length / 2, loadCase, LoadDirection.Global));
            e1.Loads.Add(new FrameTrapezoidalLoad(0, 0, -15, -7, LoadDirection.Global, loadCase, 0.9, 5.3));
            e1.Loads.Add(new FrameUniformLoad(0, -12, LoadDirection.Local, loadCase, 6.3, 7));
            e1.Loads.Add(new FramePointLoad(0, -5, 0, 6.5, loadCase, LoadDirection.Global));

            structure.SetUpMeshingPoints();

            Assert.IsTrue(e1.DiscreteLocations.Count == 16);

            Assert.IsTrue(e1.DiscreteLocations.Contains(0));
            Assert.IsTrue(e1.DiscreteLocations.Contains(0.9));
            Assert.IsTrue(e1.DiscreteLocations.Contains(4.7));
            Assert.IsTrue(e1.DiscreteLocations.Contains(6.3));
            Assert.IsTrue(e1.DiscreteLocations.Contains(7));
            Assert.IsTrue(e1.DiscreteLocations.Contains(6.5));
            Assert.IsTrue(e1.DiscreteLocations.Contains(1));
            Assert.IsTrue(e1.DiscreteLocations.Contains(2));
            Assert.IsTrue(e1.DiscreteLocations.Contains(3));
            Assert.IsTrue(e1.DiscreteLocations.Contains(4));
            Assert.IsTrue(e1.DiscreteLocations.Contains(5));
            Assert.IsTrue(e1.DiscreteLocations.Contains(6));
            Assert.IsTrue(e1.DiscreteLocations.Contains(7));
            Assert.IsTrue(e1.DiscreteLocations.Contains(8));
            Assert.IsTrue(e1.DiscreteLocations.Contains(9));

        }
    }
}
