using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace L33t.UI
{
    public class ListMenu : Menu
    {
        public List<Menu> Menus;
        public MenuButton MenuButton;

        public float Hight;

        protected override void Start()
        {
            base.Start();
            Populate();
        }

        public void Populate()
        {
            for (int i = 0; i < Menus.Count; i++)
            {
                MenuButton MB = Instantiate(MenuButton, transform);
                MB.transform.localPosition = new Vector3(0, i * Hight, 0);
                int index = i;
                MB.Button.onClick.AddListener(
                    () =>
                    {
                        MenuManager.NewMenu(Menus[index]);
                    });
                MB.Text.text = Menus[i].name;
            }
        }
    }
    public class MenuItem : UIBehaviour 
    {
        public int hight;
        public Dictionary<string,UIBehaviour> Elements;
    }
    public class ActionMenuData : ScriptableObject 
    {
        public List<UnityEngine.Events.UnityAction> Actions;
    }
    public class ListActionMenu : Menu
    {
        public ActionMenuData Actions;
        public MenuButton MenuButton;

        public float Hight;

        protected override void Start()
        {
            base.Start();
            Populate();
        }

        public void Populate()
        {
            for (int i = 0; i < Actions.Actions.Count; i++)
            {
                MenuButton MB = Instantiate(MenuButton, transform);
                MB.transform.localPosition = new Vector3(0, i * Hight, 0);
                int index = i;
                MB.Button.onClick.AddListener(Actions.Actions[index]);
                //MB.Text.text = Menus[i].name;
            }
        }
    }
}