using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public Image gauge;

    private void Awake()
    {
        LoginProcess();
    }

    public void LoginProcess()
    {
        StopAllCoroutines();

        gauge.fillAmount = 0f;

        StartCoroutine(LoginRoutine());
    }

    private IEnumerator LoginRoutine()
    {
        float minLoadingTime = 2f;
        float startTime = Time.time;

        bool isFinished = false;
        bool isSuccess = false;

        StartCoroutine(GaugeRoutine(minLoadingTime));

        WaterfallProcess waterfall = new();

        waterfall.Add(GetStartInfoSet);

        waterfall.Start(result =>
        {
            isSuccess = result;
            isFinished = true;
        });

        yield return new WaitUntil(() => isFinished);

        float elapsed = Time.time - startTime;

        if (elapsed < minLoadingTime)
        {
            yield return new WaitForSeconds(minLoadingTime - elapsed);
        }

        if (!isSuccess)
        {
            Debug.LogError("로그인 실패");
            yield break;
        }

        SceneManager.LoadScene("Home", LoadSceneMode.Single);
    }

    private IEnumerator GaugeRoutine(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            gauge.fillAmount = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        gauge.fillAmount = 1f;
    }

    private void GetStartInfoSet(Action<bool> onFinished)
    {
        StartInfoSet data = SaveManager.Load();
        ModelCenter.Recipe.Set(data.recipeInfos);

        onFinished?.Invoke(true);
    }
}