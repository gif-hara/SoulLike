using R3;
using UnityEngine;

namespace SoulLike
{
    public class UserData
    {
        private ReactiveProperty<int> experience = new(0);

        public ReadOnlyReactiveProperty<int> Experience => experience;

        public void AddExperience(int amount)
        {
            experience.Value += amount;
        }
    }
}
