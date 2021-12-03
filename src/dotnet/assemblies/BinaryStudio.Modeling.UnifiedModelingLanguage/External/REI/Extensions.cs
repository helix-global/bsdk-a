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

        public static IEnumerable<IREICOMCategory> AsEnumerable(this IREICOMCategoryCollection source) {
            if (source != null) {
                var c = source.Count;
                for (Int16 i = 1; i <= c; i++) {
                    yield return source.GetAt(i);
                    }
                }
            }

        #region M:ToArray(IRoseCategoryCollection):IRoseCategory[]
        public static IREICOMCategory[] ToArray(this IREICOMCategoryCollection source) {
            if (source != null) {
                var c = source.Count;
                var r = new IREICOMCategory[c];
                for (Int16 i = 1; i <= c; i++) {
                    r[i - 1] = source.GetAt(i);
                    }
                return r;
                }
            return EmptyArray<IREICOMCategory>.Value;
            }
        #endregion
        #region M:ToArray(IRoseClassCollection):IRoseClass[]
        public static IREICOMClass[] ToArray(this IREICOMClassCollection source) {
            if (source != null) {
                var c = source.Count;
                var r = new IREICOMClass[c];
                for (Int16 i = 1; i <= c; i++) {
                    r[i - 1] = source.GetAt(i);
                    }
                return r;
                }
            return EmptyArray<IREICOMClass>.Value;
            }
        #endregion
        #region M:ToArray(IRoseAttributeCollection):IRoseAttribute[]
        public static IREICOMAttribute[] ToArray(this IREICOMAttributeCollection source) {
            if (source != null) {
                var c = source.Count;
                var r = new IREICOMAttribute[c];
                for (Int16 i = 1; i <= c; i++) {
                    r[i - 1] = source.GetAt(i);
                    }
                return r;
                }
            return EmptyArray<IREICOMAttribute>.Value;
            }
        #endregion
        #region M:ToArray(IRoseAssociationCollection):IRoseAssociation[]
        public static IREICOMAssociation[] ToArray(this IREICOMAssociationCollection source) {
            if (source != null) {
                var c = source.Count;
                var r = new IREICOMAssociation[c];
                for (Int16 i = 1; i <= c; i++) {
                    r[i - 1] = source.GetAt(i);
                    }
                return r;
                }
            return EmptyArray<IREICOMAssociation>.Value;
            }
        #endregion
        }
    }