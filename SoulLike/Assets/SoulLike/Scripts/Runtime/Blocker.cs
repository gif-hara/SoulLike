using System.Collections.Generic;

namespace SoulLike
{
    public sealed class Blocker
    {
        private readonly HashSet<string> topics = new();

        public void Block(string topic)
        {
            topics.Add(topic);
        }

        public void Unblock(string topic)
        {
            topics.Remove(topic);
        }

        public bool IsBlocked => topics.Count > 0;

        public void Reset()
        {
            topics.Clear();
        }
    }
}
