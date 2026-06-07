using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform), typeof(Graphic)), DisallowMultipleComponent]
public class ImageExtension : BaseMeshEffect
{
    [SerializeField] private bool m_Horizontal = false;
    [SerializeField] private bool m_Veritical = false;

    public bool Horizontal
    {
        get { return this.m_Horizontal; }
        set { this.m_Horizontal = value; }
    }

    public bool Vertical
    {
        get { return this.m_Veritical; }
        set { this.m_Veritical = value; }
    }

    private RectTransform _mRect;

    private RectTransform MyRectTransform
    {
        get
        {
            if (_mRect == null)
            {
                _mRect = GetComponent<RectTransform>();
            }

            return _mRect;
        }
    }

    private BoxCollider _mCollider;

    private BoxCollider MyCollider
    {
        get
        {
            if (_mCollider == null)
            {
                _mCollider = GetComponent<BoxCollider>();
            }

            return _mCollider;
        }
    }

    public bool autoResizeBoxCollider = false;

    protected override void Awake()
    {
        AutoResizeBoxCollider();
    }

    public void AutoResizeBoxCollider()
    {
        if (autoResizeBoxCollider && MyCollider != null && MyRectTransform != null)
        {
            MyCollider.size = new Vector3(MyRectTransform.sizeDelta.x, MyRectTransform.sizeDelta.y, MyCollider.size.z);
        }
    }

    public override void ModifyMesh(VertexHelper verts)
    {
        RectTransform rt = this.transform as RectTransform;

        for (int i = 0; i < verts.currentVertCount; ++i)
        {
            UIVertex uiVertex = new UIVertex();
            verts.PopulateUIVertex(ref uiVertex, i);

            // Modify positions
            uiVertex.position = new Vector3(
                (this.m_Horizontal ? (uiVertex.position.x + (rt.rect.center.x - uiVertex.position.x) * 2) : uiVertex.position.x),
                (this.m_Veritical ? (uiVertex.position.y + (rt.rect.center.y - uiVertex.position.y) * 2) : uiVertex.position.y),
                uiVertex.position.z
            );

            // Apply
            verts.SetUIVertex(uiVertex, i);
        }
    }

#if UNITY_EDITOR

    public void LateUpdate()
    {
        if (MyRectTransform.hasChanged)
        {
            AutoResizeBoxCollider();
        }
    }

    protected override void OnValidate()
    {
        var components = gameObject.GetComponents(typeof(BaseMeshEffect));
        foreach (var comp in components)
        {
            if (comp.GetType() != typeof(ImageExtension))
            {
                UnityEditorInternal.ComponentUtility.MoveComponentUp(this);
            }
            else break;
        }
        this.GetComponent<Graphic>().SetVerticesDirty();
        base.OnValidate();
    }

#endif
}