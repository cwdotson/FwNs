namespace FwNs.Core
{
    using System;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

    public delegate Expression<Func<TA, TB, TC, TR>> XprsnFunk<TA, TB, TC, TR>();
}

