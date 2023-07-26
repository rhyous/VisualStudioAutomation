using System;

namespace Rhyous.Tools
{
    /// <summary>
    /// A class that allows retrying a func.
    /// </summary>
    /// <remarks>Source: https://github.com/rhyous/EntityAnywhere/blob/EAF/Api/Interfaces/Interfaces.Common/Tools/Retry.cs</remarks>
    public class Retry
    {
        public const int DefaultRetries = 2;
        public const int DefaultRetryDelay = 150;

        internal TResult Run<T, TResult>(Func<T, TResult> method, T input1, int retries = DefaultRetries, int defaultRetryDelay = DefaultRetryDelay)
        {
            try
            {
                if (method == null)
                {
                    retries = 0;
                    throw new ArgumentNullException(nameof(method));
                }
                return method(input1);
            }
            catch (Exception e)
            {
                if (retries > 0)
                {
                    System.Threading.Thread.Sleep(defaultRetryDelay);
                    return Run(method, input1, --retries);
                }
                throw e;
            }
        }
        internal TResult Run<T1, T2, TResult>(Func<T1, T2, TResult> method, T1 input1, T2 input2, int retries = DefaultRetries, int defaultRetryDelay = DefaultRetryDelay)
        {
            try
            {
                if (method == null)
                {
                    retries = 0;
                    throw new ArgumentNullException(nameof(method));
                }
                return method(input1, input2);
            }
            catch (Exception e)
            {
                if (retries > 0)
                {
                    System.Threading.Thread.Sleep(defaultRetryDelay);
                    return Run(method, input1, input2, --retries);
                }
                throw e;
            }
        }

        internal void Run<T>(Action<T> method, T input1, int retries = DefaultRetries, int defaultRetryDelay = DefaultRetryDelay)
        {
            try
            {
                if (method == null)
                {
                    retries = 0;
                    throw new ArgumentNullException(nameof(method));
                }
                method(input1);
            }
            catch (Exception e)
            {
                if (retries > 0)
                {
                    System.Threading.Thread.Sleep(defaultRetryDelay);
                    Run(method, input1, --retries);
                }
                 throw e;
            }
        }

        internal void Run<T1, T2>(Action<T1, T2> method, T1 input1, T2 input2, int retries = DefaultRetries, int defaultRetryDelay = DefaultRetryDelay)
        {
            try
            {
                if (method == null)
                {
                    retries = 0;
                    throw new ArgumentNullException(nameof(method));
                }
                method(input1, input2);
            }
            catch (Exception e)
            {
                if (retries > 0)
                {
                    System.Threading.Thread.Sleep(defaultRetryDelay);
                    Run(method, input1, input2, --retries);
                }
                throw e;
            }
        }
    }
}