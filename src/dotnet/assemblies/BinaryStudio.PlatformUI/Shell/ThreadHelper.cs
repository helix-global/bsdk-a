using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using BinaryStudio.PlatformUI.Properties;

namespace BinaryStudio.PlatformUI.Shell
    {
    public abstract class ThreadHelper
        {
        private static ThreadHelper _generic;

        private static Dispatcher uiThreadDispatcher;

        public static ThreadHelper Generic
            {
            get
                {
                if (_generic == null)
                    {
                    _generic = new GenericThreadHelper();
                    }
                return _generic;
                }
            }

        //public static JoinableTaskContext JoinableTaskContext
        //{
        //	get
        //	{
        //		return ((IVsTaskSchedulerService2)VsTaskLibraryHelper.ServiceInstance).GetAsyncTaskContext() as JoinableTaskContext;
        //	}
        //}

        //public static JoinableTaskFactory JoinableTaskFactory
        //{
        //	get
        //	{
        //		return ThreadHelper.JoinableTaskContext.get_Factory();
        //	}
        //}

        private static Dispatcher DispatcherForUIThread
            {
            get
                {
                if (uiThreadDispatcher == null && Application.Current != null)
                    {
                    uiThreadDispatcher = Application.Current.Dispatcher;
                    }
                return uiThreadDispatcher;
                }
            }

        [Conditional("DEBUG")]
        private static void ValidateDispatcherSanity()
            {
            }

        internal static void SetUIThread()
            {
            uiThreadDispatcher = Dispatcher.CurrentDispatcher;
            }

        protected abstract IDisposable GetInvocationWrapper();

        //private static IVsInvokerPrivate GetInvoker()
        //{
        //	object obj;
        //	int num = ServiceProvider.GlobalProvider.QueryService(typeof(SVsUIThreadInvokerPrivate), out obj);
        //	if (obj == null && !ErrorHandler.IsRejectedRpcCall(num))
        //	{
        //		throw new InvalidOperationException(string.Format(Resources.Culture, Resources.General_MissingServiceWithHR, new object[]
        //		{
        //			typeof(SVsUIThreadInvokerPrivate).FullName,
        //			num
        //		}));
        //	}
        //	IVsInvokerPrivate vsInvokerPrivate = obj as IVsInvokerPrivate;
        //	if (obj != null && vsInvokerPrivate == null)
        //	{
        //		throw new InvalidCastException(string.Format(Resources.Culture, Resources.General_NoServiceInterface, new object[]
        //		{
        //			typeof(SVsUIThreadInvokerPrivate).FullName,
        //			typeof(IVsInvokerPrivate).FullName
        //		}));
        //	}
        //	return vsInvokerPrivate;
        //}

        public static Boolean CheckAccess()
            {
            var dispatcherForUIThread = DispatcherForUIThread;
            return dispatcherForUIThread != null && dispatcherForUIThread.CheckAccess();
            }

        public static void ThrowIfOnUIThread([CallerMemberName] String callerMemberName = "")
            {
            if (!CheckAccess())
                {
                return;
                }
            var message = String.Format(CultureInfo.CurrentCulture, "{0} must be called on a background thread.", new Object[]
            {
                callerMemberName
            });
            throw new COMException(message, -2147417842);
            }

        public static void ThrowIfNotOnUIThread([CallerMemberName] String callerMemberName = "")
            {
            if (CheckAccess())
                {
                return;
                }
            var message = String.Format(CultureInfo.CurrentCulture, "{0} must be called on the UI thread.", new Object[]
            {
                callerMemberName
            });
            throw new COMException(message, -2147417842);
            }

        #if NET40
        private void InvokeOnUIThread(InvokableBase invokable) {
            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, TimeSpan.FromMilliseconds(100), new Action(()=>{
                invokable.Invoke();
                }));
            }
        #else
        private void InvokeOnUIThread(InvokableBase invokable)
            {
            var flag = false;
            Int32 num;
            while (true)
                {
                //IVsInvokerPrivate invoker = ThreadHelper.GetInvoker();
                //if (invoker != null) {
                //    num = invoker.Invoke(invokable);
                //    if (!flag && num == -2147417848 && invokable.Exception == null) {
                //        flag = true;
                //        }
                //    else if (ErrorHandler.Succeeded(num) || !ErrorHandler.IsRejectedRpcCall(num)) {
                //        break;
                //        }
                //    }
                var task = UIThreadReentrancyScope.TryExecuteActionAsyncInternal(invokable, 100);
                if (task != null)
                    {
                    var result = task.GetAwaiter().GetResult();
                    if (result)
                        {
                        return;
                        }
                    }
                }
            if (invokable.Exception != null)
                {
                throw WrapException(invokable.Exception);
                }
            ErrorHandler.ThrowOnFailure(num);
            }
        #endif

        private static Exception WrapException(Exception inner)
            {
            Exception result = null;
            try
                {
                result = (Exception)Activator.CreateInstance(inner.GetType(), new Object[]
                {
                    inner.Message,
                    inner
                });
                }
            catch
                {
                try
                    {
                    result = (Exception)Activator.CreateInstance(inner.GetType(), new Object[]
                    {
                        inner
                    });
                    }
                catch
                    {
                    result = new TargetInvocationException(inner);
                    }
                }
            return result;
            }

        [DebuggerStepThrough]
        public void Invoke(Action action)
            {
            using (GetInvocationWrapper())
                {
                if (CheckAccess())
                    {
                    action();
                    }
                else
                    {
                    var invokable = new InvokableAction(action, true);
                    InvokeOnUIThread(invokable);
                    }
                }
            }

        [DebuggerStepThrough]
        public TResult Invoke<TResult>(Func<TResult> method)
            {
            TResult result;
            using (GetInvocationWrapper())
                {
                if (CheckAccess())
                    {
                    result = method();
                    }
                else
                    {
                    var invokableFunction = new InvokableFunction<TResult>(method);
                    InvokeOnUIThread(invokableFunction);
                    result = invokableFunction.Result;
                    }
                }
            return result;
            }

        public void BeginInvoke(Action action)
            {
            BeginInvoke(DispatcherPriority.Normal, action);
            }

        public void BeginInvoke(DispatcherPriority priority, Action action)
            {
            var dispatcherForUIThread = DispatcherForUIThread;
            if (dispatcherForUIThread == null)
                {
                throw new InvalidOperationException(Resources.ThreadHelper_UIThreadDispatcherUnavailable);
                }
            dispatcherForUIThread.BeginInvoke(priority, action);
            }

        #if NET40

        #else
        public async Task InvokeAsync(Action executeAction, Func<Boolean> onRpcCallFailed = null)
            {
            await Task.Run(async delegate
            {
                while (true)
                    {
                    try
                        {
                        Invoke(executeAction);
                        break;
                        }
                    catch (InvalidComObjectException)
                        {
                        break;
                        }
                    catch (COMException ex)
                        {
                        if (ex.HResult != -2147417856)
                            {
                            throw;
                            }
                        if (onRpcCallFailed != null && onRpcCallFailed())
                            {
                            break;
                            }
                        }
                    await Task.Delay(100);
                    }
                });
            }
        #endif
        }
    }