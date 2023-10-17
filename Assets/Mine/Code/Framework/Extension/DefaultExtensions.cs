#if UNIRX_SUPPORT
#endif
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Mine.Code.Framework.Extension
{
    /// <summary>
    /// 확장 메소드 정의
    /// </summary>
    public static class DefaultExtensions
    {
        #region For Component

        public static T AddComponent<T>(this Component component) where T : Component
        {
            return component.gameObject.AddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            if (!component.TryGetComponent(out T result)) result = component.AddComponent<T>();

            return result;
        }

        public static bool HasComponent<T>(this Component component) where T : Component
        {
            return component.GetComponent<T>() != null;
        }

        #endregion

        #region For GameObjects

        //컴포넌트를 갖고 오고 없으면 추가해서 갖고 온다.
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            return go.TryGetComponent<T>(out var component) ? component : go.AddComponent<T>();
        }

        public static bool HasComponent<T>(this GameObject go) where T : Component
        {
            return go.GetComponent<T>() != null;
        }

        public static void DestroyChildren(this Transform trans)
        {
            for (var i = trans.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(trans.GetChild(i).gameObject);
            }
        }

        #endregion

        #region For Rigidbody

        public static void ChangeDirection(this Rigidbody rigidbody, Vector3 direction)
        {
            rigidbody.velocity = direction * rigidbody.velocity.magnitude;
        }

        #endregion

        #region For Transform

        public static void AddChildren(this Transform transform, GameObject gameObject)
        {
            gameObject.transform.parent = transform;
        }

        public static void AddChildren(this Transform transform, GameObject[] gameObjects)
        {
            Array.ForEach(gameObjects, child => child.transform.parent = transform);
        }

        public static void AddChildren(this Transform transform, Component[] components)
        {
            Array.ForEach(components, child => child.transform.parent = transform);
        }

        public static void ResetChildPositions(this Transform transform, bool recursive = false)
        {
            foreach (Transform child in transform)
            {
                child.position = Vector3.zero;

                if (recursive)
                {
                    child.ResetChildPositions(true);
                }
            }
        }

        public static void SetChildLayer(this Transform transform, string layerName, bool recursive = false)
        {
            var layer = LayerMask.NameToLayer(layerName);
            SetChildLayersHelper(transform, layer, recursive);
        }

        static void SetChildLayersHelper(Transform transform, int layer, bool recursive)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.layer = layer;

                if (recursive)
                {
                    SetChildLayersHelper(child, layer, true);
                }
            }
        }


        //transform Initialize
        public static void InitializeTransform(this Transform trans)
        {
            trans.position = Vector3.zero;
            trans.localRotation = Quaternion.identity;
            trans.localScale = Vector3.one;
        }

        //transform change X Value
        public static void SetX(this Transform transform, float x)
        {
            var position = transform.position;
            position.x = x;
            transform.position = position;
        }

        //transform change X Value
        public static void SetY(this Transform transform, float y)
        {
            var position = transform.position;
            position.y = y;
            transform.position = position;
        }

        //transform change X Value
        public static void SetZ(this Transform transform, float z)
        {
            var position = transform.position;
            position.z = z;
            transform.position = position;
        }

        public static Vector2 SetX(this Vector2 vec, float x)
        {
            return new Vector2(x, vec.y);
        }

        public static Vector2 SetY(this Vector2 vec, float y)
        {
            return new Vector2(vec.x, y);
        }

        #endregion

        #region For RectTransform

#if UNITASK_SUPPORT
        public static async UniTask Stretch(this RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            await UniTask.Yield();
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.localRotation = Quaternion.identity;
        }
#endif

        #endregion

        #region For Vector3

        public static Vector3 GetCloset(this Vector3 position, IEnumerable<Vector3> otherPosition)
        {
            var closet = Vector3.zero;
            var shortestDistance = Mathf.Infinity;

            foreach (var otherPos in otherPosition)
            {
                var distance = (position - otherPos).sqrMagnitude;

                if (distance < shortestDistance)
                {
                    closet = otherPos;
                    shortestDistance = distance;
                }
            }

            return closet;
        }

        public static Vector3 Offset(this Vector3 position, Vector3 offset) => position + offset;
        public static Vector3 XYToXZ(this Vector2 position) => new Vector3(position.x, 0, position.y);
        public static Vector3Int ToVector3Int(this Vector3 position) => new Vector3Int((int)position.x, (int)position.y, (int)position.z);
        public static Vector3 DropY(this Vector3 position) => new Vector3(position.x, 0, position.z);
        public static Vector3 DropZ(this Vector3 position) => new Vector3(position.x, position.y, 0);

        // 두 Vector3 간에 sqrMagnitude를 이용한 거리 구하기
        public static float DistanceSqr(this Vector3 position, Vector3 otherPosition) => (position - otherPosition).sqrMagnitude;

        // 두 Vector3 간에 sqrMagnitude를 이용하여 거리를 구하고 거리가 지정된 거리보다 작으면 true를 반환한다.
        public static bool IsClose(this Vector3 position, Vector3 otherPosition, float distance) => position.DistanceSqr(otherPosition) < distance * distance;

        // Vector3의 y 값을 새로운 값으로 대체
        public static Vector3 SetY(this Vector3 position, float y) => new Vector3(position.x, y, position.z);
        
        // Vector3의 값이 주어진 범위 내에 있는지 확인
        public static bool IsInRange(this Vector3 position, Vector3 min, Vector3 max) 
            => position.x >= min.x && position.x <= max.x && position.y >= min.y && position.y <= max.y && position.z >= min.z && position.z <= max.z;
        
        // 주어진 두 Vector3 사이의 랜덤한 위치
        public static Vector3 RandomPosition(this Vector3 position, Vector3 otherPosition)
        {
            var x = Random.Range(position.x, otherPosition.x);
            var y = Random.Range(position.y, otherPosition.y);
            var z = Random.Range(position.z, otherPosition.z);
            return new Vector3(x, y, z);
        }
        
        // 주어진 두 Vector2 사이의 랜덤한 위치
        public static Vector2 RandomPosition(this Vector2 position, Vector2 otherPosition)
        {
            var x = Random.Range(position.x, otherPosition.x);
            var y = Random.Range(position.y, otherPosition.y);
            return new Vector2(x, y);
        }

        #endregion

        #region For Animator

        /// <summary>
        /// 현재 재생중인 애니메이션이 종료하였는가?
        /// </summary>
        /// <param name="self">애니메이터 자신</param>
        /// <returns>애니메이션 종료되었는지 여부</returns>
        public static bool IsCompleted(this Animator self)
        {
            return self.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f - self.GetAnimatorTransitionInfo(0).duration;
        }

        /// <summary>
        /// 현재 재생중인 애니메이션이 지정한 스테이트에서 종료되었는지 확인
        /// </summary>
        /// <param name="self">애니메이터 자신</param>
        /// <param name="stateHash">설정 스테이트의 해쉬 </param>
        /// <returns>지정된 해쉬 도달 여부</returns>
        public static bool IsCompleted(this Animator self, int stateHash)
        {
            return self.GetCurrentAnimatorStateInfo(0).shortNameHash == stateHash && self.IsCompleted();
        }

        /// <summary>
        /// 현재 재생중인 애니메이션 지정비율을 지나쳤는가? normalizeTime이기 떄문에
        /// 비율로 생각해야함.
        /// </summary>
        /// <param name="self">애니메이터 자신</param>
        /// <param name="normalizeTime">지정 비율 시간</param>
        /// <returns>애니메이션이 현재 지정된 구간을 지나가는지 여부</returns>
        public static bool IsPassed(this Animator self, float normalizeTime)
        {
            return self.GetCurrentAnimatorStateInfo(0).normalizedTime > normalizeTime;
        }

        /// <summary>
        /// 애니메이션을 최초부터 재생
        /// </summary>
        /// <param name="self">애니메이터 자신</param>
        /// <param name="shortNameHash">애니메이션의 해쉬</param>
        public static void PlayBegin(this Animator self, int shortNameHash)
        {
            self.Play(shortNameHash, 0, 0.0f);
        }

        #endregion

        #region For Choose Or Shuffle

        public static T GetRandomItem<T>(this IList<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            for (var i = list.Count - 1; i > 1; i--)
            {
                var j = Random.Range(0, i + 1);
                (list[j], list[i]) = (list[i], list[j]);
            }
        }

        public static int Choose(this float[] probs)
        {
            float total = 0;

            foreach (float elem in probs)
            {
                total += elem;
            }

            float randomPoint = Random.value * total;

            for (int i = 0; i < probs.Length; i++)
            {
                if (randomPoint < probs[i])
                {
                    return i;
                }
                else
                {
                    randomPoint -= probs[i];
                }
            }

            return probs.Length - 1;
        }

        #endregion
    }
}