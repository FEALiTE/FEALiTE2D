namespace FEALiTE2D.Loads
{
    /// <summary>
    /// Represent a class for load cases.
    /// </summary>
    public class LoadCase
    {
        /// <summary>
        /// Creates new instance of a <see cref="LoadCase"/>.
        /// </summary>
        public LoadCase()
        {
            this.IsLinearCase = true;
            LoadCaseDuration = LoadCaseDuration.Permanent;
        }


        /// <summary>
        /// Creates new instance of a <see cref="LoadCase"/>.
        /// </summary>
        ///<param name="label">load case label.</param>
        /// <param name="type">Load case type.</param>
        public LoadCase(string label, LoadCaseType type) : this()
        {
            this.Label = label;
            this.LoadCaseType = type;
        }


        /// <summary>
        /// Name of the <see cref="LoadCase"/>
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Type of the <see cref="LoadCase"/>.
        /// </summary>
        public LoadCaseType LoadCaseType { get; set; }

        /// <summary>
        /// Duration of the <see cref="LoadCase"/>.
        /// </summary>
        public LoadCaseDuration LoadCaseDuration { get; set; }

        /// <summary>
        /// Gets whether it's a linear load case or not.
        /// </summary>
        public bool IsLinearCase { get; }

        /// <inheritdoc/>
        public override string ToString() => $"{Label}, {LoadCaseType.ToString()}";

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, null) || ReferenceEquals(obj, null))
            {
                return false;
            }
            if (this.GetType() != typeof(LoadCase) || obj.GetType() != typeof(LoadCase))
            {
                return false;
            }
            if (this.Label != ((LoadCase)obj).Label)
            {
                return false;
            }
            if (this.LoadCaseType != ((LoadCase)obj).LoadCaseType)
            {
                return false;
            }
            return true;
        }

        /// <inheritdoc/>
        public static bool operator ==(LoadCase lc1, LoadCase lc2)
        {
            if (ReferenceEquals(lc1, null))
            {
                return false;
            }
            return lc1.Equals(lc2);
        }

        /// <inheritdoc/>
        public static bool operator !=(LoadCase lc1, LoadCase lc2)
        {
            if (ReferenceEquals(lc1, null))
            {
                return false;
            }
            return !lc1.Equals(lc2);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int result = 0;
            result += Label.GetHashCode();
            result += LoadCaseType.GetHashCode();
            return result;
        }

    }

    /// <summary>
    /// Represents load case type.
    /// </summary>
    public enum LoadCaseType
    {
        SelfWeight = 0,
        Dead,
        Live,
        Wind,
        Seismic,
        Accidental,
        Shrinkage,
    }

    /// <summary>
    /// Represents Load Duration Classes.
    /// </summary>
    public enum LoadCaseDuration
    {
        Permanent,
        LongTerm,
        MediumTerm,
        ShortTerm,
        Instantaneous
    }
}
