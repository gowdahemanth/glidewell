using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;

namespace CrudService.Model
{
    public class PersonComparer : IEqualityComparer<Person>
    {
        public bool Equals(Person? x, Person? y)
        {
            if(x != null && y != null)
            {
                return x.FirstName == y.FirstName &&
               x.LastName == y.LastName;
            }
            return false;
        }

        public int GetHashCode([DisallowNull] Person obj)
        {
            return obj.FirstName.GetHashCode() ^
               obj.LastName.GetHashCode();
        }
    }
}
