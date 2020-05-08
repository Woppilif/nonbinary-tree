using ListTreesLibrary;
using System;
using System.IO;
using System.Windows.Forms;
namespace Nonbinary_Tree
{
    public partial class Form1 : Form
    {
        private Tree<FileManager> vs;

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Примитивное деревце
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            RefreshTree();
            var root = new FileManager("Object");
            vs.Add(root, root);
            vs.Add(new FileManager("Class1"), root);
            vs.Add(new FileManager("Class2"), root);
            FillTree();
            FillGrid();
        }

        private void RefreshTree()
        {
            vs = null;
            vs = new Tree<FileManager>();
            vs.SetDeletable(new FileManager());
        }

        private void FillGrid()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Add("Parent", "Parent");
            dataGridView1.Columns.Add("Child", "Child");
            foreach (var item in vs.vs)
            {
                if (item != null)
                {

                    dataGridView1.Rows.Add(item.Value);
                    foreach (var child in item.Children)
                    {
                        if (child != null)
                        {
                            dataGridView1.Rows.Add("", child.Value);
                        }
                    }
                }
            }
            dataGridView1.ClearSelection();

        }

        /// <summary>
        /// Заполнение дерева
        /// </summary>
        private void FillTree()
        {
            treeView1.Nodes.Clear();
            TreeNode tree = new TreeNode();
            Add(tree, vs.vs);
            if (tree.Nodes.Count > 0)
            {
                treeView1.Nodes.Add(tree.Nodes[0]);
            }
            treeView1.Refresh();
            treeView1.ExpandAll();
        }

        /// <summary>
        /// Добавление к элементу потомка
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (newValue.Text == "")
            {
                return;
            }

            if (vs.Search(new FileManager(newValue.Text, false)) != null)
            {
                return;
            }

            if (treeView1.SelectedNode != null)
            {
                if (treeView1.Nodes.Count == 0)
                {
                    vs.Add(
                        new FileManager(newValue.Text),
                        new FileManager(newValue.Text)
                        );
                }
                else
                {
                    var res = vs.Search(new FileManager(treeView1.SelectedNode.Text, false));
                    vs.Add(
                        new FileManager(newValue.Text),
                        res
                        );
                }
            }
            else if (dataGridView1.SelectedCells != null && dataGridView1.SelectedCells.Count != 0)
            {
                if (dataGridView1.Rows.Count == 1)
                {
                    vs.Add(
                        new FileManager(newValue.Text),
                        new FileManager(newValue.Text)
                        );
                }
                else
                {
                    var res = vs.Search(new FileManager(dataGridView1.SelectedCells[0].Value.ToString(), false));
                    vs.Add(
                        new FileManager(newValue.Text),
                        res
                        );
                }
            }
            else
            {
                MessageBox.Show("Выберите хотя бы один элемент");
                return;
            }

            newValue.Text = string.Empty;
            treeView1.Refresh();
            FillGrid();
            FillTree();
        }

        /// <summary>
        /// Метод для рекурсивного добавления листьев
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="ts"></param>
        public void Add(TreeNode tree, CustomList<Parent<FileManager>> ts)
        {
            if (ts == null)
            {
                return;
            }
            if (ts.Value == null)
            {
                return;
            }

            var root = tree.Nodes.Add(ts.Value.Value.ToString());
            foreach (var item in ts.Value.Children)
            {
                var tempParent = ts;
                bool added = false;
                while (tempParent != null)
                {
                    if (item == null)
                    {
                        break;
                    }

                    if (tempParent.Value.Value.ToString() == item.Value.ToString())
                    {
                        Add(root, tempParent);
                        added = true;
                    }
                    tempParent = tempParent.Next;
                }
                if (added == false)
                {
                    if (item != null)
                    {
                        root.Nodes.Add(item.Value.ToString());
                    }
                }
            }
        }
        /// <summary>
        /// Удаление потомков или целых ветвей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                var res = vs.Search(new FileManager(treeView1.SelectedNode.Text, false));
                if (res == null)
                {
                    return;
                }
                vs.Delete(res);
                newValue.Text = string.Empty;
                FillGrid();
                FillTree();
                return;
            }
            else if (dataGridView1.SelectedCells != null && dataGridView1.SelectedCells.Count != 0)
            {
                var res = vs.Search(new FileManager(dataGridView1.SelectedCells[0].Value.ToString(), false));
                if (res == null)
                {
                    return;
                }
                vs.Delete(res);
                newValue.Text = string.Empty;
                FillGrid();
                FillTree();
                return;
            }
            else
            {
                MessageBox.Show("Выберите хотя бы один элемент");
                return;
            }
        }

        /// <summary>
        /// Сохраняем через виндусовый FileDialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = "C:\\Users\\%USERNAME%\\Desktop";
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(saveFileDialog1.OpenFile()))
                {
                    foreach (var item in vs.vs)
                    {
                        if (item != null)
                        {
                            sw.WriteLine($"p#{item.Value.ToString()}");
                            foreach (var child in item.Children)
                            {
                                if (child != null)
                                {
                                    sw.WriteLine($"c#{child.Value.ToString()}");
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Загрузка файла. По умолчанию открывается рабочий стол
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;
            RefreshTree();
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "C:\\Users\\%USERNAME%\\Desktop";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    string currentParent = "";
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        string line;
                        bool create;
                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] tempLine = line.Split('#');
                            create = true;
                            if (tempLine[0] == "p")
                            {
                                currentParent = tempLine[1];
                                if (vs.vs.Value == null)
                                {
                                    if (new FileManager(currentParent, false).FileExists() == true)
                                    {
                                        create = false;
                                    }
                                    vs.Add(new FileManager(currentParent, create), new FileManager(currentParent, create));
                                }
                            }
                            if (tempLine[0] == "c")
                            {
                                if (new FileManager(tempLine[1], false).FileExists() == true)
                                {
                                    create = false;
                                }
                                vs.Add(new FileManager(tempLine[1], create), new FileManager(currentParent, false));
                            }

                        }
                    }
                    FillTree();
                    FillGrid();
                }
            }

        }

        /// <summary>
        /// Двойной клик для вывода контента выбранного элемента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            var res = vs.Search(new FileManager(treeView1.SelectedNode.Text, false));
            if (res != null)
            {
                MessageBox.Show(res.Read());
            }
        }

        /// <summary>
        /// Поиск элемента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            if(newValue.Text == "")
            {
                return;
            }
            var res = vs.Search(new FileManager(newValue.Text, false));
            if (res != null)
            {
                MessageBox.Show(res.Read(),"Поиск успешен!");
            }
            else
            {
                MessageBox.Show("Ничего не найдено...", ":(");
            }
        }
    }
}
