using System.Collections.Generic;

public class ProductModel : AbstractModel
{
    private Dictionary<int, ProductInfo> _productInfosById = new();
    private Dictionary<eInventoryItemType, List<ProductInfo>> _productInfosByType = new();
    private Dictionary<eInventoryItemCategory, List<ProductInfo>> _productInfosByCategory = new();

    public void Set(List<ProductSaveInfo> saveInfos)
    {
        _productInfosById.Clear();
        _productInfosByType.Clear();
        _productInfosByCategory.Clear();

        var productData = DataManager.Instance.GetData<ProductData>();
        foreach (var data in productData.Datas)
        {
            ProductSaveInfo saveInfo = saveInfos?.Find(d => d.id == data.Key);
            ProductInfo productInfo = new ProductInfo(data.Key, saveInfo);

            // _productInfosById
            if (productInfo.IsVaild)
            {
                _productInfosById.Add(data.Key, productInfo);
            }

            // _productInfosByType
            if (!_productInfosByType.TryGetValue(data.Value.ItemType, out var typeList))
            {
                typeList = new List<ProductInfo>();
                _productInfosByType.Add(data.Value.ItemType, typeList);
            }
            if (productInfo.IsVaild)
            {
                typeList.Add(productInfo);
            }

            // _productInfosByCategory
            if (!_productInfosByCategory.TryGetValue(data.Value.Category, out var categoryList))
            {
                categoryList = new List<ProductInfo>();
                _productInfosByCategory.Add(data.Value.Category, categoryList);
            }
            if (productInfo.IsVaild)
            {
                categoryList.Add(productInfo);
            }
        }
    }

    /// <summary>
    /// 타입으로 상품 리스트 가져오기
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    public List<ProductInfo> GetProductListByType(eInventoryItemType itemType)
    {
        if (_productInfosByType.TryGetValue(itemType, out var list))
        {
            return list;
        }

        return null;
    }

    /// <summary>
    /// 카테고리로 상품 리스트 가져오기
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    public List<ProductInfo> GetProductListByCategory(eInventoryItemCategory category)
    {
        if (_productInfosByCategory.TryGetValue(category, out var list))
        {
            return list;
        }

        return null;
    }

    /// <summary>
    /// 상품 구매
    /// </summary>
    public void Buy(ProductInfo info)
    {
        if (!_productInfosById.TryGetValue(info.Id, out var product)) return;

        ProductSaveInfo saveInfo = product.SaveInfo;
        saveInfo ??= new();
        saveInfo.id = info.Id;
        saveInfo.buyCnt += 1;

        SaveManager.Save(saveInfo);
        this.Set(ModelCenter.StartInfoSetData.productSaveInfo);

        ModelCenter.Player.UseGold(info.sellPrice);
        ModelCenter.Inventory.AddItem(info.InventoryItemID, info.Amount);
    }
}