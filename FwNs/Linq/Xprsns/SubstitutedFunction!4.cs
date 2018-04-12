namespace FwNs.Linq.Xprsns
{
    using System;

    internal class SubstitutedFunction<T1, TResult1, T2, TResult2> : Function<T2, TResult1> where TResult2: T1
    {
        private readonly Function<T1, TResult1> _func1;
        private readonly Function<T2, TResult2> _func2;

        public SubstitutedFunction(Function<T1, TResult1> func1, Function<T2, TResult2> func2) : base(Lambda.Substitute<T1, TResult1, T2, TResult2>(func1.Expression, func2.Expression))
        {
            this._func1 = func1;
            this._func2 = func2;
        }

        protected override Func<T2, TResult1> Compile()
        {
            <>c__DisplayClass3_0<T1, TResult1, T2, TResult2> class_1;
            Func<T1, TResult1> func1 = this._func1.AsDelegate();
            Func<T2, TResult2> func2 = this._func2.AsDelegate();
            return new Func<T2, TResult1>(class_1, this.<Compile>b__0);
        }
    }
}

