using System.Collections.Generic;

namespace UnityEngine.UI
{
    public abstract class LoopScrollRectCustom : LoopScrollRect, LoopScrollDataSource, LoopScrollPrefabSource
    {
        public enum ePivot
        {
            Start,
            Center,
            End,
        }

        #region Inspector

        public GameObject listItem;

        #endregion

        public delegate void OnProvideDataEvent(Transform transform, int idx);
        private OnProvideDataEvent _onProvideData = null;
        public OnProvideDataEvent OnProvideData
        {
            get
            {
                return _onProvideData;
            }
            set
            {
                dataSource = this;
                prefabSource = this;
                _onProvideData = value;
            }
        }

        private Stack<Transform> _pool = new Stack<Transform>();

        //protected override void Awake()
        //{
        //    dataSource = this;
        //    prefabSource = this;
        //    base.Awake();
        //}

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

        void LoopScrollDataSource.ProvideData(Transform transform, int idx)
        {
            this.OnProvideData?.Invoke(transform, idx);
        }
    }
}