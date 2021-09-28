using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D controller;
    float jumpCooldown = 0;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey("w") && IsGrounded() && jumpCooldown <= Time.fixedTime) {
            jumpCooldown = Time.fixedTime + .4f;
            controller.AddForce(new Vector2(0,10f),ForceMode2D.Impulse);
        }
        if (Input.GetKey("d"))
            controller.AddForce(new Vector2(20f,0));
        if (Input.GetKey("a"))
            controller.AddForce(new Vector2(-20f,0));
    }

    void Update() {
        if (Input.GetKey("s"))
            MapController.Dig(Mathf.RoundToInt(transform.position.x),Mathf.RoundToInt(transform.position.y));
    
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
