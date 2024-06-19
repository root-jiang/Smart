using System;
using System.Collections.Generic;

namespace FaultTreeAnalysis.Behavior.Common
{
    /// <summary>
    /// 忽略顺序的元素集合比较
    ///     用于比较集合对象的一致性
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MultiSetComparer<T> : IEqualityComparer<IEnumerable<T>>
    {
        /// <summary>
        /// 比较器
        /// </summary>
        private readonly IEqualityComparer<T> m_iecComparer;

        /// <summary>
        /// 构造比较器
        /// </summary>
        /// <param name="iecComparer">比较器</param>
        public MultiSetComparer(IEqualityComparer<T> iecComparer = null)
        {
            m_iecComparer = iecComparer ?? EqualityComparer<T>.Default;
        }

        /// <summary>
        /// 对象比较
        /// </summary>
        /// <param name="ieFirst">首个对象</param>
        /// <param name="ieSecond">此个对象</param>
        /// <returns>true: 两个对象一致, 否则不一致</returns>
        public bool Equals(IEnumerable<T> ieFirst, IEnumerable<T> ieSecond)
        {
            //基本比较
            if (null == ieFirst)
            {
                return (null == ieSecond);
            }
            if (null == ieSecond)
            {
                return false;
            }
            if (ReferenceEquals(ieFirst, ieSecond))
            {
                return true;
            }

            //大小比较
            ICollection<T> ieFirstCollection = ieFirst as ICollection<T>;
            ICollection<T> ieSecondCollection = ieSecond as ICollection<T>;
            //if ((ieFirst is ICollection<T> ieFirstCollection)
            //    && (ieSecond is ICollection<T> ieSecondCollection))
            if((null != ieFirstCollection)
                && (null != ieSecondCollection))
            {
                if (ieFirstCollection.Count != ieSecondCollection.Count)
                {
                    return false;
                }

                if (0x00 == ieFirstCollection.Count)
                {
                    return true;
                }
            }

            //元素比较
            return !HaveMismatchedElement(ieFirst, ieSecond);
        }

        /// <summary>
        /// 遗漏元素比较处理
        /// </summary>
        /// <param name="ieFirst">元素集合</param>
        /// <param name="ieSecond">元素集合</param>
        /// <returns></returns>
        private bool HaveMismatchedElement(IEnumerable<T> ieFirst, IEnumerable<T> ieSecond)
        {
            //统计元素数
            int iFirstNullCount;
            int iSecondNullCount;
            Dictionary<T, int> dicFirstElementCounts = GetElementCounts(ieFirst, out iFirstNullCount);
            Dictionary<T, int> dicSecondElementCounts = GetElementCounts(ieSecond, out iSecondNullCount);

            //统计值大小比较
            if ((iFirstNullCount != iSecondNullCount)
                || (dicFirstElementCounts.Count != dicSecondElementCounts.Count))
            {
                return true;
            }

            //元素映射比较
            foreach (var kvp in dicFirstElementCounts)
            {
                int iFirstElementCount = kvp.Value;
                int iSecondElementCount;
                dicSecondElementCounts.TryGetValue(kvp.Key, out iSecondElementCount);

                //元素数不等
                if (iFirstElementCount != iSecondElementCount)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 构造元素分布图
        /// </summary>
        /// <param name="ieEnumerable">元素集合</param>
        /// <param name="iNullCount">数量</param>
        /// <returns></returns>
        private Dictionary<T, int> GetElementCounts(IEnumerable<T> ieEnumerable, out int iNullCount)
        {
            Dictionary<T, int> dicElementCount= new Dictionary<T, int>(m_iecComparer);
            iNullCount = 0x00;

            //统计元素数量图表
            foreach (T element in ieEnumerable)
            {
                if (element == null)
                {
                    iNullCount++;
                }
                else
                {
                    int iNum;
                    dicElementCount.TryGetValue(element, out iNum);
                    iNum++;
                    dicElementCount[element] = iNum;
                }
            }

            return dicElementCount;
        }

        /// <summary>
        /// calc hash code
        /// </summary>
        /// <param name="ieEnumerable">元素集合</param>
        /// <returns></returns>
        public int GetHashCode(IEnumerable<T> ieEnumerable)
        {
            if (ieEnumerable == null)
            {
                throw new ArgumentNullException(nameof(ieEnumerable));
            }

            int iHash = 0x11;

            foreach (T val in ieEnumerable)
            {
                iHash ^= ((null == val) ? 0x2A : m_iecComparer.GetHashCode(val));
            }

            return iHash;
        }
    }
}
