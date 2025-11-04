using UnityEngine;

public class playerMove : MonoBehaviour
{
    private Rigidbody2D pad;
    private Vector2 initial;
    public float displacement;

    public Animator animator;

    void Start()
    {
        pad = GetComponent<Rigidbody2D>();
        initial = pad.transform.localPosition;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            animator.SetBool("isRunning", true);
            if (initial.x <= 9.75f)
                initial.x += displacement;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            animator.SetBool("isRunning", true);
            if (initial.x > -9.75f)
                initial.x -= displacement;
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        pad.MovePosition(initial);
    }
}
