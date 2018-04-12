namespace FwNs.Core
{
    using System;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

    public delegate Expression<Func<TA, TB, TR>> XprsnFunk<TA, TB, TR>();
}

