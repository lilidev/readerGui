﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ReaderGui
{
    public partial class Form2 : Form
    {

        string str_conf_model;
        string[] str_arr_conf_protocols;
        string[] str_arr_conf_ICs;

        string[] str_arr_icName_1;
        string[] str_arr_icName_2;
        string[] str_arr_icName_3;

        string[] str_arr_command_1;
        string[] str_arr_command_2;
        string[] str_arr_command_3;

        ReadFeigJson readFeigJson;
        FeigJson feigJson;
        Form1 form1;

        public Form2(Form1 form1)
        {
            InitializeComponent();
            initFeigJson();
            this.form1 = form1;


        }

        private void initFeigJson()
        {
            readFeigJson = new ReadFeigJson();
            readFeigJson.readFromFile(Form1.jsonPath);
            feigJson = new FeigJson();

            InitComboBoxes();
            InitlistBoxSelectModel();
            toolStripStatusLabel1.Text = "Operation Status";//DateTime.Now.ToShortDateString();

        }


        private void InitComboBoxes()
        {
            comboBoxReaderManufacturer.Items.Clear();
            comboBoxReaderManufacturer.Items.Add(readFeigJson.feigJsonList.ReaderManufacturer);
            comboBoxReaderManufacturer.SelectedIndex = 0;

            comboBoxAvailableModels.Items.Clear();
            comboBoxAvailableModels.Items.AddRange(readFeigJson.feigJsonList.AvailableModels);
            comboBoxAvailableModels.SelectedIndex = 0;

            comboBoxAvailableProtocols.Items.Clear();
            comboBoxAvailableProtocols.Items.AddRange(readFeigJson.feigJsonList.AvailableProtocols);
            comboBoxAvailableProtocols.SelectedIndex = 0;

            comboBoxAvailableICs.Items.Clear();
            comboBoxAvailableICs.Items.AddRange(readFeigJson.feigJsonList.AvailableICs);
            comboBoxAvailableICs.SelectedIndex = 0;

        }

        private void InitlistBoxSelectModel()
        {
            listBoxSelectModel.SelectionMode = SelectionMode.One;
            listBoxSelectModel.BeginUpdate();

            if (listBoxSelectModel.Items.Count > 0)
            {
                listBoxSelectModel.Items.Clear();
            }

            foreach (var item in readFeigJson.feigJsonList.getModels())
                listBoxSelectModel.Items.Add(item);
            listBoxSelectModel.EndUpdate();
        }

        private void listBoxSelectModel_SelectedIndexChanged(object sender, EventArgs e)
        {

            contentClear();
            string selectModel = listBoxSelectModel.SelectedItem.ToString();
            feigJson = getSelectedReaderJson(selectModel);

            textBoxFeigModel.Text = selectModel;
            str_conf_model = selectModel;
            textBoxSupportedICs.Text = Util.StrArrayToStr(feigJson.SupportedICs, ";");
            initCheckBoxComboBoxSupportedProtocols();
            initCheckBoxComboBoxSupportedICs();

            if (feigJson.SetupCommands.Count > 0)
            {
                initCheckBoxComboBoxICsName1();
            }
            if (feigJson.SetupCommands.Count > 1)
            {
                initCheckBoxComboBoxICsName2();
            }
            if (feigJson.SetupCommands.Count > 2)
            {
                initCheckBoxComboBoxICsName3();
            }

        }

        private FeigJson getSelectedReaderJson(string selectedModel)
        {
            FeigJson feig_temp = new FeigJson();
            foreach (FeigJson temp in readFeigJson.feigJsonList.ReaderConfig)
            {
                if (string.Equals(temp.Model, selectedModel))
                {
                    feig_temp = temp;
                    return feig_temp;
                }
            }
            return null;
        }


        private void contentInit()
        {
            //initCheckBoxComboBoxSupportedProtocols();
            foreach (var item in checkBoxComboBoxSupportedProtocols.CheckBoxItems)
            {
                item.Checked = false;
            }
            checkBoxComboBoxSupportedProtocols.Text = "Select";

            //initCheckBoxComboBoxSupportedICs();
            foreach (var item in checkBoxComboBoxSupportedICs.CheckBoxItems)
            {
                item.Checked = false;
            }
            checkBoxComboBoxSupportedICs.Text = "Select";

            foreach (var item in checkBoxComboBoxICsName1.CheckBoxItems)
            {
                item.Checked = false;
            }
            checkBoxComboBoxICsName1.Text = "Select";

            foreach (var item in checkBoxComboBoxICsName2.CheckBoxItems)
            {
                item.Checked = false;
            }
            checkBoxComboBoxICsName2.Text = "Select";

            foreach (var item in checkBoxComboBoxICsName3.CheckBoxItems)
            {
                item.Checked = false;
            }
            checkBoxComboBoxICsName3.Text = "Select";
        }
        private void contentClear()
        {

            textBoxSupportedProtocols.Clear();
            textBoxSupportedICs.Clear();
            textBoxICsName1.Clear();
            textBoxICsName2.Clear();
            textBoxICsName3.Clear();

            textBoxICsCommand1.Clear();
            textBoxICsCommand2.Clear();
            textBoxICsCommand3.Clear();

            checkBoxComboBoxSupportedProtocols.Text = "";
            checkBoxComboBoxSupportedICs.Text = "";

            checkBoxComboBoxICsName1.Text = "";
            checkBoxComboBoxICsName2.Text = "";
            checkBoxComboBoxICsName3.Text = "";


        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            string model_temp = str_conf_model;

            List<string> availableModel_list = Util.strArrayToList(readFeigJson.feigJsonList.AvailableModels);
            availableModel_list.Add(model_temp);
            readFeigJson.feigJsonList.AvailableModels = Util.ListToStrArray(availableModel_list);

            readFeigJson.feigJsonList.ReaderConfig.Add(CreateFeig(str_conf_model));
            List<string> modelList = readFeigJson.feigJsonList.getModels().ToList();
            modelList.Add(model_temp.Trim());
            readFeigJson.writeToFile(Form1.jsonPath);

            initFeigJson();
            toolStripStatusLabel1.Text = model_temp + " is added to file successfully";

        }

        private FeigJson CreateFeig(string model)
        {
            List<CommandJson> commandJsonList = new List<CommandJson>();
            CommandJson temp;

            if (!(str_arr_icName_1 == null || str_arr_icName_1.Length == 0))
            {
                temp.icName = str_arr_icName_1;
                temp.icCommand = str_arr_command_1;
                commandJsonList.Add(temp);
            }
            if (!(str_arr_icName_2 == null || str_arr_icName_2.Length == 0))
            {
                temp.icName = str_arr_icName_2;
                temp.icCommand = str_arr_command_2;
                commandJsonList.Add(temp);
            }
            if (!(str_arr_icName_3 == null || str_arr_icName_3.Length == 0))
            {
                temp.icName = str_arr_icName_3;
                temp.icCommand = str_arr_command_3;
                commandJsonList.Add(temp);
            }
            FeigJson feigJsonTemp = new FeigJson(model, str_arr_conf_protocols, str_arr_conf_ICs, commandJsonList);
            return feigJsonTemp;
        }


        private void buttonDelete_Click(object sender, EventArgs e)
        {


            string selectModel = listBoxSelectModel.SelectedItem.ToString();

            if (MessageBox.Show("Are you sure to delete " + selectModel + " from file", "Notice", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                if (readFeigJson.feigJsonList.ReaderConfig.Count > 1)
                {
                    deleteFeig(selectModel);

                    List<string> modelList = readFeigJson.feigJsonList.getModels().ToList();
                    modelList.Remove(selectModel);

                    List<string> availableModel_list = Util.strArrayToList(readFeigJson.feigJsonList.AvailableModels);
                    availableModel_list.Remove(selectModel);
                    readFeigJson.feigJsonList.AvailableModels = Util.ListToStrArray(availableModel_list);

                    readFeigJson.writeToFile(Form1.jsonPath);

                    initFeigJson();
                    toolStripStatusLabel1.Text = selectModel + " is removed from file successfully";
                }
                else
                {
                    MessageBox.Show("Last model can not be removed in list!");
                }

            }

        }

        private void buttonModify_Click(object sender, EventArgs e)
        {

            string selectModel = listBoxSelectModel.SelectedItem.ToString();

            if (MessageBox.Show("Are you sure to update " + selectModel + " to file", "Notice", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                deleteFeig(selectModel);

                CreateFeig(selectModel);
                readFeigJson.feigJsonList.ReaderConfig.Add(CreateFeig(str_conf_model));
                readFeigJson.writeToFile(Form1.jsonPath);

                initFeigJson();
                toolStripStatusLabel1.Text = selectModel + " is updated to file successfully";
            }
        }

        private void deleteFeig(string model)
        {
            feigJson = getSelectedReaderJson(model);

            readFeigJson.feigJsonList.ReaderConfig.Remove(feigJson);

        }



        private void textBoxFeigModel_TextChanged(object sender, EventArgs e)
        {

            contentClear();

            buttonAdd.Enabled = false;
            str_conf_model = textBoxFeigModel.Text.ToString();


        }



        private void textBoxICsCommand1_TextChanged(object sender, EventArgs e)
        {
            List<string> strLines = new List<string>();
            strLines.AddRange(textBoxICsCommand1.Lines);
            str_arr_command_1 = strLines.ToArray();
        }

        private void textBoxICsCommand2_TextChanged(object sender, EventArgs e)
        {
            List<string> strLines = new List<string>();
            strLines.AddRange(textBoxICsCommand2.Lines);
            str_arr_command_2 = strLines.ToArray();
        }

        private void textBoxICsCommand3_TextChanged(object sender, EventArgs e)
        {
            List<string> strLines = new List<string>();
            strLines.AddRange(textBoxICsCommand3.Lines);
            str_arr_command_3 = strLines.ToArray();
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            form1.InitData(false);
        }

        private void textBoxFeigModel_Validated(object sender, EventArgs e)
        {

            string temp = textBoxFeigModel.Text;
            if (readFeigJson.feigJsonList.getModels().Contains(temp))
            {
                MessageBox.Show("Model ID is already existed. Please check!");
            }
            else if (string.IsNullOrEmpty(temp))
            {
                MessageBox.Show("Model ID cannot be empty");
            }
            else
            {

                toolStripStatusLabel1.Text = "Now adding a new one";
                buttonAdd.Enabled = true;
                contentInit();
            }
        }

        private void initCheckBoxComboBoxSupportedProtocols()
        {
            checkBoxComboBoxSupportedProtocols.Items.Clear();
            checkBoxComboBoxSupportedProtocols.Items.AddRange(readFeigJson.feigJsonList.AvailableProtocols);

            string checkedProtocols = "";
            int count = 0;
            foreach (var checkBoxItem in checkBoxComboBoxSupportedProtocols.CheckBoxItems)
            {
                checkBoxItem.Checked = false;
                if (feigJson.SupportedProtocols.Contains(checkBoxItem.Text))
                {
                    checkBoxItem.Checked = true;
                    checkedProtocols += checkBoxItem.Text + ";";
                    count++;
                }

            }

            if (checkedProtocols.Length > 0)
                checkedProtocols = checkedProtocols.Remove(checkedProtocols.Length - 1);
            checkBoxComboBoxSupportedProtocols.Text = count.ToString() + " Selected";//checkedProd;

            textBoxSupportedProtocols.Text = checkedProtocols;
        }

        private void initCheckBoxComboBoxSupportedICs()
        {
            checkBoxComboBoxSupportedICs.Items.Clear();
            checkBoxComboBoxSupportedICs.Items.AddRange(readFeigJson.feigJsonList.AvailableICs);

            string checkedICs = "";
            int count = 0;
            foreach (var checkBoxItem in checkBoxComboBoxSupportedICs.CheckBoxItems)
            {
                checkBoxItem.Checked = false;
                if (feigJson.SupportedICs.Contains(checkBoxItem.Text))
                {
                    checkBoxItem.Checked = true;
                    checkedICs += checkBoxItem.Text + ";";
                    count++;
                }

            }

            if (checkedICs.Length > 0)
                checkedICs = checkedICs.Remove(checkedICs.Length - 1);
            checkBoxComboBoxSupportedICs.Text = count.ToString() + " Selected";//checkedProd;

            textBoxSupportedICs.Text = checkedICs;

        }

        private void initCheckBoxComboBoxICsName1()
        {
            checkBoxComboBoxICsName1.Items.Clear();
            checkBoxComboBoxICsName1.Items.AddRange(feigJson.SupportedICs);
            int count = 0;
            string checkedICsName1 = "";
            foreach (var checkBoxItem in checkBoxComboBoxICsName1.CheckBoxItems)
            {

                if (feigJson.SetupCommands[0].icName.Contains(checkBoxItem.Text))
                {
                    checkBoxItem.Checked = true;
                    checkedICsName1 += checkBoxItem.Text + ";";
                    count++;
                }

            }

            if (checkedICsName1.Length > 0)
                checkedICsName1 = checkedICsName1.Remove(checkedICsName1.Length - 1);
            textBoxICsName1.Text = checkedICsName1;
            checkBoxComboBoxICsName1.Text = count.ToString() + " Selected";
            textBoxICsCommand1.Text = Util.StrArrayToStr(feigJson.SetupCommands[0].icCommand, "\r\n");

        }

        private void initCheckBoxComboBoxICsName2()
        {
            checkBoxComboBoxICsName2.Items.Clear();
            checkBoxComboBoxICsName2.Items.AddRange(feigJson.SupportedICs);

            int count = 0;
            string checkedICsName2 = "";
            foreach (var checkBoxItem in checkBoxComboBoxICsName2.CheckBoxItems)
            {

                if (feigJson.SetupCommands[1].icName.Contains(checkBoxItem.Text))
                {
                    checkBoxItem.Checked = true;
                    checkedICsName2 += checkBoxItem.Text + ";";
                    count++;
                }

            }
            if (checkedICsName2.Length > 0)
                checkedICsName2 = checkedICsName2.Remove(checkedICsName2.Length - 1);
            textBoxICsName2.Text = checkedICsName2;
            checkBoxComboBoxICsName2.Text = count.ToString() + " Selected";
            textBoxICsCommand2.Text = Util.StrArrayToStr(feigJson.SetupCommands[1].icCommand, "\r\n");
        }

        private void initCheckBoxComboBoxICsName3()
        {
            checkBoxComboBoxICsName3.Items.Clear();
            checkBoxComboBoxICsName3.Items.AddRange(feigJson.SupportedICs);

            int count = 0;
            string checkedICsName3 = "";
            foreach (var checkBoxItem in checkBoxComboBoxICsName3.CheckBoxItems)
            {

                if (feigJson.SetupCommands[2].icName.Contains(checkBoxItem.Text))
                {
                    checkBoxItem.Checked = true;
                    checkedICsName3 += checkBoxItem.Text + ";";
                    count++;
                }

            }
            if (checkedICsName3.Length > 0)
                checkedICsName3 = checkedICsName3.Remove(checkedICsName3.Length - 1);
            textBoxICsName3.Text = checkedICsName3;
            checkBoxComboBoxICsName3.Text = count.ToString() + " Selected";
            textBoxICsCommand3.Text = Util.StrArrayToStr(feigJson.SetupCommands[2].icCommand, "\r\n");
        }



        private void checkBoxComboBoxSupportedProtocols_CheckBoxCheckedChanged(object sender, EventArgs e)
        {

            string checkedProtocols = "";
            int count = 0;

            List<string> strList = new List<string>();
            foreach (var item in checkBoxComboBoxSupportedProtocols.CheckBoxItems)
            {
                if (item.Checked == true)
                {
                    checkedProtocols += item.Text + ";";
                    count++;
                    strList.Add(item.Text);
                }

            }
            str_arr_conf_protocols = strList.ToArray();
            if (checkedProtocols.Length > 0)
                checkedProtocols = checkedProtocols.Remove(checkedProtocols.Length - 1);
            checkBoxComboBoxSupportedProtocols.Text = count.ToString() + " Selected";
            textBoxSupportedProtocols.Text = checkedProtocols;
        }

        private void checkBoxComboBoxSupportedICs_CheckBoxCheckedChanged(object sender, EventArgs e)
        {
            string checkedICs = "";
            int count = 0;
            List<string> strList = new List<string>();
            foreach (var item in checkBoxComboBoxSupportedICs.CheckBoxItems)
            {
                if (item.Checked == true)
                {
                    checkedICs += item.Text + ";";
                    count++;
                    strList.Add(item.Text);
                }

            }

            str_arr_conf_ICs = strList.ToArray();
            if (checkedICs.Length > 0)
                checkedICs = checkedICs.Remove(checkedICs.Length - 1);
            checkBoxComboBoxSupportedICs.Text = count.ToString() + " Selected";//checkedProd;

            textBoxSupportedICs.Text = checkedICs;
        }

        private void checkBoxComboBoxICsName1_CheckBoxCheckedChanged(object sender, EventArgs e)
        {
            string checkedICsName1 = "";
            int count = 0;
            List<string> strList = new List<string>();
            foreach (var item in checkBoxComboBoxICsName1.CheckBoxItems)
            {
                if (item.Checked == true)
                {
                    checkedICsName1 += item.Text + ";";//checkedProd + "'" + item.Text + "',";
                    count++;
                    strList.Add(item.Text);
                }

            }
            str_arr_icName_1 = strList.ToArray();
            if (checkedICsName1.Length > 0)
                checkedICsName1 = checkedICsName1.Remove(checkedICsName1.Length - 1);
            checkBoxComboBoxICsName1.Text = count.ToString() + " Selected";//checkedProd;

            textBoxICsName1.Text = checkedICsName1;
        }

        private void checkBoxComboBoxICsName2_CheckBoxCheckedChanged(object sender, EventArgs e)
        {
            string checkedICsName2 = "";
            int count = 0;
            List<string> strList = new List<string>();
            foreach (var item in checkBoxComboBoxICsName2.CheckBoxItems)
            {
                if (item.Checked == true)
                {
                    checkedICsName2 += item.Text + ";";//checkedProd + "'" + item.Text + "',";
                    count++;
                    strList.Add(item.Text);
                }

            }
            str_arr_icName_2 = strList.ToArray();
            if (checkedICsName2.Length > 0)
                checkedICsName2 = checkedICsName2.Remove(checkedICsName2.Length - 1);
            checkBoxComboBoxICsName2.Text = count.ToString() + " Selected";//checkedProd;

            textBoxICsName2.Text = checkedICsName2;
        }

        private void checkBoxComboBoxICsName3_CheckBoxCheckedChanged(object sender, EventArgs e)
        {
            string checkedICsName3 = "";
            int count = 0;

            List<string> strList = new List<string>();
            foreach (var item in checkBoxComboBoxICsName3.CheckBoxItems)
            {
                if (item.Checked == true)
                {
                    checkedICsName3 += item.Text + ";";//checkedProd + "'" + item.Text + "',";

                    count++;
                    strList.Add(item.Text);
                }

            }
            str_arr_icName_3 = strList.ToArray();
            if (checkedICsName3.Length > 0)
                checkedICsName3 = checkedICsName3.Remove(checkedICsName3.Length - 1);
            textBoxICsName3.Text = checkedICsName3;
            checkBoxComboBoxICsName3.Text = count.ToString() + " Selected";//checkedProd;

        }
    }
}
