using UnityEngine;

public class BoardMove : MonoBehaviour
{
    private bool moved = false;

    // Конечное локальное положение
    private Vector3 targetLocalPosition = new Vector3(
        -51.3844757f,
        56.6100006f,
        -28.0100002f);

    // Конечный локальный поворот
    private Quaternion targetLocalRotation = Quaternion.Euler(
        -105.062f,
        -0.8009949f,
        178.578f);

    public void LowerBeam()
    {
        if (moved)
            return;

        moved = true;

        transform.localPosition = targetLocalPosition;
        transform.localRotation = targetLocalRotation;
    }
}