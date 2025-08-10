using HK;
using UnityEngine;

namespace SoulLike
{
    public readonly struct AnimatorParameter
    {
        public string Name { get; }

        public int Hash { get; }

        public AnimatorParameter(string name)
        {
            Name = name;
            Hash = Animator.StringToHash(name);
        }

        public class DictionaryList : DictionaryList<string, AnimatorParameter>
        {
            public DictionaryList() : base(x => x.Name)
            {
            }
        }
    }
}
