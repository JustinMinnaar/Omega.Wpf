using Jem.CommonLibrary22;

namespace Jem.ProfilingLibrary23
{
    public class JProIdentifierResult
    {
        public JProIdentifierResult()
        {
        }

        public JProIdentifierResult(JProIdentifier jIdentifier, double score, CTheory? theory)
        {
            JIdentifier = jIdentifier;
            Score = score;
            Theory = theory;
        }

        public override string ToString() => $"Identifier:'{JIdentifier}' Score:'{Score}' Theory:'{Theory}'";

        public JProIdentifier? JIdentifier { get; set; }
        public CTheory? Theory { get; set; }
        public double Score { get; set; }
    }
}