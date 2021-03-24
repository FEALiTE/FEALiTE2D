using FEALiTE2D.Elements;
using System.Linq;

namespace FEALiTE2D.Plotter
{
    /// <summary>
    /// A Class that draws Structure, NFD, SFD, BMD and deflection as a command line input.
    /// </summary>
    public class CADPlotter
    {
        /// <summary>
        /// Draw The Structure as AutoCAD Command line.
        /// </summary>
        /// <param name="structure">a structure to draw</param>
        /// <param name="Scalefactor">scale factor</param>
        public static void DrawStructure(FEALiTE2D.Structure.Structure structure, double Scalefactor = 1)
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter("structure.txt"))
            {
                foreach (IElement f in structure.Elements)
                {
                    Node2D node1 = f.Nodes.First();
                    Node2D node2 = f.Nodes.Last();
                    writer.WriteLine($"line\r\n{Scalefactor * node1.X},{Scalefactor * node1.Y} {Scalefactor * node2.X},{Scalefactor * node2.Y}\r\n\r\n");
                }
            }
        }

        /// <summary>
        /// Draw internal forces as AutoCAD Command line.
        /// </summary>
        /// <param name="structure">a structure to draw</param>
        /// <param name="loadCase">load case to get internal forces</param>
        /// <param name="Scalefactor">scale factor</param>
        public static void DrawInternalForces(FEALiTE2D.Structure.Structure structure, FEALiTE2D.Loads.LoadCase loadCase, double Scalefactor = 1)
        {
            using (System.IO.StreamWriter NFDwriter = new System.IO.StreamWriter("NFD.txt"))
            using (System.IO.StreamWriter SFDwriter = new System.IO.StreamWriter("SFD.txt"))
            using (System.IO.StreamWriter BMDwriter = new System.IO.StreamWriter("BMD.txt"))
            {
                foreach (IElement e in structure.Elements)
                {
                    var segs = structure.Results.GetElementInternalForces(e, loadCase);

                    foreach (var segment in segs)
                    {
                        Point2D p1 = Point2D.PointLocationOnLine(e, segment.x1);

                        Point2D p2NFD = Point2D.PointPerpendicularToLine(e, segment.x1, segment.Internalforces1.Fx * -Scalefactor);
                        Point2D p2SFD = Point2D.PointPerpendicularToLine(e, segment.x1, segment.Internalforces1.Fy * -Scalefactor);
                        Point2D p2BMD = Point2D.PointPerpendicularToLine(e, segment.x1, segment.Internalforces1.Mz * -Scalefactor);

                        Point2D p3NFD = Point2D.PointPerpendicularToLine(e, segment.x2, segment.Internalforces2.Fx * -Scalefactor);
                        Point2D p3SFD = Point2D.PointPerpendicularToLine(e, segment.x2, segment.Internalforces2.Fy * -Scalefactor);
                        Point2D p3BMD = Point2D.PointPerpendicularToLine(e, segment.x2, segment.Internalforces2.Mz * -Scalefactor);

                        Point2D p4 = Point2D.PointLocationOnLine(e, segment.x2);

                        // write nfd
                        NFDwriter.WriteLine($"line\r\n{p1.x},{p1.y} {p2NFD.x},{p2NFD.y}\r\n\r\n");
                        NFDwriter.WriteLine($"line\r\n{p2NFD.x},{p2NFD.y} {p3NFD.x},{p3NFD.y}\r\n\r\n");
                        NFDwriter.WriteLine($"line\r\n{p3NFD.x},{p3NFD.y} {p4.x},{p4.y}\r\n\r\n");

                        // write sfd
                        SFDwriter.WriteLine($"line\r\n{p1.x},{p1.y} {p2SFD.x},{p2SFD.y}\r\n\r\n");
                        SFDwriter.WriteLine($"line\r\n{p2SFD.x},{p2SFD.y} {p3SFD.x},{p3SFD.y}\r\n\r\n");
                        SFDwriter.WriteLine($"line\r\n{p3SFD.x},{p3SFD.y} {p4.x},{p4.y}\r\n\r\n");

                        // write bmd
                        BMDwriter.WriteLine($"line\r\n{p1.x},{p1.y} {p2BMD.x},{p2BMD.y}\r\n\r\n");
                        BMDwriter.WriteLine($"line\r\n{p2BMD.x},{p2BMD.y} {p3BMD.x},{p3BMD.y}\r\n\r\n");
                        BMDwriter.WriteLine($"line\r\n{p3BMD.x},{p3BMD.y} {p4.x},{p4.y}\r\n\r\n");
                    }
                }
            }
        }

        /// <summary>
        /// Draw deformed shape as AutoCAD Command line.
        /// </summary>
        /// <param name="structure">a structure to draw</param>
        /// <param name="loadCase">load case to get internal forces</param>
        /// <param name="Scalefactor">scale factor</param>
        public static void DrawDefelectionShape(FEALiTE2D.Structure.Structure structure, FEALiTE2D.Loads.LoadCase loadCase, double Scalefactor = 1)
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter("Deflection.txt"))
            {
                foreach (IElement e in structure.Elements)
                {
                    var segs = structure.Results.GetElementLocalDisplacement(e, loadCase);

                    foreach (var segment in segs)
                    {
                        Point2D p1 = Point2D.PointLocationOnLine(e, segment.x1);
                        Point2D p2def = Point2D.PointForDeflection(e, segment.x1, segment.Displacement1.Ux * Scalefactor, segment.Displacement1.Uy * Scalefactor);
                        Point2D p3def = Point2D.PointForDeflection(e, segment.x2, segment.Displacement2.Ux * Scalefactor, segment.Displacement2.Uy * Scalefactor);
                        Point2D p4 = Point2D.PointLocationOnLine(e, segment.x2);

                        writer.WriteLine($"line\r\n{p1.x},{p1.y} {p2def.x},{p2def.y}\r\n\r\n");
                        writer.WriteLine($"line\r\n{p2def.x},{p2def.y} {p3def.x},{p3def.y}\r\n\r\n");
                        writer.WriteLine($"line\r\n{p3def.x},{p3def.y} {p4.x},{p4.y}\r\n\r\n");
                    }
                }
            }
        }
    }
}