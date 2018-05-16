# Monogame/Crux
This addon contains the set of various tools for basic UI and text formatting on MonoGame.
# StringBuilder sample
```c#
var t = "{blue}MonoGame is free {green}software used by \ngame {blue:p} developers" +
+ "to make their {blue:h}Windows \nand Windows Phone games run on other systems.";
var sb = StringBuilder(font, t, new Vector2());
sb.Render(spriteBatch);
```
The result is shown below:

![Sample#1](https://image.ibb.co/esJx5d/2018_05_10_18_54_34.gif)
