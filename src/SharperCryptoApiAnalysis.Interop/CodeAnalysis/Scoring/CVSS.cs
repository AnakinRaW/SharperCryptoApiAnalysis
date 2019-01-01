using System;

namespace SharperCryptoApiAnalysis.Interop.CodeAnalysis.Scoring
{
    /// <summary>
    /// CV Scoring System Base
    /// </summary>
    public abstract class CVSS
    {
        protected readonly double _baseScore;
    
        public CVSSScore BaseScore { get; }
        public double ImpactScore { get; }
        public double ExploitabilityScore { get; }

        public abstract CVSSVersion Version { get; }


        protected CVSS(double baseScore, double impactScore, double exploitabilityScore)
        {
            if (baseScore < 0)
                throw new ArgumentOutOfRangeException();
            if (impactScore < 0)
                throw new ArgumentOutOfRangeException();
            if (exploitabilityScore < 0)
                throw new ArgumentOutOfRangeException();

            _baseScore = baseScore;
            ImpactScore = impactScore;
            ExploitabilityScore = exploitabilityScore;
        }

        protected abstract CVSSScore ValueToScore(double value);

        public override string ToString()
        {
            return $"CVSS Version: {Version} - {BaseScore}; Impact Score:{ImpactScore}; Exploitability Score:{ExploitabilityScore}";
        }

        /// <summary>
        /// CVSS Version
        /// </summary>
        public enum CVSSVersion
        {
            V2,
            V3
        }
    }
}
