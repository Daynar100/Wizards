using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float jumpCooldown = 0;
    private Vector3 velocity = Vector3.zero;

    private CapsuleCollider2D cCollider;
    void Awake() {
        cCollider = GetComponent<CapsuleCollider2D>();
    }


    void Update() {

        if (IsGrounded()) {
            if (Input.GetKey("w") && jumpCooldown <= Time.fixedTime) {
                jumpCooldown = Time.fixedTime + .4f;
                velocity.y = 10f;
            } else if (velocity.y < 0) {
                velocity.y = 0f;
            }
        } else {
            velocity.y -= 9.1f * Time.deltaTime;
        }
        velocity.x = 0;
        if (Input.GetKey("d"))
            velocity.x = 9f;
        if (Input.GetKey("a"))
            velocity.x = -9f;

        if (Input.GetKey("s"))
            MapController.Dig(Mathf.RoundToInt(transform.position.x),Mathf.RoundToInt(transform.position.y));

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
    
        if (Input.GetKeyDown("w")) {
            MessageEncoder e = new MessageEncoder();
            e.Encode(MessageType.PlayerAction);
            e.Encode((int)0);
            e.Encode(true);
            e.Encode(EncodePosition().ToArray());
            NetworkController.networkControllerInstance.SendData(e);
        }
        if (Input.GetKeyUp("w")) {
            MessageEncoder e = new MessageEncoder();
            e.Encode(MessageType.PlayerAction);
            e.Encode((int)0);
            e.Encode(false);
            e.Encode(EncodePosition().ToArray());
            NetworkController.networkControllerInstance.SendData(e);
        }
        if (Input.GetKeyDown("a")) {
            MessageEncoder e = new MessageEncoder();
            e.Encode(MessageType.PlayerAction);
            e.Encode((int)1);
            e.Encode(true);
            e.Encode(EncodePosition().ToArray());
            NetworkController.networkControllerInstance.SendData(e);
        }
        if (Input.GetKeyUp("a")) {
            MessageEncoder e = new MessageEncoder();
            e.Encode(MessageType.PlayerAction);
            e.Encode((int)1);
            e.Encode(false);
            e.Encode(EncodePosition().ToArray());
            NetworkController.networkControllerInstance.SendData(e);
        }
        if (Input.GetKeyDown("s")) {
            MessageEncoder e = new MessageEncoder();
            e.Encode(MessageType.PlayerAction);
            e.Encode((int)2);
            e.Encode(true);
            e.Encode(EncodePosition().ToArray());
            NetworkController.networkControllerInstance.SendData(e);
        }
        if (Input.GetKeyUp("s")) {
            MessageEncoder e = new MessageEncoder();
            e.Encode(MessageType.PlayerAction);
            e.Encode((int)2);
            e.Encode(false);
            e.Encode(EncodePosition().ToArray());
            NetworkController.networkControllerInstance.SendData(e);
        }
        if (Input.GetKeyDown("d")) {
            MessageEncoder e = new MessageEncoder();
            e.Encode(MessageType.PlayerAction);
            e.Encode((int)3);
            e.Encode(true);
            e.Encode(EncodePosition().ToArray());
            NetworkController.networkControllerInstance.SendData(e);
        }
        if (Input.GetKeyUp("d")) {
            MessageEncoder e = new MessageEncoder();
            e.Encode(MessageType.PlayerAction);
            e.Encode((int)3);
            e.Encode(false);
            e.Encode(EncodePosition().ToArray());
            NetworkController.networkControllerInstance.SendData(e);
        }
    }

    private bool IsGrounded() {
        Vector2 topLeft = new Vector2(transform.position.x - .3f,transform.position.y - .7f);
        Vector2 bottomRight = new Vector2(transform.position.x + .3f,transform.position.y - .7f);
        return (Physics2D.Linecast(topLeft, bottomRight).collider != null);
    }

    List<byte> EncodePosition()
    {
        List<byte> bytes = new List<byte>();
        bytes.AddRange(System.BitConverter.GetBytes(transform.position.x));
        bytes.AddRange(System.BitConverter.GetBytes(transform.position.y));
        bytes.AddRange(System.BitConverter.GetBytes(transform.position.z));
        return bytes;
    }
}
