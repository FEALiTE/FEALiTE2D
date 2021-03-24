using FEALiTE2D.Elements;
using FEALiTE2D.Plotter;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace FEALiTE2D.Tests.Plotter
{
    public class TestCADPlotter
    {

        [Test]
        public void TestDeflection()
        {
            IElement e = new FrameElement2D(new Node2D(0, 0, "n1"), new Node2D(4, 2, "n2"), "e1");
         
            //generate arbitrary points
            Point2D p = Point2D.PointForDeflection(e, 3, .17, .26);
            Assert.AreEqual(p.x, 2.71905866, 1e-4);
            Assert.AreEqual(p.y, 1.65021817, 1e-4);

            p = Point2D.PointForDeflection(e, 3, -.06, .13);
            Assert.AreEqual(p.x, 2.57147817, 1e-4);
            Assert.AreEqual(p.y, 1.43108351, 1e-4);


        }

    }
}
