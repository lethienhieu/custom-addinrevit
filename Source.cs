using System;
using System.Reflection;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RvtException = Autodesk.Revit.Exceptions;
// Alias các control WinForms để tránh trùng tên với Autodesk.Revit.UI / DB
using Form = System.Windows.Forms.Form;
using TextBox = System.Windows.Forms.TextBox;
using Label = System.Windows.Forms.Label;
using Button = System.Windows.Forms.Button;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace TuanDimensionBelow
{
    // ---------------------------------------------------------------------
    //  ExternalApplication: tạo tab "TUAN" + 1 button trên ribbon
    // ---------------------------------------------------------------------
    public class App : IExternalApplication
    {
        private const string TabName = "TUAN";

        public Result OnStartup(UIControlledApplication application)
        {
            // Tạo tab (bọc try/catch phòng khi tab đã tồn tại)
            try { application.CreateRibbonTab(TabName); } catch { }

            RibbonPanel panel = application.CreateRibbonPanel(TabName, "Dimension Tools");

            string assemblyPath = Assembly.GetExecutingAssembly().Location;

            PushButtonData btnData = new PushButtonData(
                "TuanDimBelowBtn",
                "Set\nBelow Text",
                assemblyPath,
                "TuanDimensionBelow.SetBelowCommand");

            btnData.ToolTip =
                "Nhập 1 đoạn text, nhấn Apply, rồi click liên tiếp vào từng dimension " +
                "để điền text đó vào phần Below. Nhấn ESC để dừng.";

            panel.AddItem(btnData);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }

    // ---------------------------------------------------------------------
    //  ExternalCommand: lệnh duy nhất
    // ---------------------------------------------------------------------
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class SetBelowCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            if (uidoc == null)
            {
                message = "Không có tài liệu nào đang mở.";
                return Result.Cancelled;
            }
            Document doc = uidoc.Document;

            // 1) Hộp thoại nhập text
            string belowText;
            using (TextInputForm form = new TextInputForm())
            {
                if (form.ShowDialog() != DialogResult.OK)
                    return Result.Cancelled;          // đóng / hủy hộp thoại
                belowText = form.InputText;
            }

            // 2) Vòng lặp pick dimension -> điền Below -> lặp tiếp, ESC để dừng
            int count = 0;
            while (true)
            {
                Reference pickedRef;
                try
                {
                    pickedRef = uidoc.Selection.PickObject(
                        ObjectType.Element,
                        new DimensionSelectionFilter(),
                        "Click vào dimension để điền Below (ESC = dừng)");
                }
                catch (RvtException.OperationCanceledException)
                {
                    break;                            // người dùng nhấn ESC
                }

                Dimension dim = doc.GetElement(pickedRef) as Dimension;
                if (dim == null) continue;

                using (Transaction t = new Transaction(doc, "Set Dimension Below"))
                {
                    t.Start();
                    ApplyBelow(dim, belowText);
                    t.Commit();
                }
                count++;
            }

            if (count > 0)
                TaskDialog.Show("TUAN", $"Đã điền Below cho {count} dimension.");

            return Result.Succeeded;
        }

        /// <summary>
        /// Điền text vào phần Below của dimension.
        /// Dimension 1 đoạn: dùng thuộc tính Below của chính nó.
        /// Dimension nhiều đoạn: gán Below cho mọi segment (fallback an toàn).
        /// </summary>
        private static void ApplyBelow(Dimension dim, string text)
        {
            if (dim.NumberOfSegments == 0)
            {
                dim.Below = text;
            }
            else
            {
                foreach (DimensionSegment seg in dim.Segments)
                    seg.Below = text;
            }
        }
    }

    // ---------------------------------------------------------------------
    //  Filter: chỉ cho phép chọn Dimension
    // ---------------------------------------------------------------------
    public class DimensionSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem) => elem is Dimension;
        public bool AllowReference(Reference reference, XYZ position) => false;
    }

    // ---------------------------------------------------------------------
    //  Hộp thoại nhập text (WinForms)
    // ---------------------------------------------------------------------
    public class TextInputForm : Form
    {
        private TextBox _txt;
        public string InputText { get; private set; } = string.Empty;

        public TextInputForm()
        {
            Text = "TUAN - Below Text";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new System.Drawing.Size(360, 120);

            Label lbl = new Label
            {
                Text = "Nhập text cho phần Below:",
                Left = 12,
                Top = 12,
                Width = 336
            };

            _txt = new TextBox
            {
                Left = 12,
                Top = 36,
                Width = 336
            };

            Button btnApply = new Button
            {
                Text = "Apply",
                Left = 192,
                Top = 74,
                Width = 75,
                DialogResult = DialogResult.OK
            };
            btnApply.Click += (s, e) => { InputText = _txt.Text; };

            Button btnCancel = new Button
            {
                Text = "Cancel",
                Left = 273,
                Top = 74,
                Width = 75,
                DialogResult = DialogResult.Cancel
            };

            Controls.Add(lbl);
            Controls.Add(_txt);
            Controls.Add(btnApply);
            Controls.Add(btnCancel);

            AcceptButton = btnApply;
            CancelButton = btnCancel;
        }
    }
}
