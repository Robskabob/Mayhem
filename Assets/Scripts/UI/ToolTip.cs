using L33t.Equipment;
using L33t.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ToolTip : MonoBehaviour
{
    public Cam Cam;
    public Equipment Item;
    public Text text;
    public StatMenu StatMenu;

    void Start()
    {
        
    }

    public float wait;
    // Update is called once per frame
    void Update()
    {
        //transform.position = (Vector2)Cam.Camera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouse = Cam.Camera.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            Collider2D col = Physics2D.OverlapCircle(mouse, 1, 1 << 10);
            if (col == null)
                return;
            Equipment E = col.GetComponentInParent<Equipment>();
            if (E != null)
            {
                if (Item && Vector2.Distance(E.transform.position, mouse) >= Vector2.Distance(Item.transform.position, mouse))
                    return;
                Item = E;
                StatMenu.gameObject.SetActive(true);
                StatMenu.Setup(E.name,E.GetStats());
                text.text = Item.PrintStats();
                wait = 1;
                return;
            }
            return;
        }

        if (Item) 
        {
            StatMenu.UpdateValues(Item.GetStats());
            transform.position = Item.transform.position;
            if (2 < Vector2.Distance(Item.transform.position, mouse))
            {
                wait -= Time.deltaTime;
                if (wait < 0)
                {
                    text.text = "";
                    StatMenu.gameObject.SetActive(false);
                }
            }
            else
                wait = 1;
        }
    }
}
