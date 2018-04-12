namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    internal static class RBTreeUtils<TKey>
    {
        [IteratorStateMachine(typeof(<Nodes>d__1))]
        public static IEnumerable<RBNodeBase<TKey>> Nodes(ITree<TKey> source)
        {
            if (source.Root == null)
            {
                yield break;
            }
            if ((source.Root.Left != null) || (source.Root.Right != null))
            {
                Stack<RBNodeBase<TKey>> <stack>5__1 = new Stack<RBNodeBase<TKey>>();
                <stack>5__1.Push(source.Root);
                while (<stack>5__1.Count > 0)
                {
                    RBNodeBase<TKey> <rBNodeBase>5__2 = <stack>5__1.Pop();
                    yield return <rBNodeBase>5__2;
                    if (<rBNodeBase>5__2.Right != null)
                    {
                        <stack>5__1.Push(<rBNodeBase>5__2.Right);
                    }
                    if (<rBNodeBase>5__2.Left != null)
                    {
                        <stack>5__1.Push(<rBNodeBase>5__2.Left);
                    }
                    <rBNodeBase>5__2 = null;
                }
                <stack>5__1 = null;
                yield break;
            }
            yield return source.Root;
        }

        [IteratorStateMachine(typeof(<NodesAscending>d__2))]
        public static IEnumerable<RBNodeBase<TKey>> NodesAscending(ITree<TKey> source)
        {
            if (source.Root == null)
            {
                yield break;
            }
            if ((source.Root.Left != null) || (source.Root.Right != null))
            {
                Stack<RBNodeBase<TKey>> <stack>5__1 = new Stack<RBNodeBase<TKey>>();
                for (RBNodeBase<TKey> base2 = source.Root; base2 != null; base2 = base2.Left)
                {
                    <stack>5__1.Push(base2);
                }
                while (<stack>5__1.Count > 0)
                {
                    RBNodeBase<TKey> item = <stack>5__1.Pop();
                    yield return item;
                    item = item.Right;
                    while (item != null)
                    {
                        <stack>5__1.Push(item);
                        item = item.Left;
                    }
                    item = null;
                }
                <stack>5__1 = null;
                yield break;
            }
            yield return source.Root;
        }

        [IteratorStateMachine(typeof(<NodesDescending>d__3))]
        public static IEnumerable<RBNodeBase<TKey>> NodesDescending(ITree<TKey> source)
        {
            if (source.Root == null)
            {
                yield break;
            }
            if ((source.Root.Left != null) || (source.Root.Right != null))
            {
                Stack<RBNodeBase<TKey>> <stack>5__1 = new Stack<RBNodeBase<TKey>>();
                for (RBNodeBase<TKey> base2 = source.Root; base2 != null; base2 = base2.Right)
                {
                    <stack>5__1.Push(base2);
                }
                while (<stack>5__1.Count > 0)
                {
                    RBNodeBase<TKey> item = <stack>5__1.Pop();
                    yield return item;
                    item = item.Left;
                    while (item != null)
                    {
                        <stack>5__1.Push(item);
                        item = item.Right;
                    }
                    item = null;
                }
                <stack>5__1 = null;
                yield break;
            }
            yield return source.Root;
        }

        [CompilerGenerated]
        private sealed class <Nodes>d__1 : IEnumerable<RBNodeBase<TKey>>, IEnumerable, IEnumerator<RBNodeBase<TKey>>, IDisposable, IEnumerator
        {
            private int <>1__state;
            private RBNodeBase<TKey> <>2__current;
            private int <>l__initialThreadId;
            private RBTreeUtils<TKey>.ITree source;
            public RBTreeUtils<TKey>.ITree <>3__source;
            private Stack<RBNodeBase<TKey>> <stack>5__1;
            private RBNodeBase<TKey> <rBNodeBase>5__2;

            [DebuggerHidden]
            public <Nodes>d__1(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Environment.get_CurrentManagedThreadId();
            }

            private bool MoveNext()
            {
                switch (this.<>1__state)
                {
                    case 0:
                        this.<>1__state = -1;
                        if (this.source.Root == null)
                        {
                            break;
                        }
                        if ((this.source.Root.Left != null) || (this.source.Root.Right != null))
                        {
                            this.<stack>5__1 = new Stack<RBNodeBase<TKey>>();
                            this.<stack>5__1.Push(this.source.Root);
                            while (this.<stack>5__1.Count > 0)
                            {
                                this.<rBNodeBase>5__2 = this.<stack>5__1.Pop();
                                this.<>2__current = this.<rBNodeBase>5__2;
                                this.<>1__state = 2;
                                return true;
                            Label_00C5:
                                this.<>1__state = -1;
                                if (this.<rBNodeBase>5__2.Right != null)
                                {
                                    this.<stack>5__1.Push(this.<rBNodeBase>5__2.Right);
                                }
                                if (this.<rBNodeBase>5__2.Left != null)
                                {
                                    this.<stack>5__1.Push(this.<rBNodeBase>5__2.Left);
                                }
                                this.<rBNodeBase>5__2 = null;
                            }
                            this.<stack>5__1 = null;
                            break;
                        }
                        this.<>2__current = this.source.Root;
                        this.<>1__state = 1;
                        return true;

                    case 1:
                        this.<>1__state = -1;
                        break;

                    case 2:
                        goto Label_00C5;

                    default:
                        return false;
                }
                return false;
            }

            [DebuggerHidden]
            IEnumerator<RBNodeBase<TKey>> IEnumerable<RBNodeBase<TKey>>.GetEnumerator()
            {
                RBTreeUtils<TKey>.<Nodes>d__1 d__;
                if ((this.<>1__state == -2) && (this.<>l__initialThreadId == Environment.get_CurrentManagedThreadId()))
                {
                    this.<>1__state = 0;
                    d__ = (RBTreeUtils<TKey>.<Nodes>d__1) this;
                }
                else
                {
                    d__ = new RBTreeUtils<TKey>.<Nodes>d__1(0);
                }
                d__.source = this.<>3__source;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<FwNs.Linq.Xprsns.RBNodeBase<TKey>>.GetEnumerator();
            }

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            void IDisposable.Dispose()
            {
            }

            RBNodeBase<TKey> IEnumerator<RBNodeBase<TKey>>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.<>2__current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.<>2__current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <NodesAscending>d__2 : IEnumerable<RBNodeBase<TKey>>, IEnumerable, IEnumerator<RBNodeBase<TKey>>, IDisposable, IEnumerator
        {
            private int <>1__state;
            private RBNodeBase<TKey> <>2__current;
            private int <>l__initialThreadId;
            private RBTreeUtils<TKey>.ITree source;
            public RBTreeUtils<TKey>.ITree <>3__source;
            private Stack<RBNodeBase<TKey>> <stack>5__1;
            private RBNodeBase<TKey> <rBNodeBase2>5__2;

            [DebuggerHidden]
            public <NodesAscending>d__2(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Environment.get_CurrentManagedThreadId();
            }

            private bool MoveNext()
            {
                switch (this.<>1__state)
                {
                    case 0:
                        this.<>1__state = -1;
                        if (this.source.Root == null)
                        {
                            break;
                        }
                        if ((this.source.Root.Left != null) || (this.source.Root.Right != null))
                        {
                            this.<stack>5__1 = new Stack<RBNodeBase<TKey>>();
                            for (RBNodeBase<TKey> base2 = this.source.Root; base2 != null; base2 = base2.Left)
                            {
                                this.<stack>5__1.Push(base2);
                            }
                            while (this.<stack>5__1.Count > 0)
                            {
                                this.<rBNodeBase2>5__2 = this.<stack>5__1.Pop();
                                this.<>2__current = this.<rBNodeBase2>5__2;
                                this.<>1__state = 2;
                                return true;
                            Label_00ED:
                                this.<stack>5__1.Push(this.<rBNodeBase2>5__2);
                                this.<rBNodeBase2>5__2 = this.<rBNodeBase2>5__2.Left;
                            Label_010F:
                                if (this.<rBNodeBase2>5__2 != null)
                                {
                                    goto Label_00ED;
                                }
                                this.<rBNodeBase2>5__2 = null;
                            }
                            this.<stack>5__1 = null;
                            break;
                        }
                        this.<>2__current = this.source.Root;
                        this.<>1__state = 1;
                        return true;

                    case 1:
                        this.<>1__state = -1;
                        break;

                    case 2:
                        this.<>1__state = -1;
                        this.<rBNodeBase2>5__2 = this.<rBNodeBase2>5__2.Right;
                        goto Label_010F;

                    default:
                        return false;
                }
                return false;
            }

            [DebuggerHidden]
            IEnumerator<RBNodeBase<TKey>> IEnumerable<RBNodeBase<TKey>>.GetEnumerator()
            {
                RBTreeUtils<TKey>.<NodesAscending>d__2 d__;
                if ((this.<>1__state == -2) && (this.<>l__initialThreadId == Environment.get_CurrentManagedThreadId()))
                {
                    this.<>1__state = 0;
                    d__ = (RBTreeUtils<TKey>.<NodesAscending>d__2) this;
                }
                else
                {
                    d__ = new RBTreeUtils<TKey>.<NodesAscending>d__2(0);
                }
                d__.source = this.<>3__source;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<FwNs.Linq.Xprsns.RBNodeBase<TKey>>.GetEnumerator();
            }

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            void IDisposable.Dispose()
            {
            }

            RBNodeBase<TKey> IEnumerator<RBNodeBase<TKey>>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.<>2__current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.<>2__current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <NodesDescending>d__3 : IEnumerable<RBNodeBase<TKey>>, IEnumerable, IEnumerator<RBNodeBase<TKey>>, IDisposable, IEnumerator
        {
            private int <>1__state;
            private RBNodeBase<TKey> <>2__current;
            private int <>l__initialThreadId;
            private RBTreeUtils<TKey>.ITree source;
            public RBTreeUtils<TKey>.ITree <>3__source;
            private Stack<RBNodeBase<TKey>> <stack>5__1;
            private RBNodeBase<TKey> <rBNodeBase2>5__2;

            [DebuggerHidden]
            public <NodesDescending>d__3(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Environment.get_CurrentManagedThreadId();
            }

            private bool MoveNext()
            {
                switch (this.<>1__state)
                {
                    case 0:
                        this.<>1__state = -1;
                        if (this.source.Root == null)
                        {
                            break;
                        }
                        if ((this.source.Root.Left != null) || (this.source.Root.Right != null))
                        {
                            this.<stack>5__1 = new Stack<RBNodeBase<TKey>>();
                            for (RBNodeBase<TKey> base2 = this.source.Root; base2 != null; base2 = base2.Right)
                            {
                                this.<stack>5__1.Push(base2);
                            }
                            while (this.<stack>5__1.Count > 0)
                            {
                                this.<rBNodeBase2>5__2 = this.<stack>5__1.Pop();
                                this.<>2__current = this.<rBNodeBase2>5__2;
                                this.<>1__state = 2;
                                return true;
                            Label_00ED:
                                this.<stack>5__1.Push(this.<rBNodeBase2>5__2);
                                this.<rBNodeBase2>5__2 = this.<rBNodeBase2>5__2.Right;
                            Label_010F:
                                if (this.<rBNodeBase2>5__2 != null)
                                {
                                    goto Label_00ED;
                                }
                                this.<rBNodeBase2>5__2 = null;
                            }
                            this.<stack>5__1 = null;
                            break;
                        }
                        this.<>2__current = this.source.Root;
                        this.<>1__state = 1;
                        return true;

                    case 1:
                        this.<>1__state = -1;
                        break;

                    case 2:
                        this.<>1__state = -1;
                        this.<rBNodeBase2>5__2 = this.<rBNodeBase2>5__2.Left;
                        goto Label_010F;

                    default:
                        return false;
                }
                return false;
            }

            [DebuggerHidden]
            IEnumerator<RBNodeBase<TKey>> IEnumerable<RBNodeBase<TKey>>.GetEnumerator()
            {
                RBTreeUtils<TKey>.<NodesDescending>d__3 d__;
                if ((this.<>1__state == -2) && (this.<>l__initialThreadId == Environment.get_CurrentManagedThreadId()))
                {
                    this.<>1__state = 0;
                    d__ = (RBTreeUtils<TKey>.<NodesDescending>d__3) this;
                }
                else
                {
                    d__ = new RBTreeUtils<TKey>.<NodesDescending>d__3(0);
                }
                d__.source = this.<>3__source;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<FwNs.Linq.Xprsns.RBNodeBase<TKey>>.GetEnumerator();
            }

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            void IDisposable.Dispose()
            {
            }

            RBNodeBase<TKey> IEnumerator<RBNodeBase<TKey>>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.<>2__current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.<>2__current;
                }
            }
        }

        internal interface ITree
        {
            RBNodeBase<TKey> Root { get; }
        }
    }
}

