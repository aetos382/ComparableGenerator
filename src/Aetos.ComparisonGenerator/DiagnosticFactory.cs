using System.Collections.Generic;

using Microsoft.CodeAnalysis;

namespace Aetos.ComparisonGenerator
{
    internal static class DiagnosticFactory
    {
        public static Diagnostic TypeIsNotPartial(
            string typeName,
            Location location,
            IEnumerable<Location>? additionalLocations = null)
        {
            return Diagnostic.Create(
                DiagnosticDescriptors.TypeIsNotPartial,
                location,
                additionalLocations,
                typeName);
        }

        public static Diagnostic TypeIsStatic(
            string typeName,
            Location location,
            IEnumerable<Location>? additionalLocations = null)
        {
            return Diagnostic.Create(
                DiagnosticDescriptors.TypeIsStatic,
                location,
                additionalLocations,
                typeName);
        }

        public static Diagnostic NoMembers(
            string typeName,
            Location location,
            IEnumerable<Location>? additionalLocations = null)
        {
            return Diagnostic.Create(
                DiagnosticDescriptors.NoMembers,
                location,
                additionalLocations,
                typeName);
        }

        public static Diagnostic MemberIsNotComparable(
            string typeName,
            string memberName,
            string memberTypeName,
            Location location,
            IEnumerable<Location>? additionalLocations = null)
        {
            return Diagnostic.Create(
                DiagnosticDescriptors.MemberTypeIsNotComparable,
                location,
                additionalLocations,
                typeName,
                memberName,
                memberTypeName);
        }
    }
}
