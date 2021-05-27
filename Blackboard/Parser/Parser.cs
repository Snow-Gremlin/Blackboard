using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blackboard.Core;

namespace Blackboard.Parser {
    public class Parser {

        private Driver driver;

        public Parser(Driver driver) {
            this.driver = driver;
        }

        public void Read(params string[] input) =>
            this.Read(input as IEnumerable<string>);

        public void Read(IEnumerable<string> input, string separator = "\n") {
        }



    }
}
