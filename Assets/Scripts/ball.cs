using UnityEngine;

public class ball : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody2D rb;
    public float speed;
    public Vector2 direction;

    public int brickCount=0;

    public scoreManager score;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        direction = Vector2.one.normalized; //(1,1)
        score = GameObject.FindGameObjectWithTag("logic").GetComponent<scoreManager>();  

    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = direction * speed;
    }

    void OnTriggerEnter2D(Collider2D collison)
    {
        if (collison.gameObject.CompareTag("paddle"))
            direction.y = -direction.y;
        else if (collison.gameObject.CompareTag("brick"))
        {
            direction.y = -direction.y;
            Destroy(collison.gameObject);
            brickCount++;
            Debug.Log("Brick count: " + brickCount);
            score.addScore(1);  // whenever hits a brick send 1

        }
        else if (collison.gameObject.CompareTag("sideWall"))
            direction.x = -direction.x;
        else if (collison.gameObject.CompareTag("bottomWall"))
        {
            Debug.Log("Game over");
            gameObject.SetActive(false);
            score.addScore(0);  // whenever hits a floor send 0

        }
        else if (collison.gameObject.CompareTag("topWall"))
            direction.y = -direction.y;
    }
}
