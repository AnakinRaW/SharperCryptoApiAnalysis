using System;

namespace SharperCryptoApiAnalysis.Interop.CodeAnalysis.Scoring
{
    /// <summary>
    /// CVSS V2
    /// </summary>
    /// <seealso cref="SharperCryptoApiAnalysis.Interop.CodeAnalysis.Scoring.CVSS" />
    public class NVDCVSSV2 : CVSS
    {
        public override CVSSVersion Version => CVSSVersion.V2;

        public NVDCVSSV2(double baseScore, double impactScore, double exploitabilityScore) : base(baseScore, impactScore, exploitabilityScore)
        {
        }

        protected override CVSSScore ValueToScore(double value)
        {
            if (value < 4)
                return new CVSSScore(value, "Low");
            if (value < 7)
                return new CVSSScore(value, "Medium");
            if (value <= 10)
                return new CVSSScore(value, "High");
            throw new ArgumentOutOfRangeException();
        }
    }
}