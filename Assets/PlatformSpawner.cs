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
        new PlatformData { name = "Platform_1", x =  5f, y = -1f, width = 3f, height = 0.5f },
        new PlatformData { name = "Platform_2", x = 10f, y =  0f, width = 3f, height = 0.5f },
        new PlatformData { name = "Platform_3", x = 15f, y = -2f, width = 3f, height = 0.5f },
        new PlatformData { name = "Platform_4", x = 20f, y =  1f, width = 3f, height = 0.5f },
        new PlatformData { name = "Platform_5", x = 27f, y = -1f, width = 4f, height = 0.5f },
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
