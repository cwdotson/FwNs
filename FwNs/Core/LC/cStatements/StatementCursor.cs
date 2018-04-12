namespace FwNs.Core.LC.cStatements
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cResults;
    using System;

    public class StatementCursor : StatementQuery
    {
        public static StatementCursor[] EmptyArray = new StatementCursor[0];

        public StatementCursor(Session session, QueryExpression query) : base(session, query)
        {
        }

        public override Result GetResult(Session session)
        {
            object[] routineArguments = session.sessionContext.RoutineArguments;
            Result chainedResult = (Result) routineArguments[routineArguments.Length - 1];
            Result chainedResult = chainedResult;
            while (chainedResult != null)
            {
                if (base.GetCursorName().Name.Equals(chainedResult.GetMainString()))
                {
                    chainedResult.navigator.Close();
                    if (chainedResult == chainedResult)
                    {
                        chainedResult = chainedResult.GetChainedResult();
                    }
                }
                if (chainedResult.GetChainedResult() == null)
                {
                    break;
                }
                chainedResult = chainedResult.GetChainedResult();
            }
            routineArguments[routineArguments.Length - 1] = chainedResult;
            Result result3 = base.queryExpression.GetResult(session, 0);
            result3.SetStatement(this);
            if (result3.IsError())
            {
                return result3;
            }
            result3.SetMainString(base.GetCursorName().Name);
            if (chainedResult == null)
            {
                routineArguments[routineArguments.Length - 1] = result3;
            }
            else
            {
                ((Result) routineArguments[routineArguments.Length - 1]).AddChainedResult(result3);
            }
            return Result.UpdateZeroResult;
        }
    }
}

