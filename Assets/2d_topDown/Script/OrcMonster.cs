using UnityEngine;

[ExecuteAlways]
public class OrcMonster : MonoBehaviour
{
    [SerializeField] private Texture2D idleSheet;
    [SerializeField] private Texture2D walkSheet;
    [SerializeField] private int frameWidth = 100;
    [SerializeField] private int frameHeight = 100;
    [SerializeField] private float animationFrameRate = 8f;
    [SerializeField] private float moveSpeed = 1.15f;
    [SerializeField] private float patrolRadius = 1.4f;
    [SerializeField] private float waitDuration = 1.1f;
    [SerializeField] private Vector2 shadowOffset = new Vector2(0f, -0.23f);
    [SerializeField] private Vector2 shadowScale = new Vector2(0.46f, 0.18f);
    [SerializeField] private Color shadowColor = new Color(0f, 0f, 0f, 0.38f);

    private SpriteRenderer spriteRenderer;
    private SpriteRenderer shadowRenderer;
    private Sprite[] idleFrames;
    private Sprite[] walkFrames;
    private Vector3 homePosition;
    private Vector3 targetPosition;
    private float waitTimer;
    private float animationTimer;
    private int animationFrame;
    private bool isWalking;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        spriteRenderer.sortingOrder = 9;
        idleFrames = BuildFrames(idleSheet);
        walkFrames = BuildFrames(walkSheet);
        homePosition = transform.position;
        targetPosition = homePosition;
        waitTimer = Random.Range(0f, waitDuration);

        EnsureShadow();
        ShowFirstFrame();
    }

    private void OnEnable()
    {
        if (!Application.isPlaying)
        {
            EnsurePreview();
        }
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            EnsurePreview();
            return;
        }

        if (!isWalking)
        {
            waitTimer -= Time.deltaTime;
            Animate(idleFrames);

            if (waitTimer <= 0f)
            {
                PickNewTarget();
            }

            return;
        }

        Vector3 previousPosition = transform.position;
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        Vector3 moveDelta = transform.position - previousPosition;

        if (Mathf.Abs(moveDelta.x) > 0.0001f)
        {
            spriteRenderer.flipX = moveDelta.x < 0f;
        }

        Animate(walkFrames);

        if (Vector3.Distance(transform.position, targetPosition) <= 0.03f)
        {
            isWalking = false;
            waitTimer = waitDuration + Random.Range(0f, 0.8f);
            animationFrame = 0;
            ShowFirstFrame();
        }
    }

    private void EnsurePreview()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = 9;
        }

        if (spriteRenderer.sprite == null && idleSheet != null)
        {
            idleFrames = BuildFrames(idleSheet);
            ShowFirstFrame();
        }

        if (transform.Find("Shadow") == null)
        {
            EnsureShadow();
        }
    }

    private void PickNewTarget()
    {
        Vector2 offset = Random.insideUnitCircle * patrolRadius;
        targetPosition = homePosition + new Vector3(offset.x, offset.y, 0f);
        isWalking = true;
        animationFrame = 0;
        animationTimer = 0f;
    }

    private void Animate(Sprite[] frames)
    {
        if (frames == null || frames.Length == 0)
        {
            return;
        }

        animationTimer += Time.deltaTime;
        float frameDuration = 1f / Mathf.Max(1f, animationFrameRate);

        if (animationTimer >= frameDuration)
        {
            animationTimer = 0f;
            animationFrame = (animationFrame + 1) % frames.Length;
        }

        spriteRenderer.sprite = frames[Mathf.Clamp(animationFrame, 0, frames.Length - 1)];
        spriteRenderer.color = Color.white;
    }

    private void ShowFirstFrame()
    {
        Sprite[] frames = idleFrames != null && idleFrames.Length > 0 ? idleFrames : walkFrames;

        if (frames == null || frames.Length == 0)
        {
            return;
        }

        spriteRenderer.sprite = frames[0];
        spriteRenderer.color = Color.white;
    }

    private Sprite[] BuildFrames(Texture2D sheet)
    {
        if (sheet == null || frameWidth <= 0 || frameHeight <= 0)
        {
            return System.Array.Empty<Sprite>();
        }

        int columns = Mathf.Max(1, sheet.width / frameWidth);
        int rows = Mathf.Max(1, sheet.height / frameHeight);
        Sprite[] frames = new Sprite[columns * rows];
        int index = 0;

        for (int row = rows - 1; row >= 0; row--)
        {
            for (int column = 0; column < columns; column++)
            {
                frames[index++] = Sprite.Create(
                    sheet,
                    new Rect(column * frameWidth, row * frameHeight, frameWidth, frameHeight),
                    new Vector2(0.5f, 0.5f),
                    frameWidth
                );
            }
        }

        return frames;
    }

    private void EnsureShadow()
    {
        Transform shadowTransform = transform.Find("Shadow");

        if (shadowTransform == null)
        {
            GameObject shadowObject = new GameObject("Shadow");
            shadowTransform = shadowObject.transform;
            shadowTransform.SetParent(transform);
        }

        shadowTransform.localPosition = new Vector3(shadowOffset.x, shadowOffset.y, 0.02f);
        shadowTransform.localRotation = Quaternion.identity;
        shadowTransform.localScale = new Vector3(shadowScale.x, shadowScale.y, 1f);

        shadowRenderer = shadowTransform.GetComponent<SpriteRenderer>();

        if (shadowRenderer == null)
        {
            shadowRenderer = shadowTransform.gameObject.AddComponent<SpriteRenderer>();
        }

        shadowRenderer.sprite = CreateShadowSprite();
        shadowRenderer.color = shadowColor;
        shadowRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
        shadowRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
    }

    private Sprite CreateShadowSprite()
    {
        const int width = 64;
        const int height = 32;

        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        Vector2 center = new Vector2((width - 1) * 0.5f, (height - 1) * 0.5f);
        float radiusX = width * 0.45f;
        float radiusY = height * 0.38f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float normalizedX = (x - center.x) / radiusX;
                float normalizedY = (y - center.y) / radiusY;
                float distance = normalizedX * normalizedX + normalizedY * normalizedY;
                float alpha = Mathf.Clamp01(1f - distance);
                alpha = Mathf.SmoothStep(0f, 1f, alpha);
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply();

        return Sprite.Create(
            texture,
            new Rect(0f, 0f, width, height),
            new Vector2(0.5f, 0.5f),
            width
        );
    }
}
