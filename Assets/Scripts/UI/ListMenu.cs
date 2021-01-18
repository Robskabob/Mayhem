using System.Collections.Generic;
using UnityEngine;

public class ListMenu : Menu
{
    public List<Menu> Menus;
    public MenuButton MenuButton;

    public float Hight;

	protected override void Start()
	{
        Populate();
	}

	public void Populate() 
    {
        for (int i = 0; i < Menus.Count; i++)
        {
            Debug.Log(i);
            MenuButton MB = Instantiate(MenuButton,transform);
            MB.transform.localPosition = new Vector3(0, i * Hight, 0);
                    int index = i;
            MB.Button.onClick.AddListener(
                () => {
                    Debug.Log(index);
                    NextMenu(Menus[index]);
                });
            MB.Text.text = Menus[i].name;
        }
    }
}
