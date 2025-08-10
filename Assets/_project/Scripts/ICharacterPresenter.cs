using System;
using R3;

namespace CharacterCreation
{
    public interface ICharacterPresenter : IPresenter
    {
        R3.Observable<Character> NextClicked { get; }
    }
}

