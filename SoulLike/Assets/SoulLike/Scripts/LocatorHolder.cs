using System;
using HK;
using UnityEngine;

namespace SoulLike
{
    public class LocatorHolder : MonoBehaviour
    {
        [SerializeField]
        private Element.DictionaryList elements;

        public Transform Get(string name)
        {
            return elements.Get(name).Transform;
        }

        [Serializable]
        public class Element
        {
            [SerializeField]
            private string name;

            [SerializeField]
            private Transform transform;
            public Transform Transform => transform;

            [Serializable]
            public class DictionaryList : DictionaryList<string, Element>
            {
                public DictionaryList() : base(x => x.name)
                {
                }
            }
        }
    }
}
