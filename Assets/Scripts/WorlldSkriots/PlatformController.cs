using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [Header("Настройки движения")]
    public float lowerDistance = 3f; // На сколько метров опустить вниз
    public float speed = 2f;

    private Vector3 targetLocalPosition;
    private bool shouldMove = false;
    private bool isDown = false;
    private PuzzleManager puzzleManager;

    private void Start()
    {
        // Целевая позиция — текущая, но смещенная вниз по Y
        targetLocalPosition = transform.localPosition + Vector3.down * lowerDistance;
        puzzleManager = FindFirstObjectByType<PuzzleManager>();
    }

    private void Update()
    {
        if (shouldMove)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetLocalPosition, speed * Time.deltaTime);

            if (Vector3.Distance(transform.localPosition, targetLocalPosition) < 0.01f)
            {
                transform.localPosition = targetLocalPosition;
                shouldMove = false;
                isDown = true;
                Debug.Log("Платформа опустилась и замкнула контакт!");

                // Платформа опустилась — триггерим проверку всей головоломки
                if (puzzleManager != null)
                {
                    puzzleManager.CheckPuzzleDelayed();
                }
            }
        }
    }

    public void LowerPlatform()
    {
        if (shouldMove || isDown) return;
        shouldMove = true;
    }

    // Менеджер будет вызывать этот метод, чтобы узнать, замкнула ли платформа цепь
    public bool IsPlatformDown()
    {
        return isDown;
    }
}