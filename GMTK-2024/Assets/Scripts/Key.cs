using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] int status;
    [SerializeField] GameObject keyBox;
    const int ENTRANCE = 0, SCATTERING = 1, IDLE = 2, GATHERING = 3, REST = 4;
    public Transform targetNode, gatherNode;

    GameObject activeKeyBox;
    void FixedUpdate()
    {
        if (status == SCATTERING)
        {
            transform.position += (targetNode.position - transform.position).normalized * speed;
            if ((targetNode.position - transform.position).sqrMagnitude < 1)
            {
                transform.position = targetNode.position;
                status = IDLE;

                if (useKeyBox)
                {
                    activeKeyBox = Instantiate(keyBox, transform.position, Quaternion.identity);
                }
            }
        }
        if (status == IDLE)
        {
            if ((PlayerController.self.transform.position - transform.position).sqrMagnitude < 1 && !activeKeyBox)
            {
                status = GATHERING;
                LevelManager.OnKeyCollect();
                useKeyBox = false;
                GameManager.SoundManager.PlaySound(SoundType.KEYCOLLECT, 0.9f, 1.1f, 1, transform.position);
            }
        }
        if (status == GATHERING)
        {
            transform.position += (gatherNode.position - transform.position).normalized * speed;
            if ((gatherNode.position - transform.position).sqrMagnitude < 2)
            {
                transform.position = gatherNode.position;
                status = REST;
            }
        }
    }

    bool useKeyBox;
    public void UseKeyBox()
    {
        useKeyBox = true;
    }

    public void Scatter(Transform target, Transform gather)
    {
        targetNode = target;
        gatherNode = gather;
        status = SCATTERING;
    }
}
