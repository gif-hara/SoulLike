using System.Collections.Generic;

namespace SoulLike
{
    public sealed class Brocker
    {
        private readonly HashSet<string> topics = new();

        public void Brock(string topic)
        {
            topics.Add(topic);
        }

        public void Unbrock(string topic)
        {
            topics.Remove(topic);
        }

        public bool IsBrocked()
        {
            return topics.Count > 0;
        }
    }
}
