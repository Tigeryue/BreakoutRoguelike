using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;

public class HandCollision : MonoBehaviour
{
    private UnityEngine.Vector2 PreLoc;
    private UnityEngine.Vector2 AftLoc;
    private UnityEngine.Vector2 IntLoc;
    private UnityEngine.Vector2 ballSpeed;
    public float maxSpeed = 10;
    public float speedMulti = 10;

    // Start is called before the first frame update
    void Start()
    {
        PreLoc = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        AftLoc = transform.position;
        IntLoc = AftLoc - PreLoc;
        PreLoc = AftLoc;
        Debug.Log("IntLoc: " + IntLoc);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ball")
        {
            ballSpeed = new UnityEngine.Vector2(other.gameObject.GetComponent<Rigidbody2D>().velocity.x + IntLoc.x , other.gameObject.GetComponent<Rigidbody2D>().velocity.y + IntLoc.y);

            other.gameObject.GetComponent<Rigidbody2D>().velocity = new UnityEngine.Vector2( Mathf.Min(ballSpeed.x, maxSpeed), Mathf.Min(ballSpeed.y, maxSpeed)) * speedMulti;
            
            Debug.Log("touch ball" + IntLoc);
        }

    }
}
