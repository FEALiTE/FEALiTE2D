using FEALiTE2D.CrossSections;
using FEALiTE2D.Elements;
using FEALiTE2D.Materials;
using FEALiTE2D.Tests.Helper;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace FEALiTE2D.Tests.Elements
{
    public class FrameElement2DTestTandK
    {
        Node2D n1;
        Node2D n2;
        Node2D n3;
        FrameElement2D e1;
        FrameElement2D e2;
        IMaterial material;
        IFrame2DSection section;

        [SetUp]
        public void Setup()
        {
            n1 = new Node2D(0, 0, "n1");
            n2 = new Node2D(0, 24*12, "n2");
            n3 = new Node2D(16*12, 12*12, "n3");
            material = new GenericIsotropicMaterial() { E = 29e3, U = 0.2, Alpha = 1e-5, Gama = 7.85, MaterialType = MaterialType.Steel, Label = "Steel" };
            section = new Generic2DSection(28.2, 28.2, 28.2, 833, 833, 0, 0, 0, material);

            e1 = new FrameElement2D(n1, n2, "f1") { CrossSection = section };
            e2 = new FrameElement2D(n2, n3, "f2") { CrossSection = section };


        }

        [Test]
        public void Test1()
        {
            var kl1 = e1.LocalStiffnessMatrix;
            var kl2 = e2.LocalStiffnessMatrix;

            var T1 = e1.TransformationMatrix;
            var T2 = e2.TransformationMatrix;

            var kg1 = e1.GlobalStiffnessMatrix;
            var kg2 = e2.GlobalStiffnessMatrix;

            var kl2expeectedCS = new CSparse.Storage.CoordinateStorage<double>(6, 6, 20);
            kl2expeectedCS.At(0, 0, 3407.5);
            kl2expeectedCS.At(3, 3, 3407.5);
            kl2expeectedCS.At(0, 3, -3407.5);
            kl2expeectedCS.At(3, 0, -3407.5);
            
            kl2expeectedCS.At(1, 1, 20.96961);
            kl2expeectedCS.At(4, 4, 20.96961);
            kl2expeectedCS.At(1, 4, -20.96961);
            kl2expeectedCS.At(4, 1, -20.96961);
            
            kl2expeectedCS.At(1, 2, 2516.3541666);
            kl2expeectedCS.At(1, 5, 2516.3541666);
            kl2expeectedCS.At(2, 1, 2516.3541666);
            kl2expeectedCS.At(5, 1, 2516.3541666);

            kl2expeectedCS.At(2, 4, -2516.3541666);
            kl2expeectedCS.At(4, 2, -2516.3541666);
            kl2expeectedCS.At(5, 4, -2516.3541666);
            kl2expeectedCS.At(4, 5, -2516.3541666);

            kl2expeectedCS.At(2, 5, 402616.666666/2);
            kl2expeectedCS.At(5, 2, 402616.666666/2);
            
            kl2expeectedCS.At(2, 2, 402616.666666);
            kl2expeectedCS.At(5, 5, 402616.666666);


            var kl2Expected = CSparse.Double.SparseMatrix.OfIndexed(kl2expeectedCS) as CSparse.Double.SparseMatrix;
           
            for (int i = 0; i < kl1.RowCount; i++)
            {
                for (int j = 0; j < kl1.ColumnCount; j++)
                {
                    Assert.AreEqual(kl2.At(i, j), kl2Expected.At(i, j), 1e-5);
                }
            }

            var T2expeectedCS = new CSparse.Storage.CoordinateStorage<double>(6, 6, 10);
            T2expeectedCS.At(0, 0, 0.8);
            T2expeectedCS.At(1, 1, 0.8);
            T2expeectedCS.At(3, 3, 0.8);
            T2expeectedCS.At(4, 4, 0.8);
          
            T2expeectedCS.At(1, 0, 0.6);
            T2expeectedCS.At(0, 1, -0.6);
            T2expeectedCS.At(3, 4, -0.6);
            T2expeectedCS.At(4, 3, 0.6);

            T2expeectedCS.At(2, 2, 1);
            T2expeectedCS.At(5,5, 1);

            var T2Expected = CSparse.Double.SparseMatrix.OfIndexed(T2expeectedCS) as CSparse.Double.SparseMatrix;
            for (int i = 0; i < kl1.RowCount; i++)
            {
                for (int j = 0; j < kl1.ColumnCount; j++)
                {
                    Assert.AreEqual(T2.At(i, j), T2Expected.At(i, j), 1e-5);
                }
            }

            //Console.WriteLine(kg2.PrintSparseMatrix());

            Assert.IsTrue(kl2Expected.IsSymmetric());
            Assert.IsTrue(kl1.IsSymmetric());
            Assert.IsTrue(kl2.IsSymmetric());

            Assert.IsTrue(kg1.IsSymmetric());
            //Assert.IsTrue(kg2.IsSymmetric());
           
            for (int i = 0; i < kg2.RowCount; i++)
            {
                for (int j = 0; j < kg2.ColumnCount; j++)
                {
                    Assert.AreEqual(kg2.At(i, j), kg2.At(j, i), 1e-8);
                }
            }


        }
    }
}
