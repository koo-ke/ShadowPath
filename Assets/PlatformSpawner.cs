using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct PlatformData
    {
        public string name;
        public float x, y;
        public float width, height;
    }

    [SerializeField] private PlatformData[] platforms = new PlatformData[]
    {
        // ─── 導入区間（X=0〜18）: 平坦・広い足場で操作に慣れる ───
        new PlatformData { name = "Platform_1",  x =  0f, y = -2f, width = 8f,   height = 0.5f },
        new PlatformData { name = "Platform_2",  x = 10f, y = -2f, width = 4f,   height = 0.5f },
        new PlatformData { name = "Platform_3",  x = 17f, y = -2f, width = 3f,   height = 0.5f },

        // ─── 中盤区間（X=22〜42）: 高低差でジャンプ練習 ───
        new PlatformData { name = "Platform_4",  x = 22f, y = -1f, width = 3f,   height = 0.5f },
        new PlatformData { name = "Platform_5",  x = 27f, y =  0f, width = 3f,   height = 0.5f },
        new PlatformData { name = "Platform_6",  x = 32f, y =  1f, width = 2.5f, height = 0.5f },
        new PlatformData { name = "Platform_7",  x = 37f, y = -1f, width = 2.5f, height = 0.5f },
        new PlatformData { name = "Platform_8",  x = 42f, y =  0f, width = 2.5f, height = 0.5f },

        // ─── 後半区間（X=47〜62）: 間隔広め・クイックステップ活用 ───
        new PlatformData { name = "Platform_9",  x = 47f, y = -1f, width = 2f,   height = 0.5f },
        new PlatformData { name = "Platform_10", x = 52f, y =  1f, width = 2f,   height = 0.5f },
        new PlatformData { name = "Platform_11", x = 57f, y =  0f, width = 2f,   height = 0.5f },
        new PlatformData { name = "Platform_12", x = 62f, y = -1f, width = 2f,   height = 0.5f },

        // ─── ゴール区間（X=67）: 広めの着地プラットフォーム ───
        new PlatformData { name = "Platform_13", x = 67f, y =  0f, width = 5f,   height = 0.5f },
    };

    private void Start()
    {
        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer == -1)
        {
            Debug.LogError("[PlatformSpawner] 'Ground' layer が見つかりません。" +
                           "Project Settings > Tags and Layers で追加してください。");
            return;
        }

        for (int i = 0; i < platforms.Length; i++)
        {
            SpawnPlatform(platforms[i], groundLayer);
        }

        // ─── ギミック生成 ───
        SpawnMovingPlatform(new Vector2(25f, 1f),   new Vector2(3f, 0f),  new Vector2(3f, 0.5f));
        SpawnSpike(         new Vector2(30f, -2.5f), new Vector2(2f, 0.3f));
        SpawnSpike(         new Vector2(55f, -0.5f), new Vector2(2f, 0.3f));
        SpawnCrumblingPlatform(new Vector2(45f, 0f), new Vector2(3f, 0.5f));
        SpawnBouncePad(     new Vector2(50f, -2f),   new Vector2(2f, 0.3f));
    }

    // ─── ギミック生成メソッド ───

    private void SpawnMovingPlatform(Vector2 position, Vector2 pointBOffset, Vector2 size)
    {
        GameObject obj = new GameObject("MovingPlatform");
        obj.transform.SetParent(transform);
        obj.transform.position = new Vector3(position.x, position.y, 0f);
        MovingPlatform mp = obj.AddComponent<MovingPlatform>();
        mp.Configure(pointBOffset, size);
    }

    private void SpawnSpike(Vector2 position, Vector2 size)
    {
        GameObject obj = new GameObject("SpikeTrap");
        obj.transform.SetParent(transform);
        obj.transform.position = new Vector3(position.x, position.y, 0f);
        SpikeTrap trap = obj.AddComponent<SpikeTrap>();
        trap.Configure(size.x, size.y);
    }

    private void SpawnCrumblingPlatform(Vector2 position, Vector2 size)
    {
        GameObject obj = new GameObject("CrumblingPlatform");
        obj.transform.SetParent(transform);
        obj.transform.position = new Vector3(position.x, position.y, 0f);
        CrumblingPlatform cp = obj.AddComponent<CrumblingPlatform>();
        cp.Configure(size);
    }

    private void SpawnBouncePad(Vector2 position, Vector2 size)
    {
        GameObject obj = new GameObject("BouncePad");
        obj.transform.SetParent(transform);
        obj.transform.position = new Vector3(position.x, position.y, 0f);
        BouncePad bp = obj.AddComponent<BouncePad>();
        bp.Configure(size);
    }

    private void SpawnPlatform(PlatformData data, int layer)
    {
        GameObject platform = new GameObject(data.name);
        platform.layer = layer;
        platform.transform.SetParent(transform);
        platform.transform.position = new Vector3(data.x, data.y, 0f);
        platform.transform.localScale = new Vector3(data.width, data.height, 1f);

        // SpriteRenderer（白テクスチャから1x1ユニットのSquareを生成、色は黒）
        SpriteRenderer sr = platform.AddComponent<SpriteRenderer>();
        sr.sprite = Sprite.Create(
            Texture2D.whiteTexture,
            new Rect(0, 0, 4, 4),
            Vector2.one * 0.5f,
            4f
        );
        sr.color = Color.black;

        // BoxCollider2D（size=1x1でTransformのscaleが当たり判定に反映される）
        BoxCollider2D col = platform.AddComponent<BoxCollider2D>();
        col.size = Vector2.one;
    }
}
