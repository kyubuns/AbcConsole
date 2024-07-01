# AbcConsole
Mobile-friendly debug console!

![output](https://user-images.githubusercontent.com/961165/108616000-c10b1e80-744c-11eb-9fed-af97f620c6b7.gif)

## Features

### Check and copy logs

### Autocomplete for mobile comfort

## Description

It is simple, but has all the necessary elements.  
Check and copy logs generated on the actual machine.  
You can define your own debug methods, and enter and execute them.  
You only need to enter a few characters of the method name, and you can comfortably use AUTOCOMPLETE to enter the name!!

Checked by iOS, Android.  
(On Android, due to Unity's limitations, you need to close the keyboard and then select ui buttons.)  
Of course, it can also be used with Editor, Windows, and Mac.  
You can start it with the tilde key, and the Tab key, up and down keys, etc. also work like a common console.

## Instructions

- Package Manager
  - Import [AnKuchen](https://github.com/kyubuns/AnKuchen) `https://github.com/kyubuns/AnKuchen.git?path=Unity/Assets/AnKuchen`
  - Import AbcConsole `https://github.com/kyubuns/AbcConsole.git?path=Assets/AbcConsole`

## How to use

### Setup

Place the AbcConsole GameObject in your first scene from the menu Assets > Create > AbcConsole.  
You can open the AbcConsole in the upper left button.

### Define debug command

All you have to do is define the method with the AbcCommand Attribute.  
Primitive types can be used as arguments.

```csharp
public static class OriginalCommand
{
    [AbcCommand("Show App Version")]
    public static void AppVersion()
    {
        Debug.Log($"AppVersion is {Application.version}");
    }

    [AbcCommand("Message To DebugLog")]
    public static void Echo(string message)
    {
        Debug.Log(message);
    }
}
```

### FPS Counter

The FPS Counter is included in AbcConsole.  
You can view it by typing [FpsCounter] in the Console.  
In an environment where [FrameTimingManager](https://docs.unity3d.com/ja/2020.3/ScriptReference/PlayerSettings-enableFrameTimingStats.html) is enabled, more detailed information will be displayed.

<img width="843" alt="Screen Shot 2021-09-23 at 18 14 31" src="https://user-images.githubusercontent.com/961165/134482324-54f68469-5ed0-4f0c-891c-693078391f1a.png">

## FAQ

### I want to change the color and location of the TriggerButton.

You can change AbcConsole GameObject.  
My recommendation is to make the color of AbcConsole/Canvas/TriggerButton/Base transparent.

### I want to turn it off when I do a production build.

Delete AbcConsole GameObject.  
Or, Instantiate only when necessary, as follows. (Recommend)

```csharp
[Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
private void CreateAbcConsoleForDebug()
{
    Instantiate(abcConsolePrefab);
}
```

### I want to use a trigger other than the Button.

Disable AbcConsole/Canvas/TriggerButton Object.  
You can also use the OnClickTriggerButton method of the Root Component (which AbcConsole has).

## Requirements

- Requires Unity2019.4 or later

## License

MIT License (see [LICENSE](LICENSE))

## SpecialThanks

- nkjzm/UnityMobileKeyboardSample
  - https://github.com/nkjzm/UnityMobileKeyboardSample

