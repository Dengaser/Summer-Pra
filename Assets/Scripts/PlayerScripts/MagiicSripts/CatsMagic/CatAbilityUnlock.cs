using UnityEngine;
using System.Collections;

public class CatAbilityUnlock : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    [Header("Настройки мяуканья")]
    [SerializeField] private AudioClip[] meowSounds; 
    [SerializeField] private float minMeowInterval = 5f;
    [SerializeField] private float maxMeowInterval = 12f;

    public AudioSource audioSource;

    private bool isCollected = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        StartCoroutine(MeowRoutine());
    }

    private IEnumerator MeowRoutine()
    {
        while (!isCollected)
        {
            float randomWait = Random.Range(minMeowInterval, maxMeowInterval);
            yield return new WaitForSeconds(randomWait);

            
            if (!isCollected && meowSounds != null && meowSounds.Length > 0)
            {
                
                int randomIndex = Random.Range(0, meowSounds.Length);

                
                if (meowSounds[randomIndex] != null)
                {
                    audioSource.PlayOneShot(meowSounds[randomIndex]);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;

        if (other.CompareTag(playerTag))
        {
           
            SwitchMagic playerMagic = other.GetComponent<SwitchMagic>();

            if (playerMagic != null)
            {
                isCollected = true;

                
                playerMagic.UnlockNextAbility();

                
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("На объекте игрока не найден скрипт SwitchMagic!");
            }
        }
    }
}