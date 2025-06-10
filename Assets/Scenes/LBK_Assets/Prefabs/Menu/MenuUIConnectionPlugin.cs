using UnityEngine;
using Fusion.Menu;
using GodOfArcher;

namespace GodOfArcher
{
    public abstract class MenuConnectionPlugin : MonoBehaviour
    {
        public abstract IFusionMenuConnection Create(MenuConnectionBehaviour connectionBehaviour);
    }
}
