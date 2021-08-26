namespace Serpen.Uni.DesignPattern
{
    class Singleton {
        private Singleton() {}

        static readonly System.Lazy<Singleton> instance = new System.Lazy<Singleton>();

        public static Singleton Instance => instance.Value;

        public string Method() => "Hello Singleton";
    }
}