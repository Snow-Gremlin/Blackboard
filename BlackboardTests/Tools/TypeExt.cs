// Ignore Spelling: Implicits

using Blackboard.Core.Extensions;
using Blackboard.Core.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace BlackboardTests.Tools {

    /// <summary>These are extensions to Blackboard Types for testing.</summary>
    static class TypeExt {

        /// <summary>Checks that the expected inheritors are correct for the given type.</summary>
        /// <param name="t">The type to check the inheritors of.</param>
        /// <param name="exp">The names of the inheritors for the given type.</param>
        /// <returns>The given type so that method calls can be chained.</returns>
        public static Type CheckInheritors(this Type t, string exp) {
            Assert.AreEqual(exp, t.Inheritors.Join(", "), "{0} inherits these types.", t);
            return t;
        }

        /// <summary>Checks that the expected implements are correct for the given type.</summary>
        /// <param name="t">The type to check the implements of.</param>
        /// <param name="exp">The names of the implements for the given type.</param>
        /// <returns>The given type so that method calls can be chained.</returns>
        public static Type CheckImplicits(this Type t, string exp) {
            Assert.AreEqual(exp, t.Implicits.Join(", "), "{0} can implicitly be cast to these types.", t);
            return t;
        }

        /// <summary>Checks that the expected explicits are correct for the given type.</summary>
        /// <param name="t">The type to check the explicits of.</param>
        /// <param name="exp">The names of the explicits for the given type.</param>
        /// <returns>The given type so that method calls can be chained.</returns>
        public static Type CheckExplicits(this Type t, string exp) {
            Assert.AreEqual(exp, t.Explicits.Join(", "), "{0} can explicitly be cast to these types.", t);
            return t;
        }
    }
}
