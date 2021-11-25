using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace BinaryStudio.PlatformUI.Shell
    {
    public static class UIThreadReentrancyScope
        {
        private static readonly Object _lockObj = new Object();
        private static readonly Queue<PendingRequest> _queue = new Queue<PendingRequest>();
        private static TaskCompletionSource<Object> _queueHasElement = new TaskCompletionSource<Object>();

        private static Task RequestWaiter
            {
            get
                {
                lock (_lockObj)
                    return _queueHasElement.Task;
                }
            }

        private static Boolean ExecuteOne()
            {
            ThreadHelper.ThrowIfNotOnUIThread("ExecuteOne");
            var completeEvent = (TaskCompletionSource<Boolean>)null;
            var action = (InvokableBase)null;
            lock (_lockObj)
                {
                var pendingRequest = (PendingRequest)null;
                while (_queue.Count > 0 && pendingRequest == null)
                    {
                    pendingRequest = _queue.Dequeue();
                    if (pendingRequest.Revoked)
                        pendingRequest = null;
                    }
                if (pendingRequest != null)
                    pendingRequest.InitiateExecute(out completeEvent, out action);
                if (_queue.Count == 0)
                    {
                    _queueHasElement.TrySetResult(null);
                    _queueHasElement = new TaskCompletionSource<Object>();
                    }
                }
            if (completeEvent == null)
                return false;
            var num = action.Invoke();
            if (ErrorHandler.Succeeded(num))
                completeEvent.TrySetResult(true);
            else
                completeEvent.TrySetException(Marshal.GetExceptionForHR(num));
            return true;
            }

        private static void Flush()
            {
            do
                ;
            while (ExecuteOne());
            }

        private static void ClearQueue()
            {
            lock (_lockObj)
                {
                if (_queue.Count == 0)
                    return;
                var pendingRequestList = (List<PendingRequest>)null;
                while (_queue.Count > 0)
                    {
                    var pendingRequest = _queue.Dequeue();
                    if (!pendingRequest.AllowCleanup)
                        {
                        if (pendingRequestList == null)
                            pendingRequestList = new List<PendingRequest>();
                        pendingRequestList.Add(pendingRequest);
                        }
                    else
                        pendingRequest.SkipExecution();
                    }
                if (pendingRequestList != null)
                    {
                    pendingRequestList.Reverse();
                    foreach (var pendingRequest in pendingRequestList)
                        _queue.Enqueue(pendingRequest);
                    }
                if (_queue.Count != 0)
                    return;
                _queueHasElement.TrySetResult(null);
                _queueHasElement = new TaskCompletionSource<Object>();
                }
            }

        private static Task<Boolean> Dequeue(PendingRequest pr)
            {
            var task = (Task<Boolean>)null;
            lock (_lockObj)
                {
                task = pr.SkipExecution();
                while (_queue.Count > 0 && _queue.Peek().Revoked)
                    _queue.Dequeue();
                if (_queue.Count == 0)
                    {
                    _queueHasElement.TrySetResult(null);
                    _queueHasElement = new TaskCompletionSource<Object>();
                    }
                }
            return task;
            }

        #if !NET40
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static async Task EnqueueActionAsync(Action action)
            {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            //if (ThreadHelper.CheckAccess())
            //      {
            //          await TaskScheduler.Default;
            //      }

            var pendingRequest = new PendingRequest(new InvokableAction(action, false), false);
            lock (_lockObj)
                {
                _queue.Enqueue(pendingRequest);
                _queueHasElement.TrySetResult(null);
                }
            }
        #endif
        #if !NET40
        internal static async Task<Boolean> TryExecuteActionAsyncInternal(InvokableBase action, Int32 timeout = -1)
            {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            ThreadHelper.ThrowIfOnUIThread("TryExecuteActionAsyncInternal");
            var pr = new PendingRequest(action, true);
            lock (_lockObj)
                {
                _queue.Enqueue(pr);
                _queueHasElement.TrySetResult(null);
                }
            if (timeout != -1)
                {
                var delayCancellation = new CancellationTokenSource();
                var task = await Task.WhenAny((Task)pr.Waiter, Task.Delay(timeout, delayCancellation.Token));
                delayCancellation.Cancel();
                delayCancellation = null;
                }
            else
                {
                var num = await pr.Waiter ? 1 : 0;
                }
            return await Dequeue(pr);
            }
        #endif

        #if !NET40
        /// <summary>
        /// Wait for task completion. If this is called on the UI thread, it provides a reentrancy point for Thread helper.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="cancel">The cancelation token.</param>
        /// <param name="ms"></param>
        /// <returns>True if the task is completed, otherwise false.</returns>
        public static Boolean WaitOnTaskComplete(Task task, CancellationToken cancel, Int32 ms)
            {
            if (!ThreadHelper.CheckAccess())
                return task.Wait(ms, cancel);
            return WaitOnTaskCompleteInternal(task, cancel, ms);
            }
        #endif

        #if !NET40
        private static Boolean WaitOnTaskCompleteInternal(Task task, CancellationToken cancel, Int32 ms)
            {
            ThreadHelper.ThrowIfNotOnUIThread("WaitOnTaskCompleteInternal");
            var tasks = new Task[2]
            {
        task,
        null
            };
            var flag1 = false;
            Boolean flag2;
            while (!task.IsCompleted)
                {
                tasks[1] = RequestWaiter;
                var stopwatch = Stopwatch.StartNew();
                switch (Task.WaitAny(tasks, ms, cancel))
                    {
                    case 0:
                    flag1 = task.Wait(ms, cancel);
                    break;
                    case 1:
                    Flush();
                    break;
                    default:
                    flag1 = false;
                    break;
                    }
                stopwatch.Stop();
                var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                if (ms != -1)
                    {
                    if (elapsedMilliseconds < 0L || elapsedMilliseconds >= ms)
                        {
                        flag2 = false;
                        goto label_11;
                        }
                    else
                        ms -= (Int32)elapsedMilliseconds;
                    }
                }
            task.GetAwaiter().GetResult();
            flag2 = true;
            label_11:
            ClearQueue();
            return flag2;
            }
        #endif

        internal class PendingRequest
            {
            private TaskCompletionSource<Boolean> WorkCompleteEvent { get; }

            private InvokableBase InvokeAction { get; set; }

            private Boolean Started { get; set; }

            internal Boolean AllowCleanup { get; }

            internal Boolean Revoked
                {
                get
                    {
                    return InvokeAction == null;
                    }
                }

            internal Task<Boolean> Waiter
                {
                get
                    {
                    return WorkCompleteEvent.Task;
                    }
                }

            internal PendingRequest(InvokableBase action, Boolean guaranteeExecution)
                {
                InvokeAction = action;
                Started = false;
                WorkCompleteEvent = new TaskCompletionSource<Boolean>();
                AllowCleanup = !guaranteeExecution;
                }

            internal void InitiateExecute(out TaskCompletionSource<Boolean> completeEvent, out InvokableBase action)
                {
                if (!Revoked)
                    {
                    completeEvent = WorkCompleteEvent;
                    action = InvokeAction;
                    Started = true;
                    }
                else
                    {
                    completeEvent = null;
                    action = null;
                    }
                InvokeAction = null;
                }

            internal Task<Boolean> SkipExecution()
                {
                if (!Started)
                    {
                    WorkCompleteEvent.TrySetResult(false);
                    InvokeAction = null;
                    }
                return Waiter;
                }
            }
        }
    }