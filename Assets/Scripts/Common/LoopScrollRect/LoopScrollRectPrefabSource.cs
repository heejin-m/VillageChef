using System.Collections.Generic;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(LoopScrollRect))]
    public class LoopScrollRectPrefabSource : MonoBehaviour, LoopScrollPrefabSource
    {
        #region Inspector

        public GameObject listItem;

        #endregion

        private LoopScrollRect _loopScrollRect = null;
        private Stack<Transform> _pool = new Stack<Transform>();

        private void Awake()
        {
            _loopScrollRect ??= GetComponent<LoopScrollRect>();
            _loopScrollRect.prefabSource = this;
        }

        public GameObject GetObject(int index)
        {
            Transform candidate = null;
            if (_pool.Count == 0)
            {
                GameObject go = Instantiate(listItem);
                candidate = go.transform;
            }
            else
            {
                candidate = _pool.Pop();
            }
            candidate.name = $"listItem: {index}";
            candidate.gameObject.SetActive(true);
            return candidate.gameObject;
        }

        public void ReturnObject(Transform trans)
        {
            trans.gameObject.SetActive(false);
            trans.SetParent(transform, false);
            _pool.Push(trans);
        }
    }
}