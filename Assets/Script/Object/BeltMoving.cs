using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltMoving : MonoBehaviour
{
    public enum Direction
    {
        Z_positive,Z_nagative,X_positive,X_nagative,Y_positive,Y_nagative
    }

    public Direction direction;
    public float speed = 0.5f;
    private PlayerMovement playerMovement;
    private void Start()
    {
        playerMovement = GameManager.Instance.player.GetComponent<PlayerMovement>();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (GameManager.Instance.PAUSE == true)
            return;

        if(collision.collider.CompareTag("Player"))
        {
            Vector3 dir = Vector3.zero;
            switch(direction)
            {
                case Direction.Z_positive:
                    dir = transform.forward;
                    break;
                case Direction.Z_nagative:
                    dir = -transform.forward;
                    break;
                case Direction.X_positive:
                    dir = transform.right;
                    break;
                case Direction.X_nagative:
                    dir = -transform.right;
                    break;
                case Direction.Y_positive:
                    dir = transform.up;
                    break;
                case Direction.Y_nagative:
                    dir = -transform.up;
                    break;

            }

            playerMovement.Move(dir * speed,Time.deltaTime);
        }
    }
}
