using Blackboard.Core;
using Blackboard.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace BlackboardTests {

    /// <summary>A collection of testing tools.</summary>
    static public class TestTools {

        /// <summary>Check an exception from the given action.</summary>
        /// <param name="hndl">The action to perform.</param>
        /// <param name="exp">The lines of the expected exception message.</param>
        static public void CheckException(S.Action hndl, params string[] exp) {
            S.Exception ex = Assert.ThrowsException<Exception>(hndl);
            List<string> messages = new();
            while (ex != null) {
                messages.Add(ex.Message);
                ex = ex.InnerException;
            }
            Assert.AreEqual(exp.Join("\n"), messages.Join("\n"));
        }
    }
}
