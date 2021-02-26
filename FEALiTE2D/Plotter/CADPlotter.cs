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


    }
}
