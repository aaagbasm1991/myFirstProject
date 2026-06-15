using UnityEngine;
using UnityEngine.InputSystem;

public class ClickToMoveSquare : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float stopDistance = 0.03f;
    [SerializeField] private float animationFrameRate = 10f;
    [SerializeField] private Color squareColor = new Color(0.1f, 0.85f, 0.95f, 1f);
    [SerializeField] private Vector2 shadowOffset = new Vector2(0f, -0.2f);
    [SerializeField] private Vector2 shadowScale = new Vector2(0.42f, 0.16f);
    [SerializeField] private Color shadowColor = new Color(0f, 0f, 0f, 0.35f);
    [SerializeField] private Sprite[] idleDownFrames;
    [SerializeField] private Sprite[] idleUpFrames;
    [SerializeField] private Sprite[] idleLeftFrames;
    [SerializeField] private Sprite[] idleRightFrames;
    [SerializeField] private Sprite[] runDownFrames;
    [SerializeField] private Sprite[] runUpFrames;
    [SerializeField] private Sprite[] runLeftFrames;
    [SerializeField] private Sprite[] runRightFrames;

    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer shadowRenderer;
    private Vector3 targetPosition;
    private Vector2 lastMoveDirection = Vector2.down;
    private bool hasTarget;
    private float animationTimer;
    private int animationFrame;

    private void Awake()
    {
        mainCamera = Camera.main;
        targetPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        spriteRenderer.sortingOrder = 10;
        EnsureShadow();
        ShowIdleFrame();
    }

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            SetTargetFromMouse();
        }

        if (!hasTarget)
        {
            Animate(false);
            return;
        }

        Vector3 previousPosition = transform.position;
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );
        Vector3 moveDelta = transform.position - previousPosition;

        if (moveDelta.sqrMagnitude > 0.000001f)
        {
            lastMoveDirection = moveDelta.normalized;
        }

        Animate(true);

        if (Vector3.Distance(transform.position, targetPosition) <= stopDistance)
        {
            transform.position = targetPosition;
            hasTarget = false;
            animationFrame = 0;
            ShowIdleFrame();
        }
    }

    private void SetTargetFromMouse()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera == null)
        {
            return;
        }

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 screenPosition = new Vector3(
            mousePosition.x,
            mousePosition.y,
            -mainCamera.transform.position.z
        );

        targetPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        targetPosition.z = transform.position.z;

        Vector3 directionToTarget = targetPosition - transform.position;

        if (directionToTarget.sqrMagnitude > 0.000001f)
        {
            lastMoveDirection = directionToTarget.normalized;
        }

        animationFrame = 0;
        animationTimer = 0f;
        hasTarget = true;
    }

    private void Animate(bool isRunning)
    {
        Sprite[] frames = isRunning ? GetRunFrames() : GetIdleFrames();

        if (frames == null || frames.Length == 0)
        {
            EnsureFallbackSquare();
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

    private void ShowIdleFrame()
    {
        Sprite[] frames = GetIdleFrames();

        if (frames == null || frames.Length == 0)
        {
            EnsureFallbackSquare();
            return;
        }

        spriteRenderer.sprite = frames[0];
        spriteRenderer.color = Color.white;
    }

    private Sprite[] GetRunFrames()
    {
        if (Mathf.Abs(lastMoveDirection.x) > Mathf.Abs(lastMoveDirection.y))
        {
            return lastMoveDirection.x < 0f ? runLeftFrames : runRightFrames;
        }

        return lastMoveDirection.y < 0f ? runDownFrames : runUpFrames;
    }

    private Sprite[] GetIdleFrames()
    {
        if (Mathf.Abs(lastMoveDirection.x) > Mathf.Abs(lastMoveDirection.y))
        {
            return lastMoveDirection.x < 0f ? idleLeftFrames : idleRightFrames;
        }

        return lastMoveDirection.y < 0f ? idleDownFrames : idleUpFrames;
    }

    private void EnsureFallbackSquare()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        texture.filterMode = FilterMode.Point;

        spriteRenderer.sprite = Sprite.Create(
            texture,
            new Rect(0f, 0f, 1f, 1f),
            new Vector2(0.5f, 0.5f),
            1f
        );
        spriteRenderer.color = squareColor;
        spriteRenderer.sortingOrder = 10;
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
