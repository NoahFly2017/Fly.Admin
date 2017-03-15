﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fly.Core.Identity
{
    internal static class TaskExtensions
    {
        public struct CultureAwaiter<T> : ICriticalNotifyCompletion, INotifyCompletion
        {
            private readonly Task<T> _task;
            public bool IsCompleted
            {
                get
                {
                    return this._task.IsCompleted;
                }
            }
            public CultureAwaiter(Task<T> task)
            {
                this._task = task;
            }
            public TaskExtensions.CultureAwaiter<T> GetAwaiter()
            {
                return this;
            }
            public T GetResult()
            {
                return this._task.GetAwaiter().GetResult();
            }
            public void OnCompleted(Action continuation)
            {
                throw new NotImplementedException();
            }
            public void UnsafeOnCompleted(Action continuation)
            {
                CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
                CultureInfo currentUiCulture = Thread.CurrentThread.CurrentUICulture;
                this._task.ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(delegate
                {
                    CultureInfo currentCulture2 = Thread.CurrentThread.CurrentCulture;
                    CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                    Thread.CurrentThread.CurrentUICulture = currentUiCulture;
                    try
                    {
                        continuation();
                    }
                    finally
                    {
                        Thread.CurrentThread.CurrentCulture = currentCulture2;
                        Thread.CurrentThread.CurrentUICulture = currentUICulture;
                    }
                });
            }
        }
        public struct CultureAwaiter : ICriticalNotifyCompletion, INotifyCompletion
        {
            private readonly Task _task;
            public bool IsCompleted
            {
                get
                {
                    return this._task.IsCompleted;
                }
            }
            public CultureAwaiter(Task task)
            {
                this._task = task;
            }
            public TaskExtensions.CultureAwaiter GetAwaiter()
            {
                return this;
            }
            public void GetResult()
            {
                this._task.GetAwaiter().GetResult();
            }
            public void OnCompleted(Action continuation)
            {
                throw new NotImplementedException();
            }
            public void UnsafeOnCompleted(Action continuation)
            {
                CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
                CultureInfo currentUiCulture = Thread.CurrentThread.CurrentUICulture;
                this._task.ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(delegate
                {
                    CultureInfo currentCulture2 = Thread.CurrentThread.CurrentCulture;
                    CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                    Thread.CurrentThread.CurrentUICulture = currentUiCulture;
                    try
                    {
                        continuation();
                    }
                    finally
                    {
                        Thread.CurrentThread.CurrentCulture = currentCulture2;
                        Thread.CurrentThread.CurrentUICulture = currentUICulture;
                    }
                });
            }
        }
        public static TaskExtensions.CultureAwaiter<T> WithCurrentCulture<T>(this Task<T> task)
        {
            return new TaskExtensions.CultureAwaiter<T>(task);
        }
        public static TaskExtensions.CultureAwaiter WithCurrentCulture(this Task task)
        {
            return new TaskExtensions.CultureAwaiter(task);
        }
    }
}
