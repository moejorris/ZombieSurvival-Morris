
using UnityEditor.Callbacks;
using UnityEngine;

public class TestCharacterController : MonoBehaviour
{
    float xInput;
    float yInput;

    [SerializeField] float speed = 5;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");

        Vector3 move = new(xInput, 0, yInput);

        if(move.magnitude > 1)
        {
            move.Normalize();
        }

        rb.velocity = move * speed;
    }
}
