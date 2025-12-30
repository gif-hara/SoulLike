using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace SoulLike.ActorControllers.Abilities
{
    public sealed class ActorEffect : IActorAbility
    {
        private Actor actor;

        private readonly Dictionary<string, Element> elements = new();

        public void Activate(Actor actor, CancellationToken cancellationToken)
        {
            this.actor = actor;
            foreach (var effectObject in actor.EffectObjects)
            {
                elements.Add(effectObject.name, new Element(effectObject));
            }
        }

        public void SetActive(string effectName, bool active)
        {
            if (elements.TryGetValue(effectName, out var element))
            {
                element.SetActive(active);
            }
        }

        public void Reset()
        {
            foreach (var element in elements.Values)
            {
                element.Reset();
            }
        }

        public class Element
        {
            private readonly GameObject effectObject;

            private readonly bool initialActive;

            public Element(GameObject effectObject)
            {
                this.effectObject = effectObject;
                initialActive = effectObject.activeSelf;
            }

            public void SetActive(bool active)
            {
                effectObject.SetActive(active);
            }

            public void Reset()
            {
                effectObject.SetActive(initialActive);
            }
        }
    }
}
