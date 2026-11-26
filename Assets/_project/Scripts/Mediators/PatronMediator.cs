using R3;
using UnityEngine;

namespace CharacterCreation
{
    public sealed class PatronMediator
    {
        private readonly PresenterViewFactory _factory;
        private readonly Subject<Unit> _finished = new();
        public Observable<Unit> Finished => _finished;

        public PatronMediator(PresenterViewFactory factory) => _factory = factory;

        public void Show()
        {
            var presenter = _factory.Create<PatronWizardView>() as PatronWizardPresenter;
            if (presenter == null)
            {
                Debug.LogError("[PatronMediator] Не удалось открыть мастер покровителя: отсутствует префаб или презентер.");
                _finished.OnNext(Unit.Default);
                return;
            }
            presenter.Finished.Take(1).Subscribe(_ =>
            {
                presenter.Dispose();
                _finished.OnNext(Unit.Default);
            });
        }
    }
}
