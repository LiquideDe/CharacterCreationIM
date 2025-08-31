using System;

namespace CharacterCreation
{
    public class FinderData : INameIndex
    {
        private readonly INameProvider[] _providers;
        public FinderData(INameProvider[] providers) => _providers = providers;

        public bool TryGet<T>(string name, out T value) where T : class
        {
            foreach (var p in _providers)
            {
                if (p.ItemType == typeof(T) && p.TryGet(name, out var obj))
                {
                    value = (T)obj;
                    return true;
                }
            }
            value = null;
            return false;
        }
    }

    public interface INameProvider
    {
        Type ItemType { get; }
        bool TryGet(string name, out object value);
    }

    public interface INameIndex
    {
        bool TryGet<T>(string name, out T value) where T : class;
    }
}

