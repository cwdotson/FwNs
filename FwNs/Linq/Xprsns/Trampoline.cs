namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal class Trampoline
    {
        [ThreadStatic]
        private static Trampoline _current;
        private Thread _curThread = Thread.CurrentThread;
        private SynchronizationContext _ctx = SynchronizationContext.Current;
        private Queue<WorkItem> _tasks;

        private Trampoline()
        {
        }

        private void DoWork(WorkItem item)
        {
            SecurityExtensions.WithMemberAccessCheck(item.DoWork);
        }

        public void EnsureRunning(Action withTrampoline)
        {
            if (this.IsRunning)
            {
                withTrampoline.Invoke();
            }
            else
            {
                this.Post(withTrampoline);
            }
        }

        public T EnsureRunning<T>(Func<T> withTrampoline)
        {
            <>c__DisplayClass11_0<T> class_1;
            T result = default(T);
            Action action = new Action(class_1, this.<EnsureRunning>b__0);
            this.EnsureRunning(action);
            return result;
        }

        public bool ExecutePostedTasks()
        {
            if ((this._tasks != null) && (this._tasks.Count != 0))
            {
                while (this._tasks.Count > 0)
                {
                    this.DoWork(this._tasks.Dequeue());
                }
                return true;
            }
            return false;
        }

        internal Action MakePosted(Action action)
        {
            <>c__DisplayClass17_0 class_1;
            return new Action(class_1, this.<MakePosted>b__0);
        }

        private void Post(WorkItem task)
        {
            if (this._curThread == Thread.CurrentThread)
            {
                this.PostThreadUnsafe(task);
            }
            else
            {
                if (this._ctx == null)
                {
                    throw new InvalidOperationException(Errorz.MultiThreadAccess);
                }
                this._ctx.Post(t => this.PostThreadUnsafe((WorkItem) t), task);
            }
        }

        public void Post(Action task)
        {
            WorkItem item = new WorkItem {
                DoWork = task
            };
            this.Post(item);
        }

        private void PostThreadUnsafe(WorkItem task)
        {
            if (this._tasks != null)
            {
                this._tasks.Enqueue(task);
            }
            else
            {
                this._tasks = new Queue<WorkItem>();
                try
                {
                    this.DoWork(task);
                    this.ExecutePostedTasks();
                }
                finally
                {
                    this._tasks = null;
                }
            }
        }

        public static Trampoline Current
        {
            get
            {
                Trampoline trampoline = _current;
                if (trampoline == null)
                {
                    trampoline = _current = new Trampoline();
                }
                return trampoline;
            }
        }

        public bool IsRunning
        {
            get
            {
                return (this._tasks > null);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WorkItem
        {
            public Action DoWork;
        }
    }
}

