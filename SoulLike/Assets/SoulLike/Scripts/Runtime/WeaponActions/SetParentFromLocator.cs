using System;
using SoulLike.ActorControllers;
using SoulLike.ActorControllers.Abilities;
using UnityEngine;

namespace SoulLike.WeaponActions
{
    [Serializable]
    public sealed class SetParentFromLocator : IWeaponAction
    {
        [SerializeField]
        private Transform target;

        [SerializeField]
        private string locatorName;

        [SerializeField]
        private Vector3 offsetPosition;

        [SerializeField]
        private Vector3 offsetRotation;

        [SerializeField]
        private Vector3 offsetScale = Vector3.one;

        public void Invoke(Weapon weapon, Actor actor)
        {
            var locatorHolder = actor.GetAbility<ActorSceneViewHandler>().SceneView.LocatorHolder;
            var locatorTransform = locatorHolder.Get(locatorName);
            if (locatorTransform == null)
            {
                Debug.LogWarning($"Locator '{locatorName}' not found on actor '{actor.name}'.");
                return;
            }

            target.SetParent(locatorTransform);
            target.localPosition = offsetPosition;
            target.localEulerAngles = offsetRotation;
            target.localScale = offsetScale;
        }
    }
}
