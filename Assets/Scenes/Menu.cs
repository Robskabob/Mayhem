using UnityEngine.EventSystems;

public class Menu : UIBehaviour 
{
    public Menu Last;
    public void NextMenu(Menu next) 
    {
        next.Last = this;
        next.Open();
        Close();
    }
    public void Open() 
    {
        gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
}
