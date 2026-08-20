using UnityEngine;

public class TestingBookAnimation : MonoBehaviour
{
   void Start()
    {
        // Try Animator first (if you set up an Animator Controller)
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play("Scene"); // replace with your actual clip name if different
            Debug.Log("Playing via Animator: Scene");
            return;
        }

        // Fallback: legacy Animation component
        Animation anim = GetComponent<Animation>();
        if (anim != null)
        {
            anim.Play(); // plays default clip
            Debug.Log("Playing via Animation component");
            return;
        }

        Debug.LogWarning("No Animator or Animation component found on this GameObject.");
    } 
}
