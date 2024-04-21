using RaktarKeszletDasHaus.Models;



namespace RaktarKeszletDasHaus
{



    public partial class Form1 : Form
    {
        private HCAllCategory selectedCategory;
        private TermekAdatok selectedTermek;
        private string selectedTermekName = string.Empty;
        private string selectedTermekSKU = string.Empty;
        private List<HCAllCategory> categories;
        private List<TermekAdatok> TermekekListaDataSource;
        private List<TermekAdatok> OriginalTermekLista;
        private BindingSource DGBindigSource;
        private bool selectionAllowed = false;
        private ApiDataManager apiDataManager;

        // DasHausColorPalette
        private Color DasHausBlue = Color.FromArgb(20, 33, 61);
        private Color DasHausYellow = Color.FromArgb(252, 163, 17);
        private Color DasHausGrey = Color.FromArgb(229, 229, 229);
        private Color DasHausBlack = Color.FromArgb(0, 0, 0);
        private Color DasHausWhite = Color.FromArgb(255, 255, 255);

        public Form1()
        {
            InitializeComponent();

            // Adat kont�nerek defini�l�sa
            apiDataManager = new ApiDataManager();
            TermekekListaDataSource = new List<TermekAdatok>();
            OriginalTermekLista = new List<TermekAdatok>();
            categories = new List<HCAllCategory>();
            selectedCategory = new HCAllCategory();
            selectedTermek = new TermekAdatok();

            // Kezdeti term�klista defini�l�sa
            this.categories = apiDataManager.Categories;
            this.OriginalTermekLista = apiDataManager.Products.ToList();
            this.TermekekListaDataSource = OriginalTermekLista.ToList();

            // Kateg�ri�k be�ll�t�sa
            comboBox1.DataSource = categories;
            comboBox1.ValueMember = "Bvin";
            comboBox1.DisplayMember = "Name";
            comboBox1.SelectedIndex = 0;

            // A t�bl�zat adatbek�t�se �s felt�lt�se
            DGBindigSource = new BindingSource();
            DGBindigSource.DataSource = TermekekListaDataSource;
            dataGridView1.DataSource = DGBindigSource;

            // A t�bl�zat megjelen�s�nek be�ll�t�sa
            SetDataGridView();


        }

        private void Form1_Load(object sender, EventArgs e)
        {

            // Define the border style of the form to a dialog box.
            FormBorderStyle = FormBorderStyle.FixedDialog;

            // Set the MaximizeBox to false to remove the maximize box.
            MaximizeBox = false;

            // Set the MinimizeBox to false to remove the minimize box.
            MinimizeBox = false;

            // Set the start position of the form to the center of the screen.
            StartPosition = FormStartPosition.CenterScreen;

            saveButton.BackColor = DasHausWhite;
            saveButton.ForeColor = DasHausBlack;
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            // Kateg�ria v�laszt�s�l term�klist�z�s a t�bl�zatban
            FilterResults();
        }

        private void panel4_MouseClick(object sender, MouseEventArgs e)
        {
            // Az adatok teljes �jrat�lt�se a kliensappba & inicializ�l�sa a formnak
            apiDataManager.GetData();

            this.categories = apiDataManager.Categories.ToList();
            this.OriginalTermekLista = apiDataManager.Products.ToList();
            this.TermekekListaDataSource = OriginalTermekLista.ToList();

            comboBox1.DataSource = categories;
            comboBox1.ValueMember = "Bvin";
            comboBox1.DisplayMember = "Name";
            comboBox1.SelectedIndex = 0;

            textBox1.Clear();
            textBox2.Clear();

            // T�bl�zat felt�lt�se adatokkal
            DGBindigSource.DataSource = TermekekListaDataSource;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            // A t�bl�zat egyik sor�nak kiv�laszt�sakor lefut� k�d
            if (selectionAllowed && (DataGridViewRow)dataGridView1.CurrentRow != null)
            {
                // Fels� egyedi term�kadatok s�v friss�t�se & tooltippek hozz�ad�sa
                DataGridViewRow tmp = (DataGridViewRow)dataGridView1.CurrentRow;
                selectedTermek.SKUColumn = tmp.Cells["SKUColumn"].Value.ToString();
                selectedTermek.BvinColumn = tmp.Cells["BvinColumn"].Value.ToString();
                selectedTermek.ListPriceColumn = Convert.ToDecimal(tmp.Cells["ListPriceColumn"].Value.ToString());
                selectedTermek.ProductNameColumn = tmp.Cells["ProductNameColumn"].Value.ToString();
                selectedTermek.CategoryColumn = tmp.Cells["CategoryColumn"].Value.ToString();
                termekNevL.Text = selectedTermek.ProductNameColumn;
                toolTip1.SetToolTip(termekNevL, termekNevL.Text);
                skuNevL.Text = selectedTermek.SKUColumn;
                toolTip1.SetToolTip(skuNevL, skuNevL.Text);
                kategoriaNevL.Text = selectedTermek.CategoryColumn;
                toolTip1.SetToolTip(kategoriaNevL, kategoriaNevL.Text);
                int tmpint = (int)selectedTermek.ListPriceColumn;
                arNevL.Text = tmpint.ToString() + " Ft";
                toolTip1.SetToolTip(arNevL, arNevL.Text);
                bvinNevL.Text = selectedTermek.BvinColumn;
                toolTip1.SetToolTip(bvinNevL, bvinNevL.Text);

                // K�szlet m�dos�t� mez�k felt�lt�se (online/bolti)
                tmp.Cells["LocalInventoryColumnTmp"].Value = tmp.Cells["LocalInventoryColumn"].Value;
                textBox3.Text = tmp.Cells["LocalInventoryColumnTmp"].Value.ToString();
                tmp.Cells["OnlineInventoryColumnTmp"].Value = tmp.Cells["OnlineInventoryColumn"].Value;
                textBox4.Text = tmp.Cells["OnlineInventoryColumnTmp"].Value.ToString();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Term�klista friss�t�s sz�vegv�ltoz�skor
            FilterResults();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // Term�klista friss�t�s sz�vegv�ltoz�skor
            FilterResults();
        }

        private void clear_productname_Click(object sender, EventArgs e)
        {
            // Term�kn�v mez�inek t�rl�se majd term�klistfriss�t�s
            textBox1.Clear();
            FilterResults();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // SKU sz�veg mez� tartalm�nak t�rl�se majd term�klistafriss�t�s
            textBox2.Clear();
            FilterResults();
        }

        private void localInvAdd_Click(object sender, EventArgs e)
        {
            int tmp = Convert.ToInt32(textBox3.Text.ToString());
            tmp += 1;
            textBox3.Text = tmp.ToString();

            DataGridViewRow tmpRow = (DataGridViewRow)dataGridView1.CurrentRow;
            tmpRow.Cells["LocalInventoryColumnTmp"].Value = tmp.ToString();
        }

        private void localInvSub_Click(object sender, EventArgs e)
        {
            int tmp = Convert.ToInt32(textBox3.Text.ToString());
            if (tmp > 0)
            {
                tmp -= 1;
            }
            textBox3.Text = tmp.ToString();

            DataGridViewRow tmpRow = (DataGridViewRow)dataGridView1.CurrentRow;
            tmpRow.Cells["LocalInventoryColumnTmp"].Value = tmp.ToString();
        }

        private void onlineInvAdd_Click(object sender, EventArgs e)
        {
            int tmp = Convert.ToInt32(textBox4.Text.ToString());
            tmp += 1;
            textBox4.Text = tmp.ToString();

            DataGridViewRow tmpRow = (DataGridViewRow)dataGridView1.CurrentRow;
            tmpRow.Cells["OnlineInventoryColumnTmp"].Value = tmp.ToString();
        }

        private void onlineInvSub_Click(object sender, EventArgs e)
        {
            int tmp = Convert.ToInt32(textBox4.Text.ToString());
            if (tmp > 0)
            {
                tmp -= 1;
            }
            textBox4.Text = tmp.ToString();

            DataGridViewRow tmpRow = (DataGridViewRow)dataGridView1.CurrentRow;
            tmpRow.Cells["OnlineInventoryColumnTmp"].Value = tmp.ToString();
        }

        private async void saveButton_Click(object sender, EventArgs e)
        {

            if (selectionAllowed && (TermekAdatok)dataGridView1.CurrentRow.DataBoundItem != null)
            {
                // A K�szlet m�dos�t�s elment�se POST utas�t�ssal
                TermekAdatok tmpRow = (TermekAdatok)dataGridView1.CurrentRow.DataBoundItem;
                int localInvNew = tmpRow.LocalInventoryColumnTmp;
                int localInvOld = tmpRow.LocalInventoryColumn;
                int onlineInvNew = tmpRow.OnlineInventoryColumnTmp;
                int onlineInvOld = tmpRow.OnlineInventoryColumn;
                string? termekName = tmpRow.ProductNameColumn;
                string? termekSKU = tmpRow.SKUColumn;
                string? invBvin = tmpRow.OnlineInventoryBvinColumn;
                string? productBvin = tmpRow.BvinColumn;

                DialogResult confirmResult = MessageBox.Show(
                    $"Biztos szeretn�d m�dos�tani az al�bbi K�szletet:\n\nN�v: {termekName}\nSKU: {termekSKU}\n\nR�gi Bolti k�szlet: {localInvOld}\n�J Bolti k�szlet: {localInvNew}\nR�gi Online k�szlet: {onlineInvOld}\n�J Online k�szlet: {onlineInvNew}",
                    "Biztos szeretn�l M�dos�t�sokat v�gezni?",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirmResult == DialogResult.Yes)
                {
                    // K�szlet M�dos�t�s v�ghezvitele
                    await apiDataManager.PostInventoryUpdate(productBvin, invBvin, localInvNew, localInvOld, onlineInvNew, onlineInvOld);
                    TermekAdatok tmp = apiDataManager.GetOneProductData((TermekAdatok)dataGridView1.CurrentRow.DataBoundItem);

                    var originalItems = this.OriginalTermekLista.FirstOrDefault(r => r.BvinColumn == tmp.BvinColumn);
                    originalItems = tmp;
                    var termekDatasrc = this.TermekekListaDataSource.FirstOrDefault(r => r.BvinColumn == tmp.BvinColumn);
                    termekDatasrc = tmp;

                    DGBindigSource.DataSource = TermekekListaDataSource;
                    dataGridView1.Refresh();
                }
            }
        }

        private void FilterResults()
        {
            // Form komponensek adatainak begy�jt�se
            selectedCategory = (HCAllCategory)comboBox1.SelectedItem;
            selectedTermekName = textBox1.Text;
            selectedTermekSKU = textBox2.Text;

            // 1. Minden_textbox_�res
            if (selectedTermekName.Equals(string.Empty) && selectedTermekSKU.Equals(string.Empty))
            {
                // Filter for all categories
                if (selectedCategory.Bvin == "all-cat-0")
                {
                    var searchResult = (from products in OriginalTermekLista
                                        select products).ToList();
                    TermekekListaDataSource = searchResult;
                }
                else // only for a specified category
                {
                    var searchResult = (from products in OriginalTermekLista
                                        where products.CategoryColumn.Equals(selectedCategory.Name)
                                        select products).ToList();
                    TermekekListaDataSource = searchResult;
                }
            }
            // 2. A termeknev nem �res, skun�v �res
            else if (!selectedTermekName.Equals(string.Empty) && selectedTermekSKU.Equals(string.Empty))
            {
                // Filter for all categories
                if (selectedCategory.Bvin == "all-cat-0")
                {
                    var searchResult = (from products in OriginalTermekLista
                                        where products.ProductNameColumn.ToLower().Contains(selectedTermekName.ToLower())
                                        select products).ToList();
                    TermekekListaDataSource = searchResult;
                }
                else // only for a specified category
                {
                    var searchResult = (from products in OriginalTermekLista
                                        where products.CategoryColumn.Equals(selectedCategory.Name) && products.ProductNameColumn.ToLower().Contains(selectedTermekName.ToLower())
                                        select products).ToList();
                    TermekekListaDataSource = searchResult;
                }
            }
            // 3. A termeknev �res, skun�v nem �res
            else if (selectedTermekName.Equals(string.Empty) && !selectedTermekSKU.Equals(string.Empty))
            {
                // Filter for all categories
                if (selectedCategory.Bvin == "all-cat-0")
                {
                    var searchResult = (from products in OriginalTermekLista
                                        where products.SKUColumn.ToLower().StartsWith(selectedTermekSKU.ToLower())
                                        select products).ToList();
                    TermekekListaDataSource = searchResult;
                }
                else // only for a specified category
                {
                    var searchResult = (from products in OriginalTermekLista
                                        where products.CategoryColumn.Equals(selectedCategory.Name) && products.SKUColumn.ToLower().StartsWith(selectedTermekSKU.ToLower())
                                        select products).ToList();
                    TermekekListaDataSource = searchResult;
                }
            }
            // 4. A termeknev nem �res, skun�v nem �res
            else if (!selectedTermekName.Equals(string.Empty) && !selectedTermekSKU.Equals(string.Empty))
            {
                // Filter for all categories
                if (selectedCategory.Bvin == "all-cat-0")
                {
                    var searchResult = (from products in OriginalTermekLista
                                        where products.SKUColumn.ToLower().StartsWith(selectedTermekSKU.ToLower()) && products.ProductNameColumn.ToLower().Contains(selectedTermekName.ToLower())
                                        select products).ToList();
                    TermekekListaDataSource = searchResult;
                }
                else // only for a specified category
                {
                    var searchResult = (from products in OriginalTermekLista
                                        where products.CategoryColumn.Equals(selectedCategory.Name) && products.SKUColumn.ToLower().StartsWith(selectedTermekSKU.ToLower()) && products.ProductNameColumn.ToLower().Contains(selectedTermekName.ToLower())
                                        select products).ToList();
                    TermekekListaDataSource = searchResult;
                }
            }

            // Ha nincs kiv�lasztva semmi a T�bl�zatb�l -> a c�mek jelzik + letiltom az input gombokat
            if (TermekekListaDataSource.Count <= 0)
            {
                ClearSelectedProductData();
                localInvAdd.Enabled = false;
                localInvSub.Enabled = false;
                onlineInvAdd.Enabled = false;
                onlineInvSub.Enabled = false;
                saveButton.Enabled = false;
            }
            else
            {
                localInvAdd.Enabled = true;
                localInvSub.Enabled = true;
                onlineInvAdd.Enabled = true;
                onlineInvSub.Enabled = true;
                saveButton.Enabled = true;
            }

            // A friss term�ksz�r�s hozz�rendel�se a datagridview-hoz
            DGBindigSource.DataSource = TermekekListaDataSource;

        }

        private void SetDataGridView()
        {

            //Adatt�bla M�dos�t�sa �s be�ll�t�sa
            dataGridView1.BackgroundColor = DasHausWhite;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ScrollBars = ScrollBars.Vertical;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToOrderColumns = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView1.ColumnHeadersHeight = 50;
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = DasHausBlue;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = DasHausWhite;
            dataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = DasHausBlue;
            dataGridView1.ColumnHeadersDefaultCellStyle.SelectionForeColor = DasHausWhite;
            dataGridView1.RowsDefaultCellStyle.SelectionForeColor = DasHausWhite;
            dataGridView1.RowsDefaultCellStyle.SelectionBackColor = DasHausYellow;
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridView1.GridColor = DasHausGrey;

            int dgw = dataGridView1.Width;

            dataGridView1.Columns["CategoryColumn"].DisplayIndex = 0;
            dataGridView1.Columns["CategoryColumn"].HeaderText = "Kateg�ria";
            dataGridView1.Columns["CategoryColumn"].Width = Convert.ToInt32(dgw * 0.17);
            dataGridView1.Columns["SKUColumn"].DisplayIndex = 1;
            dataGridView1.Columns["SKUColumn"].HeaderText = "SKU";
            dataGridView1.Columns["SKUColumn"].Width = Convert.ToInt32(dgw * 0.15);
            dataGridView1.Columns["ProductNameColumn"].DisplayIndex = 2;
            dataGridView1.Columns["ProductNameColumn"].HeaderText = "Term�kn�v";
            dataGridView1.Columns["ProductNameColumn"].Width = Convert.ToInt32(dgw * 0.48);
            dataGridView1.Columns["LocalInventoryColumn"].DisplayIndex = 3;
            dataGridView1.Columns["LocalInventoryColumn"].HeaderText = "Bolti k�szlet";
            dataGridView1.Columns["LocalInventoryColumn"].Width = Convert.ToInt32(dgw * 0.10);
            dataGridView1.Columns["OnlineInventoryColumn"].DisplayIndex = 4;
            dataGridView1.Columns["OnlineInventoryColumn"].HeaderText = "Online k�szlet";
            dataGridView1.Columns["OnlineInventoryColumn"].Width = Convert.ToInt32(dgw * 0.10);
            dataGridView1.Columns["BvinColumn"].Visible = false;
            dataGridView1.Columns["BvinColumn"].DisplayIndex = 5;
            dataGridView1.Columns["ListPriceColumn"].Visible = false;
            dataGridView1.Columns["ListPriceColumn"].DisplayIndex = 6;
            dataGridView1.Columns["LocalInventoryColumnTmp"].Visible = false;
            dataGridView1.Columns["LocalInventoryColumnTmp"].DisplayIndex = 7;
            dataGridView1.Columns["OnlineInventoryColumnTmp"].Visible = false;
            dataGridView1.Columns["OnlineInventoryColumnTmp"].DisplayIndex = 8;
            dataGridView1.Columns["CategoryBvinColumn"].Visible = false;
            dataGridView1.Columns["CategoryBvinColumn"].DisplayIndex = 9;
            dataGridView1.Columns["OnlineInventoryBvinColumn"].Visible = false;
            dataGridView1.Columns["OnlineInventoryBvinColumn"].DisplayIndex = 10;


            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].ReadOnly = true;
                dataGridView1.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            selectionAllowed = true;
        }

        private void ClearSelectedProductData()
        {
            // Forms komponensek alaphelyzetbe �ll�t�sa 
            termekNevL.Text = "nincs term�k kiv�lasztva";
            toolTip1.SetToolTip(termekNevL, termekNevL.Text);
            skuNevL.Text = "nincs term�k kiv�lasztva";
            toolTip1.SetToolTip(skuNevL, skuNevL.Text);
            kategoriaNevL.Text = "nincs term�k kiv�lasztva";
            toolTip1.SetToolTip(kategoriaNevL, kategoriaNevL.Text);
            arNevL.Text = "nincs term�k kiv�lasztva";
            toolTip1.SetToolTip(arNevL, arNevL.Text);
            bvinNevL.Text = "nincs term�k kiv�lasztva";
            toolTip1.SetToolTip(bvinNevL, bvinNevL.Text);
            textBox3.Text = string.Empty;
            textBox4.Text = string.Empty;
        }

    }
}
