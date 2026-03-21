using UnityEngine;

public class Stage2PlatformSpawner : MonoBehaviour
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
        // ─── スタート地面（Y=-2）: 広めで落ち着いてスタート ───
        new PlatformData { name = "Platform_1",  x =  0f, y = -2f,  width = 8f,  height = 0.5f },

        // ─── 登り区間 下部（Y=0〜10）: 左右交互・間隔小さめ ───
        new PlatformData { name = "Platform_2",  x =  3f, y =  0f,  width = 3f,  height = 0.5f },
        new PlatformData { name = "Platform_3",  x = -3f, y =  2f,  width = 3f,  height = 0.5f },
        new PlatformData { name = "Platform_4",  x =  3f, y =  4f,  width = 3f,  height = 0.5f },
        new PlatformData { name = "Platform_5",  x = -3f, y =  6f,  width = 3f,  height = 0.5f },
        new PlatformData { name = "Platform_6",  x =  3f, y =  8f,  width = 3f,  height = 0.5f },

        // ─── 登り区間 中部（Y=11〜22）: 足場が少し小さくなる ───
        new PlatformData { name = "Platform_7",  x = -3f, y = 11f,  width = 2.5f, height = 0.5f },
        new PlatformData { name = "Platform_8",  x =  3f, y = 14f,  width = 2.5f, height = 0.5f },
        new PlatformData { name = "Platform_9",  x = -3f, y = 17f,  width = 2.5f, height = 0.5f },
        new PlatformData { name = "Platform_10", x =  3f, y = 20f,  width = 2.5f, height = 0.5f },

        // ─── 登り区間 上部（Y=24〜34）: さらに小さく・横ズレ大きめ ───
        new PlatformData { name = "Platform_11", x = -4f, y = 24f,  width = 2f,  height = 0.5f },
        new PlatformData { name = "Platform_12", x =  4f, y = 28f,  width = 2f,  height = 0.5f },
        new PlatformData { name = "Platform_13", x = -4f, y = 32f,  width = 2f,  height = 0.5f },

        // ─── ゴール台（Y=36）: 広めで安心して着地 ───
        new PlatformData { name = "Platform_14", x =  0f, y = 36f,  width = 6f,  height = 0.5f },
    };

    private void Start()
    {
        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer == -1)
        {
            Debug.LogError("[Stage2PlatformSpawner] 'Ground' layer が見つかりません。" +
                           "Project Settings > Tags and Layers で追加してください。");
            return;
        }

        foreach (var data in platforms)
        {
            SpawnPlatform(data, groundLayer);
        }
    }

    private void SpawnPlatform(PlatformData data, int layer)
    {
        GameObject platform = new GameObject(data.name);
        platform.layer = layer;
        platform.transform.SetParent(transform);
        platform.transform.position = new Vector3(data.x, data.y, 0f);
        platform.transform.localScale = new Vector3(data.width, data.height, 1f);

        SpriteRenderer sr = platform.AddComponent<SpriteRenderer>();
        sr.sprite = Sprite.Create(
            Texture2D.whiteTexture,
            new Rect(0, 0, 4, 4),
            Vector2.one * 0.5f,
            4f
        );
        sr.color = Color.black;

        BoxCollider2D col = platform.AddComponent<BoxCollider2D>();
        col.size = Vector2.one;
    }
}
