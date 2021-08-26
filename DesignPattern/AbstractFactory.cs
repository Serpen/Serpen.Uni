namespace Serpen.Uni.DesignPattern {
    interface IFactory {
        IProduct GetProduct();
    }
    
    class AbstractFactory : IFactory {
        private readonly IFactory factory;

        AbstractFactory(IFactory factory) {
            this.factory = factory;
        }

        public IProduct GetProduct() => factory.GetProduct();
    }

    class StringFactory : IFactory {
        public IProduct GetProduct() => new StringProduct();
    }

    class IntFactory : IFactory {
        public IProduct GetProduct() => new IntProduct();
    }

    interface IProduct {
        string Method();
    }

    public class StringProduct : IProduct {
        public string Method() => "eins";
    }

    public class IntProduct : IProduct {
        public string Method() => 1.ToString();
    }
}