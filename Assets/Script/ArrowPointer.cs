using UnityEngine;
using System.Collections;

public class ArrowPointer : MonoBehaviour
{
    public Vector3 target;
    [SerializeField] float hide_distance;
    [SerializeField] SpriteRenderer arrow_sprite;
    bool target_close = false;
    [SerializeField] float arrow_pointing_interval;
   
    // Update is called once per frame
    void Update()
    {
        ArrowRotation();
        CheckDistance();
        CheckGameOver();
    }

    void CheckGameOver()
    {
        // Disable those when game over
        if (GameData.is_game_over) Destroy(this.gameObject);
    }

    void ArrowRotation()
    {
        //if (target_close) return;
        Vector3 target_direction = (target - transform.position).normalized;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, target_direction);
    }
    void CheckDistance()
    {
        if (Vector2.Distance(target, transform.position) < hide_distance)
        {
            //if (target_close == false) StartCoroutine(PointAtItem());
            target_close = true;
            //transform.position = target;
        }
        else if (!arrow_sprite.enabled) target_close = false;
    }

    IEnumerator PointAtItem()
    {
        arrow_sprite.gameObject.transform.eulerAngles = new Vector3(0, 0, 180f);

        while (target_close)
        {
            Vector3 arrow_position1 = new Vector3(0, 0, 0);
            Vector3 arrow_position2 = new Vector3(0, 0.5f, 0);

            transform.localPosition = (transform.localPosition == arrow_position1) ? arrow_position2 : arrow_position1;

            yield return new WaitForSeconds(arrow_pointing_interval);
        }

        arrow_sprite.gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
        transform.localPosition = new Vector3(0, 0, 0);
        transform.position = transform.parent.transform.position;
    }

}
