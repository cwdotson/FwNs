namespace FwNs.Linq.Xprsns
{
    using System;

    internal enum OperatorType
    {
        Relation,
        Selection,
        Projection,
        Join,
        CartesianProduct,
        Expansion,
        Ordering,
        Grouping,
        GroupJoin,
        Aggregation,
        Other,
        Union,
        Concat
    }
}

