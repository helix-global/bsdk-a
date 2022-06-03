
#if NET35
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security;
using System.Text;

namespace System
    {
    using LocalResources=BinaryStudio.PlatformCore.Properties.Resources;

    [DebuggerDisplay("Count = {InnerExceptionCount}")]
    [Serializable]
    public class AggregateException : Exception
        {
        private ReadOnlyCollection<Exception> m_innerExceptions;

        /// <summary>Initializes a new instance of the <see cref="T:System.AggregateException" /> class with a system-supplied message that describes the error.</summary>
        public AggregateException()
            :base(LocalResources.AggregateException_ctor_DefaultMessage)
            {
            m_innerExceptions = new ReadOnlyCollection<Exception>((IList<Exception>)new Exception[0]);
            }

        /// <summary>Initializes a new instance of the <see cref="T:System.AggregateException" /> class with a specified message that describes the error.</summary>
        /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</param>
        public AggregateException(string message)
            :base(message)
            {
            m_innerExceptions = new ReadOnlyCollection<Exception>((IList<Exception>)new Exception[0]);
            }

        /// <summary>Initializes a new instance of the <see cref="T:System.AggregateException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
        /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not <see langword="null" />, the current exception is raised in a <see langword="catch" /> block that handles the inner exception.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="innerException" /> argument is null.</exception>
        public AggregateException(string message, Exception innerException)
          : base(message, innerException)
            {
            if (innerException == null)
                throw new ArgumentNullException(nameof(innerException));
            this.m_innerExceptions = new ReadOnlyCollection<Exception>((IList<Exception>)new Exception[1]
            {
        innerException
            });
            }

        /// <summary>Initializes a new instance of the <see cref="T:System.AggregateException" /> class with references to the inner exceptions that are the cause of this exception.</summary>
        /// <param name="innerExceptions">The exceptions that are the cause of the current exception.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="innerExceptions" /> argument is null.</exception>
        /// <exception cref="T:System.ArgumentException">An element of <paramref name="innerExceptions" /> is null.</exception>
        public AggregateException(IEnumerable<Exception> innerExceptions)
            :this(LocalResources.AggregateException_ctor_DefaultMessage, innerExceptions)
            {
            }

        /// <summary>Initializes a new instance of the <see cref="T:System.AggregateException" /> class with references to the inner exceptions that are the cause of this exception.</summary>
        /// <param name="innerExceptions">The exceptions that are the cause of the current exception.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="innerExceptions" /> argument is null.</exception>
        /// <exception cref="T:System.ArgumentException">An element of <paramref name="innerExceptions" /> is null.</exception>
        public AggregateException(params Exception[] innerExceptions)
            :this(LocalResources.AggregateException_ctor_DefaultMessage, innerExceptions)
            {
            }

        /// <summary>Initializes a new instance of the <see cref="T:System.AggregateException" /> class with a specified error message and references to the inner exceptions that are the cause of this exception.</summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerExceptions">The exceptions that are the cause of the current exception.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="innerExceptions" /> argument is null.</exception>
        /// <exception cref="T:System.ArgumentException">An element of <paramref name="innerExceptions" /> is null.</exception>
        public AggregateException(string message, IEnumerable<Exception> innerExceptions)
            :this(message, innerExceptions as IList<Exception> ?? (innerExceptions == null ? (IList<Exception>)null : (IList<Exception>)new List<Exception>(innerExceptions)))
            {
            }

        /// <summary>Initializes a new instance of the <see cref="T:System.AggregateException" /> class with a specified error message and references to the inner exceptions that are the cause of this exception.</summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerExceptions">The exceptions that are the cause of the current exception.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="innerExceptions" /> argument is null.</exception>
        /// <exception cref="T:System.ArgumentException">An element of <paramref name="innerExceptions" /> is null.</exception>
        public AggregateException(string message, params Exception[] innerExceptions)
            :this(message, (IList<Exception>)innerExceptions)
            {
            }

        private AggregateException(string message, IList<Exception> innerExceptions)
          : base(message, innerExceptions == null || innerExceptions.Count <= 0 ? (Exception)null : innerExceptions[0])
            {
            if (innerExceptions == null)
                throw new ArgumentNullException(nameof(innerExceptions));
            Exception[] exceptionArray = new Exception[innerExceptions.Count];
            for (int index = 0; index < exceptionArray.Length; ++index)
                {
                exceptionArray[index] = innerExceptions[index];
                if (exceptionArray[index] == null)
                    throw new ArgumentException(LocalResources.AggregateException_ctor_InnerExceptionNull);
                }
            this.m_innerExceptions = new ReadOnlyCollection<Exception>((IList<Exception>)exceptionArray);
            }

        //internal AggregateException(
        //  IEnumerable<ExceptionDispatchInfo> innerExceptionInfos)
        //    :this(Resources.AggregateException_ctor_DefaultMessage, innerExceptionInfos)
        //    {
        //    }

        //internal AggregateException(
        //  string message,
        //  IEnumerable<ExceptionDispatchInfo> innerExceptionInfos)
        //  : this(message, innerExceptionInfos as IList<ExceptionDispatchInfo> ?? (innerExceptionInfos == null ? (IList<ExceptionDispatchInfo>)null : (IList<ExceptionDispatchInfo>)new List<ExceptionDispatchInfo>(innerExceptionInfos)))
        //    {
        //    }

        //private AggregateException(string message, IList<ExceptionDispatchInfo> innerExceptionInfos)
        //  : base(message, innerExceptionInfos == null || innerExceptionInfos.Count <= 0 || innerExceptionInfos[0] == null ? (Exception)null : innerExceptionInfos[0].SourceException)
        //    {
        //    if (innerExceptionInfos == null)
        //        throw new ArgumentNullException(nameof(innerExceptionInfos));
        //    Exception[] exceptionArray = new Exception[innerExceptionInfos.Count];
        //    for (int index = 0; index < exceptionArray.Length; ++index)
        //        {
        //        ExceptionDispatchInfo innerExceptionInfo = innerExceptionInfos[index];
        //        if (innerExceptionInfo != null)
        //            exceptionArray[index] = innerExceptionInfo.SourceException;
        //        if (exceptionArray[index] == null)
        //            throw new ArgumentException(Resources.AggregateException_ctor_InnerExceptionNull);
        //        }
        //    m_innerExceptions = new ReadOnlyCollection<Exception>((IList<Exception>)exceptionArray);
        //    }

        /// <summary>Initializes a new instance of the <see cref="T:System.AggregateException" /> class with serialized data.</summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info" /> argument is null.</exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The exception could not be deserialized correctly.</exception>
        [SecurityCritical]
        protected AggregateException(SerializationInfo info, StreamingContext context)
          : base(info, context)
            {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            Exception[] exceptionArray = info.GetValue(nameof(InnerExceptions), typeof(Exception[])) as Exception[];
            if (exceptionArray == null)
                throw new SerializationException(LocalResources.AggregateException_DeserializationFailure);
            m_innerExceptions = new ReadOnlyCollection<Exception>((IList<Exception>)exceptionArray);
            }

        /// <summary>Initializes a new instance of the <see cref="T:System.AggregateException" /> class with serialized data.</summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info" /> argument is null.</exception>
        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            base.GetObjectData(info, context);
            Exception[] array = new Exception[this.m_innerExceptions.Count];
            this.m_innerExceptions.CopyTo(array, 0);
            info.AddValue("InnerExceptions", (object)array, typeof(Exception[]));
            }

        /// <summary>Returns the <see cref="T:System.AggregateException" /> that is the root cause of this exception.</summary>
        /// <returns>The <see cref="T:System.AggregateException" /> that is the root cause of this exception.</returns>
        public override Exception GetBaseException()
            {
            Exception exception = (Exception)this;
            for (AggregateException aggregateException = this; aggregateException != null && aggregateException.InnerExceptions.Count == 1; aggregateException = exception as AggregateException)
                exception = exception.InnerException;
            return exception;
            }

        /// <summary>Gets a read-only collection of the <see cref="T:System.Exception" /> instances that caused the current exception.</summary>
        /// <returns>A read-only collection of the <see cref="T:System.Exception" /> instances that caused the current exception.</returns>
        public ReadOnlyCollection<Exception> InnerExceptions
            {
            get
                {
                return this.m_innerExceptions;
                }
            }

        /// <summary>Invokes a handler on each <see cref="T:System.Exception" /> contained by this <see cref="T:System.AggregateException" />.</summary>
        /// <param name="predicate">The predicate to execute for each exception. The predicate accepts as an argument the <see cref="T:System.Exception" /> to be processed and returns a Boolean to indicate whether the exception was handled.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="predicate" /> argument is null.</exception>
        /// <exception cref="T:System.AggregateException">An exception contained by this <see cref="T:System.AggregateException" /> was not handled.</exception>
        public void Handle(Func<Exception, bool> predicate)
            {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            List<Exception> exceptionList = (List<Exception>)null;
            for (int index = 0; index < this.m_innerExceptions.Count; ++index)
                {
                if (!predicate(this.m_innerExceptions[index]))
                    {
                    if (exceptionList == null)
                        exceptionList = new List<Exception>();
                    exceptionList.Add(this.m_innerExceptions[index]);
                    }
                }
            if (exceptionList != null)
                throw new AggregateException(this.Message, (IList<Exception>)exceptionList);
            }

        /// <summary>Flattens an <see cref="T:System.AggregateException" /> instances into a single, new instance.</summary>
        /// <returns>A new, flattened <see cref="T:System.AggregateException" />.</returns>
        public AggregateException Flatten()
            {
            List<Exception> exceptionList = new List<Exception>();
            List<AggregateException> aggregateExceptionList = new List<AggregateException>();
            aggregateExceptionList.Add(this);
            int num = 0;
            while (aggregateExceptionList.Count > num)
                {
                IList<Exception> innerExceptions = (IList<Exception>)aggregateExceptionList[num++].InnerExceptions;
                for (int index = 0; index < innerExceptions.Count; ++index)
                    {
                    Exception exception = innerExceptions[index];
                    if (exception != null)
                        {
                        AggregateException aggregateException = exception as AggregateException;
                        if (aggregateException != null)
                            aggregateExceptionList.Add(aggregateException);
                        else
                            exceptionList.Add(exception);
                        }
                    }
                }
            return new AggregateException(this.Message, (IList<Exception>)exceptionList);
            }

        /// <summary>Creates and returns a string representation of the current <see cref="T:System.AggregateException" />.</summary>
        /// <returns>A string representation of the current exception.</returns>
        public override string ToString()
            {
            string str = base.ToString();
            for (int index = 0; index < this.m_innerExceptions.Count; ++index)
                str = string.Format((IFormatProvider)CultureInfo.InvariantCulture, LocalResources.AggregateException_ToString, (object)str, (object)Environment.NewLine, (object)index, (object)this.m_innerExceptions[index].ToString(), (object)"<---", (object)Environment.NewLine);
            return str;
            }

        private int InnerExceptionCount
            {
            get
                {
                return this.InnerExceptions.Count;
                }
            }
        }
    }
#endif