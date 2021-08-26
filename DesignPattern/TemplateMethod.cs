namespace Serpen.Uni.DesignPattern
{
    abstract class TemplateMethodClass {
        public void TemplateMethod() {
            firstStep();
            secondStep();
            thirdStep();
        }

        public abstract void firstStep();
        public abstract void secondStep();
        public abstract void thirdStep();

    }
}