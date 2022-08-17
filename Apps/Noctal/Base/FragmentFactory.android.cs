using Java.Lang;

namespace Noctal;

class FragmentFactory : AndroidX.Fragment.App.FragmentFactory
{
    public interface IFragmentFactoryListener
    {
        AndroidX.Fragment.App.Fragment? OnCreateFragment(string className);
    }
    private readonly Dictionary<string, IFragmentFactoryListener> listenerMap = new();

    public override AndroidX.Fragment.App.Fragment Instantiate(ClassLoader classLoader, string className)
    {
        if (listenerMap.TryGetValue(className, out var listener))
        {
            var frag = listener.OnCreateFragment(className);
            if (frag is not null)
            {
                return frag;
            }

        }

        return base.Instantiate(classLoader, className);
    }

    public void RegisterListener(Type type, IFragmentFactoryListener listener)
    {
        var className = Java.Lang.Class.FromType(type).Name;
        listenerMap[className] = listener;
    }
}
