# FEALiTE2D
A finite element analysis library for 2D frame, beam and truss elements using C#.


## Features
* [x] Analysis of frame, beam, truss/link members.
* [x] Various load types:
  * [x] Frame point load.
  * [x] Frame uniform load.
  * [x] Frame trapezoidal load.
  * [x] Nodal point load.
  * [x] Support displacement.
* [x] Loads can be set in local and global coordinate system.
* [x] Nodes can have local and global coordinate system.
* [x] Node Supports have 3 degrees of freedom.
* [ ] Elastic supports using translational and rotational springs *[under development]*.
* [ ] Set members behaviour to be in tension only or compression only during analysis *[under development]*.
* [ ] Deactivate elements during analysis *[under development]*.
* [x] Variety of predefined cross-sections.
* [x] Linear mesher for better analysis results.

 
## Sign convention for loads in local coordinate system. 
![Loads - Local](https://user-images.githubusercontent.com/21183259/110693723-69acd280-81f0-11eb-87d5-8278dce86012.png)


## Sign convention for loads in global coordinate system. 
![Loads - Global](https://user-images.githubusercontent.com/21183259/110690077-281a2880-81ec-11eb-900c-8b2cc9c64b67.png)

## Example
Here is a 2D framed strcture subjected to various loading conditions
![Example1-Loads](https://user-images.githubusercontent.com/21183259/110694187-fce60800-81f0-11eb-8c27-06d883906f06.png)

```C#

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
            n2.SupportDisplacementLoad.Add(new SupportDisplacementLoad(10E-3, -5E-3, -2.5 * Math.PI / 180, loadCase));
            e3.Loads.Add(new FramePointLoad(0, 0, 7.5, e3.Length / 2, LoadDirection.Global, loadCase));
            e4.Loads.Add(new FrameTrapezoidalLoad(0, 0, -15, -7, LoadDirection.Global, loadCase, 0.9, 2.7));
            e5.Loads.Add(new FrameUniformLoad(0, -12, LoadDirection.Local, loadCase));
            n3.NodalLoads.Add(new NodalLoad(80, 0, 0, LoadDirection.Global, loadCase));
            n5.NodalLoads.Add(new NodalLoad(40, 0, 0, LoadDirection.Global, loadCase));
            n1.NodalLoads.Add(new NodalLoad(40, 0, 0, LoadDirection.Global, loadCase));

            structure.LinearMesher.NumberSegements = 35;
            structure.Solve();
        }
```
