using AspectInjector.Broker;
using OTILib.Util;
using rest1.Controllers;
using System.Reflection;
using System.Transactions;

namespace rest1.Aspects
{
    [Aspect(Scope.Global)]
    public class TransactionAspect
    {
        [Advice(Kind.Around, Targets = Target.Method)]
        public object HandleTransaction(
            [Argument(Source.Target)] Func<object[], object> method,
            [Argument(Source.Arguments)] object[] args,
            [Argument(Source.Method)] MethodBase methodInfo)
        {
            try
            {
                Console.WriteLine($"🔄 Begin Transaction - {methodInfo.Name}");
                OtiLogger.log1($"🔄 Begin Transaction - {methodInfo.Name}");
                DbTransactionManager.Begin();

                var result = method(args);

                Console.WriteLine($"✅ Commit Transaction - {methodInfo.Name}");
                OtiLogger.log1($"✅ Commit Transaction - {methodInfo.Name}");
                DbTransactionManager.Commit();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Rollback Transaction - {methodInfo.Name} : {ex.Message}");
                OtiLogger.log1($"❌ Rollback Transaction - {methodInfo.Name} : {ex.Message}");
                DbTransactionManager.Rollback();
                throw; // 예외 다시 던지기
            }
        }
        // [Advice(Kind.Before, Targets = Target.Method)]
        // public void OnEnter()
        // {
        //     Console.WriteLine("🔄 Begin Transaction");
        //     DbTransactionManager.Begin();
        // }
        // 
        // [Advice(Kind.After, Targets = Target.Method)]
        // public void OnSuccess()
        // {
        //     Console.WriteLine("✅ Commit Transaction");
        //     DbTransactionManager.Commit();
        // }
        // 
        // [Advice(Kind.AfterThrowing, Targets = Target.Method)]
        // public void OnError()
        // {
        //     Console.WriteLine("❌ Rollback Transaction");
        //     DbTransactionManager.Rollback();
        // }
    }
}
