using System;
using System.Collections.Generic;

namespace Entitas {
    public static class ES {
        public static Type[] Types<T>() {
            return new [] { typeof(T) };
        }

        public static Type[] Types<T1, T2>() {
            return new [] {
                typeof(T1),
                typeof(T2)
            };
        }

        public static Type[] Types<T1, T2, T3>() {
            return new [] {
                typeof(T1),
                typeof(T2),
                typeof(T3)
            };
        }

        public static Type[] Types<T1, T2, T3, T4>() {
            return new [] {
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4)
            };
        }

        public static Type[] Types<T1, T2, T3, T4, T5>() {
            return new [] {
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5)
            };
        }

        public static Type[] Types<T1, T2, T3, T4, T5, T6>() {
            return new [] {
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6)
            };
        }

        public static Type[] Types<T1, T2, T3, T4, T5, T6, T7>() {
            return new [] {
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6),
                typeof(T7)
            };
        }

        public static Type[] Types<T1, T2, T3, T4, T5, T6, T7, T8>() {
            return new [] {
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6),
                typeof(T7),
                typeof(T8)
            };
        }

        public static Type[] Types<T1, T2, T3, T4, T5, T6, T7, T8, T9>() {
            return new [] {
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6),
                typeof(T7),
                typeof(T8),
                typeof(T9)
            };
        }
    }
}

