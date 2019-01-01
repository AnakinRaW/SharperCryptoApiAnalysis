using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharperCryptoApiAnalysis.Interop.CodeAnalysis.Extensions
{
    public static class BlockSyntaxExtensions
    {
        /// <summary>
        /// Checks whether a block has an assignment of a syntax token with a given name.
        /// </summary>
        /// <param name="blockSyntax">The block syntax.</param>
        /// <param name="checkIdentifierName">The syntax token.</param>
        /// <param name="memberName">Name of the assignment.</param>
        /// <returns><see langword="true"/> if assignment was found; <see langword="false"/> otherwise</returns>
        public static bool AssignsMemberWithName(this BlockSyntax blockSyntax, SyntaxToken checkIdentifierName, string memberName)
        {
            return AssignsMemberWithName(blockSyntax, checkIdentifierName, memberName, out _);
        }

        /// <summary>
        /// Checks whether a block has an assignment of a syntax token with a given name.
        /// </summary>
        /// <param name="blockSyntax">The block syntax.</param>
        /// <param name="checkIdentifierName">The syntax token.</param>
        /// <param name="memberName">Name of the assignment.</param>
        /// <param name="assignmentExpression">The assignment expression.</param>
        /// <returns></returns>
        public static bool AssignsMemberWithName(this BlockSyntax blockSyntax, SyntaxToken checkIdentifierName,
            string memberName, out AssignmentExpressionSyntax assignmentExpression)
        {
            assignmentExpression = default(AssignmentExpressionSyntax);

            var s = blockSyntax?.DescendantNodes().OfType<AssignmentExpressionSyntax>().ToList();
            foreach (var assignment in s)
            {
                if (assignment.Left is MemberAccessExpressionSyntax memberAccess)
                {
                    if (memberAccess.Expression is IdentifierNameSyntax identifierName)
                    {
                        if (identifierName.Identifier.Value == checkIdentifierName.Value)
                        {
                            if (memberAccess.Name.Identifier.Value.Equals(memberName))
                            {
                                assignmentExpression = assignment;
                                return true;
                            }
                        }
                    }
                }
            }

            return false;

        }
    }
}
