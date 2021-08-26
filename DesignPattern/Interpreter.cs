using System.Collections.Generic;

namespace Serpen.Uni.DesignPattern {
    interface IEntry {
        string Resolve();
    }

    class Terminal : IEntry {
        private readonly char Character;

        public Terminal(char c) {
            this.Character = c;
        }

        public string Resolve() => Character.ToString();


    }

    class Variable : IEntry {
        List<IEntry> List;

        Variable(params IEntry[] entries) {
            List = new List<IEntry>(entries);
        }

        public string Resolve() {
            string concat = "";
            foreach (var entry in List)
                concat += entry.Resolve();

            return concat;
        }

        public static readonly Variable variable = new Variable(
            new Variable(new Terminal('H'), new Terminal('a'), new Variable(new Terminal('l'), new Terminal('l')), new Terminal('o')),
            new Terminal(' '),
            new Variable(new Terminal('W'), new Terminal('e'), new Terminal('l'), new Terminal('t')));
    }
}