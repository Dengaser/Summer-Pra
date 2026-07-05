using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
public class ZombieAttackTelekinesis : Telekinesis
{
    [Header("Настройки толчка зомби")]
    public float zombiePushForce = 40f;   // Сила отталкивания для зомби
    public float zombieDamage = 30f;      // Урон зомби
    public MouseButton zombieButton = MouseButton.Left;

    
    protected override void Update()
    {
        base.Update();
        if (Input.GetMouseButtonDown((int)zombieButton) || Input.GetKeyDown(KeyCode.R))
        {
            
            TryAttackZombie();

        }
    }


    private void TryAttackZombie()
    {
        if(playerCamera == null) playerCamera = Camera.main;

        Vector3 rayDirection = playerTransform.forward;
        rayDirection.y = 0;
        rayDirection.Normalize();

        Vector3 rayStartPoint = playerTransform.position + Vector3.up * eyeHeight + playerTransform.forward * 0.5f;

        RaycastHit hit;
        Vector3 boxHalfExtents = new Vector3(boxWidth / 2f, boxHeight / 2f, 0.1f);

        if(Physics.BoxCast(rayStartPoint, boxHalfExtents,rayDirection, out  hit, playerTransform.rotation, pushDistance))
        {
            if(hit.collider.CompareTag("Enemy") || hit.collider.GetComponent<HealthUniversal>() != null) 
            {
                GameObject zombie = hit.collider.gameObject;
                Animator zombieAnimator = zombie.GetComponent<Animator>();
                if (zombieAnimator != null)
                {
                    zombieAnimator.SetTrigger("Hit"); // Вместо "Hit" впишите точное имя вашего триггера из Unity
                }
                HealthUniversal zombieHealth = zombie.GetComponent<HealthUniversal>();
                if(zombieHealth != null )
                {
                    zombieHealth.TakeDamage(zombieDamage);
                }

                Rigidbody rb = zombie.GetComponent<Rigidbody>();
                NavMeshAgent navMeshAgent = zombie.GetComponent<NavMeshAgent>();
                if(navMeshAgent != null)
                {
                    navMeshAgent.velocity = rayDirection * zombiePushForce;
                }
                else if (rb != null)
                {
                    
                    rb.isKinematic = false;
                    rb.AddForce(rayDirection * zombiePushForce, ForceMode.Impulse);
                }

                if (AudioSource != null && push != null)
                {
                    AudioSource.PlayOneShot(push);
                }
            }
        }
    }
}
