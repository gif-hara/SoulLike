using System;
using System.Collections.Generic;

namespace HK
{
    /// <summary>
    /// 時間に関するクラス
    /// </summary>
    public sealed class Time
    {
        public static readonly Time Root = new();

        private readonly Time parent;

        private readonly List<Time> children = new();

        private float _timeScale = 1.0f;

        public event Action UpdatedTimeScale;

        public float timeScale
        {
            set
            {
                this._timeScale = value;
                UpdatedTimeScale?.Invoke();
                foreach (var child in this.children)
                {
                    child.UpdatedTimeScale?.Invoke();
                }
            }
            get => this._timeScale;
        }

        public float totalTimeScale => this.GetTimeScaleRecursive(1.0f);

        private float GetTimeScaleRecursive(float value)
        {
            if (this.parent != null)
            {
                return this.parent.GetTimeScaleRecursive(value * this.timeScale);
            }
            else
            {
                return UnityEngine.Time.timeScale * value * this.timeScale;
            }
        }

        public float deltaTime => UnityEngine.Time.deltaTime * this.totalTimeScale;

        public Time(Time parent = null)
        {
            this.parent = parent;
            this.parent?.children.Add(this);
        }

        ~Time()
        {
            this.parent?.children.Remove(this);
        }
    }
}