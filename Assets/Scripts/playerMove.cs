using UnityEngine;

public class playerMove : MonoBehaviour
{
    Rigidbody2D pad;
    Vector2 initial;
    public float displacement;

    public Animator animator;

    AudioManager audioManager;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pad = GetComponent<Rigidbody2D>();
        initial = pad.transform.localPosition;
        audioManager = GameObject.FindGameObjectWithTag("music").GetComponent<AudioManager>();
       
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKey(KeyCode.RightArrow)))
        {

            animator.SetBool("isRunning", true);
            if (initial.x <= 9.75)
                initial.x = initial.x + displacement;
        }
        else if ((Input.GetKey(KeyCode.LeftArrow)))
        {
            if (initial.x > -9.75)
                initial.x = initial.x - displacement;
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        pad.MovePosition(initial);
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("brick"))
        {   Debug.Log("Stumble Sound");
            audioManager.PlaySXF(audioManager.stumbleSound);
        }
        
    }

}
