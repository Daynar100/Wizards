using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkUserController : MonoBehaviour
{
    Rigidbody2D controller;
    float jumpCooldown = 0;
    bool left, right, jump, dig;
    private Vector3 velocity = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Rigidbody2D>();
    }


    void Update() {
        if (IsGrounded()) {
            if (jump && jumpCooldown <= Time.fixedTime) {
                jumpCooldown = Time.fixedTime + .4f;
                velocity.y = 10f;
            } else if (velocity.y < 0) {
                velocity.y = 0f;
            }
        } else {
            velocity.y -= 9.1f * Time.deltaTime;
        }
        velocity.x = 0;
        if (left)
            velocity.x = 9f;
        if (right)
            velocity.x = -9f;

        Vector3 lastPosition = transform.position;
        transform.position += velocity * Time.deltaTime;
        if (Physics2D.OverlapBox(new Vector2(transform.position.x,transform.position.y + 0.4f), new Vector2(0.6f,2f), 0) != null) {
            transform.position = lastPosition;
            int exit = 10;
            while(Physics2D.OverlapBox(new Vector2(transform.position.x,transform.position.y + 0.4f), new Vector2(0.6f,2f), 0) == null && exit > 0) {
                lastPosition = transform.position;
                transform.position += velocity * Time.deltaTime * 0.1f;
                --exit;
            }
            transform.position = lastPosition;
        }

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
