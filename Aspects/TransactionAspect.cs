using AspectInjector.Broker;
using OTILib.Util;
using rest1.Controllers;
using rest1.Attibutes;
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
            var attr = methodInfo.GetCustomAttribute<TransactionAttribute>();
            var propagation = attr?.Propagation ?? TransactionPropagation.Required;
            var requiresNew = propagation == TransactionPropagation.RequiresNew;

            var isRootTransaction = DbTransactionManager.Current == null || requiresNew;

            try
            {
                if (isRootTransaction)
                    OtiLogger.log1($"🔄 Begin Transaction - {methodInfo.Name}");
                else
                    OtiLogger.log1($"▶ Reusing Transaction - {methodInfo.Name}");

                DbTransactionManager.Begin(requiresNew);

                var result = method(args);

                DbTransactionManager.Commit();

                if (isRootTransaction)
                    OtiLogger.log1($"✅ Commit Transaction - {methodInfo.Name}");

                return result;
            }
            catch (Exception ex)
            {
                DbTransactionManager.Rollback();
                OtiLogger.log1($"❌ Rollback Transaction - {methodInfo.Name} : {ex.Message}");
                throw;
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
