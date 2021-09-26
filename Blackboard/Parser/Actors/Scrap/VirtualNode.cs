using Blackboard.Core;
using Blackboard.Core.Nodes.Interfaces;
using PetiteParser.Scanner;

namespace Blackboard.Parser.Actors {
    internal class VirtualNode: IActor {

        public readonly string Name;

        public readonly Type Type;

        public readonly VirtualNodeSet VirtualNodes;

        public VirtualNode(Location location, VirtualNodeSet virtualNodes, string name, Type type) {
            this.Location = location;
            this.Name = name;
            this.Type = type;
            this.VirtualNodes = virtualNodes;
        }

        public Location Location { get; private set; }

        public Type Returns() => this.Type;

        public IActor Prepare(VirtualNodeSet virtualNodes, Options option) => this;

        public INode Perform() {
            throw new System.NotImplementedException();
        }
    }
}
