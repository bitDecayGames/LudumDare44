using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    public const float speed = 1.5f;
    
    public Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
        if (transform.parent.name.Equals("Woman3"))
        {
            animator.Play("DLWalk");
        }
        else
        {
            animator.Play("URWalk");   
        }
    }

    private void Update()
    {
        if (transform.parent.name.Equals("Woman3"))
        {
            transform.position = transform.parent.position += new Vector3(-1, -.5f, 0) * Time.deltaTime * speed;
        }
        else
        {
            transform.position = transform.parent.position += new Vector3(1, .5f, 0) * Time.deltaTime * speed;
        }        
    }
}