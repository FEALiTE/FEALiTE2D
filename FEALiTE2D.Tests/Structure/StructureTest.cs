using FEALiTE2D.Elements;
using FEALiTE2D.Materials;
using FEALiTE2D.CrossSections;
using System;
using System.Collections.Generic;
using System.Text;
using FEALiTE2D.Loads;
using NUnit.Framework;

namespace FEALiTE2D.Tests.Structure
{
    public class StructureTest
    {
        // confirmed with robot
        [Test]
        public void TestStructure()
        {
            // units are kN, m
            FEALiTE2D.Structure.Structure structure = new FEALiTE2D.Structure.Structure();

            Node2D n1 = new Node2D(0, 0, "n1");
            Node2D n2 = new Node2D(9, 0, "n2");
            Node2D n3 = new Node2D(0, 6, "n3");
            Node2D n4 = new Node2D(9, 6, "n4");
            Node2D n5 = new Node2D(0, 12, "n5");
            n1.Restrain(NodalDegreeOfFreedom.UX, NodalDegreeOfFreedom.UY, NodalDegreeOfFreedom.RZ); //fully restrained
            n2.Restrain(NodalDegreeOfFreedom.UX, NodalDegreeOfFreedom.UY, NodalDegreeOfFreedom.RZ); //fully restrained

            structure.AddNode(n1, n2, n3, n4, n5);
            IMaterial material = new GenericIsotropicMaterial() { E = 30E6, U = 0.2, Label = "Steel", Alpha = 0.000012, Gama = 39885, MaterialType = MaterialType.Steel };
            IFrame2DSection section = new Generic2DSection(0.075, 0.075, 0.075, 0.000480, 0.000480, 0.000480 * 2, 0.1, 0.1, material);

            FrameElement2D e1 = new FrameElement2D(n1, n3, "e1") { CrossSection = section };
            FrameElement2D e2 = new FrameElement2D(n2, n4, "e2") { CrossSection = section };
            FrameElement2D e3 = new FrameElement2D(n3, n5, "e3") { CrossSection = section };
            FrameElement2D e4 = new FrameElement2D(n3, n4, "e4") { CrossSection = section };
            FrameElement2D e5 = new FrameElement2D(n4, n5, "e5") { CrossSection = section };
            structure.AddElement(new[] { e1, e2, e3, e4, e5 });

            LoadCase loadCase = new LoadCase("live", LoadCaseType.Live);
            structure.LoadCasesToRun.Add(loadCase);
            n2.SupportDisplacementLoad.Add(new SupportDisplacementLoad(10E-3, -5E-3, 2.5 * Math.PI / 180, loadCase));
            e3.Loads.Add(new FramePointLoad(0, 0, 7.5, e3.Length / 2, loadCase, LoadDirection.Global));
            e4.Loads.Add(new FrameTrapezoidalLoad(0, 0, -15, -7, LoadDirection.Global, loadCase, 0.9, 2.7));
            e5.Loads.Add(new FrameUniformLoad(0, -12, LoadDirection.Local, loadCase));
            n3.NodalLoads.Add(new NodalLoad(80, 0, 0, LoadDirection.Global, loadCase));
            n5.NodalLoads.Add(new NodalLoad(40, 0, 0, LoadDirection.Global, loadCase));

            structure.Solve();

            var nd1 = structure.GetNodeGlobalDisplacement(n1, loadCase);
            var nd2 = structure.GetNodeGlobalDisplacement(n2, loadCase);
            var nd3 = structure.GetNodeGlobalDisplacement(n3, loadCase);
            var nd4 = structure.GetNodeGlobalDisplacement(n4, loadCase);
            var nd5 = structure.GetNodeGlobalDisplacement(n5, loadCase);

            Assert.AreEqual(nd1.Ux, 0);
            Assert.AreEqual(nd1.Uy, 0);
            Assert.AreEqual(nd1.Rz, 0);
            Assert.AreEqual(nd2.Ux, 0.01);
            Assert.AreEqual(nd2.Uy, -0.005);
            Assert.AreEqual(nd2.Rz, 0.043633231299858237);
            Assert.AreEqual(nd3.Ux, 0.26589188069505026);
            Assert.AreEqual(nd3.Uy, 0.00034194310937903029);
            Assert.AreEqual(nd3.Rz, -0.029312603676573);
            Assert.AreEqual(nd4.Ux, 0.26621001441150061);
            Assert.AreEqual(nd4.Uy, -0.0052123431093790149);
            Assert.AreEqual(nd4.Rz, -0.021088130059077989);
            Assert.AreEqual(nd5.Ux, 0.27076612292955626);
            Assert.AreEqual(nd5.Uy, 0.00065149930914949782);
            Assert.AreEqual(nd5.Rz, 0.019752367356605849);
        }

        // confirmed with robot
        [Test]
        public void TestStructure2()
        {
            // units are kN, m
            FEALiTE2D.Structure.Structure structure = new FEALiTE2D.Structure.Structure();

            Node2D n1 = new Node2D(0, 0, "n1");
            Node2D n2 = new Node2D(120, 240, "n2");
            Node2D n3 = new Node2D(360, 240, "n3");
            n1.Restrain(NodalDegreeOfFreedom.UX, NodalDegreeOfFreedom.UY, NodalDegreeOfFreedom.RZ); //fully restrained
            n3.Restrain(NodalDegreeOfFreedom.UX, NodalDegreeOfFreedom.UY, NodalDegreeOfFreedom.RZ); //fully restrained

            structure.AddNode(n1, n2, n3);
            IMaterial material = new GenericIsotropicMaterial() { E = 29E3, U = 0.2, Label = "Concrete", Alpha = 0.000012, Gama = 24.53, MaterialType = MaterialType.Concrete };
            IFrame2DSection section = new Generic2DSection(11.8, 11.8, 11.8, 310, 310, 310 * 2, 0.1, 0.1, material);

            FrameElement2D e1 = new FrameElement2D(n1, n2, "e1") { CrossSection = section };
            FrameElement2D e2 = new FrameElement2D(n2, n3, "e2") { CrossSection = section };
            structure.AddElement(new[] { e1, e2 });

            LoadCase loadCase = new LoadCase("live", LoadCaseType.Live);
            e1.Loads.Add(new FramePointLoad(0, -90, 0, e1.Length / 2, loadCase, LoadDirection.Global));
            e2.Loads.Add(new FrameUniformLoad(0, -0.125, LoadDirection.Global, loadCase, 0, 0));
            n2.NodalLoads.Add(new NodalLoad(0, 0, -1500, LoadDirection.Global, loadCase));
            structure.LoadCasesToRun.Add(loadCase);

            structure.Solve();

            var nd1 = structure.GetNodeGlobalDisplacement(n1, loadCase);
            var nd2 = structure.GetNodeGlobalDisplacement(n2, loadCase);
            var nd3 = structure.GetNodeGlobalDisplacement(n1, loadCase);

            Assert.AreEqual(nd1.Ux, 0);
            Assert.AreEqual(nd1.Uy, 0);
            Assert.AreEqual(nd1.Rz, 0);
            Assert.AreEqual(nd3.Ux, 0);
            Assert.AreEqual(nd3.Uy, 0);
            Assert.AreEqual(nd3.Rz, 0);
            Assert.AreEqual(nd2.Ux, 0.021301404056102882);
            Assert.AreEqual(nd2.Uy, -0.06732180013884803);
            Assert.AreEqual(nd2.Rz, -0.0025498997289483097);
        }

    }
}
