using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq.Expressions;


namespace TSharp.Core
{

    /// <summary>
    /// 通用对象相等比较器
    /// <para>by tangjingbo at 2011/10/9 23:49</para>
    /// </summary>
    static class CommonEqualityComparer
    {
        /// <summary>
        /// Gets the ordinal ignore case.
        /// </summary>
        public static IEqualityComparer<string> OrdinalIgnoreCase
        {
            get
            {
                return New<string>((x, y) => string.Equals(x, y, StringComparison.OrdinalIgnoreCase)
                                   , i => i.GetHashCode());
            }
        }

        /// <summary>
        /// Gets the current culture ignore case.
        /// </summary>
        public static IEqualityComparer<string> CurrentCultureIgnoreCase
        {
            get
            {
                return New<string>((x, y) => string.Equals(x, y, StringComparison.CurrentCultureIgnoreCase)
                           , i => i.GetHashCode());
            }
        }
        /// <summary>
        /// News the specified equals.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="equals">The equals.</param>
        /// <param name="hashCode">The hash code.</param>
        /// <returns></returns>
        public static IEqualityComparer<T> New<T>(Func<T, T, bool> @equals, Func<T, int> hashCode)
        {
            return new CommonEqualityComparer<T>(equals, hashCode);
        }

        /// <summary>
        /// 计算多个值对象的Hash Code
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="base">基础Hash值.可以默认为7，一般为第一个对象的hash值</param>
        /// <param name="fileds">The fileds.</param>
        /// <returns></returns>
        public static int SumStructHashcode<T>(this int @base, params T[] fileds) where T : struct
        {
            if (null != fileds)
            {
                foreach (var item in fileds)
                    @base = 31 * @base + item.GetHashCode();
            }
            return @base;
        }
        /// <summary>
        /// 计算多个引用对象的Hash Code
        /// </summary>
        /// <param name="base">基础Hash值.可以默认为7，一般为第一个对象的hash值</param>
        /// <param name="fileds">The fileds.</param>
        /// <returns></returns>
        public static int SumClassHashcode(this int @base, params object[] fileds)
        {

            if (null != fileds)
            {
                foreach (var item in fileds)
                    if (item != null)
                        @base = 31 * @base + item.GetHashCode();
            }
            return @base;
        }
    }
    /// <summary>
    /// 通用对象相等比较器
    /// <para>by tangjingbo at 2011/10/9 23:50</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CommonEqualityComparer<T> : IEqualityComparer<T>, ICloneable
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonEqualityComparer&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="equals">The equals.</param>
        /// <param name="hashCode">The hash code.</param>
        public CommonEqualityComparer(Func<T, T, bool> @equals, Func<T, int> hashCode)
        {
            this.equals = equals;
            this.hashCode = hashCode;
        }

        private readonly Func<T, T, bool> equals;
        private readonly Func<T, int> hashCode;

        bool IEqualityComparer<T>.Equals(T x, T y)
        {
            if (Object.ReferenceEquals(x, y)) return true;
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;
            return equals(x, y);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The type of obj is a reference type and obj is null.</exception>
        int IEqualityComparer<T>.GetHashCode(T obj)
        {
            if (Object.ReferenceEquals(obj, null)) return 0;
            return hashCode(obj);
        }


        /// <summary>
        /// 创建作为当前实例副本的新对象。
        /// </summary>
        /// <returns>
        /// 作为此实例副本的新对象。
        /// </returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }


}