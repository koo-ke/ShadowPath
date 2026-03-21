using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    [SerializeField] private string nextScene = "";
    private const string FallbackScene = "TitleScene";

    private void Start()
    {
        GoalPoint goal = FindFirstObjectByType<GoalPoint>();
        if (goal != null)
        {
            goal.OnCleared += OnStageCleared;
        }
        else
        {
            Debug.LogWarning("[StageManager] GoalPoint が見つかりません。");
        }
    }

    private void OnStageCleared()
    {
        string target = !string.IsNullOrEmpty(nextScene) && SceneExistsInBuild(nextScene)
            ? nextScene
            : FallbackScene;
        Debug.Log($"[StageManager] シーン遷移 → {target}");
        SceneManager.LoadScene(target);
    }

    private static bool SceneExistsInBuild(string sceneName)
    {
        return Application.CanStreamedLevelBeLoaded(sceneName);
    }
}
