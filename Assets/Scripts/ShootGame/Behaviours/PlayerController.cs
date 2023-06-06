using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //tốc độ di chuyển của đối tượng 
    public float moveSpeed;

    //trạng thái di chuyển 
    private bool isMoving;

    //input từ bàn phím
    private Vector2 input;

    //điều khiển animator
    private Animator animator;

    public LayerMask solidObjectsLayer;

    public LayerMask interactableLayer;

    public LayerMask waterLayer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        //kiểm tra trạng thái di chuyển 
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            //Debug.Log("This is input.x" + input.x);
            //Debug.Log("This is input.y" + input.y);

            if (input.x != 0) input.y = 0;

            //kiểm tra tương tác với bàn phím 
            if(input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);

                //biến lưu vị trí 
                var targetPos = transform.position;

                targetPos.x += input.x;
                targetPos.y += input.y;

                if(IsWalkable(targetPos))
                StartCoroutine(Move(targetPos));
            }
        }
        animator.SetBool("isMoving", isMoving);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Interact();
        }
    }

    void Interact()
    {
        var facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        var interactPos = transform.position + facingDir;

        //Debug.DrawLine(transform.position, interactPos, Color.red, 1f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.2f, interactableLayer);
        if(collider != null)
        {
            Debug.Log("Con động vật ở đây");
        }
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        //
        while((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            //
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        isMoving = false;
    }

    private bool IsWalkable(Vector3 targetPos)
    {
       if (Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer | interactableLayer 
                                                                      | waterLayer) != null)
        {
            return false;
        }
        return true;
    }
}
