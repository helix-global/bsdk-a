using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BinaryStudio.DataProcessing
    {
    public class DBQueryProvider : IQueryProvider
        {
        public DBQueryFactory Factory { get; }
        public DBQueryProvider(DBQueryFactory factory)
            {
            if (factory == null) { throw new ArgumentNullException(nameof(factory)); }
            Factory = factory;
            }

        internal DBQueryProvider()
            {
            }

        /// <summary>Constructs an <see cref="T:System.Linq.IQueryable"/> object that can evaluate the query represented by a specified expression tree.</summary>
        /// <returns>An <see cref="T:System.Linq.IQueryable"/> that can evaluate the query represented by the specified expression tree.</returns>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        public IQueryable CreateQuery(Expression expression) {
            if (expression == null) { throw new ArgumentNullException(nameof(expression)); }
            var type = FindGenericType(typeof(IQueryable<>), expression.Type);
            if (type == null) { throw new ArgumentOutOfRangeException(nameof(expression)); }
            return Create<IQueryable>(Factory.GetQueryType(), type, expression);
            }

        private IEnumerable<Object> FetchFromCommand(DbCommand expression) {
            if (expression == null) { throw new ArgumentNullException(nameof(expression)); }
            using (var reader = expression.ExecuteReader()) {
                while (reader.Read()) {
                    var r = new Object[reader.FieldCount];
                    for (var i = 0; i < reader.FieldCount; i++) {
                        r[i] = reader[i];
                        }
                    yield return r;
                    }
                }
            }

        public IQueryable CreateQuery(DbCommand expression) {
            if (expression == null) { throw new ArgumentNullException(nameof(expression)); }
            return new EnumerableQuery<Object>(FetchFromCommand(expression));
            }

        /// <summary>Constructs an <see cref="T:System.Linq.IQueryable`1"/> object that can evaluate the query represented by a specified expression tree.</summary>
        /// <returns>An <see cref="T:System.Linq.IQueryable`1"/> that can evaluate the query represented by the specified expression tree.</returns>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        /// <typeparam name="TElement">The type of the elements of the <see cref="T:System.Linq.IQueryable`1" /> that is returned.</typeparam>
        public virtual IQueryable<TElement> CreateQuery<TElement>(Expression expression) {
            if (expression == null) { throw new ArgumentNullException(nameof(expression)); }
            if (!typeof(IQueryable<TElement>).IsAssignableFrom(expression.Type)) { throw new ArgumentOutOfRangeException(nameof(expression)); }
            return Create<IQueryable<TElement>>(Factory.GetQueryType(), typeof(TElement), expression);
            }

        /// <summary>Executes the query represented by a specified expression tree.</summary>
        /// <returns>The value that results from executing the specified query.</returns>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        public Object Execute(Expression expression)
            {
            if (expression == null) { throw new ArgumentNullException(nameof(expression)); }
            var type = FindGenericType(typeof(IQueryExecutor<>), expression.Type);
            if (type == null) { throw new ArgumentOutOfRangeException(nameof(expression)); }
            return Create<IQueryExecutor>(Factory.GetExecutorType(), type, expression).Execute();
            }

        /// <summary>Executes the strongly-typed query represented by a specified expression tree.</summary>
        /// <returns>The value that results from executing the specified query.</returns>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        /// <typeparam name="TResult">The type of the value that results from executing the query.</typeparam>
        public TResult Execute<TResult>(Expression expression)
            {
            if (expression == null) { throw new ArgumentNullException(nameof(expression)); }
            return Create<IQueryExecutor<TResult>>(Factory.GetExecutorType(), typeof(TResult), expression).Execute();
            }

        #region M:FindGenericType(Type,Type):Type
        internal static Type FindGenericType(Type definition, Type type) {
            while (type != null && type != typeof(Object)) {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == definition) { return type; }
                if (definition.IsInterface) {
                    var interfaces = type.GetInterfaces();
                    foreach (var i in interfaces) {
                        var gtype = FindGenericType(definition, i);
                        if (gtype != null) {
                            return gtype;
                            }
                        }
                    }
                type = type.BaseType;
                }
            return null;
            }
        #endregion
        #region M:Create<T>(Type,Type,Expression):T
        internal static T Create<T>(Type type, Type elementType, Expression expression) {
            var r = type.MakeGenericType(elementType);
            return (T)Activator.CreateInstance(
                r, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null, new Object[]
                    {
                    expression
                    }, null);
            }
        #endregion
        }
    }