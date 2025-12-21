using System;
using System.Collections.Generic;
using SoulLike.ActorControllers.Abilities;
using UnityEngine;
using UnityEngine.Assertions;

namespace SoulLike.ActorControllers
{
    public class Actor : MonoBehaviour
    {
        [field: SerializeField]
        public List<GameObject> EffectObjects { get; private set; } = new();

        public ActorEvent Event { get; } = new ActorEvent();

        public ActorBrain Brain { get; } = new ActorBrain();

        private readonly Dictionary<Type, IActorAbility> abilities = new();

        void Awake()
        {
            Brain.Activate(this);
        }

        public void ActivateAbilities()
        {
            foreach (var ability in abilities.Values)
            {
                ability.Activate(this);
            }
        }

        public T AddAbility<T>() where T : IActorAbility, new()
        {
            var instance = new T();
            abilities[typeof(T)] = instance;
            return instance;
        }

        public T AddAbility<T>(T ability) where T : IActorAbility
        {
            abilities[typeof(T)] = ability;
            return ability;
        }

        public T GetAbility<T>() where T : class, IActorAbility
        {
            abilities.TryGetValue(typeof(T), out var ability);
            Assert.IsNotNull(ability, $"Ability of type {typeof(T)} not found on actor {name}.");
            return ability as T;
        }

        public bool TryGetAbility<T>(out T ability) where T : class, IActorAbility
        {
            if (abilities.TryGetValue(typeof(T), out var foundAbility))
            {
                ability = foundAbility as T;
                return ability != null;
            }
            ability = null;
            return false;
        }
    }
}
