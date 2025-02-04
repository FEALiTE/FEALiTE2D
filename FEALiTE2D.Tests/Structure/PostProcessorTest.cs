using System;
using FEALiTE2D.CrossSections;
using FEALiTE2D.Elements;
using FEALiTE2D.Loads;
using FEALiTE2D.Materials;
using NUnit.Framework;

namespace FEALiTE2D.Tests.Structure
{
    [TestFixture]
    public class PostProcessorTest
    {
        private FEALiTE2D.Structure.Structure _structure;
        private FrameElement2D _elem;
        private LoadCase _lc;
        private const double _L = 1.0;
        
        [SetUp]
        public void SetUp()
        {
            var material = new GenericIsotropicMaterial()
            {
                E = 1000.0,
                U = 0.3,
                Label = "Mat1"
            };
            var section = new Generic2DSection(1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, material);

            _structure = new FEALiTE2D.Structure.Structure();
            
            var n1 = new Node2D(0.0, 0.0, "n1");
            var n2 = new Node2D(_L, 0.0, "n2");

            n1.Support = new NodalSupport(true, true, false);
            n2.Support = new NodalSupport(false, true, false);
            
            _elem = new FrameElement2D(n1, n2, "e1")
            {
                CrossSection = section,
            };

            _lc = new LoadCase("General", LoadCaseType.Dead);
            
            _structure.AddElement(_elem, true);
            _structure.LoadCasesToRun.Add(_lc);
        }
        
        [Test]
        public void GetElementInternalForcesAt_WithUniformLoad_ReturnsCorrectForce()
        {
            _elem.Loads.Add(new FrameUniformLoad(0.0, -1.0, LoadDirection.Local, _lc));
            
            _structure.Solve();

            var postProcessor = _structure.Results;

            var f1 = postProcessor.GetElementInternalForcesAt(_elem, _lc, 0.0);
            var f2 = postProcessor.GetElementInternalForcesAt(_elem, _lc, 1.0);

            var expectedLeftEndForce = new double[] {0.0, 0.5, 0.0}; // Fx, Fy, Mz
            var expectedRightEndForce = new double[] {0.0, -0.5, 0.0}; // Fx, Fy, Mz

            Assert.Multiple(() =>
            {
                Assert.That(new double[] { f1.Fx, f1.Fy, f1.Mz }, Is.EqualTo(expectedLeftEndForce).Within(1E-3));
                Assert.That(new double[] { f2.Fx, f2.Fy, f2.Mz }, Is.EqualTo(expectedRightEndForce).Within(1E-3));
            });
        }
        
        [Test]
        public void GetElementInternalForcesAt_WithPartialUniformLoad_EdgeSegment_ReturnsCorrectForce()
        {
            var w = 0.39023287672392115;
            var a = 0.16751904679908325;
            var b = 0.637225136662688 - a;
            var c = _L - a - b;
            
            _elem.Loads.Add(new FrameUniformLoad(0.0, w, LoadDirection.Local, _lc, a, c));
            
            _structure.LinearMesher.NumberSegements = 4;
            _structure.Solve();
            
            var postProcessor = _structure.Results;

            var x = 1.0;
            var expectedForce = new[] {0.0, 0.0737, 0.0}; // Fx, Fy, Mz
            
            var f = postProcessor.GetElementInternalForcesAt(_elem, _lc, x);
            var actualForce = new[] { f.Fx, f.Fy, f.Mz };
            
            Assert.That(actualForce, Is.EqualTo(expectedForce).Within(1E-3));
        }
        
        
        [Test]
        public void GetElementInternalForcesAt_WithPartialUniformLoad_ReturnsCorrectForce()
        {
            var w = -1.0;
            var a = 0.25;
            var b = 0.5;
            var c = _L - a - b;
            
            _elem.Loads.Add(new FrameUniformLoad(0.0, w, LoadDirection.Local, _lc, a, c));
            _structure.Solve();
            
            var postProcessor = _structure.Results;

            var x = 0.5;
            var R1 = Math.Abs(w) * b / (2 * _L) * (2 * c + b);
            var Mmax = R1 * (a + R1 / (2 * Math.Abs(w)));
            var expectedForce = new[] {0.0, 0.0, -Mmax}; // Fx, Fy, Mz
            
            var f = postProcessor.GetElementInternalForcesAt(_elem, _lc, x);
            var actualForce = new[] { f.Fx, f.Fy, f.Mz };
            
            Assert.That(actualForce, Is.EqualTo(expectedForce).Within(1E-3));
        }

        [Test]
        public void GetElementInternalForcesAt_WithTrapezoidalLoad_ReturnsCorrectForce()
        {
            var w = -1.0;
            _elem.Loads.Add(new FrameTrapezoidalLoad(0.0, 0.0, 0.0, w, LoadDirection.Local, _lc));
            
            _structure.Solve();
            
            var postProcessor = _structure.Results;
            
            var W = Math.Abs(w) * _L / 2;
            var x = 0.5;
            var Mx = W * x / (3 * _L * _L) * (_L * _L - x * x);
            var Vx = W / 3 - W * x * x / (_L * _L);
            var expectedForce = new[] {0.0, Vx, -Mx}; // Fx, Fy, Mz
            
            var f = postProcessor.GetElementInternalForcesAt(_elem, _lc, x);
            var actualForce = new[] { f.Fx, f.Fy, f.Mz };
            
            Assert.That(actualForce, Is.EqualTo(expectedForce).Within(1E-3));
        }

        [Test]
        public void GetElementInternalForcesAt_WithPartialTrapezoidalLoad_ReturnCorrectForce()
        {
            var w = -1.0;
            var a = 0.25;
            var b = 0.5;
            var c = _L - a - b;
            _elem.Loads.Add(new FrameTrapezoidalLoad(0.0, 0.0, 0.0, w, LoadDirection.Local, _lc, a, c));
            
            _structure.Solve();

            var x = 0.5;
            
            var postProcessor = _structure.Results;
            var f = postProcessor.GetElementInternalForcesAt(_elem, _lc, x);
            var actualForce = new[] { f.Fx, f.Fy, f.Mz };
        }
    }
}