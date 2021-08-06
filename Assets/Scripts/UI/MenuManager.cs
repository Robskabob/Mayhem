using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace L33t.UI
{
	public class MenuManager : UIBehaviour
    {
        public Stack<Menu> MenuStack;
        public List<Menu> MenuHistory;//forward and back
        public Menu Current;

		protected override void Start()
		{
            if (Menu.MenuManager == null)
                Menu.MenuManager = this;
            else
			{
                Debug.LogError("There is already a MenuManager Destroing Extra");
                Destroy(this);
			}
		}
		private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.LeftBracket))
            {
                LastMenu();
            }
            if (Input.GetKeyDown(KeyCode.RightBracket))
            {
                NextMenu();
            }
        }
        public void LastMenu()
        {
            if (Current.Last == null)
                return;
            Current.Close();
            Current.Last.Next = Current;
            Current = Current.Last;
            Current.Open();
        }
        public void NewMenu(Menu next)
        {
            Current.Close();
            next.Last = Current;
            Current.Next = next;
            Current = next;
            Current.Open();
        }
        public void NextMenu()
        {
            if (Current.Next == null)
                return;
            Current.Close();
            Current.Next.Last = Current;
            Current = Current.Next;
            Current.Open();
        }
    }
}