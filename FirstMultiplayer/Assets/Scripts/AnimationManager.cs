using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimationManager : MonoBehaviour
{
    [SerializeField] private bool active = true;
    [SerializeField] private Animator anim;
    private InputController inputController;
    // Start is called before the first frame update
    void Awake()
    {
        inputController = FindFirstObjectByType<InputController>();

        if (inputController)
        {
            inputController.OnMoveInputUpdate += MoveInputUpdate;
        }
    }

    public void SetActive(bool active)
    {
        this.active = active;
    }

    public void Disable()
    {
        if (inputController)
        {
            inputController.OnMoveInputUpdate -= MoveInputUpdate;
        }
    }

    private void OnDestroy()
    {
        if (inputController)
        {
            inputController.OnMoveInputUpdate -= MoveInputUpdate;
        }
    }

    private void MoveInputUpdate(Vector2 vector)
    {
        if (!active) return;
        anim.SetFloat("BlendX", vector.x);
        anim.SetFloat("BlendY", vector.y);
    }

    public void SetDeadAnimation(bool isDead)
    {
        anim.SetBool("Dead", isDead);
    }
}
