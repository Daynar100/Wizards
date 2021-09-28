using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkUserController : MonoBehaviour
{
    Rigidbody2D controller;
    float jumpCooldown = 0;
    bool left, right, jump, dig;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (jump && IsGrounded() && jumpCooldown <= Time.fixedTime) {
            jumpCooldown = Time.fixedTime + .4f;
            controller.AddForce(new Vector2(0,10f),ForceMode2D.Impulse);
        }
        if (right)
            controller.AddForce(new Vector2(20f,0));
        if (left)
            controller.AddForce(new Vector2(-20f,0));
    }

    void Update() {
        if (dig)
            MapController.Dig(Mathf.RoundToInt(transform.position.x),Mathf.RoundToInt(transform.position.y));
    }

    private bool IsGrounded() {
        Vector2 topLeft = new Vector2(transform.position.x - .3f,transform.position.y - .7f);
        Vector2 bottomRight = new Vector2(transform.position.x + .3f,transform.position.y - .7f);
        return (Physics2D.Linecast(topLeft, bottomRight).collider != null);
    }

    public void NetworkUpdate(MessageDecoder d) {
        switch(d.ReadInt()) {
            case 3:
                right = d.ReadBool();
            break;
            case 2:
                dig = d.ReadBool();
            break;
            case 1:
                left = d.ReadBool();
            break;
            case 0:
            default:
                jump = d.ReadBool();
            break;

        }
        SetPosition(d);
    }

    public void SetPosition(MessageDecoder decoder)
    {
        transform.position = new Vector3(decoder.ReadFloat(),decoder.ReadFloat(),decoder.ReadFloat());
    }
}
