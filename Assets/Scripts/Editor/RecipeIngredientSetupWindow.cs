using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RecipeIngredientSetupWindow : EditorWindow
{
    private const string RecipeDataPath = "Assets/AddressableAssets/Json/RecipeData.json";
    private const string IngredientDataPath = "Assets/AddressableAssets/Json/IngredientData.json";
    private const string InventoryItemDataPath = "Assets/AddressableAssets/Json/InventoryItemData.json";

    private enum IngredientApplyMode
    {
        Set,
        Add,
    }

    private int _ingredientAmount = 99;
    private IngredientApplyMode _ingredientApplyMode = IngredientApplyMode.Set;
    private bool _clampToMaxStack = true;

    [MenuItem("Tools/Debug/Recipe & Ingredient Setup")]
    private static void Open()
    {
        GetWindow<RecipeIngredientSetupWindow>("Recipe & Ingredient Setup");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("전체 레시피 / 재료 세팅", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "현재 세이브 파일에 전체 레시피를 해금하고 모든 재료를 지급합니다.\n" +
            "플레이 중 실행하면 런타임 모델에도 즉시 반영됩니다.",
            MessageType.Info);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("재료 설정", EditorStyles.boldLabel);
        _ingredientApplyMode = (IngredientApplyMode)EditorGUILayout.EnumPopup("적용 방식", _ingredientApplyMode);
        _ingredientAmount = Mathf.Max(1, EditorGUILayout.IntField("지급 수량", _ingredientAmount));
        _clampToMaxStack = EditorGUILayout.Toggle("최대 보유 수량 제한", _clampToMaxStack);

        EditorGUILayout.Space();

        if (GUILayout.Button("모든 레시피 해금"))
        {
            ApplySetup(true, false);
        }

        if (GUILayout.Button("모든 재료 지급"))
        {
            ApplySetup(false, true);
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("레시피 + 재료 모두 세팅", GUILayout.Height(32f)))
        {
            ApplySetup(true, true);
        }
    }

    private void ApplySetup(bool setupRecipes, bool setupIngredients)
    {
        string targetDescription = setupRecipes && setupIngredients
            ? "모든 레시피와 재료"
            : setupRecipes ? "모든 레시피" : "모든 재료";

        if (!EditorUtility.DisplayDialog(
                "세이브 데이터 변경",
                $"현재 세이브 데이터에 {targetDescription}를 세팅하시겠습니까?",
                "세팅",
                "취소"))
        {
            return;
        }

        RecipeDatabase recipeDatabase = null;
        IngredientDatabase ingredientDatabase = null;
        InventoryItemDatabase inventoryItemDatabase = null;

        if (setupRecipes && !TryLoadDatabase(RecipeDataPath, out recipeDatabase))
        {
            return;
        }

        if (setupRecipes && recipeDatabase.rows == null)
        {
            Debug.LogError($"레시피 데이터가 비어 있습니다.\n{RecipeDataPath}");
            return;
        }

        if (setupIngredients &&
            (!TryLoadDatabase(IngredientDataPath, out ingredientDatabase) ||
             !TryLoadDatabase(InventoryItemDataPath, out inventoryItemDatabase)))
        {
            return;
        }

        if (setupIngredients &&
            (ingredientDatabase.rows == null || inventoryItemDatabase.rows == null))
        {
            Debug.LogError("재료 또는 인벤토리 아이템 데이터가 비어 있습니다.");
            return;
        }

        StartInfoSet saveData = Application.isPlaying && ModelCenter.StartInfoSetData != null
            ? ModelCenter.StartInfoSetData
            : SaveManager.Load();

        int addedRecipeCount = setupRecipes ? SetupRecipes(saveData, recipeDatabase) : 0;
        int updatedIngredientCount = setupIngredients
            ? SetupIngredients(saveData, ingredientDatabase, inventoryItemDatabase)
            : 0;

        SaveManager.Save(saveData);
        RefreshRuntimeModels(saveData, setupRecipes, setupIngredients);

        string resultMessage = $"세팅 완료\n새로 해금한 레시피: {addedRecipeCount}개\n세팅한 재료: {updatedIngredientCount}개";
        Debug.Log(resultMessage);
        EditorUtility.DisplayDialog("세팅 완료", resultMessage, "확인");
    }

    private static int SetupRecipes(StartInfoSet saveData, RecipeDatabase database)
    {
        saveData.recipeSaveInfos ??= new List<RecipeSaveInfo>();

        HashSet<int> ownedRecipeIds = new();
        foreach (RecipeSaveInfo saveInfo in saveData.recipeSaveInfos)
        {
            ownedRecipeIds.Add(saveInfo.id);
        }

        int addedCount = 0;
        foreach (Recipe recipe in database.rows)
        {
            if (!ownedRecipeIds.Add(recipe.id))
            {
                continue;
            }

            saveData.recipeSaveInfos.Add(new RecipeSaveInfo { id = recipe.id });
            addedCount++;
        }

        return addedCount;
    }

    private int SetupIngredients(
        StartInfoSet saveData,
        IngredientDatabase ingredientDatabase,
        InventoryItemDatabase inventoryItemDatabase)
    {
        saveData.inventoryItemSaveInfos ??= new List<InventoryItemSaveInfo>();

        Dictionary<int, InventoryItemSaveInfo> saveInfoById = new();
        foreach (InventoryItemSaveInfo saveInfo in saveData.inventoryItemSaveInfos)
        {
            saveInfoById.TryAdd(saveInfo.id, saveInfo);
        }

        Dictionary<int, int> maxStackById = new();
        foreach (InventoryItem item in inventoryItemDatabase.rows)
        {
            maxStackById[item.id] = item.maxStack;
        }

        HashSet<int> processedInventoryItemIds = new();
        foreach (Ingredient ingredient in ingredientDatabase.rows)
        {
            int itemId = ingredient.inventoryItemId;
            if (!processedInventoryItemIds.Add(itemId))
            {
                continue;
            }

            if (!saveInfoById.TryGetValue(itemId, out InventoryItemSaveInfo saveInfo))
            {
                saveInfo = new InventoryItemSaveInfo { id = itemId };
                saveData.inventoryItemSaveInfos.Add(saveInfo);
                saveInfoById.Add(itemId, saveInfo);
            }

            long amount = _ingredientApplyMode == IngredientApplyMode.Add
                ? (long)saveInfo.cnt + _ingredientAmount
                : _ingredientAmount;

            int maxAmount = int.MaxValue;
            if (_clampToMaxStack && maxStackById.TryGetValue(itemId, out int maxStack) && maxStack > 0)
            {
                maxAmount = maxStack;
            }

            saveInfo.cnt = (int)Math.Max(0L, Math.Min(amount, maxAmount));
        }

        return processedInventoryItemIds.Count;
    }

    private static bool TryLoadDatabase<T>(string assetPath, out T database) where T : class
    {
        database = null;

        TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
        if (textAsset == null)
        {
            Debug.LogError($"데이터 파일을 찾을 수 없습니다.\n{assetPath}");
            return false;
        }

        database = JsonUtility.FromJson<T>(textAsset.text);
        if (database == null)
        {
            Debug.LogError($"데이터 파일을 읽을 수 없습니다.\n{assetPath}");
            return false;
        }

        return true;
    }

    private static void RefreshRuntimeModels(
        StartInfoSet saveData,
        bool refreshRecipes,
        bool refreshIngredients)
    {
        if (!Application.isPlaying)
        {
            return;
        }

        ModelCenter.StartInfoSetData = saveData;
        if (!DataManager.IsLive)
        {
            Debug.LogWarning("세이브 데이터는 변경했지만 DataManager가 초기화되지 않아 런타임 모델은 갱신하지 않았습니다.");
            return;
        }

        if (refreshRecipes && DataManager.Instance.GetData<RecipeData>()?.Datas != null)
        {
            ModelCenter.Recipe.Set(saveData.recipeSaveInfos);
        }

        if (refreshIngredients && DataManager.Instance.GetData<InventoryItemData>()?.Datas != null)
        {
            ModelCenter.Inventory.Set(saveData.inventoryItemSaveInfos);
        }
    }
}
