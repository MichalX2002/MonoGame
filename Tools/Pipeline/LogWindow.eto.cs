using Eto.Forms;
using Eto.Drawing;

namespace MonoGame.Tools.Pipeline
{
    public partial class LogWindow : Form
    {
        private TextArea _textAreaLog;
        private Button _buttonCopy;
        
        private void InitializeComponent()
        {
            Title = "Crash Report";
            Size = new Size(800, 400);

            var layout1 = new DynamicLayout
            {
                Padding = new Padding(4),
                Spacing = new Size(4, 4)
            };

            var layout2 = new DynamicLayout();
            layout2.BeginHorizontal();

            var label1 = new Label
            {
                VerticalAlignment = VerticalAlignment.Bottom,
                Text = "Pipeline Tool has crashed, please copy the following exception when reporting the error: "
            };
            layout2.Add(label1, true, true);

            _buttonCopy = new Button
            {
                Text = "Copy to Clipboard"
            };

            if (!Global.UseHeaderBar)
                layout2.Add(_buttonCopy, false, true);

            layout1.Add(layout2, true, false);

            _textAreaLog = new TextArea
            {
                ReadOnly = true,
                Wrap = false
            };
            layout1.Add(_textAreaLog, true, true);

            Content = layout1;

            Closed += LogWindow_Closed;
            _buttonCopy.Click += ButtonCopy_Click;
        }

    }
}
