using HK;
using UnityEngine;

namespace SoulLike
{
    public class AnimatorParameterController
    {
        private readonly Animator animator;

        private readonly Element.DictionaryList elements = new();

        private class Element
        {
            public readonly string name;

            public readonly int hash;

            public Element(string name)
            {
                this.name = name;
                hash = Animator.StringToHash(name);
            }

            public class DictionaryList : DictionaryList<string, Element>
            {
                public DictionaryList() : base(x => x.name)
                {
                }
            }
        }

        public AnimatorParameterController(Animator animator)
        {
            this.animator = animator;
        }

        private Element GetElement(string name)
        {
            if (!elements.TryGetValue(name, out var element))
            {
                element = new Element(name);
                elements.Add(element);
            }
            return element;
        }

        public void SetBool(string name, bool value)
        {
            var element = GetElement(name);
            animator.SetBool(element.hash, value);
        }

        public void SetTrigger(string name)
        {
            var element = GetElement(name);
            animator.SetTrigger(element.hash);
        }

        public void SetFloat(string name, float value)
        {
            var element = GetElement(name);
            animator.SetFloat(element.hash, value);
        }

        public void SetInt(string name, int value)
        {
            var element = GetElement(name);
            animator.SetInteger(element.hash, value);
        }

        public void ResetTrigger(string name)
        {
            var element = GetElement(name);
            animator.ResetTrigger(element.hash);
        }
    }
}
