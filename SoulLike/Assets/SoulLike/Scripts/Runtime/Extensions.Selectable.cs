using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace SoulLike
{
    public static partial class Extensions
    {
        public static void SetNavigationVertical(this IList<Selectable> self)
        {
            for (var i = 0; i < self.Count; i++)
            {
                var n = self[i].navigation;
                var up = i - 1;
                var down = i + 1;
                n.mode = Navigation.Mode.Explicit;
                n.selectOnUp = up >= 0 ? self[up] : self[self.Count - 1];
                n.selectOnDown = down < self.Count ? self[down] : self[0];
                self[i].navigation = n;
            }
        }

        public static void SetNavigationVertical(this IList<Button> self)
        {
            SetNavigationVertical(self.Cast<Selectable>().ToList());
        }

        public static void SetNavigationHorizontal(this IList<Selectable> self)
        {
            for (var i = 0; i < self.Count; i++)
            {
                var n = self[i].navigation;
                var left = i - 1;
                var right = i + 1;
                n.mode = Navigation.Mode.Explicit;
                n.selectOnLeft = left >= 0 ? self[left] : self[self.Count - 1];
                n.selectOnRight = right < self.Count ? self[right] : self[0];
                self[i].navigation = n;
            }
        }

        public static void SetNavigationHorizontal(this IList<Button> self)
        {
            SetNavigationHorizontal((IList<Selectable>)self);
        }
    }
}
