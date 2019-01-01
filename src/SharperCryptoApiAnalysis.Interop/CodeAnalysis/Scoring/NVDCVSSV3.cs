using System;

namespace SharperCryptoApiAnalysis.Interop.CodeAnalysis.Scoring
{
    /// <summary>
    /// CVSS 3
    /// </summary>
    /// <seealso cref="SharperCryptoApiAnalysis.Interop.CodeAnalysis.Scoring.CVSS" />
    public class NVDCVSSV3 : CVSS
    {
        public override CVSSVersion Version => CVSSVersion.V3;

        public NVDCVSSV3(double baseScore, double impactScore, double exploitabilityScore) : base(baseScore, impactScore, exploitabilityScore)
        {
        }

        protected override CVSSScore ValueToScore(double value)
        {
            if (value == 0)
                return new CVSSScore(value, "None");
            if (value < 4)
                return new CVSSScore(value, "Low");
            if (value < 7)
                return new CVSSScore(value, "Medium");
            if (value < 9)
                return new CVSSScore(value, "High");
            if (value <= 10)
                return new CVSSScore(value, "Critical");
            throw new ArgumentOutOfRangeException();
        }
    }
}