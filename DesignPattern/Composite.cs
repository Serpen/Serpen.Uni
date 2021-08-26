namespace Serpen.Uni.DesignPattern {
    interface IComposite {
        string Method();
    }

    class Component : IComposite {
        public string Method() => "single string";
    }

    class Composite : IComposite {

        readonly System.Collections.Generic.List<Component> items = new();

        public void Add(Component component) => items.Add(component);
        public string Method() {
            string concat = "";
            foreach (Component comp in items)
                concat += comp.Method();

            return concat;
        }
    }

}