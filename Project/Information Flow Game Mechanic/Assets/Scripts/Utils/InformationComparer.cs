using System.Collections.Generic;
using Information_Flow;

namespace Utils
{
    public class InformationComparer : IEqualityComparer<Information>
    {
        public bool Equals(Information x, Information y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Not == y.Not && Equals(x.Subject, y.Subject) && x.Verb == y.Verb && Equals(x.Object, y.Object) && Equals(x.Adjective, y.Adjective) && Equals(x.Location, y.Location);
        }

        public int GetHashCode(Information obj)
        {
            unchecked
            {
                var hashCode = obj.Not.GetHashCode();
                hashCode = (hashCode * 397) ^ (obj.Subject != null ? obj.Subject.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)obj.Verb;
                hashCode = (hashCode * 397) ^ (obj.Object != null ? obj.Object.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.Adjective != null ? obj.Adjective.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.Location != null ? obj.Location.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
