using AspectInjector.Broker;
using rest1.Aspects;

namespace rest1.Attibutes
{
    [Injection(typeof(TransactionAspect))]
    [AttributeUsage(AttributeTargets.Method)]
    public class TransactionAttribute : Attribute
    {
    }
}
