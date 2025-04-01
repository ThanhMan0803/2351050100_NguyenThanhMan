using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
namespace WindowsFormsApp1
{
    public partial class MainForm : Form
    {
        private bool fileAlreadySaved;      // Kiểm tra file đã được lưu chưa
        private bool fileUpdated;           // Kiểm tra file có thay đổi chưa lưu
        private string currentFileName;     // Lưu tên file hiện tại
        private FontDialog fontDialog = new FontDialog(); // Hộp thoại chọn font
        //Print
        private PrintDocument printDocument = new PrintDocument();
        private PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
        //Dark mode
        private bool isDarkMode = false;

        public MainForm()
        {
            InitializeComponent();
        }



        private void fIleToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        //About
        private void wordPadByThanhMẫnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Đây là WordPad của Thanh Mẫn\n \n  Đại học Mở TP.Hồ Chí Minh", "WordPad: ", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        //Exit
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn chắc chắn muốn thoát!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
        //Nút New
        //Kiểm tra người dùng có muốn lưu những thay đổi khi mở 1 file
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newMenuMethod();
        }

        private void newMenuMethod()
        {
            if (fileUpdated)
            {
                DialogResult result = MessageBox.Show("Bạn có muốn lưu những thay đổi?", "File Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                switch (result)
                {
                    case DialogResult.Yes:
                        SaveFileUpdated();
                        ClearScreen();
                        break;
                    case DialogResult.No:
                        ClearScreen();
                        break;


                }
            }
            else
            {
                ClearScreen();
            }
            undoToolStripMenuItem.Enabled = false;
            redoToolStripMenuItem.Enabled = false;
            toolStripUndo.Enabled = false;
            toolStripRedo.Enabled = false;

            undoToolStripMenuItem1.Enabled = false;
            redoToolStripMenuItem1.Enabled = true;

            MessagetoolStripStatusLabel.Text = "Đã tạo mới thành công! ";
        }

        //Mở File
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openMenuMethod();

        }

        private void openMenuMethod()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|Rich Text Files(*.rtf)|*.rtf";
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (Path.GetExtension(openFileDialog.FileName) == ".txt")
                {
                    using (StreamReader sr = new StreamReader(openFileDialog.FileName, Encoding.UTF8)) // Thử UTF-8
                    {
                        mainRichTextBox.Text = sr.ReadToEnd();
                    }
                }


                if (Path.GetExtension(openFileDialog.FileName) == ".rtf")
                {
                    mainRichTextBox.LoadFile(openFileDialog.FileName, RichTextBoxStreamType.RichText);
                }
                this.Text = Path.GetFileName(openFileDialog.FileName) + " - WordPad";
                fileAlreadySaved = true;
                fileUpdated = false;
                currentFileName = openFileDialog.FileName;
                MessagetoolStripStatusLabel.Text = "File đang mở: " + Path.GetFileName(openFileDialog.FileName);

            }
            //Tắt undo redo khi mở file
            undoToolStripMenuItem.Enabled = false;
            redoToolStripMenuItem.Enabled = false;
            toolStripUndo.Enabled = false;
            toolStripRedo.Enabled = false;
            undoToolStripMenuItem1.Enabled = false;
            redoToolStripMenuItem1.Enabled = true;

        }

        //Nút SaveAs
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            savefile();

        }

        private void savefile()
        {// Tạo hộp thoại lưu file mới
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            // Thiết lập bộ lọc định dạng file 
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|Rich Text Files (*.rtf)|*.rtf";

            // Hiển thị hộp thoại và chờ người dùng chọn
            DialogResult SaveFileResult = saveFileDialog.ShowDialog();

            // Nếu người dùng chọn Save (không phải Cancel)
            if (SaveFileResult == DialogResult.OK)
            {
                // Xử lý lưu file .txt
                if (Path.GetExtension(saveFileDialog.FileName) == ".txt")
                {
                    // Sử dụng StreamWriter để ghi nội dung dạng plain text
                    // - false: ghi đè file nếu đã tồn tại
                    // - UTF8: hỗ trợ Unicode (Tiếng Việt, ký tự đặc biệt)
                    using (StreamWriter writer = new StreamWriter(
                        saveFileDialog.FileName,
                        false,
                        System.Text.Encoding.UTF8))
                    {
                        writer.Write(mainRichTextBox.Text);
                    }
                }

                // Xử lý lưu file .rtf
                if (Path.GetExtension(saveFileDialog.FileName) == ".rtf")
                {
                    // Sử dụng phương thức SaveFile của RichTextBox
                    // để giữ lại định dạng (font, màu sắc, căn lề...)
                    mainRichTextBox.SaveFile(
                        saveFileDialog.FileName,
                        RichTextBoxStreamType.RichText);
                }

                // Cập nhật giao diện và trạng thái:
                // 1. Đổi tiêu đề cửa sổ (VD: "document1.txt - WordPad")
                this.Text = Path.GetFileName(saveFileDialog.FileName) + " - WordPad";

                // 2. Đánh dấu file đã được lưu
                fileAlreadySaved = true;

                // 3. Đặt lại trạng thái "thay đổi chưa lưu"
                fileUpdated = false;

                // 4. Lưu đường dẫn file cho lần save tiếp theo
                currentFileName = saveFileDialog.FileName;
            }
        }


        //FormLoad
        private void MainForm_Load(object sender, EventArgs e)
        {
            fileAlreadySaved = false;
            fileUpdated = false;
            currentFileName = "";
            CapsLockStripStatusLabel.Text = Control.IsKeyLocked(Keys.CapsLock) ? "CAPS LOCK ĐANG BẬT!" : "Caps Lock tắt";
            isDarkMode = Properties.Settings.Default.DarkMode;
            ApplyDarkMode();
        }


        private void mainRichTextBox_TextChanged(object sender, EventArgs e)
        {

            fileUpdated = true;
            undoToolStripMenuItem.Enabled = true;
            toolStripUndo.Enabled = true;
            undoToolStripMenuItem1.Enabled = true;
            redoToolStripMenuItem1.Enabled = false;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileUpdated();
        }

        private void SaveFileUpdated()
        {
            if (fileAlreadySaved) // Nếu file đã được lưu trước đó
            {
                if (Path.GetExtension(currentFileName) == ".txt")
                {
                    mainRichTextBox.SaveFile(currentFileName, RichTextBoxStreamType.PlainText); // Lưu dưới dạng .txt
                }
                if (Path.GetExtension(currentFileName) == ".rtf")
                {
                    mainRichTextBox.SaveFile(currentFileName, RichTextBoxStreamType.RichText); // Lưu dưới dạng .rtf
                }
                fileUpdated = false; // Đánh dấu file đã được lưu
            }
            else // Nếu file chưa từng được lưu (chưa có tên file)
            {
                if (fileUpdated) // Nếu có thay đổi chưa lưu
                {
                    savefile(); // Gọi Save As để chọn vị trí lưu
                }
                else
                {
                    ClearScreen(); // Xóa nội dung nếu không cần lưu
                }
            }
        }

        //ClearScreen
        private void ClearScreen()
        {
            mainRichTextBox.Clear();
            fileUpdated = false;
            this.Text = "WordPad";
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {

            // Kiểm tra nếu RichTextBox có thể hoàn tác 
            if (mainRichTextBox.CanUndo)
            {
                // Thực hiện thao tác Undo 
                mainRichTextBox.Undo();

                // Cập nhật trạng thái của các nút Undo và Redo dựa vào khả năng hoàn tác/làm lại
                undoToolStripMenuItem.Enabled = mainRichTextBox.CanUndo; // Bật/tắt nút Undo trên menu
                toolStripUndo.Enabled = mainRichTextBox.CanUndo; // Bật/tắt nút Undo trên thanh công cụ
                redoToolStripMenuItem.Enabled = mainRichTextBox.CanRedo; // Bật/tắt nút Redo trên menu
                toolStripRedo.Enabled = mainRichTextBox.CanRedo; // Bật/tắt nút Redo trên thanh công cụ
            }
        }
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainRichTextBox.SelectAll();
        }
        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Kiểm tra nếu RichTextBox có thể làm lại 
            if (mainRichTextBox.CanRedo)
            {
                // Thực hiện thao tác Redo 
                mainRichTextBox.Redo();

                // Cập nhật trạng thái của các nút Undo và Redo sau khi thực hiện Redo
                undoToolStripMenuItem.Enabled = mainRichTextBox.CanUndo; // Bật/tắt nút Undo trên menu
                toolStripUndo.Enabled = mainRichTextBox.CanUndo; // Bật/tắt nút Undo trên thanh công cụ
                redoToolStripMenuItem.Enabled = mainRichTextBox.CanRedo; // Bật/tắt nút Redo trên menu
                toolStripRedo.Enabled = mainRichTextBox.CanRedo; // Bật/tắt nút Redo trên thanh công cụ
            }
        }
        //DateTime
        private void dateTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {

            mainRichTextBox.SelectedText = DateTime.Now.ToString();
        }

        //In đậm,Nghiêng,gạch,FONT...(viết hàm)
        private void FontTextStyle(FontStyle fontStyle)
        {
            mainRichTextBox.SelectionFont = new Font(mainRichTextBox.Font, fontStyle);
        }
        private void boldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontTextStyle(FontStyle.Bold);
        }

        private void italicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontTextStyle(FontStyle.Italic);
        }

        private void underlineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontTextStyle(FontStyle.Underline);
        }

        private void strikeThroughToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontTextStyle(FontStyle.Strikeout);
        }

        private void normalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontTextStyle(FontStyle.Regular);
        }


        //FONT
        private void formatFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatFont();

        }

        private void formatFont()
        {
            // Cho phép chọn màu chữ trong hộp thoại FontDialog
            fontDialog.ShowColor = true;

            // Hiển thị nút "Apply" để áp dụng font mà không cần đóng hộp thoại
            fontDialog.ShowApply = true;

            // Gán sự kiện Apply để xử lý thay đổi khi nhấn nút "Apply"
            fontDialog.Apply += new System.EventHandler(font_Apply_Dialog);

            // Hiển thị hộp thoại FontDialog và lấy kết quả
            DialogResult result = fontDialog.ShowDialog();

            // Kiểm tra nếu người dùng nhấn OK
            if (result == DialogResult.OK)
            {
                // Kiểm tra nếu có văn bản được chọn trong RichTextBox
                if (mainRichTextBox.SelectionLength > 0)
                {
                    // Áp dụng font đã chọn vào phần văn bản được chọn
                    mainRichTextBox.SelectionFont = fontDialog.Font;

                    // Áp dụng màu chữ đã chọn vào phần văn bản được chọn
                    mainRichTextBox.SelectionColor = fontDialog.Color;
                }
            }
        }


        private void font_Apply_Dialog(object sender, EventArgs e)
        {
            if (mainRichTextBox.SelectionLength > 0)
            {
                mainRichTextBox.SelectionColor = fontDialog.Color;
            }
        }

        //Màu chữ
        private void changeTextCorlorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeCorlor();

        }

        private void ChangeCorlor()
        {
            ColorDialog colorDialog = new ColorDialog();
            DialogResult result = colorDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (mainRichTextBox.SelectionLength > 0)
                {
                    mainRichTextBox.SelectionColor = colorDialog.Color;
                }
            }
        }

        //ToolStrip

        private void toolStripNew_Click(object sender, EventArgs e)
        {
            newMenuMethod();
        }

        private void toolStripOpen_Click(object sender, EventArgs e)
        {
            openMenuMethod();
        }

        private void toolStripSave_Click(object sender, EventArgs e)
        {
            SaveFileUpdated();
        }

        private void toolStripSaveAs_Click(object sender, EventArgs e)
        {
            savefile();
        }


        //7_Undo
        // Xử lý sự kiện khi nhấn nút "Undo" trên ToolStrip
        private void toolStripUndo_Click(object sender, EventArgs e)
        {
            // Kiểm tra nếu RichTextBox có thể hoàn tác 
            if (mainRichTextBox.CanUndo)
            {
                // Thực hiện thao tác Undo 
                mainRichTextBox.Undo();

                // Cập nhật trạng thái của các nút Undo và Redo dựa vào khả năng hoàn tác/làm lại
                undoToolStripMenuItem.Enabled = mainRichTextBox.CanUndo; // Bật/tắt nút Undo trên menu
                toolStripUndo.Enabled = mainRichTextBox.CanUndo; // Bật/tắt nút Undo trên thanh công cụ
                redoToolStripMenuItem.Enabled = mainRichTextBox.CanRedo; // Bật/tắt nút Redo trên menu
                toolStripRedo.Enabled = mainRichTextBox.CanRedo; // Bật/tắt nút Redo trên thanh công cụ
            }
        }

        // Xử lý sự kiện khi nhấn nút "Redo" trên ToolStrip
        private void toolStripRedo_Click(object sender, EventArgs e)
        {
            // Kiểm tra nếu RichTextBox có thể làm lại 
            if (mainRichTextBox.CanRedo)
            {
                // Thực hiện thao tác Redo 
                mainRichTextBox.Redo();

                // Cập nhật trạng thái của các nút Undo và Redo sau khi thực hiện Redo
                undoToolStripMenuItem.Enabled = mainRichTextBox.CanUndo; // Bật/tắt nút Undo trên menu
                toolStripUndo.Enabled = mainRichTextBox.CanUndo; // Bật/tắt nút Undo trên thanh công cụ
                redoToolStripMenuItem.Enabled = mainRichTextBox.CanRedo; // Bật/tắt nút Redo trên menu
                toolStripRedo.Enabled = mainRichTextBox.CanRedo; // Bật/tắt nút Redo trên thanh công cụ
            }
        }



        private void toolStripFontChange_Click(object sender, EventArgs e)
        {
            formatFont();
        }

        private void toolStripColor_Click(object sender, EventArgs e)
        {
            ChangeCorlor();
        }

        private void toolStripFnormal_Click(object sender, EventArgs e)
        {
            FontTextStyle(FontStyle.Regular);
        }

        private void toolStripfBold_Click(object sender, EventArgs e)
        {
            FontTextStyle(FontStyle.Bold);
        }

        private void toolStripfItalic_Click(object sender, EventArgs e)
        {
            FontTextStyle(FontStyle.Italic);
        }

        private void toolStripfUnderline_Click(object sender, EventArgs e)
        {
            FontTextStyle(FontStyle.Underline);
        }

        private void toolStripfStrike_Click(object sender, EventArgs e)
        {
            FontTextStyle(FontStyle.Strikeout);
        }

        private void toolStripPrintPV_Click(object sender, EventArgs e)
        {
            Print();
        }

        //Capslk ở mainRichtextBox
        private void mainRichTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            CapsLockStripStatusLabel.Text = Control.IsKeyLocked(Keys.CapsLock) ? "CAPS LOCK ĐANG BẬT!" : "Caps Lock tắt";
        }
        //Context Menu Strip
        private void undoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            mainRichTextBox.Undo();
            undoToolStripMenuItem.Enabled = false;
            toolStripUndo.Enabled = false;
            redoToolStripMenuItem.Enabled = true;
            toolStripRedo.Enabled = true;
            undoToolStripMenuItem1.Enabled = false;
            redoToolStripMenuItem1.Enabled = true;
        }

        private void redoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            mainRichTextBox.Redo();
            undoToolStripMenuItem.Enabled = true;
            toolStripUndo.Enabled = true;
            redoToolStripMenuItem.Enabled = false;
            toolStripRedo.Enabled = false;
            undoToolStripMenuItem1.Enabled = true;
            redoToolStripMenuItem1.Enabled = false;
        }

        private void normalToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FontTextStyle(FontStyle.Regular);
        }

        private void boldToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FontTextStyle(FontStyle.Bold);
        }

        private void italicToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FontTextStyle(FontStyle.Italic);
        }

        private void underlineToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FontTextStyle(FontStyle.Underline);
        }
        //Cut, Copy, Paste
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mainRichTextBox.SelectionLength > 0)
            {
                Clipboard.SetText(mainRichTextBox.SelectedText);
                mainRichTextBox.SelectedText = "";
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mainRichTextBox.SelectionLength > 0)
            {
                Clipboard.SetText(mainRichTextBox.SelectedText);

            }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                mainRichTextBox.SelectedText = Clipboard.GetText();
            }
        }
        ///---------------------------------------------------------------------------------

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (mainRichTextBox.SelectionLength > 0)
            {
                Clipboard.SetText(mainRichTextBox.SelectedText);
                mainRichTextBox.SelectedText = "";
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (mainRichTextBox.SelectionLength > 0)
            {
                Clipboard.SetText(mainRichTextBox.SelectedText);

            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                mainRichTextBox.SelectedText = Clipboard.GetText();
            }
        }

        //----------------------------------------------------------------
        private void cutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (mainRichTextBox.SelectionLength > 0)
            {
                Clipboard.SetText(mainRichTextBox.SelectedText);
                mainRichTextBox.SelectedText = "";
            }
        }

        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (mainRichTextBox.SelectionLength > 0)
            {
                Clipboard.SetText(mainRichTextBox.SelectedText);

            }
        }

        private void pasteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                mainRichTextBox.SelectedText = Clipboard.GetText();
            }
        }
        //PRINT
        private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Lấy nội dung của mainRichTextBox
            string content = mainRichTextBox.Text;

            // Chọn font để in
            Font printFont = mainRichTextBox.Font;

            // Vẽ nội dung lên trang in
            e.Graphics.DrawString(content, printFont, Brushes.Black, new RectangleF(50, 50, e.PageBounds.Width - 100, e.PageBounds.Height - 100));

            // Không có trang tiếp theo
            e.HasMorePages = false;
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Print();
        }

        private void Print()
        {
            // Gán sự kiện PrintPage
            printDocument.PrintPage += new PrintPageEventHandler(printDocument_PrintPage);

            // Thiết lập tài liệu cần xem trước
            printPreviewDialog.Document = printDocument;

            // Hiển thị hộp thoại xem trước
            printPreviewDialog.ShowDialog();
        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Print();
        }

        private void toolStripPrint_Click(object sender, EventArgs e)
        {
            Print();
        }


        //Căn giữa, trái phải
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            mainRichTextBox.SelectionAlignment = HorizontalAlignment.Center;
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            mainRichTextBox.SelectionAlignment = HorizontalAlignment.Left;
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            mainRichTextBox.SelectionAlignment = HorizontalAlignment.Right;
        }
        //newWindow
        private void OpenNewWindow()
        {
            try
            {
                Process.Start(Application.ExecutablePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể mở cửa sổ mới: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void newWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có muốn tạo cửa sổ mới ?", "New Window", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes) { OpenNewWindow(); }

        }
        //Dark mode
        private void ApplyDarkMode()
        {
            // Kiểm tra nếu Dark Mode đang được bật
            if (isDarkMode)
            {
                // Đặt màu nền của form chính thành màu đen
                this.BackColor = Color.Black;
                // Đặt màu chữ của form chính thành màu trắng
                this.ForeColor = Color.White;

                // Đặt màu nền của RichTextBox chính thành màu xám tối
                mainRichTextBox.BackColor = Color.FromArgb(30, 30, 30);
                // Đặt màu chữ trong RichTextBox thành màu trắng
                mainRichTextBox.ForeColor = Color.White;

                // Đặt màu nền của MenuStrip thành màu xám tối
                menuStrip1.BackColor = Color.FromArgb(45, 45, 48);
                // Đặt màu chữ trong MenuStrip thành màu trắng
                menuStrip1.ForeColor = Color.White;
            }
            else
            {
                // Nếu không phải Dark Mode, chuyển về chế độ sáng (Light Mode)

                // Đặt màu nền của form chính thành màu nền mặc định của hệ thống
                this.BackColor = SystemColors.Control;
                // Đặt màu chữ của form chính thành màu chữ mặc định của hệ thống
                this.ForeColor = SystemColors.ControlText;

                // Đặt màu nền của RichTextBox thành màu trắng
                mainRichTextBox.BackColor = Color.White;
                // Đặt màu chữ trong RichTextBox thành màu đen
                mainRichTextBox.ForeColor = Color.Black;

                // Đặt màu nền của MenuStrip thành màu nền mặc định của hệ thống
                menuStrip1.BackColor = SystemColors.Control;
                // Đặt màu chữ trong MenuStrip thành màu chữ mặc định của hệ thống
                menuStrip1.ForeColor = SystemColors.ControlText;
            }
        }

        // Sự kiện khi người dùng nhấn vào nút chuyển đổi Dark Mode
        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            // Đảo ngược trạng thái của Dark Mode
            isDarkMode = !isDarkMode;
            // Áp dụng chế độ Dark Mode hoặc Light Mode mới
            ApplyDarkMode();

            // Lưu trạng thái Dark Mode vào các cài đặt của ứng dụng
            Properties.Settings.Default.DarkMode = isDarkMode;
            // Lưu các thay đổi cài đặt
            Properties.Settings.Default.Save();
        }

    }
}