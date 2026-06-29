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

    private enum GoldApplyMode
    {
        Set,
        Add,
    }

    private int _ingredientAmount = 99;
    private IngredientApplyMode _ingredientApplyMode = IngredientApplyMode.Set;
    private bool _clampToMaxStack = true;
    private long _goldAmount = 10000;
    private GoldApplyMode _goldApplyMode = GoldApplyMode.Set;
    private Vector2 _scrollPosition;

    [MenuItem("Tools/Debug/Recipe & Ingredient Setup")]
    private static void Open()
    {
        GetWindow<RecipeIngredientSetupWindow>("Recipe & Ingredient Setup");
    }

    private void OnGUI()
    {
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

        EditorGUILayout.LabelField("플레이어 데이터 세팅", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "현재 세이브 파일의 레시피, 재료, 골드를 변경합니다.\n" +
            "플레이 중 실행하면 런타임 모델에도 즉시 반영됩니다.",
            MessageType.Info);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("레시피 설정", EditorStyles.boldLabel);
        if (GUILayout.Button("모든 레시피 해금"))
        {
            ApplySetup(true, false);
        }

        if (GUILayout.Button("모든 레시피 삭제"))
        {
            DeleteAllRecipes();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("재료 설정", EditorStyles.boldLabel);
        _ingredientApplyMode = (IngredientApplyMode)EditorGUILayout.EnumPopup("적용 방식", _ingredientApplyMode);
        _ingredientAmount = Mathf.Max(1, EditorGUILayout.IntField("지급 수량", _ingredientAmount));
        _clampToMaxStack = EditorGUILayout.Toggle("최대 보유 수량 제한", _clampToMaxStack);

        if (GUILayout.Button("모든 재료 지급"))
        {
            ApplySetup(false, true);
        }

        if (GUILayout.Button("모든 재료 삭제"))
        {
            DeleteAllIngredients();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("골드 설정", EditorStyles.boldLabel);
        _goldApplyMode = (GoldApplyMode)EditorGUILayout.EnumPopup("적용 방식", _goldApplyMode);
        _goldAmount = Math.Max(0L, EditorGUILayout.LongField("골드 수량", _goldAmount));

        if (GUILayout.Button("골드 적용"))
        {
            ApplyGold();
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("레시피 + 재료 모두 세팅", GUILayout.Height(32f)))
        {
            ApplySetup(true, true);
        }

        if (GUILayout.Button("레시피 + 재료 + 골드 모두 초기화", GUILayout.Height(32f)))
        {
            ResetAllPlayerData();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("클라이언트 저장 파일", EditorStyles.boldLabel);
        EditorGUILayout.SelectableLabel(
            SaveManager.SaveFilePath,
            EditorStyles.textField,
            GUILayout.Height(EditorGUIUtility.singleLineHeight * 2f));

        if (Application.isPlaying)
        {
            EditorGUILayout.HelpBox("저장 파일 완전 삭제는 Play Mode를 종료한 뒤 실행해주세요.", MessageType.Warning);
        }

        Color previousBackgroundColor = GUI.backgroundColor;
        GUI.backgroundColor = new Color(1f, 0.55f, 0.55f);
        using (new EditorGUI.DisabledScope(Application.isPlaying))
        {
            if (GUILayout.Button("클라이언트 저장 파일 완전 삭제", GUILayout.Height(36f)))
            {
                DeleteClientSaveFile();
            }
        }
        GUI.backgroundColor = previousBackgroundColor;

        EditorGUILayout.EndScrollView();
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

        StartInfoSet saveData = GetCurrentSaveData();

        int addedRecipeCount = setupRecipes ? SetupRecipes(saveData, recipeDatabase) : 0;
        int updatedIngredientCount = setupIngredients
            ? SetupIngredients(saveData, ingredientDatabase, inventoryItemDatabase)
            : 0;

        SaveManager.Save(saveData);
        RefreshRuntimeModels(saveData, setupRecipes, setupIngredients, false);

        string resultMessage = $"세팅 완료\n새로 해금한 레시피: {addedRecipeCount}개\n세팅한 재료: {updatedIngredientCount}개";
        Debug.Log(resultMessage);
        EditorUtility.DisplayDialog("세팅 완료", resultMessage, "확인");
    }

    private static void DeleteAllRecipes()
    {
        if (!EditorUtility.DisplayDialog(
                "모든 레시피 삭제",
                "현재 세이브 데이터의 레시피 해금 기록을 모두 삭제하시겠습니까?",
                "삭제",
                "취소"))
        {
            return;
        }

        StartInfoSet saveData = GetCurrentSaveData();
        saveData.recipeSaveInfos ??= new List<RecipeSaveInfo>();

        int deletedCount = saveData.recipeSaveInfos.Count;
        saveData.recipeSaveInfos.Clear();

        SaveManager.Save(saveData);
        RefreshRuntimeModels(saveData, true, false, false);

        string resultMessage = $"레시피 {deletedCount}개의 해금 기록을 삭제했습니다.";
        Debug.Log(resultMessage);
        EditorUtility.DisplayDialog("삭제 완료", resultMessage, "확인");
    }

    private static void DeleteAllIngredients()
    {
        if (!EditorUtility.DisplayDialog(
                "모든 재료 삭제",
                "현재 보유 중인 모든 재료의 수량을 0으로 변경하시겠습니까?",
                "삭제",
                "취소"))
        {
            return;
        }

        if (!TryLoadDatabase(IngredientDataPath, out IngredientDatabase ingredientDatabase) ||
            ingredientDatabase.rows == null)
        {
            Debug.LogError($"재료 데이터를 읽을 수 없습니다.\n{IngredientDataPath}");
            return;
        }

        StartInfoSet saveData = GetCurrentSaveData();
        int deletedCount = ClearIngredients(saveData, ingredientDatabase);

        SaveManager.Save(saveData);
        RefreshRuntimeModels(saveData, false, true, false);

        string resultMessage = $"보유 중이던 재료 {deletedCount}개의 수량을 0으로 변경했습니다.";
        Debug.Log(resultMessage);
        EditorUtility.DisplayDialog("삭제 완료", resultMessage, "확인");
    }

    private void ApplyGold()
    {
        string applyDescription = _goldApplyMode == GoldApplyMode.Add
            ? $"현재 골드에 {_goldAmount:N0}을 추가"
            : $"골드를 {_goldAmount:N0}(으)로 변경";

        if (!EditorUtility.DisplayDialog(
                "골드 설정",
                $"{applyDescription}하시겠습니까?",
                "적용",
                "취소"))
        {
            return;
        }

        StartInfoSet saveData = GetCurrentSaveData();
        saveData.playerSaveInfo ??= new PlayerSaveInfo();

        if (_goldApplyMode == GoldApplyMode.Add)
        {
            long currentGold = Math.Max(0L, saveData.playerSaveInfo.gold);
            saveData.playerSaveInfo.gold = currentGold > long.MaxValue - _goldAmount
                ? long.MaxValue
                : currentGold + _goldAmount;
        }
        else
        {
            saveData.playerSaveInfo.gold = _goldAmount;
        }

        SaveManager.Save(saveData);
        RefreshRuntimeModels(saveData, false, false, true);

        string resultMessage = $"골드를 {saveData.playerSaveInfo.gold:N0}(으)로 변경했습니다.";
        Debug.Log(resultMessage);
        EditorUtility.DisplayDialog("적용 완료", resultMessage, "확인");
    }

    private static void ResetAllPlayerData()
    {
        if (!EditorUtility.DisplayDialog(
                "플레이어 데이터 초기화",
                "모든 레시피 해금 기록과 재료를 삭제하고 골드를 0으로 변경하시겠습니까?",
                "모두 초기화",
                "취소"))
        {
            return;
        }

        if (!TryLoadDatabase(IngredientDataPath, out IngredientDatabase ingredientDatabase) ||
            ingredientDatabase.rows == null)
        {
            Debug.LogError($"재료 데이터를 읽을 수 없습니다.\n{IngredientDataPath}");
            return;
        }

        StartInfoSet saveData = GetCurrentSaveData();
        saveData.recipeSaveInfos ??= new List<RecipeSaveInfo>();
        int deletedRecipeCount = saveData.recipeSaveInfos.Count;
        saveData.recipeSaveInfos.Clear();

        int deletedIngredientCount = ClearIngredients(saveData, ingredientDatabase);
        saveData.playerSaveInfo ??= new PlayerSaveInfo();
        saveData.playerSaveInfo.gold = 0;

        SaveManager.Save(saveData);
        RefreshRuntimeModels(saveData, true, true, true);

        string resultMessage =
            $"초기화 완료\n삭제한 레시피: {deletedRecipeCount}개\n삭제한 재료: {deletedIngredientCount}개\n골드: 0";
        Debug.Log(resultMessage);
        EditorUtility.DisplayDialog("초기화 완료", resultMessage, "확인");
    }

    private static void DeleteClientSaveFile()
    {
        if (Application.isPlaying)
        {
            EditorUtility.DisplayDialog(
                "Play Mode 종료 필요",
                "Play Mode를 종료한 뒤 저장 파일을 삭제해주세요.",
                "확인");
            return;
        }

        if (!SaveManager.HasSaveFile)
        {
            EditorUtility.DisplayDialog(
                "저장 파일 없음",
                $"삭제할 저장 파일이 없습니다.\n\n{SaveManager.SaveFilePath}",
                "확인");
            return;
        }

        if (!EditorUtility.DisplayDialog(
                "클라이언트 저장 파일 완전 삭제",
                "골드, 레시피, 인벤토리, 상품, 재료 수급 정보를 포함한 모든 저장 데이터가 삭제됩니다.\n" +
                "이 작업은 되돌릴 수 없습니다.\n\n" +
                SaveManager.SaveFilePath,
                "완전 삭제",
                "취소"))
        {
            return;
        }

        if (!SaveManager.DeleteSaveFile())
        {
            EditorUtility.DisplayDialog(
                "삭제 실패",
                "저장 파일을 삭제하지 못했습니다. Console 로그를 확인해주세요.",
                "확인");
            return;
        }

        ModelCenter.StartInfoSetData = null;
        ModelCenter.ReleaseInstance();

        string resultMessage = $"클라이언트 저장 파일을 삭제했습니다.\n{SaveManager.SaveFilePath}";
        Debug.Log(resultMessage);
        EditorUtility.DisplayDialog("삭제 완료", resultMessage, "확인");
    }

    private static StartInfoSet GetCurrentSaveData()
    {
        return Application.isPlaying && ModelCenter.StartInfoSetData != null
            ? ModelCenter.StartInfoSetData
            : SaveManager.Load();
    }

    private static int ClearIngredients(StartInfoSet saveData, IngredientDatabase ingredientDatabase)
    {
        saveData.inventoryItemSaveInfos ??= new List<InventoryItemSaveInfo>();

        HashSet<int> ingredientItemIds = new();
        foreach (Ingredient ingredient in ingredientDatabase.rows)
        {
            ingredientItemIds.Add(ingredient.inventoryItemId);
        }

        HashSet<int> clearedItemIds = new();
        foreach (InventoryItemSaveInfo saveInfo in saveData.inventoryItemSaveInfos)
        {
            if (ingredientItemIds.Contains(saveInfo.id) && saveInfo.cnt != 0)
            {
                saveInfo.cnt = 0;
                clearedItemIds.Add(saveInfo.id);
            }
        }

        return clearedItemIds.Count;
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
        bool refreshIngredients,
        bool refreshPlayer)
    {
        if (!Application.isPlaying)
        {
            return;
        }

        ModelCenter.StartInfoSetData = saveData;
        if (refreshPlayer)
        {
            ModelCenter.Player.Set(saveData.playerSaveInfo);
            ModelCenter.Player.OnRefreshGold?.Invoke();
        }

        if (!refreshRecipes && !refreshIngredients)
        {
            return;
        }

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
