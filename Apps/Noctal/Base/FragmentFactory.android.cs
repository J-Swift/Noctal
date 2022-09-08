using Java.Lang;
using System.Diagnostics;

namespace Noctal;

public class FragmentFactory : AndroidX.Fragment.App.FragmentFactory
{
    public interface IFragmentFactoryOnCreateListener
    {
        AndroidX.Fragment.App.Fragment? OnCreateFragment(string className);
    }
    public interface IFragmentFactoryAfterCreateListener
    {
        void AfterCreateFragment(AndroidX.Fragment.App.Fragment fragment);
    }

    private readonly Dictionary<string, IFragmentFactoryOnCreateListener> onCreateListenerMap = new();
    private readonly Dictionary<Type, IFragmentFactoryAfterCreateListener> afterCreateListenerMap = new();

    public override AndroidX.Fragment.App.Fragment Instantiate(ClassLoader classLoader, string className)
    {
        AndroidX.Fragment.App.Fragment? frag = null;
        if (onCreateListenerMap.TryGetValue(className, out var onCreateListener))
        {
            frag = onCreateListener.OnCreateFragment(className);
        }

        if (frag is null)
        {
            frag = base.Instantiate(classLoader, className);
        }

        if (afterCreateListenerMap.TryGetValue(frag.GetType(), out var afterCreateListener))
        {
            afterCreateListener.AfterCreateFragment(frag);
        }

        return frag;
    }

    public void RegisterOnCreateListener(Type type, IFragmentFactoryOnCreateListener listener)
    {
        var className = Java.Lang.Class.FromType(type).Name;
        Debug.Assert(!onCreateListenerMap.ContainsKey(className), $"[{className}] already registered");
        onCreateListenerMap[className] = listener;
    }

    public void RegisterAfterCreateListener(Type type, IFragmentFactoryAfterCreateListener listener)
    {
        Debug.Assert(!afterCreateListenerMap.ContainsKey(type), $"[{type}] already registered");
        afterCreateListenerMap[type] = listener;
    }
}
