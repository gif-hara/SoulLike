using HK;
using UnityEngine;

namespace SoulLike
{
    public class AnimatorParameter
    {
        public readonly string name;

        public readonly int hash;

        public AnimatorParameter(string name)
        {
            this.name = name;
            hash = Animator.StringToHash(name);
        }

        public class DictionaryList : DictionaryList<string, AnimatorParameter>
        {
            public DictionaryList() : base(x => x.name)
            {
            }
        }
    }
}
