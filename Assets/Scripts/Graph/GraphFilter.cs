using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DCXR.Graph
{
    public enum Comparator
    {
        LessThan,
        LessOrEqual,
        Equal,
        GreaterThan,
        GreaterThanOrEqual,
    }

    [Serializable]
    public class GraphFilter
    {
        public GroupFeature feature;
        public Comparator comparator;
        public float value;

        public GraphFilter()
        {
            feature = GroupFeature.GameplayDur;
            comparator = Comparator.GreaterThanOrEqual;
            value = 0;
        }

        public GraphFilter(GroupFeature feature, Comparator comparator, float value)
        {
            this.feature = feature;
            this.comparator = comparator;
            this.value = value;
        }

        public static bool IsLessThan<T>(T value1, float value2) where T : IComparable<T>
        {
            if (value1 is IConvertible)
            {
                float floatValue;
                try
                {
                    floatValue = Convert.ToSingle(value1);
                }
                catch (InvalidCastException)
                {
                    throw new ArgumentException("Value cannot be converted to float.");
                }
                return floatValue < value2;
            }
            else
            {
                throw new ArgumentException("Value cannot be converted to float.");
            }
        }
    }

}
