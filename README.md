# AbcConsole
Mobile-friendly debug console system.  
UNDER DEVELOPMENT

<a href="https://www.buymeacoffee.com/kyubuns" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/default-orange.png" alt="Buy Me A Coffee" height="41" width="174"></a>

## Instructions

- Package Manager
  - Import [AnKuchen](https://github.com/kyubuns/AnKuchen) `https://github.com/kyubuns/AnKuchen.git?path=Unity/Assets/AnKuchen`
  - Import AbcConsole `https://github.com/kyubuns/AbcConsole.git?path=Assets/AbcConsole`

## How to use

### Setup

Assets > Create > AbcConsole to Startup Scene.  
You can open AbcConsole, Left Top Button.

### Define debug command

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

## FAQ

### I want to change the color and location of the TriggerButton.

You can change AbcConsole GameObject.  
My recommendation is to make the color of AbcConsole/Canvas/TriggerButton/Base transparent.

### I want to turn it off when I do a production build.

Delete AbcConsole GameObject.

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

## Buy me a coffee

Are you enjoying save time?  
Buy me a coffee if you love my code!  
https://www.buymeacoffee.com/kyubuns

## "I used it for this game!"

I'd be happy to receive reports like "I used it for this game!"  
Please contact me by email, twitter or any other means.  
(This library is MIT licensed, so reporting is NOT mandatory.)  
https://kyubuns.dev/

