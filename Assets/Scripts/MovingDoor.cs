using UnityEngine;
using System.Collections;

public class MovingDoor : MonoBehaviour
{
    [Header("Настройки движения")]
    [Tooltip("На сколько единиц вверх или вниз сдвинется дверь")]
    [SerializeField] private Vector3 moveOffset = new Vector3(0, 3f, 0);
    [SerializeField] private float speed = 2f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Coroutine moveCoroutine;

    private void Awake()
    {
        // Запоминаем стартовую позицию двери в сцене
        startPosition = transform.position;
        // Вычисляем позицию в "открытом" состоянии
        targetPosition = startPosition + moveOffset;
    }

    // Метод, который будет вызывать кнопка
    public void SetOpen(bool open)
    {
        // Выбираем, к какой точке плавно двигаться
        Vector3 destination = open ? targetPosition : startPosition;

        // Если дверь уже двигалась, останавливаем старое движение, чтобы не было конфликтов
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);

        // Запускаем плавное перемещение
        moveCoroutine = StartCoroutine(MoveRoutine(destination));
    }

    private IEnumerator MoveRoutine(Vector3 dest)
    {
        while (Vector3.Distance(transform.position, dest) > 0.01f)
        {
            // Плавно перемещаем объект из текущей точки в пункт назначения
            transform.position = Vector3.Lerp(transform.position, dest, speed * Time.deltaTime);
            yield return null; // Ждем следующего кадра
        }

        transform.position = dest; // Точно выравниваем в конце
    }
}