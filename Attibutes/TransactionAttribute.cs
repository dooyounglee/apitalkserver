using AspectInjector.Broker;
using rest1.Aspects;

namespace rest1.Attibutes
{
    [Injection(typeof(TransactionAspect))]
    [AttributeUsage(AttributeTargets.Method)]
    public class TransactionAttribute : Attribute
    {
        public TransactionPropagation Propagation { get; set; } = TransactionPropagation.Required;
    }

    public enum TransactionPropagation
    {
        Required,    // 기본: 트랜잭션 있으면 사용, 없으면 새로 시작
        RequiresNew  // 무조건 새 트랜잭션
    }
}
