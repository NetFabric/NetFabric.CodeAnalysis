namespace NetFabric.CodeAnalysis.TestData
{
    public class PropertiesAndMethods 
        : PropertiesAndMethodsBase
        , IPropertiesAndMethods
    {
        public int Property => 0;
        int IPropertiesAndMethods.ExplicitProperty => 0;

        public void Method() {}
        public void Method(int param0, string param1) {}

        void IPropertiesAndMethods.ExplicitMethod() {}
        void IPropertiesAndMethods.ExplicitMethod(int param0, string param1) {}

        public static void StaticMethod() { }
        public static void StaticMethod(int param0, string param1) { }
    }

    public abstract class PropertiesAndMethodsBase
    {
        public int InheritedProperty => 0;

        public void InheritedMethod() {}
        public void InheritedMethod(int param0, string param1) {}
    }

    public interface IPropertiesAndMethods
    {
        int ExplicitProperty { get; }

        void ExplicitMethod();
        void ExplicitMethod(int param0, string param1);
    }
}