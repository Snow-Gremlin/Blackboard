using Blackboard.Core;
using Blackboard.Core.Extensions;
using BlackboardTests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackboardTests.ParserTests {

    [TestClass]
    public class Functions {

        [TestMethod]
        public void CheckAllFunctionsAreTested() =>
            TestTools.SetEntriesMatch(
                TestTools.FuncDefTags(new Slate().Global).WhereNot(tag => tag.StartsWith(Slate.OperatorNamespace)),
                TestTools.TestTags(typeof(Functions)),
                "Tests do not match the existing function");


    }
}
