using FEALiTE2D.Elements;
using FEALiTE2D.Materials;
using FEALiTE2D.CrossSections;
using System;
using FEALiTE2D.Loads;
using NUnit.Framework;
using FEALiTE2D.Structure;

namespace FEALiTE2D.Tests.Structure
{
    public class StructureTest
    {

        // confirmed with robot
        [Test]
        public void TestStructure()
        {
            // units are kN, m
            var structure = new FEALiTE2D.Structure.Structure();

            var n1 = new Node2D(0, 0, "n1");
            var n2 = new Node2D(9, 0, "n2");
            var n3 = new Node2D(0, 6, "n3");
            var n4 = new Node2D(9, 6, "n4");
            var n5 = new Node2D(0, 12, "n5");
            n1.Support = new NodalSupport(true, true, true); //fully restrained
            n2.Support = new NodalSupport(true, true, true); //fully restrained

            structure.AddNode(n1, n2, n3, n4, n5);
            IMaterial material = new GenericIsotropicMaterial() { E = 30E6, U = 0.2, Label = "Steel", Alpha = 0.000012, Gama = 39885, MaterialType = MaterialType.Steel };
            Frame2DSection section = new Generic2DSection(0.075, 0.075, 0.075, 0.000480, 0.000480, 0.000480 * 2, 0.1, 0.1, material);

            var e1 = new FrameElement2D(n1, n3, "e1") { CrossSection = section };
            var e2 = new FrameElement2D(n2, n4, "e2") { CrossSection = section };
            var e3 = new FrameElement2D(n3, n5, "e3") { CrossSection = section };
            var e4 = new FrameElement2D(n3, n4, "e4") { CrossSection = section };
            var e5 = new FrameElement2D(n4, n5, "e5") { CrossSection = section };
            structure.AddElement(new[] { e1, e2, e3, e4, e5 });

            var loadCase = new LoadCase("live", LoadCaseType.Live);
            structure.LoadCasesToRun.Add(loadCase);
            n2.SupportDisplacementLoad.Add(new SupportDisplacementLoad(10E-3, -5E-3, -2.5 * Math.PI / 180, loadCase));
            e3.Loads.Add(new FramePointLoad(0, 0, 7.5, e3.Length / 2, LoadDirection.Global, loadCase));
            e4.Loads.Add(new FrameTrapezoidalLoad(0, 0, -15, -7, LoadDirection.Global, loadCase, 0.9, 2.7));
            e5.Loads.Add(new FrameUniformLoad(0, -12, LoadDirection.Local, loadCase));
            n3.NodalLoads.Add(new NodalLoad(80, 0, 0, LoadDirection.Global, loadCase));
            n5.NodalLoads.Add(new NodalLoad(40, 0, 0, LoadDirection.Global, loadCase));
            n1.NodalLoads.Add(new NodalLoad(40, 0, 0, LoadDirection.Global, loadCase));

            structure.LinearMesher.NumberSegments = 35;
            structure.Solve();

            var nd1 = structure.Results.GetNodeGlobalDisplacement(n1, loadCase);
            var nd2 = structure.Results.GetNodeGlobalDisplacement(n2, loadCase);
            var nd3 = structure.Results.GetNodeGlobalDisplacement(n3, loadCase);
            var nd4 = structure.Results.GetNodeGlobalDisplacement(n4, loadCase);
            var nd5 = structure.Results.GetNodeGlobalDisplacement(n5, loadCase);

            Assert.AreEqual(nd1.Ux, 0);
            Assert.AreEqual(nd1.Uy, 0);
            Assert.AreEqual(nd1.Rz, 0);
            Assert.AreEqual(nd2.Ux, 0.01);
            Assert.AreEqual(nd2.Uy, -0.005);
            Assert.AreEqual(nd2.Rz, -0.043633231299858237);
            Assert.AreEqual(nd3.Ux, 0.26589188069505026);
            Assert.AreEqual(nd3.Uy, 0.00034194310937903029);
            Assert.AreEqual(nd3.Rz, -0.029312603676573);
            Assert.AreEqual(nd4.Ux, 0.26621001441150061);
            Assert.AreEqual(nd4.Uy, -0.0052123431093790149);
            Assert.AreEqual(nd4.Rz, -0.021088130059077989);
            Assert.AreEqual(nd5.Ux, 0.27076612292955626);
            Assert.AreEqual(nd5.Uy, 0.00065149930914949782);
            Assert.AreEqual(nd5.Rz, 0.019752367356605849);

            var R1 = structure.Results.GetSupportReaction(n1, loadCase);
            var R2 = structure.Results.GetSupportReaction(n2, loadCase);

            Assert.AreEqual(R1.Fx, -182.36325573226503);
            Assert.AreEqual(R1.Fy, -128.22866601713636);
            Assert.AreEqual(R1.Mz, 497.44001602057028);
            Assert.AreEqual(R2.Fx, -49.636744267753542);
            Assert.AreEqual(R2.Fy, 79.628666017130627);
            Assert.AreEqual(R2.Mz, 94.801989825388063);
        }

        // confirmed with robot
        [Test]
        public void TestStructure2()
        {
            // units are kips, in
            var structure = new FEALiTE2D.Structure.Structure();

            var n1 = new Node2D(0, 0, "n1");
            var n2 = new Node2D(120, 240, "n2");
            var n3 = new Node2D(360, 240, "n3");
            n1.Support = new NodalSupport(true, true, true); //fully restrained
            n3.Support = new NodalSupport(true, true, true); //fully restrained

            structure.AddNode(n1, n2, n3);
            IMaterial material = new GenericIsotropicMaterial() { E = 29E3, U = 0.2, Label = "Concrete", Alpha = 0.000012, Gama = 24.53, MaterialType = MaterialType.Concrete };
            Frame2DSection section = new Generic2DSection(11.8, 11.8, 11.8, 310, 310, 310 * 2, 0.1, 0.1, material);

            var e1 = new FrameElement2D(n1, n2, "e1") { CrossSection = section };
            var e2 = new FrameElement2D(n2, n3, "e2") { CrossSection = section };
            structure.AddElement(new[] { e1, e2 });

            var loadCase = new LoadCase("live", LoadCaseType.Live);
            e1.Loads.Add(new FramePointLoad(0, -90, 0, e1.Length / 2, LoadDirection.Global, loadCase));
            e2.Loads.Add(new FrameUniformLoad(0, -0.125, LoadDirection.Global, loadCase, 0, 0));
            n2.NodalLoads.Add(new NodalLoad(0, 0, -1500, LoadDirection.Global, loadCase));
            structure.LoadCasesToRun.Add(loadCase);
            structure.LinearMesher.NumberSegments = 50;
            structure.Solve();

            Assert.AreEqual(structure.Results.GetSupportReaction(n1, loadCase), Force.FromVector(new double[] { 30.37225194999335, 102.08675797670341, 1215.9664523968904 }));
            Assert.AreEqual(structure.Results.GetSupportReaction(n2, loadCase), null);
            Assert.AreEqual(structure.Results.GetSupportReaction(n3, loadCase), Force.FromVector(new double[] { -30.37225194999336, 17.913242023296597, -854.0740487820697 }));

            var nd1 = structure.Results.GetNodeGlobalDisplacement(n1, loadCase);
            var nd2 = structure.Results.GetNodeGlobalDisplacement(n2, loadCase);
            var nd3 = structure.Results.GetNodeGlobalDisplacement(n3, loadCase);

            Assert.AreEqual(nd1.Ux, 0);
            Assert.AreEqual(nd1.Uy, 0);
            Assert.AreEqual(nd1.Rz, 0);
            Assert.AreEqual(nd3.Ux, 0);
            Assert.AreEqual(nd3.Uy, 0);
            Assert.AreEqual(nd3.Rz, 0);
            Assert.AreEqual(nd2.Ux, 0.021301404056102882);
            Assert.AreEqual(nd2.Uy, -0.06732180013884803);
            Assert.AreEqual(nd2.Rz, -0.0025498997289483097);

            var ql1 = structure.Results.GetElementLocalFixedEndForces(e1, loadCase);
            var ql2 = structure.Results.GetElementLocalFixedEndForces(e2, loadCase);
            var fg1 = structure.Results.GetElementGlobalFixedEndForces(e1, loadCase);
            var fg2 = structure.Results.GetElementGlobalFixedEndForces(e2, loadCase);

            Assert.AreEqual(ql1[0], 104.89205617337821);
            Assert.AreEqual(ql1[1], 18.488818091721274);
            Assert.AreEqual(ql1[2], 1215.9664523968904);
            Assert.AreEqual(ql1[3], -24.39360898338576);
            Assert.AreEqual(ql1[4], 21.76040550327495);
            Assert.AreEqual(ql1[5], -1654.8959631908865);

            Assert.AreEqual(ql2[0], 30.37225194999336);
            Assert.AreEqual(ql2[1], 12.086757976703401);
            Assert.AreEqual(ql2[2], 154.8959631908864);
            Assert.AreEqual(ql2[3], -30.37225194999336);
            Assert.AreEqual(ql2[4], 17.913242023296597);
            Assert.AreEqual(ql2[5], -854.0740487820697);

            Assert.AreEqual(fg1[0], 30.37225194999335);
            Assert.AreEqual(fg1[1], 102.08675797670341);
            Assert.AreEqual(fg1[2], 1215.9664523968904);
            Assert.AreEqual(fg1[3], -30.37225194999335);
            Assert.AreEqual(fg1[4], -12.08675797670338);
            Assert.AreEqual(fg1[5], -1654.8959631908865);

            Assert.AreEqual(fg2[0], 30.37225194999336);
            Assert.AreEqual(fg2[1], 12.086757976703401);
            Assert.AreEqual(fg2[2], 154.8959631908864);
            Assert.AreEqual(fg2[3], -30.37225194999336);
            Assert.AreEqual(fg2[4], 17.913242023296597);
            Assert.AreEqual(fg2[5], -854.0740487820697);

            var op = new FEALiTE2D.Plotting.Dxf.PlottingOption { NFDScaleFactor = 0.5, SFDScaleFactor = 5.0, BMDScaleFactor = 0.1, DisplacmentScaleFactor = 100, DiagramsHorizontalOffsets = 200 };
            var plotter = new FEALiTE2D. Plotting.Dxf.Plotter(structure, op);
            plotter.Plot("D:\\text.dxf", loadCase);
        }

        [Test]
        public void TestStructure2WithSettelment()
        {
            // units are kip, in
            var structure = new FEALiTE2D.Structure.Structure();

            var n1 = new Node2D(0, 0, "n1");
            var n2 = new Node2D(120, 240, "n2");
            var n3 = new Node2D(360, 240, "n3");
            n1.Support = new NodalSupport(true, true, true); //fully restrained
            n3.Support = new NodalSupport(true, true, true); //fully restrained

            structure.AddNode(n1, n2, n3);
            IMaterial material = new GenericIsotropicMaterial() { E = 29E3, U = 0.2, Label = "Concrete", Alpha = 0.000012, Gama = 24.53, MaterialType = MaterialType.Concrete };
            Frame2DSection section = new Generic2DSection(11.8, 11.8, 11.8, 310, 310, 310 * 2, 0.1, 0.1, material);

            var e1 = new FrameElement2D(n1, n2, "e1") { CrossSection = section };
            var e2 = new FrameElement2D(n2, n3, "e2") { CrossSection = section };
            structure.AddElement(new[] { e1, e2 });

            var loadCase = new LoadCase("live", LoadCaseType.Live);
            e1.Loads.Add(new FramePointLoad(0, -90, 0, e1.Length / 2, LoadDirection.Global, loadCase));
            e2.Loads.Add(new FrameUniformLoad(0, -0.125, LoadDirection.Global, loadCase, 0, 0));
            n2.NodalLoads.Add(new NodalLoad(0, 0, -1500, LoadDirection.Global, loadCase));
            n1.SupportDisplacementLoad.Add(new SupportDisplacementLoad(0, -1, 0, loadCase));
            structure.LoadCasesToRun.Add(loadCase);

            structure.Solve();

            var nd1 = structure.Results.GetNodeGlobalDisplacement(n1, loadCase);
            var nd2 = structure.Results.GetNodeGlobalDisplacement(n2, loadCase);
            var nd3 = structure.Results.GetNodeGlobalDisplacement(n3, loadCase);

            Assert.AreEqual(nd1.Ux, 0);
            Assert.AreEqual(nd1.Uy, -1);
            Assert.AreEqual(nd1.Rz, 0);
            Assert.AreEqual(nd3.Ux, 0);
            Assert.AreEqual(nd3.Uy, 0);
            Assert.AreEqual(nd3.Rz, 0);
            Assert.AreEqual(nd2.Ux, 0.017760708393401745);
            Assert.AreEqual(nd2.Uy, -1.059915467868475);
            Assert.AreEqual(nd2.Rz, 0.00074191638715062017);

            var ql1 = structure.Results.GetElementLocalFixedEndForces(e1, loadCase);
            var ql2 = structure.Results.GetElementLocalFixedEndForces(e2, loadCase);
            var fg1 = structure.Results.GetElementGlobalFixedEndForces(e1, loadCase);
            var fg2 = structure.Results.GetElementGlobalFixedEndForces(e2, loadCase);

            Assert.AreEqual(ql1[0], 98.463276589933216);
            Assert.AreEqual(ql1[1], 20.91875793338594);
            Assert.AreEqual(ql1[2], 1431.6889020967453);
            Assert.AreEqual(ql1[3], -17.964829399940768);
            Assert.AreEqual(ql1[4], 19.330465661610283);
            Assert.AreEqual(ql1[5], -1218.5971328270693);

            Assert.AreEqual(ql2[0], 25.323810050925321);
            Assert.AreEqual(ql2[1], 7.4233848457643195);
            Assert.AreEqual(ql2[2], -281.40286717293077);
            Assert.AreEqual(ql2[3], -25.323810050925321);
            Assert.AreEqual(ql2[4], 22.576615154235679);
            Assert.AreEqual(ql2[5], -1536.9847698436315);

            Assert.AreEqual(fg1[0], 25.323810050925285);
            Assert.AreEqual(fg1[1], 97.423384845764559);
            Assert.AreEqual(fg1[2], 1431.6889020967453);
            Assert.AreEqual(fg1[3], -25.323810050925282);
            Assert.AreEqual(fg1[4], -7.42338484576452);
            Assert.AreEqual(fg1[5], -1218.5971328270693);

            Assert.AreEqual(fg2[0], 25.323810050925321);
            Assert.AreEqual(fg2[1], 7.4233848457643195);
            Assert.AreEqual(fg2[2], -281.40286717293077);
            Assert.AreEqual(fg2[3], -25.323810050925321);
            Assert.AreEqual(fg2[4], 22.576615154235679);
            Assert.AreEqual(fg2[5], -1536.9847698436315);
        }


        [Test]
        public void TestContinousBeam()
        {
            // units are kN, m
            var structure = new FEALiTE2D.Structure.Structure();

            var n1 = new Node2D(0, 0, "n1");
            var n2 = new Node2D(6, 0, "n2");
            var n3 = new Node2D(10, 0, "n3");
            var n4 = new Node2D(20, 0, "n4");

            n1.Support = new NodalSupport(true, true, true); //fully restrained
            n3.Support = new NodalSupport(false, true, false); //roller support
            n4.Support = new NodalSupport(false, true, false); //roller support

            structure.AddNode(n1, n2, n3, n4);
            IMaterial material = new GenericIsotropicMaterial() { E = 28E6, U = 0.2, MaterialType = MaterialType.Concrete };
            Frame2DSection section1 = new Generic2DSection(0.03228, 0.03228, 0.03228, 0.0058, 0.0058, 0, 0, 0, material);
            Frame2DSection section2 = new Generic2DSection(1.5 * 0.03228, 1.5 * 0.03228, 1.5 * 0.1634, 1.5 * 0.0058, 1.5 * 0.0058, 0, 0, 0, material);

            var e1 = new FrameElement2D(n1, n2, "e1") { CrossSection = section2 };
            var e2 = new FrameElement2D(n2, n3, "e2") { CrossSection = section1 };
            var e3 = new FrameElement2D(n3, n4, "e3") { CrossSection = section1 };
            structure.AddElement(new[] { e1, e2, e3 });

            var loadCase = new LoadCase("live", LoadCaseType.Live);
            structure.LoadCasesToRun.Add(loadCase);
            e1.Loads.Add(new FrameTrapezoidalLoad(0, 0, -30, 0, LoadDirection.Global, loadCase, 0, 0));
            e3.Loads.Add(new FramePointLoad(0, -150, 0, e3.Length / 2, LoadDirection.Global, loadCase));
            n2.NodalLoads.Add(new NodalLoad(0, -200, 0, LoadDirection.Global, loadCase));
            n3.NodalLoads.Add(new NodalLoad(0, 0, -90, LoadDirection.Global, loadCase));

            structure.LinearMesher.NumberSegments = 20;
            structure.Solve();

            var R1 = structure.Results.GetSupportReaction(n1, loadCase);
            var R2 = structure.Results.GetSupportReaction(n2, loadCase);
            var R3 = structure.Results.GetSupportReaction(n3, loadCase);
            var R4 = structure.Results.GetSupportReaction(n4, loadCase);
            var nd1 = structure.Results.GetNodeGlobalDisplacement(n1, loadCase);
            var nd2 = structure.Results.GetNodeGlobalDisplacement(n2, loadCase);
            var nd3 = structure.Results.GetNodeGlobalDisplacement(n3, loadCase);
            var nd4 = structure.Results.GetNodeGlobalDisplacement(n4, loadCase);

            Assert.AreEqual(nd1.Ux, 0);
            Assert.AreEqual(nd1.Uy, 0);
            Assert.AreEqual(nd1.Rz, 0);
            Assert.AreEqual(nd2.Ux, 0);
            Assert.AreEqual(nd2.Uy, -0.0044728627208516182);
            Assert.AreEqual(nd2.Rz, 0.00056143270452086369);
            Assert.AreEqual(nd3.Ux, 0);
            Assert.AreEqual(nd3.Uy, 0);
            Assert.AreEqual(nd3.Rz, -0.0006841653841759686);
            Assert.AreEqual(nd4.Ux, 0);
            Assert.AreEqual(nd4.Uy, 0);
            Assert.AreEqual(nd4.Rz, 0.0032284743177037486);

            Assert.AreEqual(R1.Fx, 0);
            Assert.AreEqual(R1.Fy, 146.32690995907231);
            Assert.AreEqual(R1.Mz, 281.18656207366985);
            Assert.AreEqual(R2, null);
            Assert.AreEqual(R3.Fx, 0);
            Assert.AreEqual(R3.Fy, 243.46483628922235);
            Assert.AreEqual(R3.Mz, 0);
            Assert.AreEqual(R4.Fx, 0);
            Assert.AreEqual(R4.Fy, 50.208253751705314);
            Assert.AreEqual(R4.Mz, 0);
        }


        [Test]
        public void TestInternalForces()
        {
            // units are kN, m
            var structure = new FEALiTE2D.Structure.Structure();

            var n1 = new Node2D(0, 0, "n1");
            var n2 = new Node2D(5, 0, "n2");
            var n3 = new Node2D(10, 0, "n3");

            n1.Support = new NodalSupport(true, true, false); //hinged restrained
            n3.Support = new NodalSupport(true, true, false); //hinged restrained

            structure.AddNode(n1, n2, n3);
            IMaterial material = new GenericIsotropicMaterial() { E = 30000000, U = 0.2, MaterialType = MaterialType.Concrete };
            Frame2DSection section1 = new RectangularSection(0.25, 0.75, material);

            var e1 = new FrameElement2D(n1, n2, "e1") { CrossSection = section1 };
            var e2 = new FrameElement2D(n2, n3, "e2") { CrossSection = section1 };
            structure.AddElement(new[] { e1, e2 });

            var loadCase = new LoadCase("live", LoadCaseType.Live);
            structure.LoadCasesToRun.Add(loadCase);
            //e2.Loads.Add(new FrameUniformLoad(0, -7.5, LoadDirection.Global, loadCase));
            e1.Loads.Add(new FrameTrapezoidalLoad(100, 0, -13.5, -5.5, LoadDirection.Global, loadCase, 1.35));

            structure.LinearMesher.NumberSegments = 10;
            structure.Solve();

            var MeshSegments = structure.Results.GetElementInternalForces(e1, loadCase);

            Assert.AreEqual(MeshSegments[0].InternalForces1.Fy, 24.553854166666675);
            Assert.AreEqual(MeshSegments[0].InternalForces2.Fy, 24.553854166666675);
            Assert.AreEqual(MeshSegments[3].InternalForces2.Fy, 22.553511700913251);
            Assert.AreEqual(MeshSegments[4].InternalForces1.Fy, 22.553511700913251);
            Assert.AreEqual(MeshSegments[4].InternalForces2.Fy, 16.241867865296811);
            Assert.AreEqual(MeshSegments[5].InternalForces1.Fy, 16.241867865296811);
            Assert.AreEqual(MeshSegments[5].InternalForces2.Fy, 10.478169235159823);
            Assert.AreEqual(MeshSegments[6].InternalForces1.Fy, 10.478169235159823);
            Assert.AreEqual(MeshSegments[6].InternalForces2.Fy, 5.2624158105022882);
            Assert.AreEqual(MeshSegments[0].InternalForces1.Mz, 0, 1e-8);
            Assert.AreEqual(MeshSegments[0].InternalForces2.Mz, -12.276927083333323);
            Assert.AreEqual(MeshSegments[3].InternalForces2.Mz, -36.680139126712334);
            Assert.AreEqual(MeshSegments[4].InternalForces1.Mz, -36.680139126712326);
            Assert.AreEqual(MeshSegments[4].InternalForces2.Mz, -46.356152968036533);
            Assert.AreEqual(MeshSegments[5].InternalForces1.Mz, -46.356152968036533);
            Assert.AreEqual(MeshSegments[5].InternalForces2.Mz, -53.013331192922386);
            Assert.AreEqual(MeshSegments[6].InternalForces1.Mz, -53.013331192922372);
            Assert.AreEqual(MeshSegments[6].InternalForces2.Mz, -56.925646404109592);
        }

        [Test]
        public void TestSprings()
        {
            // units are kN, m
            var structure = new FEALiTE2D.Structure.Structure();

            var n1 = new Node2D(0, 0, "n1");
            var n2 = new Node2D(0, -5, "n2");
            var n3 = new Node2D(0, 5, "n3");
            var n4 = new Node2D(5, 0, "n3");

            n1.Support = new NodalSupport(true, true, true);
            n2.Support = new NodalSupport(true, true, true);
            n3.Support = new NodalSupport(true, true, true);
            n4.Support = new NodalSupport(false, false, true);

            structure.AddNode(n1, n2, n3, n4);

            var e1 = new SpringElement2D(n1, n4, "e1") { K = 1000 };
            var e2 = new SpringElement2D(n2, n4, "e2") { K = 1000 };
            var e3 = new SpringElement2D(n3, n4, "e3") { K = 1000 };
            structure.AddElement(new[] { e1, e2, e3 });

            var loadCase = new LoadCase("live", LoadCaseType.Live);
            structure.LoadCasesToRun.Add(loadCase);
            n4.NodalLoads.Add(new NodalLoad(0, -100, 0, LoadDirection.Global, loadCase));

            structure.Solve();

            Assert.AreEqual(structure.Results.GetSupportReaction(n1, loadCase), Force.FromVector(new double[] { 0, 0, 0 }));
            Assert.AreEqual(structure.Results.GetSupportReaction(n2, loadCase), Force.FromVector(new double[] { 50, 50, 0 }));
            Assert.AreEqual(structure.Results.GetSupportReaction(n3, loadCase), Force.FromVector(new double[] { -50, 50, 0 }));
            Assert.AreEqual(structure.Results.GetSupportReaction(n4, loadCase), Force.FromVector(new double[] { 0, 0, 0 }));

            Assert.AreEqual(structure.Results.GetNodeGlobalDisplacement(n1, loadCase), Displacement.FromVector(new double[] { 0, 0, 0 }));
            Assert.AreEqual(structure.Results.GetNodeGlobalDisplacement(n2, loadCase), Displacement.FromVector(new double[] { 0, 0, 0 }));
            Assert.AreEqual(structure.Results.GetNodeGlobalDisplacement(n3, loadCase), Displacement.FromVector(new double[] { 0, 0, 0 }));
            Assert.AreEqual(structure.Results.GetNodeGlobalDisplacement(n4, loadCase), Displacement.FromVector(new double[] { 0, -0.10, 0 }));
        }

        [Test]
        public void TestSprings2()
        {
            // units are kN, m
            var structure = new FEALiTE2D.Structure.Structure();

            var n1 = new Node2D(0, 0, "n1");
            var n2 = new Node2D(1, 0, "n2");
            var n3 = new Node2D(2, 0, "n3");
            var n4 = new Node2D(3, 0, "n4");
            var n5 = new Node2D(4, 0, "n5");

            n1.Support = new NodalSupport(true, true, true);
            n2.Support = new NodalSupport(false, true, true);
            n3.Support = new NodalSupport(false, true, true);
            n4.Support = new NodalSupport(false, true, true);
            n5.Support = new NodalSupport(true, true, true);

            structure.AddNode(n1, n2, n3, n4, n5);

            var e1 = new SpringElement2D(n1, n2, "e1") { K = 20 };
            var e2 = new SpringElement2D(n2, n3, "e2") { K = 20 };
            var e3 = new SpringElement2D(n3, n4, "e3") { K = 20 };
            var e4 = new SpringElement2D(n4, n5, "e4") { K = 20 };
            structure.AddElement(new[] { e1, e2, e3, e4 });

            var loadCase = new LoadCase("live", LoadCaseType.Live);
            structure.LoadCasesToRun.Add(loadCase);
            n3.NodalLoads.Add(new NodalLoad(5, 0, 0, LoadDirection.Global, loadCase));

            structure.Solve();

            Assert.AreEqual(structure.Results.GetSupportReaction(n1, loadCase), Force.FromVector(new double[] { -2.5, 0, 0 }));
            Assert.AreEqual(structure.Results.GetSupportReaction(n2, loadCase), Force.FromVector(new double[] { 0, 0, 0 }));
            Assert.AreEqual(structure.Results.GetSupportReaction(n3, loadCase), Force.FromVector(new double[] { 0, 0, 0 }));
            Assert.AreEqual(structure.Results.GetSupportReaction(n4, loadCase), Force.FromVector(new double[] { 0, 0, 0 }));
            Assert.AreEqual(structure.Results.GetSupportReaction(n5, loadCase), Force.FromVector(new double[] { -2.5, 0, 0 }));

            Assert.AreEqual(structure.Results.GetNodeGlobalDisplacement(n1, loadCase), Displacement.FromVector(new double[] { 0, 0, 0 }));
            Assert.AreEqual(structure.Results.GetNodeGlobalDisplacement(n2, loadCase), Displacement.FromVector(new double[] { 0.125, 0, 0 }));
            Assert.AreEqual(structure.Results.GetNodeGlobalDisplacement(n3, loadCase), Displacement.FromVector(new double[] { 0.25, 0, 0 }));
            Assert.AreEqual(structure.Results.GetNodeGlobalDisplacement(n4, loadCase), Displacement.FromVector(new double[] { 0.125, 0, 0 }));
            Assert.AreEqual(structure.Results.GetNodeGlobalDisplacement(n5, loadCase), Displacement.FromVector(new double[] { 0, 0, 0 }));

        }

        [Test]
        public void TestBeamWihEndRelease()
        {
            // units are kN, m
            var structure = new FEALiTE2D.Structure.Structure();

            var n1 = new Node2D(0, 0, "n1");
            var n2 = new Node2D(3, 0, "n2");
            var n3 = new Node2D(7, 0, "n3");

            n1.Support = new NodalSupport(true, true, false); //hinged
            n2.Support = new NodalSupport(true, true, false); //hinged
            n3.Support = new NodalSupport(true, true, false); //hinged

            structure.AddNode(n1, n2, n3);
            IMaterial material = new GenericIsotropicMaterial() { E = 30000000, U = 0.2, MaterialType = MaterialType.Concrete };
            Frame2DSection section1 = new RectangularSection(0.25, 0.75, material);

            var e1 = new FrameElement2D(n1, n2, "e1") { CrossSection = section1 };
            var e2 = new FrameElement2D(n2, n3, "e3") { CrossSection = section1, EndRelease = Frame2DEndRelease.StartRelease };
            structure.AddElement(new IElement[] { e1, e2 });

            var loadCase = new LoadCase("live", LoadCaseType.Live);
            structure.LoadCasesToRun.Add(loadCase);
            e1.Loads.Add(new FrameUniformLoad(0, -3.5, LoadDirection.Global, loadCase));
            e2.Loads.Add(new FramePointLoad(0, -6, 0, 2, LoadDirection.Global, loadCase));

            structure.LinearMesher.NumberSegments = 50;
            structure.Solve();

            var nd1 = structure.Results.GetNodeGlobalDisplacement(n1, loadCase);
            var nd2 = structure.Results.GetNodeGlobalDisplacement(n2, loadCase);
            var nd3 = structure.Results.GetNodeGlobalDisplacement(n3, loadCase);

            Assert.AreEqual(structure.Results.GetSupportReaction(n1, loadCase), new Force() { Fx = 0, Fy = 5.25, Mz = 0 });
            Assert.AreEqual(structure.Results.GetSupportReaction(n2, loadCase), new Force() { Fx = 0, Fy = 8.25, Mz = 0 });
            Assert.AreEqual(structure.Results.GetSupportReaction(n3, loadCase), new Force() { Fx = 0, Fy = 3.0, Mz = 0 });

            Assert.AreEqual(nd1.Rz, -1.49333333333e-5, 1e-5);
            Assert.AreEqual(nd2.Rz, 1.49333333333e-5, 1e-5);
            Assert.AreEqual(nd3.Rz, 2.27555555557e-5, 1e-5);
        }

        [Test]
        public void TestTrussWithSpringElementAsSupport()
        {
            var n1 = new Node2D(0, 0, "n1");
            var n2 = new Node2D(-3.5355339059327376220042218105242, 3.5355339059327376220042218105242, "n2");
            var n3 = new Node2D(-10, 0, "n3");
            var n4 = new Node2D(0, -1, "n4");

            n1.Support = new NodalSupport(false, false, true);
            n2.Support = new NodalSupport(true, true, true);
            n3.Support = new NodalSupport(true, true, true);
            n4.Support = new NodalSupport(true, true, true);

            var loadCase = new LoadCase("ll", LoadCaseType.Live);
            n1.NodalLoads.Add(new NodalLoad(0, -50, 0, LoadDirection.Global, loadCase));

            var system = new FEALiTE2D.Structure.Structure();
            system.AddNode(n1, n2, n3, n4);
            system.LoadCasesToRun.Add(loadCase);

            var e1 = new FrameElement2D(n1, n2, "e1") { EndRelease = Frame2DEndRelease.FullRelease };
            var e2 = new FrameElement2D(n1, n3, "e2") { EndRelease = Frame2DEndRelease.FullRelease };
            var e3 = new SpringElement2D(n1, n4, "e3") { K = 2000 };

            e1.CrossSection = e2.CrossSection = new FEALiTE2D.CrossSections.Generic2DSection(5.0e-4, 5.0e-4, 5.0e-4, 0.1, 0.1, 0.1, 0.1, 0.1, new GenericIsotropicMaterial() { E = 210000000, U = 0.2 });

            system.AddElement(new IElement[] { e1, e2, e3 }, false);
            system.Solve();

            Assert.AreEqual(system.Results.GetSupportReaction(n2, loadCase), new Force() { Fx = -36.20689655172414, Fy = 36.20689655172414, Mz = 0 });
            Assert.AreEqual(system.Results.GetSupportReaction(n3, loadCase), new Force() { Fx = 36.206896551724135, Fy = 0, Mz = 0 });
            Assert.AreEqual(system.Results.GetSupportReaction(n4, loadCase), new Force() { Fx = 0, Fy = 13.793103448275861, Mz = 0 });

            var d1 = system.Results.GetNodeGlobalDisplacement(n1, loadCase);
            Assert.AreEqual(d1.Ux, -0.00344827586206897, 1e-5);
            Assert.AreEqual(d1.Uy, -0.00689655172413793, 1e-5);
        }


        [Test]
        public void TestBeamWihEndReleaseAndSupportsAsSprings()
        {
            // units are kN, m
            var structure = new FEALiTE2D.Structure.Structure();

            var n1 = new Node2D(0, 0, "n1");
            var n2 = new Node2D(3, 0, "n2");
            var n3 = new Node2D(7, 0, "n3");
            var n1_ = new Node2D(0, -1, "n4");
            var n2_ = new Node2D(3, -1, "n5");
            var n3_ = new Node2D(7, -1, "n6");

            n1_.Support = new NodalSupport(true, true, true);
            n2_.Support = new NodalSupport(true, true, true);
            n3_.Support = new NodalSupport(true, true, true);

            structure.AddNode(n1, n2, n3, n1_, n2_, n3_);
            IMaterial material = new GenericIsotropicMaterial() { E = 30000000, U = 0.2, MaterialType = MaterialType.Concrete };
            Frame2DSection section1 = new RectangularSection(0.25, 0.75, material);

            var e1 = new FrameElement2D(n1, n2, "e1") { CrossSection = section1 };
            var e2 = new FrameElement2D(n2, n3, "e2") { CrossSection = section1, EndRelease = Frame2DEndRelease.StartRelease };
            var s1 = new SpringElement2D(n1, n1_, "s1") { K = 1e15 };
            var s2 = new SpringElement2D(n2, n2_, "s2") { K = 1e15 };
            var s3 = new SpringElement2D(n3, n3_, "s3") { K = 1e15 };
            structure.AddElement(new IElement[] { e1, e2, s1, s2, s3 });

            var loadCase = new LoadCase("live", LoadCaseType.Live);
            structure.LoadCasesToRun.Add(loadCase);
            e1.Loads.Add(new FrameUniformLoad(0, -3.5, LoadDirection.Global, loadCase));
            e2.Loads.Add(new FramePointLoad(0, -6, 0, 2, LoadDirection.Global, loadCase));

            structure.Solve();

            var nd1 = structure.Results.GetNodeGlobalDisplacement(n1, loadCase);
            var nd2 = structure.Results.GetNodeGlobalDisplacement(n2, loadCase);
            var nd3 = structure.Results.GetNodeGlobalDisplacement(n3, loadCase);

            Assert.AreEqual(structure.Results.GetSupportReaction(n1_, loadCase), new Force() { Fx = 0, Fy = 5.25, Mz = 0 });
            Assert.AreEqual(structure.Results.GetSupportReaction(n2_, loadCase), new Force() { Fx = 0, Fy = 8.25, Mz = 0 });
            Assert.AreEqual(structure.Results.GetSupportReaction(n3_, loadCase), new Force() { Fx = 0, Fy = 3.0, Mz = 0 });

            Assert.AreEqual(nd1.Rz, -1.49333333333e-5, 1e-5);
            Assert.AreEqual(nd2.Rz, 1.49333333333e-5, 1e-5);
            Assert.AreEqual(nd3.Rz, 2.27555555557e-5, 1e-5);
        }

        [Test]
        public void TestBeamWihEndReleaseAndSpringsSupports()
        {
            // units are kN, m
            var structure = new FEALiTE2D.Structure.Structure();

            var n1 = new Node2D(0, 0, "n1");
            var n2 = new Node2D(3, 0, "n2");
            var n3 = new Node2D(7, 0, "n3");

            n1.Support = new NodalSpringSupport(1e15, 1e15, 0);
            n2.Support = new NodalSpringSupport(1e15, 1e15, 0);
            n3.Support = new NodalSpringSupport(1e15, 1e15, 0);

            structure.AddNode(n1, n2, n3);
            IMaterial material = new GenericIsotropicMaterial() { E = 30000000, U = 0.2, MaterialType = MaterialType.Concrete };
            Frame2DSection section1 = new RectangularSection(0.25, 0.75, material);

            var e1 = new FrameElement2D(n1, n2, "e1") { CrossSection = section1 };
            var e2 = new FrameElement2D(n2, n3, "e2") { CrossSection = section1, EndRelease = Frame2DEndRelease.StartRelease };
            structure.AddElement(new IElement[] { e1, e2 });

            var loadCase = new LoadCase("live", LoadCaseType.Live);
            structure.LoadCasesToRun.Add(loadCase);
            e1.Loads.Add(new FrameUniformLoad(0, -3.5, LoadDirection.Global, loadCase));
            e2.Loads.Add(new FramePointLoad(0, -6, 0, 2, LoadDirection.Global, loadCase));

            structure.Solve();

            var nd1 = structure.Results.GetNodeGlobalDisplacement(n1, loadCase);
            var nd2 = structure.Results.GetNodeGlobalDisplacement(n2, loadCase);
            var nd3 = structure.Results.GetNodeGlobalDisplacement(n3, loadCase);

            Assert.AreEqual(structure.Results.GetSupportReaction(n1, loadCase), new Force() { Fx = 0, Fy = 5.25, Mz = 0 });
            Assert.AreEqual(structure.Results.GetSupportReaction(n2, loadCase), new Force() { Fx = 0, Fy = 8.25, Mz = 0 });
            Assert.AreEqual(structure.Results.GetSupportReaction(n3, loadCase), new Force() { Fx = 0, Fy = 3.0, Mz = 0 });

            Assert.AreEqual(nd1.Rz, -1.49333333333e-5, 1e-5);
            Assert.AreEqual(nd2.Rz, 1.49333333333e-5, 1e-5);
            Assert.AreEqual(nd3.Rz, 2.27555555557e-5, 1e-5);
        }


        // confirmed with robot
        [Test]
        public void TestStructureWithLoadCombination()
        {
            // units are kN, m
            var structure = new FEALiTE2D.Structure.Structure();

            var n1 = new Node2D(1, 0, "n1");
            var n2 = new Node2D(1, 2, "n2");
            var n3 = new Node2D(1, 4, "n3");
            var n4 = new Node2D(4, 0, "n4");
            var n5 = new Node2D(4, 2, "n5");
            var n6 = new Node2D(4, 4, "n6");
            var n7 = new Node2D(0, 4, "n7");
            var n8 = new Node2D(5, 4, "n8");
            //n1.Support = new NodalSpringSupport(10e5, 10e5, 10e5); //fully restrained
            //n4.Support = new NodalSpringSupport(10e5, 10e5, 10e5); //fully restrained
            n1.Support = new NodalSupport(true, true, true); //fully restrained
            n4.Support = new NodalSupport(true, true, true); //fully restrained

            structure.AddNode(n1, n2, n3, n4, n5, n6, n7, n8);
            IMaterial material = new GenericIsotropicMaterial() { E = 30E6, U = 0.2, Label = "Steel", Alpha = 0.000012, Gama = 39885, MaterialType = MaterialType.Steel };
            Frame2DSection Columns_Section = new CircularSection(0.4, material);
            Frame2DSection Beam_Section = new RectangularSection(0.4, 0.4, material);

            // columns
            var e1 = new FrameElement2D(n1, n2, "e1") { CrossSection = Columns_Section };
            var e2 = new FrameElement2D(n2, n3, "e2") { CrossSection = Columns_Section };
            var e3 = new FrameElement2D(n4, n5, "e3") { CrossSection = Columns_Section };
            var e4 = new FrameElement2D(n5, n6, "e4") { CrossSection = Columns_Section };

            // beams
            var e5 = new FrameElement2D(n7, n3, "e5") { CrossSection = Beam_Section };
            var e6 = new FrameElement2D(n3, n6, "e6") { CrossSection = Beam_Section };
            var e7 = new FrameElement2D(n6, n8, "e7") { CrossSection = Beam_Section };
            var e8 = new FrameElement2D(n2, n5, "e8") { CrossSection = Beam_Section, EndRelease = Frame2DEndRelease.FullRelease };
            structure.AddElement(new[] { e1, e2, e3, e4, e5, e6, e7, e8 });

            var LiveLoadCase = new LoadCase("Live", LoadCaseType.Live);
            var DeadLoadCase = new LoadCase("Dead", LoadCaseType.Dead);
            structure.LoadCasesToRun.AddRange(new[] { LiveLoadCase, DeadLoadCase });

            e5.Loads.Add(new FrameTrapezoidalLoad(0, 0, 0, -15, LoadDirection.Local, LiveLoadCase));
            e6.Loads.Add(new FrameUniformLoad(0, -15, LoadDirection.Local, LiveLoadCase));
            e7.Loads.Add(new FrameTrapezoidalLoad(0, 0, -15, 0, LoadDirection.Local, LiveLoadCase));

            n2.NodalLoads.Add(new NodalLoad(20, 0, 0, LoadDirection.Global, DeadLoadCase));

            structure.LinearMesher.NumberSegments = 30;
            structure.Solve();

            var op = new Plotting.Dxf.PlottingOption
            {
                NFDScaleFactor = 0.02,
                SFDScaleFactor = 0.05,
                BMDScaleFactor = 0.1,
                DisplacmentScaleFactor = 1000,
                DiagramsHorizontalOffsets = 2
            };
            var plotter = new Plotting.Dxf.Plotter(structure, op);
            plotter.Plot("D:\\text.dxf", LiveLoadCase);
            plotter.Plot("D:\\text2.dxf", structure.LoadCasesToRun);

            var loadCombo = new LoadCombination("uls");
            loadCombo.Add(DeadLoadCase, 1.35);
            loadCombo.Add(LiveLoadCase, 1.5);

            var R1ll = structure.Results.GetSupportReaction(n1, LiveLoadCase);
            var R4ll = structure.Results.GetSupportReaction(n4, LiveLoadCase);
            var R1dl = structure.Results.GetSupportReaction(n1, DeadLoadCase);
            var R4dl = structure.Results.GetSupportReaction(n4, DeadLoadCase);
            var R1c = structure.Results.GetSupportReaction(n1, loadCombo);
            var R4c = structure.Results.GetSupportReaction(n4, loadCombo);

            Assert.AreEqual(R1ll.Fx, -1.0545205, 1e-5);
            Assert.AreEqual(R1ll.Fy, 30, 1e-5);
            Assert.AreEqual(R1ll.Mz, 0.67707616, 1e-5);
            Assert.AreEqual(R4ll.Fx, 1.0545205, 1e-5);
            Assert.AreEqual(R4ll.Fy, 30, 1e-5);
            Assert.AreEqual(R4ll.Mz, -0.6770761, 1e-5);

            Assert.AreEqual(R1dl.Fx, -10.149307, 1e-5);
            Assert.AreEqual(R1dl.Fy, -3.0919293, 1e-5);
            Assert.AreEqual(R1dl.Mz, 15.5190549, 1e-5);
            Assert.AreEqual(R4dl.Fx, -9.8506923, 1e-5);
            Assert.AreEqual(R4dl.Fy, 3.09192933, 1e-5);
            Assert.AreEqual(R4dl.Mz, 15.2051570, 1e-5);

            Assert.AreEqual(R1c.Fx, -1.0545205 * 1.5 + 1.35 * -10.149307, 1e-5);
            Assert.AreEqual(R1c.Fy, 30 * 1.5 + 1.35 * -3.0919293, 1e-5);
            Assert.AreEqual(R1c.Mz, 0.67707616 * 1.5 + 1.35 * 15.5190549, 1e-5);
            Assert.AreEqual(R4c.Fx, 1.0545205 * 1.5 + 1.35 * -9.8506923, 1e-5);
            Assert.AreEqual(R4c.Fy, 30 * 1.5 + 1.35 * 3.09192933, 1e-5);
            Assert.AreEqual(R4c.Mz, -0.6770761 * 1.5 + 1.35 * 15.2051570, 1e-5);
        }
    }
}
