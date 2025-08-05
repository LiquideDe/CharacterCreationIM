namespace CharacterCreation
{
    public abstract class PresenterBase<TView> : IPresenter where TView : ViewBase
    {
        protected readonly TView View;
        protected readonly AudioManager AudioManager;

        protected PresenterBase(TView view, AudioManager audioManager)
        {
            View = view;
            AudioManager = audioManager;
        }
        public virtual void Dispose() { }

        public virtual void Initialize() {  }
    }
}

