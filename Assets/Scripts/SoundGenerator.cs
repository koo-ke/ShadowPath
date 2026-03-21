using UnityEngine;

public static class SoundGenerator
{
    private const int SampleRate = 44100;

    // ─── 基本波形 ───────────────────────────────────────────────

    public static AudioClip GenerateSineWave(
        float frequency, float duration, float volume = 0.5f, bool applyFade = true)
    {
        int samples = Mathf.CeilToInt(SampleRate * duration);
        float[] data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / SampleRate;
            data[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * volume;
        }
        if (applyFade) ApplyFade(data);
        return CreateClip("SineWave", data);
    }

    public static AudioClip GenerateSquareWave(
        float frequency, float duration, float volume = 0.3f, bool applyFade = true)
    {
        int samples = Mathf.CeilToInt(SampleRate * duration);
        float[] data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / SampleRate;
            float sine = Mathf.Sin(2f * Mathf.PI * frequency * t);
            data[i] = (sine >= 0f ? 1f : -1f) * volume;
        }
        if (applyFade) ApplyFade(data);
        return CreateClip("SquareWave", data);
    }

    public static AudioClip GenerateNoise(
        float duration, float volume = 0.2f, bool applyFade = true)
    {
        int samples = Mathf.CeilToInt(SampleRate * duration);
        float[] data = new float[samples];
        var rng = new System.Random();
        for (int i = 0; i < samples; i++)
            data[i] = ((float)rng.NextDouble() * 2f - 1f) * volume;
        if (applyFade) ApplyFade(data);
        return CreateClip("Noise", data);
    }

    public static AudioClip GenerateFrequencySweep(
        float startFreq, float endFreq, float duration, float volume = 0.5f, bool applyFade = true)
    {
        int samples = Mathf.CeilToInt(SampleRate * duration);
        float[] data = new float[samples];
        float phase = 0f;
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            float freq = Mathf.Lerp(startFreq, endFreq, t);
            phase += 2f * Mathf.PI * freq / SampleRate;
            data[i] = Mathf.Sin(phase) * volume;
        }
        if (applyFade) ApplyFade(data);
        return CreateClip("FrequencySweep", data);
    }

    // ─── 合成ユーティリティ ──────────────────────────────────────

    /// <summary>複数のクリップを加算合成する。長さは最長クリップに合わせる。</summary>
    public static AudioClip Mix(params AudioClip[] clips)
    {
        int maxSamples = 0;
        foreach (var c in clips)
            if (c != null) maxSamples = Mathf.Max(maxSamples, c.samples);

        float[] data = new float[maxSamples];
        float[] buf  = new float[maxSamples];

        foreach (var c in clips)
        {
            if (c == null) continue;
            System.Array.Clear(buf, 0, maxSamples);
            c.GetData(buf, 0);
            for (int i = 0; i < c.samples; i++)
                data[i] += buf[i];
        }

        // クリッピング防止
        float peak = 0f;
        foreach (var s in data) peak = Mathf.Max(peak, Mathf.Abs(s));
        if (peak > 1f)
            for (int i = 0; i < data.Length; i++) data[i] /= peak;

        return CreateClip("Mixed", data);
    }

    // ─── 内部ヘルパー ────────────────────────────────────────────

    private static void ApplyFade(float[] data)
    {
        int len = data.Length;
        int fadeLen = Mathf.Max(1, Mathf.CeilToInt(len * 0.05f));
        for (int i = 0; i < fadeLen; i++)
        {
            float t = (float)i / fadeLen;
            data[i] *= t;
            data[len - 1 - i] *= t;
        }
    }

    private static AudioClip CreateClip(string name, float[] data)
    {
        var clip = AudioClip.Create(name, data.Length, 1, SampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }
}
