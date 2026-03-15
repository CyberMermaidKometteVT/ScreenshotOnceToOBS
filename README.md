This is a Streamer.bot subcommand.

When fired off, your screen will freeze, and you'll drag-select a region of the screen with your mouse. The region selected will be put into a screenshot and added to the current OBS scene.

If you hit ESC instead while the screen is frozen, the operation will be canceled.

Note that this requires Streamer.bot to function.

DEV TO DO:
- I should really add unit tests.


FOR INITIAL DEPLOYMENT:
- Create a new Action in Streamer.bot
- Create a new "C# Method" sub-Action in Streamer.bot
  - Click the "Settings" tab near the bottom, then fill in something meaningful under Name. Mine is "ScreenshotToOBSOnce".
  - Copy all classes from CPHInline.cs to your Streamer.bot "C# Method" subcommand.
  - Copy all classes from ScreenshotSnder.cs to your Streamer.bot "C# Method" subcommand at the bottom.
  - Copy all classes and enums from SnippingToolProcessWatcher.cs to your Streamer.bot "C# Method" subcommand at the bottom.
  - Click "Find Refs" in the Sub-Action, which will add most of the references
  - Add a reference to Microsoft.CSharp.dll
    - Click the "References" tab near the bottom
    - Right-click any line in the "References" tab and click "Add Reference from file..."
    - Add "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\Microsoft.CSharp.dll"
   - Click "Compile" near the bottom of the sub-command, and make sure there are no errors.
   - Click the "OK" button
- Add a Trigger to your Action.
  - I'm not much use here, because I haven't done that. I use a third-party tool to make an API call to Streamer.bot to trigger it.
      it's a pain and is out of scope for this readme, but feel free to poke me to ask me to document the process somewhere else.

TO DEPLOY UPDATES:
- Copy all classes from CPHInline.cs to your Streamer.bot "C# Method" subcommand.
- Copy all classes from ScreenshotSnder.cs to your Streamer.bot "C# Method" subcommand at the bottom.
- Copy all classes and enums from SnippingToolProcessWatcher.cs to your Streamer.bot "C# Method" subcommand at the bottom.
- Click "Compile" near the bottom of the sub-command, and make sure there are no errors.
- Click the "OK" button