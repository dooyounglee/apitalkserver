using AspectInjector.Broker;
using rest1.Aspects;

namespace rest1.Attibutes
{
    /*
     * method위에 [Log]를 붙이면 method 시작과 끝에 log찍힘
     * 왜 "Log"냐면 Attribute가 "Log"Attribute라서.
     * LogAttrbiute일 경우 [LogAttrbiute]로 써야함.
     */
    [Injection(typeof(LogAspect))]
    [AttributeUsage(AttributeTargets.Method)]
    public class LogAttribute : Attribute
    {
    }
}
