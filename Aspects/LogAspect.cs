using AspectInjector.Broker;
using OTILib.Util;

namespace rest1.Aspects
{
    [Aspect(Scope.Global)]
    public class LogAspect
    {
        [Advice(Kind.Before, Targets = Target.Method)]
        public void LogEnter([Argument(Source.Name)] string methodName)
        {
            Console.WriteLine($"--> Entering method: {methodName}");
            OtiLogger.log1($"--> Entering method: {methodName}");
        }

        [Advice(Kind.After, Targets = Target.Method)]
        public void LogExit([Argument(Source.Name)] string methodName)
        {
            Console.WriteLine($"<-- Exiting method: {methodName}");
            OtiLogger.log1($"<-- Exiting method: {methodName}");
        }
    }
}
