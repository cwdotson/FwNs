namespace FwNs.Linq.Xprsns
{
    using System;

    internal interface IOperator : IOperatorFactory
    {
        OperatorType OpType { get; }

        bool IsOptimized { get; set; }

        bool OrderIsRemoved { get; set; }

        Type ItemType { get; }
    }
}

