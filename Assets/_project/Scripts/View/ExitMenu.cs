using UnityEngine;
using UnityEngine.UI;
using R3;

namespace CharacterCreation
{
    public class ExitMenu : MonoBehaviour
    {
        [SerializeField] private Button _buttonSaveAndExitMenu;
        [SerializeField] private Button _buttonExitMenu;
        [SerializeField] private Button _buttonSaveAndExit;
        [SerializeField] private Button _buttonExit;

        public Observable<Unit> SaveAndReturnToMenuClicked => _buttonSaveAndExitMenu.OnClickAsObservable();
        public Observable<Unit> ReturnToMenuClicked => _buttonExitMenu.OnClickAsObservable();
        public Observable<Unit> SaveAndExitClicked => _buttonSaveAndExit.OnClickAsObservable();
        public Observable<Unit> ExitClicked => _buttonExit.OnClickAsObservable();

        public void Close()
        {
            Destroy(gameObject);
        }
    }
}

