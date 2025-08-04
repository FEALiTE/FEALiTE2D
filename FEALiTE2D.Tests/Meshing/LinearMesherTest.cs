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
            Frame2DSection section = new Generic2DSection(0.075, 0.075, 0.075, 0.000480, 0.000480, 0.000480 * 2, 0.1, 0.1, material);

            structure.LinearMesher = new LinearMesher(10, 1);
            Node2D n1 = new Node2D(0, 0, "n1");
            Node2D n2 = new Node2D(0, 10, "n2");
            FrameElement2D e1 = new FrameElement2D(n1, n2, "e1") { CrossSection = section };

            structure.AddElement(e1, true);

            LoadCase loadCase = new LoadCase("live", LoadCaseType.Live);
            e1.Loads.Add(new FramePointLoad(0, 0, 7.5, e1.Length / 2, LoadDirection.Global, loadCase));
            e1.Loads.Add(new FrameTrapezoidalLoad(0, 0, -15, -7, LoadDirection.Global, loadCase, 0.9, 5.3));
            e1.Loads.Add(new FrameUniformLoad(0, -12, LoadDirection.Local, loadCase, 4, 2));
            e1.Loads.Add(new FramePointLoad(0, -5, 0, 6.5, LoadDirection.Global, loadCase));

            structure.Elements.ForEach((IElement element) => { structure.LinearMesher.SetupMeshSegments(element); });
            Assert.IsTrue(e1.MeshSegments.Count == 13);

            Assert.AreEqual(e1.MeshSegments[0].x1, 0);
            Assert.AreEqual(e1.MeshSegments[0].x2, 0.9);
            Assert.AreEqual(e1.MeshSegments[1].x1, 0.9);
            Assert.AreEqual(e1.MeshSegments[1].x2, 1);
            Assert.AreEqual(e1.MeshSegments[2].x1, 1);
            Assert.AreEqual(e1.MeshSegments[2].x2, 2);
            Assert.AreEqual(e1.MeshSegments[3].x1, 2);
            Assert.AreEqual(e1.MeshSegments[3].x2, 3);
            Assert.AreEqual(e1.MeshSegments[8].x1, 6);
            Assert.AreEqual(e1.MeshSegments[8].x2, 6.5);
        }
    }
}
