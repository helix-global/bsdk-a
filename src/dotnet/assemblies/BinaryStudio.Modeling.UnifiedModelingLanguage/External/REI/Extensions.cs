using System;
using System.Collections.Generic;

namespace RationalRose
    {
    public static class Extensions
        {
        internal class EmptyArray<T>
            {
            public static readonly T[] Value = new T[0];
            }

        public static IEnumerable<IRoseCategory> AsEnumerable(this IRoseCategoryCollection source) {
            if (source != null) {
                var c = source.Count;
                for (Int16 i = 1; i <= c; i++) {
                    yield return source.GetAt(i);
                    }
                }
            }

        #region M:ToArray(IRoseCategoryCollection):IRoseCategory[]
        public static IRoseCategory[] ToArray(this IRoseCategoryCollection source) {
            if (source != null) {
                var c = source.Count;
                var r = new IRoseCategory[c];
                for (Int16 i = 1; i <= c; i++) {
                    r[i - 1] = source.GetAt(i);
                    }
                return r;
                }
            return EmptyArray<IRoseCategory>.Value;
            }
        #endregion
        #region M:ToArray(IRoseClassCollection):IRoseClass[]
        public static IRoseClass[] ToArray(this IRoseClassCollection source) {
            if (source != null) {
                var c = source.Count;
                var r = new IRoseClass[c];
                for (Int16 i = 1; i <= c; i++) {
                    r[i - 1] = source.GetAt(i);
                    }
                return r;
                }
            return EmptyArray<IRoseClass>.Value;
            }
        #endregion
        #region M:ToArray(IRoseAttributeCollection):IRoseAttribute[]
        public static IRoseAttribute[] ToArray(this IRoseAttributeCollection source) {
            if (source != null) {
                var c = source.Count;
                var r = new IRoseAttribute[c];
                for (Int16 i = 1; i <= c; i++) {
                    r[i - 1] = source.GetAt(i);
                    }
                return r;
                }
            return EmptyArray<IRoseAttribute>.Value;
            }
        #endregion
        #region M:ToArray(IRoseAssociationCollection):IRoseAssociation[]
        public static IRoseAssociation[] ToArray(this IRoseAssociationCollection source) {
            if (source != null) {
                var c = source.Count;
                var r = new IRoseAssociation[c];
                for (Int16 i = 1; i <= c; i++) {
                    r[i - 1] = source.GetAt(i);
                    }
                return r;
                }
            return EmptyArray<IRoseAssociation>.Value;
            }
        #endregion
        }
    }