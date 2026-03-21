using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioClip titleBGM;
    [SerializeField] private AudioClip stageBGM;

    private AudioSource bgmSource;
    private AudioSource seSource;

    private Dictionary<string, AudioClip> bgmClips = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> seClips  = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.clip = null;

        seSource = gameObject.AddComponent<AudioSource>();
        seSource.loop = false;
        seSource.playOnAwake = false;
    }

    private void Start()
    {
        GenerateAllClips();
        SceneManager.sceneLoaded += OnSceneLoaded;

        // 起動シーンのBGMを再生
        PlayBGMForScene(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ─── BGM / SE 公開API ────────────────────────────────────────

    public void PlayBGM(string name)
    {
        if (!bgmClips.TryGetValue(name, out var clip))
        {
            Debug.LogWarning($"[AudioManager] BGM '{name}' が見つかりません。");
            return;
        }
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
        bgmSource.clip = null;
    }

    public void PlaySE(string name)
    {
        if (!seClips.TryGetValue(name, out var clip))
        {
            Debug.LogWarning($"[AudioManager] SE '{name}' が見つかりません。");
            return;
        }
        seSource.PlayOneShot(clip);
    }

    // ─── シーン遷移時のBGM自動切替 ──────────────────────────────

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBGMForScene(scene.name);
    }

    private void PlayBGMForScene(string sceneName)
    {
        PlayBGM(sceneName == "TitleScene" ? "title" : "stage");
    }

    // ─── クリップ生成 ────────────────────────────────────────────

    private void GenerateAllClips()
    {
        // ── BGM（SerializeFieldから登録） ──
        if (titleBGM != null) bgmClips["title"] = titleBGM;
        if (stageBGM != null) bgmClips["stage"] = stageBGM;

        // ── SE ──
        // jumpSE: 300Hz→600Hz 上昇スイープ
        seClips["jump"] = SoundGenerator.GenerateFrequencySweep(300f, 600f, 0.1f, 0.3f);

        // landSE: ホワイトノイズ 軽い着地音
        seClips["land"] = SoundGenerator.GenerateNoise(0.05f, 0.15f);

        // deathSE: 400Hz→80Hz 下降スイープ
        seClips["death"] = SoundGenerator.GenerateFrequencySweep(400f, 80f, 0.4f, 0.4f);

        // goalSE: 上昇スイープ + 800Hz正弦波 混合（達成感）
        var goalSweep = SoundGenerator.GenerateFrequencySweep(400f, 1200f, 0.5f, 0.35f);
        var goalTone  = SoundGenerator.GenerateSineWave(800f, 0.3f, 0.25f);
        seClips["goal"] = SoundGenerator.Mix(goalSweep, goalTone);

        // quickStepSE: ノイズ + 上昇スイープ 混合
        var qsNoise = SoundGenerator.GenerateNoise(0.08f, 0.20f);
        var qsSweep = SoundGenerator.GenerateFrequencySweep(200f, 400f, 0.05f, 0.15f);
        seClips["quickstep"] = SoundGenerator.Mix(qsNoise, qsSweep);

        // bounceSE: 200Hz→800Hz 上昇スイープ
        seClips["bounce"] = SoundGenerator.GenerateFrequencySweep(200f, 800f, 0.15f, 0.35f);

        // crumbleSE: ホワイトノイズ（低め音量）
        seClips["crumble"] = SoundGenerator.GenerateNoise(0.3f, 0.2f);

        Debug.Log($"[AudioManager] {bgmClips.Count} BGM / {seClips.Count} SE を生成しました。");
    }
}
