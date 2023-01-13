using netDxf;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FEALiTE2D.Plotting.Dxf
{
    /// <summary>
    /// A class for plotting diagrams in dxf format
    /// </summary>
    public class Plotter
    {

        private BoundingRectangle boundingRectangle;

        /// <summary>
        /// Create a new instance of <see cref="Plotter"/> class.
        /// </summary>
        /// <param name="structure">A structure to draw.</param>
        /// <param name="option">Plotting options.</param>
        public Plotter(FEALiTE2D.Structure.Structure structure, PlottingOption option)
        {
            this.Structure = structure ?? throw new ArgumentNullException(nameof(structure));
            this.PlottingOption = option ?? throw new ArgumentNullException(nameof(option));
            if (structure.AnalysisStatus != FEALiTE2D.Structure.AnalysisStatus.Successful)
            {
                throw new InvalidOperationException("Structure must be successfully solved before plotting ");
            }

            boundingRectangle = BoundingRectangle();
        }

        /// <summary>
        /// A structure to draw.
        /// </summary>
        public FEALiTE2D.Structure.Structure Structure { get; }

        /// <summary>
        /// Plotting options.
        /// </summary>
        public PlottingOption PlottingOption { get; }

        /// <summary>
        /// Plot a structure in dxf format.
        /// </summary>
        /// <param name="filePath">Full path of the file to save into including .dxf extension.</param>
        /// <param name="loadCase">A load case to plot.</param>
        public void Plot(string filePath, FEALiTE2D.Loads.LoadCase loadCase)
        {
            // do some checks
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException($"'{nameof(filePath)}' cannot be null or empty.", nameof(filePath));
            }

            if (!this.Structure.LoadCasesToRun.Contains(loadCase))
            {
                throw new ArgumentException("This load case is not set to be calculated!!");
            }

            // initialize a new instance of a dxf document
            netDxf.DxfDocument dxfDocument = new netDxf.DxfDocument(this.PlottingOption.DxfVersion);

            // normal force diagram
            PlotNFD(dxfDocument, loadCase, netDxf.Vector2.Zero);

            // shear force diagram
            PlotSFD(dxfDocument, loadCase, new Vector2(boundingRectangle.Width * 1 + PlottingOption.DiagramsHorizontalOffsets * 1, 0));

            // bending moment diagram
            PlotBMD(dxfDocument, loadCase, new Vector2(boundingRectangle.Width * 2 + PlottingOption.DiagramsHorizontalOffsets * 2, 0));

            // displacement diagram
            PlotDisplacement(dxfDocument, loadCase, new Vector2(boundingRectangle.Width * 3 + PlottingOption.DiagramsHorizontalOffsets * 3, 0));


            // save the file
            dxfDocument.Save(filePath);
        }

        /// <summary>
        /// Plot a structure in dxf format.
        /// </summary>
        /// <param name="filePath">Full path of the file to save into including .dxf extension.</param>
        /// <param name="loadCases">Load cases to plot.</param>
        public void Plot(string filePath, IEnumerable<FEALiTE2D.Loads.LoadCase> loadCases)
        {
            // do some checks
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException($"'{nameof(filePath)}' cannot be null or empty.", nameof(filePath));
            }

            // initialize a new instance of a dxf document
            netDxf.DxfDocument dxfDocument = new netDxf.DxfDocument(this.PlottingOption.DxfVersion);

            double cumY = 0;

            // loop through given load cases
            foreach (var loadCase in loadCases)
            {
                if (!this.Structure.LoadCasesToRun.Contains(loadCase))
                {
                    throw new ArgumentException("This load case is not set to be calculated!!");
                }

                // normal force diagram
                PlotNFD(dxfDocument, loadCase, new Vector2(boundingRectangle.Width * 0 + PlottingOption.DiagramsHorizontalOffsets * 0, cumY));

                // shear force diagram
                PlotSFD(dxfDocument, loadCase, new Vector2(boundingRectangle.Width * 1 + PlottingOption.DiagramsHorizontalOffsets * 1, cumY));

                // bending moment diagram
                PlotBMD(dxfDocument, loadCase, new Vector2(boundingRectangle.Width * 2 + PlottingOption.DiagramsHorizontalOffsets * 2, cumY));

                // displacement diagram
                PlotDisplacement(dxfDocument, loadCase, new Vector2(boundingRectangle.Width * 3 + PlottingOption.DiagramsHorizontalOffsets * 3, cumY));

                cumY += PlottingOption.DiagramsVerticalOffsets + boundingRectangle.Height;
            }

            // save the file
            dxfDocument.Save(filePath);
        }

        /// <summary>
        /// Plot a structure in dxf format.
        /// </summary>
        /// <param name="filePath">Full path of the file to save into including .dxf extension.</param>
        /// <param name="loadCombination">A load combination to plot.</param>
        public void Plot(string filePath, FEALiTE2D.Loads.LoadCombination loadCombination)
        {
            // do some checks
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException($"'{nameof(filePath)}' cannot be null or empty.", nameof(filePath));
            }

            // initialize a new instance of a dxf document
            netDxf.DxfDocument dxfDocument = new netDxf.DxfDocument(this.PlottingOption.DxfVersion);

            // normal force diagram
            PlotNFD(dxfDocument, loadCombination, netDxf.Vector2.Zero);

            // shear force diagram
            PlotSFD(dxfDocument, loadCombination, new Vector2(boundingRectangle.Width * 1 + PlottingOption.DiagramsHorizontalOffsets * 1, 0));

            // bending moment diagram
            PlotBMD(dxfDocument, loadCombination, new Vector2(boundingRectangle.Width * 2 + PlottingOption.DiagramsHorizontalOffsets * 2, 0));

            // displacement diagram
            PlotDisplacement(dxfDocument, loadCombination, new Vector2(boundingRectangle.Width * 3 + PlottingOption.DiagramsHorizontalOffsets * 3, 0));


            // save the file
            dxfDocument.Save(filePath);
        }

        /// <summary>
        /// Plot a structure in dxf format.
        /// </summary>
        /// <param name="filePath">Full path of the file to save into including .dxf extension.</param>
        /// <param name="loadCombinations">Load combinations to plot.</param>
        public void Plot(string filePath, IEnumerable<FEALiTE2D.Loads.LoadCombination> loadCombinations)
        {
            // do some checks
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException($"'{nameof(filePath)}' cannot be null or empty.", nameof(filePath));
            }

            // initialize a new instance of a dxf document
            netDxf.DxfDocument dxfDocument = new netDxf.DxfDocument(this.PlottingOption.DxfVersion);

            double cumY = 0;

            // loop through given load cases
            foreach (var loadCombination in loadCombinations)
            {
                // normal force diagram
                PlotNFD(dxfDocument, loadCombination, new Vector2(boundingRectangle.Width * 0 + PlottingOption.DiagramsHorizontalOffsets * 0, cumY));

                // shear force diagram
                PlotSFD(dxfDocument, loadCombination, new Vector2(boundingRectangle.Width * 1 + PlottingOption.DiagramsHorizontalOffsets * 1, cumY));

                // bending moment diagram
                PlotBMD(dxfDocument, loadCombination, new Vector2(boundingRectangle.Width * 2 + PlottingOption.DiagramsHorizontalOffsets * 2, cumY));

                // displacement diagram
                PlotDisplacement(dxfDocument, loadCombination, new Vector2(boundingRectangle.Width * 3 + PlottingOption.DiagramsHorizontalOffsets * 3, cumY));

                cumY += PlottingOption.DiagramsVerticalOffsets + boundingRectangle.Height;
            }

            // save the file
            dxfDocument.Save(filePath);
        }

        /// <summary>
        /// Plot the structure given a start point.
        /// </summary>
        /// <param name="dxfDocument">A dxf document to plot into.</param>
        /// <param name="startPosition">The start point of drawing the structure in XY-plan.</param>
        public void PlotStructure(netDxf.DxfDocument dxfDocument, netDxf.Vector2 startPosition)
        {
            double scale = PlottingOption.StructureScaleFactor;

            // loop through elements first to plot them
            for (int i = 0; i < this.Structure.Elements.Count; i++)
            {
                foreach (var fe in this.Structure.Elements)
                {
                    netDxf.Entities.Line line = new netDxf.Entities.Line(
                            new netDxf.Vector2((startPosition.X + fe.Nodes[0].X) * scale, (startPosition.Y + fe.Nodes[0].Y) * scale),
                            new netDxf.Vector2((startPosition.X + fe.Nodes[1].X) * scale, (startPosition.Y + fe.Nodes[1].Y) * scale))
                    {
                        Layer = this.PlottingOption.LayerOfStructure
                    };

                    dxfDocument.AddEntity(line);
                }
            }
        }

        /// <summary>
        /// Plot normal force diagram.
        /// </summary>
        /// <param name="dxfDocument">A dxf document to plot into.</param>
        /// <param name="loadCase">A load case to plot.</param>
        /// <param name="startPosition">The start point of drawing the structure in XY-plan.</param>
        public void PlotNFD(netDxf.DxfDocument dxfDocument, FEALiTE2D.Loads.LoadCase loadCase, netDxf.Vector2 startPosition)
        {
            PlotStructure(dxfDocument, startPosition);

            double scale = PlottingOption.NFDScaleFactor;

            foreach (var e in this.Structure.Elements)
            {
                var segs = Structure.Results.GetElementInternalForces(e, loadCase);

                foreach (var segment in segs)
                {
                    Vector2 p1 = e.PointLocationOnLine(segment.x1);
                    Vector2 p2NFD = e.PointPerpendicularToLine(segment.x1, segment.Internalforces1.Fx * -scale);
                    Vector2 p3NFD = e.PointPerpendicularToLine(segment.x2, segment.Internalforces2.Fx * -scale);
                    Vector2 p4 = e.PointLocationOnLine(segment.x2);

                    // write nfd
                    netDxf.Entities.Line line1 = new netDxf.Entities.Line
                    {
                        StartPoint = new Vector3(p1.X + startPosition.X, p1.Y + startPosition.Y, 0),
                        EndPoint = new Vector3(p2NFD.X + startPosition.X, p2NFD.Y + startPosition.Y, 0),
                        Layer = PlottingOption.LayerOfNFD
                    };

                    netDxf.Entities.Line line2 = new netDxf.Entities.Line
                    {
                        StartPoint = new Vector3(p2NFD.X + startPosition.X, p2NFD.Y + startPosition.Y, 0),
                        EndPoint = new Vector3(p3NFD.X + startPosition.X, p3NFD.Y + startPosition.Y, 0),
                        Layer = PlottingOption.LayerOfNFD
                    };

                    netDxf.Entities.Line line3 = new netDxf.Entities.Line
                    {
                        StartPoint = new Vector3(p3NFD.X + startPosition.X, p3NFD.Y + startPosition.Y, 0),
                        EndPoint = new Vector3(p4.X + startPosition.X, p4.Y + startPosition.Y, 0),
                        Layer = PlottingOption.LayerOfNFD
                    };

                    dxfDocument.AddEntity(line1);
                    dxfDocument.AddEntity(line2);
                    dxfDocument.AddEntity(line3);
                }
            }

            netDxf.Entities.MText text = new netDxf.Entities.MText
            {
                Layer = PlottingOption.LayerOfNFD,
                Height = 0.01 * boundingRectangle.Width,
                Value = $"Normal Force Diagram - {loadCase.Label}",
                AttachmentPoint = netDxf.Entities.MTextAttachmentPoint.MiddleCenter,
                Position = new Vector3(startPosition.X + boundingRectangle.Width / 2.0, startPosition.Y, 0)
            };

            dxfDocument.AddEntity(text);
        }

        /// <summary>
        /// Plot normal force diagram.
        /// </summary>
        /// <param name="dxfDocument">A dxf document to plot into.</param>
        /// <param name="loadCombination">A load combination to plot.</param>
        /// <param name="startPosition">The start point of drawing the structure in XY-plan.</param>
        public void PlotNFD(netDxf.DxfDocument dxfDocument, FEALiTE2D.Loads.LoadCombination loadCombination, netDxf.Vector2 startPosition)
        {
            PlotStructure(dxfDocument, startPosition);

            double scale = PlottingOption.NFDScaleFactor;

            foreach (var e in this.Structure.Elements)
            {
                var segs = Structure.Results.GetElementInternalForces(e, loadCombination);

                foreach (var segment in segs)
                {
                    Vector2 p1 = e.PointLocationOnLine(segment.x1);
                    Vector2 p2NFD = e.PointPerpendicularToLine(segment.x1, segment.Internalforces1.Fx * -scale);
                    Vector2 p3NFD = e.PointPerpendicularToLine(segment.x2, segment.Internalforces2.Fx * -scale);
                    Vector2 p4 = e.PointLocationOnLine(segment.x2);

                    // write nfd
                    netDxf.Entities.Line line1 = new netDxf.Entities.Line
                    {
                        StartPoint = new Vector3(p1.X + startPosition.X, p1.Y + startPosition.Y, 0),
                        EndPoint = new Vector3(p2NFD.X + startPosition.X, p2NFD.Y + startPosition.Y, 0),
                        Layer = PlottingOption.LayerOfNFD
                    };

                    netDxf.Entities.Line line2 = new netDxf.Entities.Line
                    {
                        StartPoint = new Vector3(p2NFD.X + startPosition.X, p2NFD.Y + startPosition.Y, 0),
                        EndPoint = new Vector3(p3NFD.X + startPosition.X, p3NFD.Y + startPosition.Y, 0),
                        Layer = PlottingOption.LayerOfNFD
                    };

                    netDxf.Entities.Line line3 = new netDxf.Entities.Line
                    {
                        StartPoint = new Vector3(p3NFD.X + startPosition.X, p3NFD.Y + startPosition.Y, 0),
                        EndPoint = new Vector3(p4.X + startPosition.X, p4.Y + startPosition.Y, 0),
                        Layer = PlottingOption.LayerOfNFD
                    };

                    dxfDocument.AddEntity(line1);
                    dxfDocument.AddEntity(line2);
                    dxfDocument.AddEntity(line3);
                }
            }

            netDxf.Entities.MText text = new netDxf.Entities.MText
            {
                Layer = PlottingOption.LayerOfNFD,
                Height = 0.01 * boundingRectangle.Width,
                Value = $"Normal Force Diagram - {loadCombination.Label}",
                AttachmentPoint = netDxf.Entities.MTextAttachmentPoint.MiddleCenter,
                Position = new Vector3(startPosition.X + boundingRectangle.Width / 2.0, startPosition.Y, 0)
            };
            
            dxfDocument.AddEntity(text);
        }

        /// <summary>
        /// Plot shear force diagram.
        /// </summary>
        /// <param name="dxfDocument">A dxf document to plot into.</param>
        /// <param name="loadCase">A load case to plot.</param>
        /// <param name="startPosition">The start point of drawing the structure in XY-plan.</param>
        public void PlotSFD(netDxf.DxfDocument dxfDocument, FEALiTE2D.Loads.LoadCase loadCase, netDxf.Vector2 startPosition)
        {
            PlotStructure(dxfDocument, startPosition);

            double scale = PlottingOption.SFDScaleFactor;

            foreach (var e in this.Structure.Elements)
            {
                var segs = Structure.Results.GetElementInternalForces(e, loadCase);

                foreach (var segment in segs)
                {
                    Vector2 p1 = e.PointLocationOnLine(segment.x1);
                    Vector2 p2SFD = e.PointPerpendicularToLine(segment.x1, segment.Internalforces1.Fy * -scale);
                    Vector2 p3SFD = e.PointPerpendicularToLine(segment.x2, segment.Internalforces2.Fy * -scale);
                    Vector2 p4 = e.PointLocationOnLine(segment.x2);

                    // write sfd
                    netDxf.Entities.Line line1 = new netDxf.Entities.Line
                    {
                        StartPoint = new Vector3(p1.X + startPosition.X, p1.Y + startPosition.Y, 0),
                        EndPoint = new Vector3(p2SFD.X + startPosition.X, p2SFD.Y + startPosition.Y, 0),
                        Layer = PlottingOption.LayerOfSFD
                    };

                    netDxf.Entities.Line line2 = new netDxf.Entities.Line
                    {
                        StartPoint = new Vector3(p2SFD.X + startPosition.X, p2SFD.Y + startPosition.Y, 0),
                        EndPoint = new Vector3(p3SFD.X + startPosition.X, p3SFD.Y + startPosition.Y, 0),
                        Layer = PlottingOption.LayerOfSFD
                    };

                    netDxf.Entities.Line line3 = new netDxf.Entities.Line
                    {
                        StartPoint = new Vector3(p3SFD.X + startPosition.X, p3SFD.Y + startPosition.Y, 0),
                        EndPoint = new Vector3(p4.X + startPosition.X, p4.Y + startPosition.Y, 0),
                        Layer = PlottingOption.LayerOfSFD
                    };

                    dxfDocument.AddEntity(line1);
                    dxfDocument.AddEntity(line2);
                    dxfDocument.AddEntity(line3);
                }
            }

            netDxf.Entities.MText text = new netDxf.Entities.MText
            {
                Layer = PlottingOption.LayerOfSFD,
                Height = 0.01 * boundingRectangle.Width,
                Value = $"Shear Force Diagram - {loadCase.Label}",
                AttachmentPoint = netDxf.Entities.MTextAttachmentPoint.MiddleCenter,
                Position = new Vector3(startPosition.X + boundingRectangle.Width / 2.0, startPosition.Y, 0)
            };
     
            dxfDocument.AddEntity(text);

        }

        /// <summary>
        /// Plot shear force diagram.
        /// </summary>
        /// <param name="dxfDocument">A dxf document to plot into.</param>
        /// <param name="loadCombination">A load combination to plot.</param>
        /// <param name="startPosition">The start point of drawing the structure in XY-plan.</param>
        public void PlotSFD(netDxf.DxfDocument dxfDocument, FEALiTE2D.Loads.LoadCombination loadCombination, netDxf.Vector2 startPosition)
        {
            PlotStructure(dxfDocument, startPosition);

            double scale = PlottingOption.SFDScaleFactor;

            foreach (var e in this.Structure.Elements)
            {
                var segs = Structure.Results.GetElementInternalForces(e, loadCombination);

                foreach (var segment in segs)
                {
                    Vector2 p1 = e.PointLocationOnLine(segment.x1);
                    Vector2 p2SFD = e.PointPerpendicularToLine(segment.x1, segment.Internalforces1.Fy * -scale);
                    Vector2 p3SFD = e.PointPerpendicularToLine(segment.x2, segment.Internalforces2.Fy * -scale);
                    Vector2 p4 = e.PointLocationOnLine(segment.x2);

                    // write sfd
                    netDxf.Entities.Line line1 = new netDxf.Entities.Line
                    {
                        StartPoint = new Vector3(p1.X + startPosition.X, p1.Y + startPosition.Y, 0),
                        EndPoint = new Vector3(p2SFD.X + startPosition.X, p2SFD.Y + startPosition.Y, 0),
                        Layer = PlottingOption.LayerOfSFD
                    };

                    netDxf.Entities.Line line2 = new netDxf.Entities.Line
                    {
                        StartPoint = new Vector3(p2SFD.X + startPosition.X, p2SFD.Y + startPosition.Y, 0),
                        EndPoint = new Vector3(p3SFD.X + startPosition.X, p3SFD.Y + startPosition.Y, 0),
                        Layer = PlottingOption.LayerOfSFD
                    };

                    netDxf.Entities.Line line3 = new netDxf.Entities.Line
                    {
                        StartPoint = new Vector3(p3SFD.X + startPosition.X, p3SFD.Y + startPosition.Y, 0),
                        EndPoint = new Vector3(p4.X + startPosition.X, p4.Y + startPosition.Y, 0),
                        Layer = PlottingOption.LayerOfSFD
                    };

                    dxfDocument.AddEntity(line1);
                    dxfDocument.AddEntity(line2);
                    dxfDocument.AddEntity(line3);
                }
            }
      
            netDxf.Entities.MText text = new netDxf.Entities.MText
            {
                Layer = PlottingOption.LayerOfSFD,
                Height = 0.01 * boundingRectangle.Width,
                Value = $"Shear Force Diagram - {loadCombination.Label}",
                AttachmentPoint = netDxf.Entities.MTextAttachmentPoint.MiddleCenter,
                Position = new Vector3(startPosition.X + boundingRectangle.Width / 2.0, startPosition.Y, 0)
            };

            dxfDocument.AddEntity(text);
        }

        /// <summary>
        /// Plot bending moment diagram.
        /// </summary>
        /// <param name="dxfDocument">A dxf document to plot into.</param>
        /// <param name="loadCase">A load case to plot.</param>
        /// <param name="startPosition">The start point of drawing the structure in XY-plan.</param>
        public void PlotBMD(netDxf.DxfDocument dxfDocument, FEALiTE2D.Loads.LoadCase loadCase, netDxf.Vector2 startPosition)
        {
            PlotStructure(dxfDocument, startPosition);

            double scale = PlottingOption.BMDScaleFactor;

            foreach (var e in this.Structure.Elements)
            {
                var segs = Structure.Results.GetElementInternalForces(e, loadCase);

                foreach (var segment in segs)
                {
                    Vector2 p1 = e.PointLocationOnLine(segment.x1);
                    Vector2 p2BMD = e.PointPerpendicularToLine(segment.x1, segment.Internalforces1.Mz * -scale);
                    Vector2 p3BMD = e.PointPerpendicularToLine(segment.x2, segment.Internalforces2.Mz * -scale);
                    Vector2 p4 = e.PointLocationOnLine(segment.x2);

                    // write BMD
                    netDxf.Entities.Line line1 = new netDxf.Entities.Line
                    {
                        StartPoint = new Vector3(p1.X + startPosition.X, p1.Y + startPosition.Y, 0),
                        EndPoint = new Vector3(p2BMD.X + startPosition.X, p2BMD.Y + startPosition.Y, 0),
                        Layer = PlottingOption.LayerOfBMD
                    };

                    netDxf.Entities.Line line2 = new netDxf.Entities.Line
                    {
                        StartPoint = new Vector3(p2BMD.X + startPosition.X, p2BMD.Y + startPosition.Y, 0),
                        EndPoint = new Vector3(p3BMD.X + startPosition.X, p3BMD.Y + startPosition.Y, 0),
                        Layer = PlottingOption.LayerOfBMD
                    };

                    netDxf.Entities.Line line3 = new netDxf.Entities.Line
                    {
                        StartPoint = new Vector3(p3BMD.X + startPosition.X, p3BMD.Y + startPosition.Y, 0),
                        EndPoint = new Vector3(p4.X + startPosition.X, p4.Y + startPosition.Y, 0),
                        Layer = PlottingOption.LayerOfBMD
                    };

                    dxfDocument.AddEntity(line1);
                    dxfDocument.AddEntity(line2);
                    dxfDocument.AddEntity(line3);
                }
            }

            netDxf.Entities.MText text = new netDxf.Entities.MText
            {
                Layer = PlottingOption.LayerOfBMD,
                Height = 0.01 * boundingRectangle.Width,
                Value = $"Bending Moment Diagram - {loadCase.Label}",
                AttachmentPoint = netDxf.Entities.MTextAttachmentPoint.MiddleCenter,
                Position = new Vector3(startPosition.X + boundingRectangle.Width / 2.0, startPosition.Y, 0)
            };

            dxfDocument.AddEntity(text);
        }


        /// <summary>
        /// Plot bending moment diagram.
        /// </summary>
        /// <param name="dxfDocument">A dxf document to plot into.</param>
        /// <param name="loadCombination">A load combination to plot.</param>
        /// <param name="startPosition">The start point of drawing the structure in XY-plan.</param>
        public void PlotBMD(netDxf.DxfDocument dxfDocument, FEALiTE2D.Loads.LoadCombination loadCombination, netDxf.Vector2 startPosition)
        {
            PlotStructure(dxfDocument, startPosition);

            double scale = PlottingOption.BMDScaleFactor;

            foreach (var e in this.Structure.Elements)
            {
                var segs = Structure.Results.GetElementInternalForces(e, loadCombination);

                foreach (var segment in segs)
                {
                    Vector2 p1 = e.PointLocationOnLine(segment.x1);
                    Vector2 p2BMD = e.PointPerpendicularToLine(segment.x1, segment.Internalforces1.Mz * -scale);
                    Vector2 p3BMD = e.PointPerpendicularToLine(segment.x2, segment.Internalforces2.Mz * -scale);
                    Vector2 p4 = e.PointLocationOnLine(segment.x2);

                    // write BMD
                    netDxf.Entities.Line line1 = new netDxf.Entities.Line
                    {
                        StartPoint = new Vector3(p1.X + startPosition.X, p1.Y + startPosition.Y, 0),
                        EndPoint = new Vector3(p2BMD.X + startPosition.X, p2BMD.Y + startPosition.Y, 0),
                        Layer = PlottingOption.LayerOfBMD
                    };

                    netDxf.Entities.Line line2 = new netDxf.Entities.Line
                    {
                        StartPoint = new Vector3(p2BMD.X + startPosition.X, p2BMD.Y + startPosition.Y, 0),
                        EndPoint = new Vector3(p3BMD.X + startPosition.X, p3BMD.Y + startPosition.Y, 0),
                        Layer = PlottingOption.LayerOfBMD
                    };

                    netDxf.Entities.Line line3 = new netDxf.Entities.Line
                    {
                        StartPoint = new Vector3(p3BMD.X + startPosition.X, p3BMD.Y + startPosition.Y, 0),
                        EndPoint = new Vector3(p4.X + startPosition.X, p4.Y + startPosition.Y, 0),
                        Layer = PlottingOption.LayerOfBMD
                    };

                    dxfDocument.AddEntity(line1);
                    dxfDocument.AddEntity(line2);
                    dxfDocument.AddEntity(line3);
                }
            }
     
            netDxf.Entities.MText text = new netDxf.Entities.MText
            {
                Layer = PlottingOption.LayerOfBMD,
                Height = 0.01 * boundingRectangle.Width,
                Value = $"Bending Moment Diagram - {loadCombination.Label}",
                AttachmentPoint = netDxf.Entities.MTextAttachmentPoint.MiddleCenter,
                Position = new Vector3(startPosition.X + boundingRectangle.Width / 2.0, startPosition.Y, 0)
            };

            dxfDocument.AddEntity(text);
        }

        /// <summary>
        /// Plot displacement diagram.
        /// </summary>
        /// <param name="dxfDocument">A dxf document to plot into.</param>
        /// <param name="loadCase">A load case to plot.</param>
        /// <param name="startPosition">The start point of drawing the structure in XY-plan.</param>
        public void PlotDisplacement(netDxf.DxfDocument dxfDocument, FEALiTE2D.Loads.LoadCase loadCase, netDxf.Vector2 startPosition)
        {
            PlotStructure(dxfDocument, startPosition);

            double scale = PlottingOption.DisplacmentScaleFactor;

            foreach (var e in this.Structure.Elements)
            {
                var segs = Structure.Results.GetElementInternalForces(e, loadCase);

                foreach (var segment in segs)
                {
                    Vector2 p2def = e.PointForDeflection(segment.x1, segment.Displacement1.Ux * scale, segment.Displacement1.Uy * scale);
                    Vector2 p3def = e.PointForDeflection(segment.x2, segment.Displacement2.Ux * scale, segment.Displacement2.Uy * scale);

                    netDxf.Entities.Line line1 = new netDxf.Entities.Line
                    {
                        StartPoint = new Vector3(p2def.X + startPosition.X, p2def.Y + startPosition.Y, 0),
                        EndPoint = new Vector3(p3def.X + startPosition.X, p3def.Y + startPosition.Y, 0),
                        Layer = PlottingOption.LayerOfDisplacement
                    };

                    dxfDocument.AddEntity(line1);
                }
            }
     
            netDxf.Entities.MText text = new netDxf.Entities.MText
            {
                Layer = PlottingOption.LayerOfDisplacement,
                Height = 0.01 * boundingRectangle.Width,
                Value = $"Bending Moment Diagram - {loadCase.Label}",
                AttachmentPoint = netDxf.Entities.MTextAttachmentPoint.MiddleCenter,
                Position = new Vector3(startPosition.X + boundingRectangle.Width / 2.0, startPosition.Y, 0)
            };

            dxfDocument.AddEntity(text);
        }

        /// <summary>
        /// Plot displacement diagram.
        /// </summary>
        /// <param name="dxfDocument">A dxf document to plot into.</param>
        /// <param name="loadCombination">A load combination to plot.</param>
        /// <param name="startPosition">The start point of drawing the structure in XY-plan.</param>
        public void PlotDisplacement(netDxf.DxfDocument dxfDocument, FEALiTE2D.Loads.LoadCombination loadCombination, netDxf.Vector2 startPosition)
        {
            PlotStructure(dxfDocument, startPosition);

            double scale = PlottingOption.DisplacmentScaleFactor;

            foreach (var e in this.Structure.Elements)
            {
                var segs = Structure.Results.GetElementInternalForces(e, loadCombination);

                foreach (var segment in segs)
                {
                    Vector2 p2def = e.PointForDeflection(segment.x1, segment.Displacement1.Ux * scale, segment.Displacement1.Uy * scale);
                    Vector2 p3def = e.PointForDeflection(segment.x2, segment.Displacement2.Ux * scale, segment.Displacement2.Uy * scale);

                    netDxf.Entities.Line line1 = new netDxf.Entities.Line
                    {
                        StartPoint = new Vector3(p2def.X + startPosition.X, p2def.Y + startPosition.Y, 0),
                        EndPoint = new Vector3(p3def.X + startPosition.X, p3def.Y + startPosition.Y, 0),
                        Layer = PlottingOption.LayerOfDisplacement
                    };

                    dxfDocument.AddEntity(line1);
                }
            }

            netDxf.Entities.MText text = new netDxf.Entities.MText
            {
                Layer = PlottingOption.LayerOfDisplacement,
                Height = 0.01 * boundingRectangle.Width,
                Value = $"Displacement Diagram - {loadCombination.Label}",
                AttachmentPoint = netDxf.Entities.MTextAttachmentPoint.MiddleCenter,
                Position = new Vector3(startPosition.X + boundingRectangle.Width / 2.0, startPosition.Y, 0)
            };

            dxfDocument.AddEntity(text);
        }


        /// <summary>
        ///  bonding box of the structure.
        /// </summary>
        private netDxf.BoundingRectangle BoundingRectangle()
        {
            List<Vector2> allPoints = new List<Vector2>(this.Structure.Nodes.Count);
            Parallel.ForEach(this.Structure.Nodes, (n) =>
            {
                allPoints.Add(new Vector2(n.X, n.Y));
            });

            return new netDxf.BoundingRectangle(allPoints);
        }
    }
}