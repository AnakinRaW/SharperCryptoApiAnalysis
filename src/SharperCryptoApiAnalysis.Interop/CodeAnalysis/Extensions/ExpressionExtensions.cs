using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Linq;

namespace SharperCryptoApiAnalysis.Interop.CodeAnalysis.Extensions
{
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Determines whether expression is derived from type.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="context">The context.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <see langword="true"/> if expression is derived from type; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsDerivedFromType(this ExpressionSyntax expression, SyntaxNodeAnalysisContext context, Type type)
        {
            if (expression == null)
                return false;

            var expressionTypeInfo = context.Compilation.GetSemanticModel(expression.SyntaxTree)?.GetTypeInfo(expression);
            return expressionTypeInfo != null && expressionTypeInfo.Value.Type.IsDerivedFromType(type.FullName);
        }

        /// <summary>
        /// Determines whether an expression is of type.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="context">The context.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <see langword="true"/> is of type otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsOfType(this ExpressionSyntax expression, SyntaxNodeAnalysisContext context, Type type)
        {
            if (expression == null)
                return false;

            var expressionTypeInfo = context.SemanticModel.GetTypeInfo(expression);

            if (expressionTypeInfo.Type.Name == type.Name)
                return true;

            return false;
        }

        /// <summary>
        /// Determines whether an expression has an initializer syntax.
        /// </summary>
        /// <param name="syntax">The syntax.</param>
        /// <returns>
        ///   <see langword="true"/> if the specified expression has an initializer; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool HasInitializer(this ExpressionSyntax syntax)
        {
            var i = syntax?.DescendantNodes().OfType<InitializerExpressionSyntax>().FirstOrDefault();
            return i != null;
        }

        /// <summary>
        /// Determines whether an invocation has at least one expression that is evaluated by a given function.
        /// </summary>
        /// <param name="expressionSyntax">The expression syntax.</param>
        /// <param name="context">The context.</param>
        /// <param name="returnEvaluation">The return evaluation.</param>
        /// <returns>
        ///   <see langword="true"/> if the evaluation was successful; otherwise, <see langword="false"/>
        /// </returns>
        public static bool HasInvocationWithAtLeastOneReturnExpression(this ExpressionSyntax expressionSyntax,
            SyntaxNodeAnalysisContext context,
            Func<SyntaxNodeAnalysisContext, ExpressionSyntax, bool> returnEvaluation)
        {
            if (expressionSyntax is InvocationExpressionSyntax invocationExpression)
            {
                var symbolInfo = context.SemanticModel.GetSymbolInfo(invocationExpression.Expression);
                var symbol = symbolInfo.Symbol;
                if (symbol == null)
                    return false;

                var declaringSyntaxReference = symbol.DeclaringSyntaxReferences.FirstOrDefault();
                var x = declaringSyntaxReference.GetSyntax();
                var returns = x.DescendantNodes().OfType<ReturnStatementSyntax>().ToList();
                foreach (var returnStatementSyntax in returns)
                {
                    if (returnEvaluation(context, returnStatementSyntax.Expression))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether an invocation has at least one compile time return value.
        /// </summary>
        /// <param name="expressionSyntax">The expression syntax.</param>
        /// <param name="context">The context.</param>
        /// <param name="evaluation">The evaluation.</param>
        /// <returns>
        ///   <see langword="true"/> if the invocation has at least one compile time return value; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool HasInvocationWithAtLeastOneCompileTimeConstantReturnExpression(this ExpressionSyntax expressionSyntax,
            SyntaxNodeAnalysisContext context,
            Func<SyntaxNodeAnalysisContext, ExpressionSyntax, bool> evaluation)
        {
            if (expressionSyntax is InvocationExpressionSyntax invocationExpression)
            {
                var symbolInfo = context.Compilation.GetSemanticModel(invocationExpression.SyntaxTree)?.GetSymbolInfo(invocationExpression.Expression);
                if (symbolInfo != null)
                {
                    var symbol = symbolInfo.Value.Symbol;
                    if (symbol == null)
                        return false;

                    var declaringSyntaxReference = symbol.DeclaringSyntaxReferences.FirstOrDefault();
                    var x = declaringSyntaxReference?.GetSyntax();
                    var returns = x?.DescendantNodes().OfType<ReturnStatementSyntax>().ToList();
                    if (returns == null)
                        return false;
                    foreach (var returnStatementSyntax in returns)
                    {
                        if (returnStatementSyntax.Expression.IsOrContainsCompileTimeConstantExpression(context, evaluation))
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether the expression is or contains on compile time value.
        /// </summary>
        /// <param name="parentExpressionSyntax">The parent expression syntax.</param>
        /// <param name="context">The context.</param>
        /// <param name="evaluation">The evaluation.</param>
        /// <returns>
        ///   <see langword="true"/> if the expression is or contains on compile time value; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsOrContainsCompileTimeConstantExpression(this ExpressionSyntax parentExpressionSyntax, SyntaxNodeAnalysisContext context,
            Func<SyntaxNodeAnalysisContext, ExpressionSyntax, bool> evaluation)
        {
            if (evaluation(context, parentExpressionSyntax))
                return true;

            var model = context.Compilation.GetSemanticModel(parentExpressionSyntax.SyntaxTree);
            var symbolInfo = model.GetSymbolInfo(parentExpressionSyntax);
            var symbol = symbolInfo.Symbol;
            if (symbolInfo.Symbol is IFieldSymbol field && field.IsConst)
            {
                return true;
            }

            if (parentExpressionSyntax is IdentifierNameSyntax && symbol != null)
            {
                var declaringSyntaxReference = symbol.DeclaringSyntaxReferences.FirstOrDefault();
                var declaration = declaringSyntaxReference?.GetSyntax();
                if (declaration is VariableDeclaratorSyntax vds)
                {
                    return evaluation(context, vds.Initializer.Value) ||
                           vds.Initializer.Value.IsOrContainsCompileTimeConstantExpression(context, evaluation);
                }
            }

            if (parentExpressionSyntax is MemberAccessExpressionSyntax memberAccessException)
                return IsOrContainsCompileTimeConstantExpression(memberAccessException.Name, context, evaluation);

            if (parentExpressionSyntax is InvocationExpressionSyntax invocation)
                return invocation.HasInvocationWithAtLeastOneCompileTimeConstantReturnExpression(context,
                    evaluation);
            return false;
        }

        /// <summary>
        /// Determines whether the expression is or contains on compile time return value.
        /// </summary>
        /// <typeparam name="T">The type of the constant value</typeparam>
        /// <param name="parentExpressionSyntax">The parent expression syntax.</param>
        /// <param name="context">The context.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <see langword="true"/> if the expression is or contains on compile time value; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsOrContainsCompileTimeConstantValue<T>(this ExpressionSyntax parentExpressionSyntax,
            SyntaxNodeAnalysisContext context, out T value)
        {
            value = default(T);

            var symbolInfo = context.Compilation.GetSemanticModel(parentExpressionSyntax.SyntaxTree)?.GetSymbolInfo(parentExpressionSyntax);
            if (symbolInfo.HasValue && symbolInfo.Value.Symbol is IFieldSymbol field && field.IsConst && TryCast(field.ConstantValue, out value))
                return true;

            if (parentExpressionSyntax is LiteralExpressionSyntax literalExpression)
            {
                var model = context.Compilation.GetSemanticModel(literalExpression.SyntaxTree);
                if (model == null)
                    return false;
                var constant = model.GetConstantValue(literalExpression);
                if (constant.HasValue)
                    return TryCast(constant.Value, out value);
            }

            T tv = default(T);
            if (parentExpressionSyntax.IsOrContainsCompileTimeConstantExpression(context, (analysisContext, syntax) =>
            {
                try
                {
                    tv = default(T);
                    var model = context.Compilation.GetSemanticModel(syntax.SyntaxTree);
                    if (model == null)
                        return false;
                    var constant = model.GetConstantValue(syntax);

                    if (constant.HasValue && TryCast(constant.Value, out tv))
                        return true;
                }
                catch
                {
                    return false;
                }
                return false;
            }))
            {
                value = tv;
                return true;
            }

            return false;
        }

        private static bool TryCast<T>(object input, out T value)
        {
            value = default(T);
            try
            {
                value = (T)input;
                return true;
            }
            catch (InvalidCastException e)
            {
                return false;
            }
        }
    }
}
