using System.Collections.Generic;

namespace FEALiTE2D.Loads
{
    /// <summary>
    /// A class for load combination results when more than one load case acts on the structure.
    /// These two "factored loads" are combined (added) to determine the resultant loads.
    /// </summary>
    public class LoadCombination : Dictionary<LoadCase, double>
    {

        /// <summary>
        /// Create new instance of <see cref="LoadCombination"/> class.
        /// </summary>
        public LoadCombination()
        {
        }

        /// <summary>
        /// Create new instance of <see cref="LoadCombination"/> class.
        /// </summary>
        /// <param name="name">name of the load combination</param>
        public LoadCombination(string name)
        {
            this.Label = name;
        }

        /// <summary>
        /// Name of the <see cref="LoadCombination"/>
        /// </summary>
        public string Label { get; set; }
    }
}
