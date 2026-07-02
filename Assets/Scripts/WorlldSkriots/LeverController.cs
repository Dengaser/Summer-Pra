using UnityEngine;

public class LeverController : MonoBehaviour
{
    [Header("Ссылка на платформу")]
    public PlatformController platform;

    private bool isPulled = false;

    public void PullLever()
    {
        if (isPulled) return; // Рычаг можно нажать только один раз
        isPulled = true;

        Debug.Log("Рычаг нажат!");

        // Тут можно запустить анимацию рычага, если она есть:
        // GetComponent<Animator>().SetTrigger("Pull");

        if (platform != null)
        {
            platform.LowerPlatform();
        }
    }
}