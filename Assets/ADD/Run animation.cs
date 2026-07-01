using UnityEngine;

public class Runanimation : MonoBehaviour
{
    public Animator myAnimator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("w")|| Input.GetKeyDown("a")||Input.GetKeyDown("s")||Input.GetKeyDown("d"))
        {
            myAnimator.SetTrigger("Interact");
        }
    }
}
