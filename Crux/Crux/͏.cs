
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

#region Control Constructor

public _rtwcn()
        {
            AbsoluteX = 10; AbsoluteY = 10;
            Size = new Point(60, 40);
            BackColor = Palette.DarkenGray;
        }

        public _rtwcn(Vector4 posform, Color? col = default) : this(posform.X, posform.Y, posform.Z, posform.W, col) { }

        public _rtwcn(Vector2 pos, Vector2 size, Color? col = default) : this(pos.X, pos.Y, size.X, size.Y, col) { }

        public _rtwcn(float x, float y, float width, float height, Color? col = default)
        {
            ForeColor = Color.White;
            AbsoluteX = x; AbsoluteY = y;
            Size = new Point((int)width, (int)height);
            BackColor = col.HasValue ? col.Value : Palette.DarkenGray;
}
#endregion