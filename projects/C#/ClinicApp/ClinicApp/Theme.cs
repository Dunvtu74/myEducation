namespace ClinicApp
{
    public static class Theme
    {
        public static Color Background = Color.FromArgb(235, 235, 235);
        public static Color Surface = Color.White;
        public static Color Primary = Color.FromArgb(51, 102, 153);
        public static Color PrimaryHover = Color.FromArgb(40, 85, 130);
        public static Color TextMain = Color.FromArgb(0, 0, 0);
        public static Color TextGray = Color.FromArgb(100, 100, 100);
        public static Color Border = Color.FromArgb(180, 180, 180);

        public static Font FontMain = new Font("Arial", 9);
        public static Font FontBold = new Font("Arial", 9, FontStyle.Bold);
        public static Font FontSmall = new Font("Arial", 8);
        public static Font FontTitle = new Font("Arial", 11, FontStyle.Bold);

        public static void StyleButton(Button btn, bool danger = false)
        {
            btn.FlatStyle = FlatStyle.System;
            btn.BackColor = SystemColors.ButtonFace;
            btn.ForeColor = TextMain;
            btn.Font = FontMain;
            btn.Cursor = Cursors.Default;
            btn.UseVisualStyleBackColor = true;
        }

        public static void StyleButtonOutline(Button btn)
        {
            btn.FlatStyle = FlatStyle.System;
            btn.BackColor = SystemColors.ButtonFace;
            btn.ForeColor = TextMain;
            btn.Font = FontMain;
            btn.UseVisualStyleBackColor = true;
        }

        public static void StyleListView(ListView lv)
        {
            lv.BackColor = Surface;
            lv.ForeColor = TextMain;
            lv.Font = FontMain;
            lv.BorderStyle = BorderStyle.Fixed3D;
        }

        public static void StyleTextBox(TextBox tb)
        {
            tb.BorderStyle = BorderStyle.Fixed3D;
            tb.BackColor = Surface;
            tb.ForeColor = TextMain;
            tb.Font = FontMain;
        }
    }
}