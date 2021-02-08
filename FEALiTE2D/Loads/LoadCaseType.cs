namespace FEALiTE2D.Loads
{
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
}
