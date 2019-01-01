namespace SharperCryptoApiAnalysis.Interop.CodeAnalysis.Scoring
{
    /// <summary>
    /// NVD Score
    /// </summary>
    public struct CVSSScore
    {
        /// <summary>
        /// The value.
        /// </summary>
        public double Value { get; }

        /// <summary>
        /// The severity name.
        /// </summary>
        public string Severity { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CVSSScore"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="severity">The severity.</param>
        public CVSSScore(double value, string severity)
        {
            Value = value;
            Severity = severity;
        }

        public override string ToString()
        {
            return $"{Value} {Severity}";
        }
    }
}