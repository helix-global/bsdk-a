using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using BinaryStudio.Security.Cryptography.Certificates.Properties;

namespace BinaryStudio.Security.Cryptography
    {
    [DebuggerDisplay("{Message}")]
    public class SecurityException :
        #if NET35
        Exception
        #else
        AggregateException
        #endif
        {
        #region P:StackTrace:String
        public override String StackTrace
            {
            get
                {
                var stacktrace = InternalStackTrace;
                return String.IsNullOrEmpty(stacktrace)
                    ? base.StackTrace
                    : stacktrace;
                }
            }
        #endregion

        #if NET35
        /// <summary>Initializes a new instance of the <see cref="SecurityException"/> class with a system-supplied message that describes the error.</summary>
        public SecurityException()
            {
            InnerExceptions = new ReadOnlyCollection<Exception>(new Exception[0]);
            }

        /// <summary>Initializes a new instance of the <see cref="SecurityException"/> class with a specified message that describes the error.</summary>
        /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</param>
        public SecurityException(String message)
            :base(message)
            {
            InnerExceptions = new ReadOnlyCollection<Exception>(new Exception[0]);
            }

        /// <summary>Initializes a new instance of the <see cref="SecurityException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
        /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException"/> parameter is not <see langword="null"/>, the current exception is raised in a <see langword="catch"/> block that handles the inner exception.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="innerException"/> argument is null.</exception>
        public SecurityException(String message, Exception innerException)
            :base(message, innerException)
            {
            if (innerException == null) { throw new ArgumentNullException(nameof(innerException)); }
            InnerExceptions = new ReadOnlyCollection<Exception>(new[]
                {
                innerException
                });
            }

        /// <summary>Initializes a new instance of the <see cref="SecurityException"/> class with a specified error message and references to the inner exceptions that are the cause of this exception.</summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerExceptions">The exceptions that are the cause of the current exception.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="innerExceptions"/> argument is null.</exception>
        /// <exception cref="T:System.ArgumentException">An element of <paramref name="innerExceptions"/> is null.</exception>
        public SecurityException(String message, IEnumerable<Exception> innerExceptions)
            : this(message, (innerExceptions as IList<Exception>) ?? ((innerExceptions == null) ? null : new List<Exception>(innerExceptions)))
        {
        }

        private SecurityException(String message, IList<Exception> innerExceptions)
            : base(message, (innerExceptions != null && innerExceptions.Count > 0) ? innerExceptions[0] : null)
        {
            if (innerExceptions == null) { throw new ArgumentNullException(nameof(innerExceptions)); }
            var array = new Exception[innerExceptions.Count];
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = innerExceptions[i];
                if (array[i] == null)
                {
                    throw new ArgumentOutOfRangeException(nameof(innerExceptions));
                }
            }
            InnerExceptions = new ReadOnlyCollection<Exception>(array);
        }

        /// <summary>Gets a read-only collection of the <see cref="T:System.Exception"/> instances that caused the current exception.</summary>
        /// <returns>A read-only collection of the <see cref="T:System.Exception"/> instances that caused the current exception.</returns>
        public ReadOnlyCollection<Exception> InnerExceptions { get; }
        #else
        /// <summary>Initializes a new instance of the <see cref="SecurityException"/> class with a system-supplied message that describes the error.</summary>
        public SecurityException()
            {
            }

        /// <summary>Initializes a new instance of the <see cref="SecurityException"/> class with a specified message that describes the error.</summary>
        /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</param>
        public SecurityException(String message)
            : base(message)
            {
            }

        /// <summary>Initializes a new instance of the <see cref="SecurityException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
        /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException"/> parameter is not <see langword="null"/>, the current exception is raised in a <see langword="catch"/> block that handles the inner exception.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="innerException"/> argument is null.</exception>
        public SecurityException(String message, Exception innerException)
            : base(message, innerException)
            {
            }

        /// <summary>Initializes a new instance of the <see cref="SecurityException"/> class with a specified error message and references to the inner exceptions that are the cause of this exception.</summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerExceptions">The exceptions that are the cause of the current exception.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="innerExceptions"/> argument is null.</exception>
        /// <exception cref="T:System.ArgumentException">An element of <paramref name="innerExceptions"/> is null.</exception>
        public SecurityException(String message, IEnumerable<Exception> innerExceptions)
            : base(message, innerExceptions)
            {
            }
        #endif

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] internal String InternalStackTrace { get; set; }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal UInt32? InternalHResult
            {
            get { return unchecked((UInt32)HResult); }
            set
                {
                if (value != null)
                    {
                    HResult = unchecked((Int32)value.Value);
                    }
                }
            }

        /// <summary>Creates and returns a string representation of the current <see cref="SecurityException"/>.</summary>
        /// <returns>A string representation of the current exception.</returns>
        public override String ToString()
            {
            var r = base.ToString();
            var i = 0;
            foreach (var e in InnerExceptions)
                {
                r = String.Format(
                    CultureInfo.InvariantCulture,
                    Resources.AggregateException,
                    r, Environment.NewLine,
                    i, e.ToString(), "<---", Environment.NewLine);
                i++;
                }
            return r;
            }
        }
    }