namespace Serpen.Uni.DesignPattern
{
    interface IDecoratable
    {
        string Method();
    }
    
    class Decorator : IDecoratable {
        readonly IDecoratable baseobject;

        public Decorator(IDecoratable baseobject) {
            this.baseobject = baseobject;
        }

        public string Method() =>
            baseobject.Method() + " meinen Senf dazu";
    }
}