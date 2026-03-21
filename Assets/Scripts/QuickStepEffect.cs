using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class QuickStepEffect : MonoBehaviour
{
    private ParticleSystem ps;

    private void Awake()
    {
        ps = BuildParticleSystem();
        GetComponent<PlayerController>().OnQuickStep += PlayEffect;
    }

    private void OnDestroy()
    {
        GetComponent<PlayerController>().OnQuickStep -= PlayEffect;
    }

    private void PlayEffect(float direction)
    {
        // 移動方向と逆向きにパーティクルを放出
        var vel = ps.velocityOverLifetime;
        vel.x = new ParticleSystem.MinMaxCurve(-direction * 6f, -direction * 3f);

        ps.Play();
    }

    private ParticleSystem BuildParticleSystem()
    {
        GameObject obj = new GameObject("QuickStepParticles");
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero;

        ParticleSystem p = obj.AddComponent<ParticleSystem>();

        // Main
        var main = p.main;
        main.loop = false;
        main.playOnAwake = false;
        main.startLifetime = 0.2f;
        main.startSpeed = 0f;          // 速度はvelocityOverLifetimeで制御
        main.startSize = new ParticleSystem.MinMaxCurve(0.08f, 0.22f);
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(0.04f, 0.04f, 0.08f, 1f),   // ほぼ黒・青み
            new Color(0.10f, 0.10f, 0.18f, 1f)
        );
        main.maxParticles = 60;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        // Emission（バースト：1回で25〜35粒）
        var emission = p.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new ParticleSystem.Burst[]
        {
            new ParticleSystem.Burst(0f, 25, 35),
        });

        // Shape（点から放出）
        var shape = p.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.15f;
        shape.radiusThickness = 1f;

        // VelocityOverLifetime（X方向は PlayEffect で上書き、Y は軽く上方向）
        var vel = p.velocityOverLifetime;
        vel.enabled = true;
        vel.space = ParticleSystemSimulationSpace.World;
        vel.x = new ParticleSystem.MinMaxCurve(0f);  // PlayEffect で上書き
        vel.y = new ParticleSystem.MinMaxCurve(0.5f, 2.0f);

        // ColorOverLifetime（末尾で透明に）
        var col = p.colorOverLifetime;
        col.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        col.color = new ParticleSystem.MinMaxGradient(grad);

        // SizeOverLifetime（徐々に縮む）
        var size = p.sizeOverLifetime;
        size.enabled = true;
        AnimationCurve curve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
        size.size = new ParticleSystem.MinMaxCurve(1f, curve);

        // Renderer（デフォルトのパーティクルマテリアルを使用）
        var renderer = p.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = 15;

        return p;
    }
}
