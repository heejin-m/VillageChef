using System;

public class LoginManager
{
    public void LoginProcess()
    {
        WaterfallProcess waterfall = new();
        waterfall.Add(GetStartInfoSet);
        waterfall.Start((result) =>
        {
            if (result)
            {
                // 로그인 완료 => 씬 전환
            }
        });
    }

    private void GetStartInfoSet(Action<bool> onFinished)
    {
        StartInfoSet data = SaveManager.Load();
        ModelCenter.Recipe.Set(data.recipeInfos);

        onFinished?.Invoke(true);
    }
}