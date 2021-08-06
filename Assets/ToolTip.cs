using L33t.Equipment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
    public Cam Cam;
    public Text text;

    void Start()
    {
        
    }

    public float wait;
    // Update is called once per frame
    void Update()
    {
        transform.position = (Vector2)Cam.Camera.ScreenToWorldPoint(Input.mousePosition);

        Collider2D col = Physics2D.OverlapCircle(transform.position,1,1<<10);
        if (col == null)
            return;
        Equipment E = col.GetComponentInParent<Equipment>();
        if (E != null)
        {
            text.text = E.PrintStats();
            wait = .1f;
            return;
        }
        wait -= Time.deltaTime;
        if (wait < 0)
        {
            text.text = "";
        }
    }
}
