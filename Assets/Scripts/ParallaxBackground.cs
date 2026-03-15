using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private struct LayerData
    {
        public Color color;
        public float parallaxSpeed;
        public float positionY;
        public int sortingOrder;
        // シルエット生成パラメータ
        public float minWidth, maxWidth;
        public float minHeight, maxHeight;
        public int count;
        public float xRange;
        public int seed;
    }

    private LayerData[] layers;
    private Transform cameraTransform;
    private Transform[] layerTransforms;

    private void Start()
    {
        layers = new LayerData[]
        {
            new LayerData  // Layer 0: 遠い山
            {
                color         = new Color(0x0a / 255f, 0x0a / 255f, 0x0f / 255f),
                parallaxSpeed = 0.95f,
                positionY     = 0f,
                sortingOrder  = -30,
                minWidth = 3f, maxWidth = 8f,
                minHeight = 2f, maxHeight = 5f,
                count = 10, xRange = 40f, seed = 100,
            },
            new LayerData  // Layer 1: 遠い木
            {
                color         = new Color(0x0f / 255f, 0x15 / 255f, 0x20 / 255f),
                parallaxSpeed = 0.7f,
                positionY     = -1f,
                sortingOrder  = -20,
                minWidth = 0.5f, maxWidth = 1.5f,
                minHeight = 2f, maxHeight = 6f,
                count = 20, xRange = 40f, seed = 200,
            },
            new LayerData  // Layer 2: 近い木
            {
                color         = new Color(0x1a / 255f, 0x20 / 255f, 0x30 / 255f),
                parallaxSpeed = 0.4f,
                positionY     = -2f,
                sortingOrder  = -10,
                minWidth = 0.3f, maxWidth = 0.8f,
                minHeight = 3f, maxHeight = 8f,
                count = 25, xRange = 40f, seed = 300,
            },
        };

        cameraTransform = Camera.main.transform;
        Camera.main.backgroundColor = new Color(0x07 / 255f, 0x0a / 255f, 0x10 / 255f);
        Camera.main.clearFlags = CameraClearFlags.SolidColor;

        layerTransforms = new Transform[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            layerTransforms[i] = CreateSilhouetteLayer(i);
        }
    }

    private void LateUpdate()
    {
        for (int i = 0; i < layerTransforms.Length; i++)
        {
            float newX = cameraTransform.position.x * (1f - layers[i].parallaxSpeed);
            layerTransforms[i].position = new Vector3(
                newX,
                layers[i].positionY,
                i + 1
            );
        }
    }

    private Transform CreateSilhouetteLayer(int index)
    {
        LayerData data = layers[index];

        GameObject parent = new GameObject($"ParallaxLayer_{index}");
        parent.transform.SetParent(transform);
        parent.transform.localScale = Vector3.one;

        Random.InitState(data.seed);

        Sprite squareSprite = Sprite.Create(
            Texture2D.whiteTexture,
            new Rect(0, 0, 4, 4),
            Vector2.one * 0.5f,
            4f
        );

        for (int i = 0; i < data.count; i++)
        {
            float w = Random.Range(data.minWidth, data.maxWidth);
            float h = Random.Range(data.minHeight, data.maxHeight);
            float x = Random.Range(-data.xRange, data.xRange);
            float y = -8f + h * 0.5f;  // 根元をY=-8に揃える

            GameObject obj = new GameObject($"Shape_{i}");
            obj.transform.SetParent(parent.transform);
            obj.transform.localPosition = new Vector3(x, y, 0f);
            obj.transform.localScale = new Vector3(w, h, 1f);

            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = squareSprite;
            sr.color = data.color;
            sr.sortingOrder = data.sortingOrder;
        }

        return parent.transform;
    }
}
