namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Linq.Expressions;

    internal interface IKeyComponent
    {
        IKeyComponent Clone(LambdaExpression keySelector, FwNs.Linq.Xprsns.Order order);
        object ToOrderKey();

        LambdaExpression KeySelector { get; }

        FwNs.Linq.Xprsns.Order Order { get; }
    }
}

