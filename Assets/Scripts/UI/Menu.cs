using UnityEngine;
using UnityEngine.EventSystems;


namespace L33t.UI
{
	public class Menu : UIBehaviour
    {
        public static MenuManager MenuManager;
        public Menu Last;
        public Menu Next;
        public MenuButton Back;
        protected override void Start()
        {
            Back?.Button.onClick.AddListener(() => MenuManager.LastMenu());
        }
        public void LastMenu() 
        {
            //Last = null;//is this needed?
            Last.Open();
            Close();        
        }
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

    public class AudioSettingsMenu : Menu 
    {
        public void SetVolume(float value) 
        {
            AudioListener.volume = value;
        }
        public void set(float value) 
        {
            AudioListener.volume = value;
        }
    }

    public class VideoSettingsMenu : Menu
	{
        public void setTexQuality(int value)
        {

        }
        public void SetAnisotropic(int value)
        {
            QualitySettings.anisotropicFiltering = (AnisotropicFiltering)value;
        }
        public void SetAASamples(int value)
        {
            QualitySettings.antiAliasing = value;
        }
        public void SetVSync(int value)
        {
            QualitySettings.vSyncCount = value;
        }
        public void SetSleepTime(int value)
        {
            Screen.sleepTimeout = value;
        }
        public void SetisFullScreen(bool value)
        {
            Screen.fullScreen = value;
        }
        public void SetisFullScreen(int value)
        {
            Screen.fullScreenMode = (FullScreenMode)value;
        }
	}
}