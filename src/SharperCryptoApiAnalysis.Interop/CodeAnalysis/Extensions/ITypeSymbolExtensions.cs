// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See Credits.txt in the project root for license information.
// Original file: roslyn/src/Workspaces/Core/Portable/Shared/Extensions/ITypeSymbolExtensions.cs
// Modifications made: Removed all methods but IEnumerable<ITypeSymbol> GetBaseTypesAndThis(this ITypeSymbol type) from original source

using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace SharperCryptoApiAnalysis.Interop.CodeAnalysis.Extensions
{
    public static class RoslynCodeAnalysisExtensions
    {
        //Copied from Roslyn source because MS thought it would be a great idea to make those extension internal only.
        /// <summary>
        /// Get all base types and itself.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A list of all types</returns>
        public static IEnumerable<ITypeSymbol> GetBaseTypesAndThis(this ITypeSymbol type)
        {
            var current = type;
            while (current != null)
            {
                yield return current;
                current = current.BaseType;
            }
        }

        /// <summary>
        /// Determines whether the symbol is derived from the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>
        ///   <see langword="true"/> if symbol is derived from type; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsDerivedFromType(this ITypeSymbol type, string typeName)
        {
            var types = type.GetBaseTypesAndThis();

            foreach (var typeSymbol in types)
            {
                if (typeSymbol.Name.Equals(typeName))
                    return true;
                var ns = BuildNamespace(typeSymbol.ContainingNamespace).TrimStart('.');
                var fullName = $"{ns}.{typeSymbol.Name}";
                if (fullName.Equals(typeName))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Builds the namespace.
        /// </summary>
        /// <param name="namespaceSymbol">The namespace symbol.</param>
        /// <returns>The full namespace name</returns>
        public static string BuildNamespace(INamespaceSymbol namespaceSymbol)
        {
            if (namespaceSymbol == null ||  namespaceSymbol.IsGlobalNamespace)
                return string.Empty;
            var s = $"{BuildNamespace(namespaceSymbol.ContainingNamespace)}.{namespaceSymbol.Name}";
            return s;
        }
    }
}
