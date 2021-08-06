using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Animator2D : MonoBehaviour
{
    public SpriteRenderer SR;
    public List<Sprite> Sprites;
    public int index;
    public float time;
    public float interval;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if(time > interval) 
        {
            time -= interval;
            index++;
            if (index >= Sprites.Count)
                index = 0;
            SR.sprite = Sprites[index];
        }
    }
}
