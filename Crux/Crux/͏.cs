
#region Text alignment by container width
//spriteBatch.Begin(SpriteSortMode.Deferred);
//{
//    spriteBatch.DrawFill(reg, Color.White * 0.2f);

//    string ss = "aaaaaaaaa bbb ccccccc dddd eeeeeee";
//    var ws = ss.Split(' ');
//    var cur = new Vector2();
//    var csz = new Vector2();
//    var bw = reg.Width;

//    foreach (var w in ws)
//    {
//        var sz = font0.MeasureString(w);
//        csz += sz;
//    }

//    csz = new Vector2(csz.X, csz.Y);

//    var intr = (bw - csz.X) / ws.Length;

//    for (int i = 0; i < ws.Length; i++)
//    {
//        var s = ws[i];
//        var l = reg.Location.ToVector2();
//        var sz = font0.MeasureString(s);



//        l.X += cur.X + (intr) * i + (intr / ws.Length * i);

//        spriteBatch.DrawString(font0, s, l.Floor(), Color.White, 0, 1.0f);

//        cur += sz;
//    }
//}
//spriteBatch.End(); 
#endregion