using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CharacterCreation
{
    public class NormalizingStringComparer : IEqualityComparer<string>
    {
        private static string Normalize(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            s = s.Replace('ё', 'е').Replace('Ё', 'Е');
            s = s.ToLowerInvariant().Trim();
            s = Regex.Replace(s, @"\s+", " ");
            return s;
        }

        public bool Equals(string x, string y) => Normalize(x) == Normalize(y);
        public int GetHashCode(string obj) => Normalize(obj).GetHashCode();
    }
}

