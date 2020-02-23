# UniversalWPF
A set of WPF Controls ported from Windows Universal

NOTE: This is mostly a work in progress. TwoPaneView is fully working, but might have issues on Windows X, as the necessary APIs there to do screen spanning are not yet exposed.
State Triggers are not working yet. RelativePanel _should_ work but needs lots of testing (please help!), SplitView is partially working, but needs some work still.

# NuGet / Usage

Install this from NuGet:

> `Install-Package UniversalWPF`

No xmlns registration needed in your xaml files! You can just use the controls prefix-less like the built-in controls. Example:

```xml
<TwoPaneView>
  <TwoPaneView.Pane1>
    <RelativePanel>
      <NumberBox />
    </RelativePanel>
  </TwoPaneView.Pane1>
</TwoPaneView>
```

## Sponsoring

If you like this library and use it a lot, consider sponsoring me. Anything helps and encourages me to keep going.

See here for details: https://github.com/sponsors/dotMorten

### Controls
 - [TwoPaneView](https://docs.microsoft.com/en-us/windows/uwp/design/controls-and-patterns/two-pane-view) - A full port of UWP's TwoPaneView control, including support the Windows X Dual-screen devices.
 
![TwoPaneView](https://user-images.githubusercontent.com/1378165/74808461-c238c700-529f-11ea-93c5-33ca1063f8fd.gif)

 - [NumberBox](https://docs.microsoft.com/en-us/windows/uwp/design/controls-and-patterns/number-box) - A full port of UWP's NumberBox control.

![numberbox](https://user-images.githubusercontent.com/1378165/75103965-70ea4980-55b7-11ea-843d-57dcc021053f.gif)

 - [RelativePanel](https://docs.microsoft.com/en-us/uwp/api/Windows.UI.Xaml.Controls.RelativePanel) - Fully implemented but needs more testing (please help and report any rendering differences between this and Universal's RelativePanel). Want to see this built-in in WPF? [Vote here](https://github.com/dotnet/wpf/issues/112)
 
 ![RelativePanel](https://cloud.githubusercontent.com/assets/1378165/10120048/b76250f0-645e-11e5-9b4d-2a0d7026a467.gif)

### In Progress

 - SplitView (Very much work in progress - doesn't animate in/out and closed compact mode isn't rendering)

 - StateTrigger / AdaptiveTrigger (API complete, functionality not so much)
 
 ![image](https://cloud.githubusercontent.com/assets/1378165/10121609/94743df6-64a9-11e5-9908-29c0aeaf3c7f.png)

 
