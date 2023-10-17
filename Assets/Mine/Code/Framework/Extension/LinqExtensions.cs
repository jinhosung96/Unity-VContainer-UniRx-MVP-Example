using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mine.Code.Framework.Extension
{
    public static class LinqExtensions
    {
        #region For Linq

        // 모든 요소에 접근하는 기능을 Linq에 추가
        public static IEnumerable<TSource> ForEach<TSource>(this IEnumerable<TSource> collection, Action<TSource> action)
        {
            for (int i = collection.Count() - 1; i >= 0; i--)
            {
                action(collection.ElementAtOrDefault(i));
            }

            return collection;
        }

        // 모든 요소에 접근 하는 기능을 Linq에 추가(index 포함)
        public static IEnumerable<TSource> ForEach<TSource>(this IEnumerable<TSource> collection, Action<TSource, int> action)
        {
            for (int i = 0; i < collection.Count(); i++)
            {
                action(collection.ElementAtOrDefault(i), i);
            }

            return collection;
        }

        // 랜덤으로 하나 선택하는 기능을 Linq에 추가
        public static TSource Choose<TSource>(this IEnumerable<TSource> collection)
        {
            return collection.ElementAtOrDefault(Random.Range(0, collection.Count()));
        }

        // 가중치 배열을 활용해 랜덤으로 하나 선택하는 기능을 Linq에 추가
        public static TSource Choose<TSource>(this IEnumerable<TSource> collection, params float[] weights)
        {
            collection = collection.Take(weights.Length);
            var total = weights.Take(collection.Count()).Sum();
            var rand = Random.Range(0.0f, total);

            for (int i = 0; i < collection.Count(); i++)
            {
                if (rand < weights[i]) return collection.ElementAtOrDefault(i);
                else rand -= weights[i];
            }

            return collection.LastOrDefault();
        }

        // Animation Curve를 활용해 가중치 배열을 만들어 랜덤으로 하나 선택하는 기능을 Linq에 추가
        public static TSource Choose<TSource>(this IEnumerable<TSource> collection, AnimationCurve curve)
        {
            var weights = new float[collection.Count()];
            for (int i = 0; i < collection.Count(); i++)
            {
                weights[i] = curve.Evaluate(i / (float)collection.Count());
            }

            return collection.Choose(weights);
        }

        // 중복 없이 여러개의 요소를 랜덤으로 선택하는 기능을 Linq에 추가
        public static IEnumerable<TSource> ChooseSet<TSource>(this IEnumerable<TSource> collection, int count)
        {
            for (int numLeft = collection.Count(); numLeft > 0; numLeft--)
            {
                float prob = (float)count / (float)numLeft;

                if (Random.value <= prob)
                {
                    count--;
                    yield return collection.ElementAtOrDefault(numLeft - 1);

                    if (count == 0) break;
                }
            }
        }

        // 랜덤으로 섞기 기능을 Linq에 추가
        public static IEnumerable<TSource> Shuffle<TSource>(this IEnumerable<TSource> collection)
        {
            var list = collection.ToList();
            for (int i = 0; i < list.Count(); i++)
            {
                int j = Random.Range(0, list.Count());
                yield return list[j];
                list[j] = list[i];
            }
        }

        // 가장 가까운 오브젝트 찾기 기능을 Linq에 추가
        public static TSource Closest<TSource>(this IEnumerable<TSource> collection, Vector3 position) where TSource : MonoBehaviour
        {
            var closest = collection.FirstOrDefault();
            float closestDistance = float.MaxValue;
            foreach (var item in collection)
            {
                float distance = Vector3.Distance(position, item.transform.position);
                if (distance < closestDistance)
                {
                    closest = item;
                    closestDistance = distance;
                }
            }

            return closest;
        }

        // Add element to collection using Linq
        // 요소를 컬렉션에 추가하는 기능을 Linq에 추가
        public static IEnumerable<TSource> Add<TSource>(this IEnumerable<TSource> collection, TSource element)
        {
            return collection.Concat(new[] { element });
        }

        // Print all elements of collection using Linq
        // 컬렉션에 대하여 모든 요소를 한 줄로 출력하는 기능을 Linq에 추가
        public static void PrintCollection<TSource>(this IEnumerable<TSource> collection)
        {
            Debug.Log(string.Join(", ", collection.Select(x => x.ToString())));
        }

        // 컬렉션에 대하여 주어진 기준에 따른 가장 큰 요소를 반환하는 기능을 Linq에 추가
        public static TSource MaxBy<TSource, TResult>(this IEnumerable<TSource> collection, Func<TSource, TResult> selector) where TResult : IComparable<TResult>
        {
            if (!collection.Any()) return default;
            return collection.Aggregate((x, y) => selector(x).CompareTo(selector(y)) > 0 ? x : y);
        }

        // 컬렉션에 대하여 주어진 기준에 따른 가장 작은 요소를 반환하는 기능을 Linq에 추가
        public static TSource MinBy<TSource, TResult>(this IEnumerable<TSource> collection, Func<TSource, TResult> selector) where TResult : IComparable<TResult>
        {
            if (!collection.Any()) return default;
            return collection.Aggregate((x, y) => selector(x).CompareTo(selector(y)) < 0 ? x : y);
        }

        #endregion
    }
}
