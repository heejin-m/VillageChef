using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoodsProperty : MonoBehaviour
{
    #region Insepctor

    public eGoodsType goodsType;
    public Image icon;
	public TMP_Text value;

    #endregion

    public void Awake()
    {
        ModelCenter.Player.OnRefreshGold += Set;

        Set();
    }

    public void OnDestroy()
    {
        ModelCenter.Player.OnRefreshGold -= Set;
    }

    public void Set()
	{
        switch (goodsType)
        {
            case eGoodsType.Gold:
                {
                    AtlasLoadManager.SetImageSprite(icon, eAtlas.CommonUI, "Coin_01");
                    var amount = ModelCenter.Player.GetGold().ToString("N0");
                    value.text = amount;
                }
                break;
            default:
                break;
        }

    }
}