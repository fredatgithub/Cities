﻿/*
The MIT License(MIT)
Copyright(c) 2015 Freddy Juhel
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Linq;
using CountriesAndCities.Properties;
using Tools;

namespace CountriesAndCities
{
  public partial class FormMain : Form
  {
    public FormMain()
    {
      InitializeComponent();
    }

    public readonly Dictionary<string, string> _languageDicoEn = new Dictionary<string, string>();
    public readonly Dictionary<string, string> _languageDicoFr = new Dictionary<string, string>();
    private string _currentLanguage = "english";
    private ConfigurationOptions _configurationOptions = new ConfigurationOptions();

    private void QuitToolStripMenuItem_Click(object sender, EventArgs e)
    {
      SaveWindowValue();
      Application.Exit();
    }

    private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
      AboutBoxApplication aboutBoxApplication = new AboutBoxApplication();
      aboutBoxApplication.ShowDialog();
    }

    private void DisplayTitle()
    {
      Assembly assembly = Assembly.GetExecutingAssembly();
      FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
      Text += string.Format(" V{0}.{1}.{2}.{3}", fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart, fvi.FilePrivatePart);
    }

    private void FormMain_Load(object sender, EventArgs e)
    {
      LoadSettingsAtStartup();
    }

    private void LoadSettingsAtStartup()
    {
      DisplayTitle();
      GetWindowValue();
      LoadLanguages();
      LoadComboBox(comboBoxSelectContinent, "Resources\\Continents.xml", "continent");
      SetLanguage(Settings.Default.LastLanguageUsed);
      AdjustAllControls();
    }

    private void LoadConfigurationOptions()
    {
      _configurationOptions.Option1Name = Settings.Default.Option1Name;
      _configurationOptions.Option2Name = Settings.Default.Option2Name;
    }

    private void SaveConfigurationOptions()
    {
      _configurationOptions.Option1Name = Settings.Default.Option1Name;
      _configurationOptions.Option2Name = Settings.Default.Option2Name;
    }

    private void LoadLanguages()
    {
      if (!File.Exists(Settings.Default.LanguageFileName))
      {
        CreateLanguageFile();
      }

      // read the translation file and feed the language
      XDocument xDoc;
      try
      {
        xDoc = XDocument.Load(Settings.Default.LanguageFileName);
      }
      catch (Exception exception)
      {
        MessageBox.Show(Resources.Error_while_loading_the + Punctuation.OneSpace +
          Settings.Default.LanguageFileName + Punctuation.OneSpace + Resources.XML_file +
          Punctuation.OneSpace + exception.Message);
        CreateLanguageFile();
        return;
      }
      var result = from node in xDoc.Descendants("term")
                   where node.HasElements
                   let xElementName = node.Element("name")
                   where xElementName != null
                   let xElementEnglish = node.Element("englishValue")
                   where xElementEnglish != null
                   let xElementFrench = node.Element("frenchValue")
                   where xElementFrench != null
                   select new
                   {
                     name = xElementName.Value,
                     englishValue = xElementEnglish.Value,
                     frenchValue = xElementFrench.Value
                   };
      foreach (var i in result)
      {
        if (!_languageDicoEn.ContainsKey(i.name))
        {
          _languageDicoEn.Add(i.name, i.englishValue);
        }
        else
        {
          MessageBox.Show(Resources.Your_XML_file_has_duplicate_like + Punctuation.OneSpace + i.name);
        }

        if (!_languageDicoFr.ContainsKey(i.name))
        {
          _languageDicoFr.Add(i.name, i.frenchValue);
        }
        else
        {
          MessageBox.Show(Resources.Your_XML_file_has_duplicate_like + Punctuation.OneSpace + i.name);
        }
      }
    }

    private static void LoadComboBox(ComboBox comboBox, string xmlfile, string tagName)
    {
      tagName = tagName.ToLower();
      if (!File.Exists(xmlfile))
      {
        CreateXmlFile(xmlfile, tagName);
      }

      XDocument xDoc;
      try
      {
        xDoc = XDocument.Load(xmlfile);
      }
      catch (Exception exception)
      {
        MessageBox.Show(Resources.Error_while_loading_the + xmlfile +
          Punctuation.OneSpace + Resources.XML_file + Punctuation.OneSpace + exception.Message);
        CreateXmlFile(xmlfile, tagName);
        return;
      }
      var result = from node in xDoc.Descendants(tagName)
                   where node.HasElements
                   let xElementName = node.Element("name")
                   where xElementName != null
                   select new
                   {
                     name = xElementName.Value,
                   };
      comboBox.Items.Clear();
      foreach (var i in result)
      {
        if (!comboBox.Items.Contains(i.name))
        {
          comboBox.Items.Add(i.name);
        }
#if DEBUG
        else
        {
          //MessageBox.Show("Your xml file has duplicate like: " + i.name);
        }
#endif
      }
    }

    private static void CreateXmlFile(string xmlfile, string xmlContentType)
    {
      throw new NotImplementedException();
    }

    private static void CreateLanguageFile()
    {
      List<string> minimumVersion = new List<string>
      {
        "<?xml version=\"1.0\" encoding=\"utf-8\" ?>",
        "<terms>",
         "<term>",
        "<name>MenuFile</name>",
        "<englishValue>File</englishValue>",
        "<frenchValue>Fichier</frenchValue>",
        "</term>",
        "<term>",
        "<name>MenuFileNew</name>",
        "<englishValue>New</englishValue>",
        "<frenchValue>Nouveau</frenchValue>",
        "</term>",
        "<term>",
        "<name>MenuFileOpen</name>",
        "<englishValue>Open</englishValue>",
        "<frenchValue>Ouvrir</frenchValue>",
        "</term>",
        "<term>",
        "<name>MenuFileSave</name>",
        "<englishValue>Save</englishValue>",
        "<frenchValue>Enregistrer</frenchValue>",
        "</term>",
        "<term>",
        "<name>MenuFileSaveAs</name>",
        "<englishValue>Save as ...</englishValue>",
        "<frenchValue>Enregistrer sous ...</frenchValue>",
        "</term>",
        "<term>",
        "<name>MenuFilePrint</name>",
        "<englishValue>Print ...</englishValue>",
        "<frenchValue>Imprimer ...</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenufilePageSetup</name>",
          "<englishValue>Page setup</englishValue>",
          "<frenchValue>Aperçu avant impression</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenufileQuit</name>",
          "<englishValue>Quit</englishValue>",
          "<frenchValue>Quitter</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuEdit</name>",
          "<englishValue>Edit</englishValue>",
          "<frenchValue>Edition</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuEditCancel</name>",
          "<englishValue>Cancel</englishValue>",
          "<frenchValue>Annuler</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuEditRedo</name>",
          "<englishValue>Redo</englishValue>",
          "<frenchValue>Rétablir</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuEditCut</name>",
          "<englishValue>Cut</englishValue>",
          "<frenchValue>Couper</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuEditCopy</name>",
          "<englishValue>Copy</englishValue>",
          "<frenchValue>Copier</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuEditPaste</name>",
          "<englishValue>Paste</englishValue>",
          "<frenchValue>Coller</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuEditSelectAll</name>",
          "<englishValue>Select All</englishValue>",
          "<frenchValue>Sélectionner tout</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuTools</name>",
          "<englishValue>Tools</englishValue>",
          "<frenchValue>Outils</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuToolsCustomize</name>",
          "<englishValue>Customize ...</englishValue>",
          "<frenchValue>Personaliser ...</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuToolsOptions</name>",
          "<englishValue>Options</englishValue>",
          "<frenchValue>Options</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuLanguage</name>",
          "<englishValue>Language</englishValue>",
          "<frenchValue>Langage</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuLanguageEnglish</name>",
          "<englishValue>English</englishValue>",
          "<frenchValue>Anglais</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuLanguageFrench</name>",
          "<englishValue>French</englishValue>",
          "<frenchValue>Français</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuHelp</name>",
          "<englishValue>Help</englishValue>",
          "<frenchValue>Aide</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuHelpSummary</name>",
          "<englishValue>Summary</englishValue>",
          "<frenchValue>Sommaire</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuHelpIndex</name>",
          "<englishValue>Index</englishValue>",
          "<frenchValue>Index</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuHelpSearch</name>",
          "<englishValue>Search</englishValue>",
          "<frenchValue>Rechercher</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuHelpAbout</name>",
          "<englishValue>About</englishValue>",
          "<frenchValue>A propos de ...</frenchValue>",
        "</term>",
        "</terms>"
      };
      StreamWriter sw = new StreamWriter(Settings.Default.LanguageFileName);
      foreach (string item in minimumVersion)
      {
        sw.WriteLine(item);
      }

      sw.Close();
    }

    private void GetWindowValue()
    {
      Width = Settings.Default.WindowWidth;
      Height = Settings.Default.WindowHeight;
      Top = Settings.Default.WindowTop < 0 ? 0 : Settings.Default.WindowTop;
      Left = Settings.Default.WindowLeft < 0 ? 0 : Settings.Default.WindowLeft;
      SetDisplayOption(Settings.Default.DisplayToolStripMenuItem);
      LoadConfigurationOptions();
    }

    private void SaveWindowValue()
    {
      Settings.Default.WindowHeight = Height;
      Settings.Default.WindowWidth = Width;
      Settings.Default.WindowLeft = Left;
      Settings.Default.WindowTop = Top;
      Settings.Default.LastLanguageUsed = frenchToolStripMenuItem.Checked ? "French" : "English";
      Settings.Default.DisplayToolStripMenuItem = GetDisplayOption();
      SaveConfigurationOptions();
      Settings.Default.Save();
    }

    private string GetDisplayOption()
    {
      if (SmallToolStripMenuItem.Checked)
      {
        return "Small";
      }

      if (MediumToolStripMenuItem.Checked)
      {
        return "Medium";
      }

      return LargeToolStripMenuItem.Checked ? "Large" : string.Empty;
    }

    private void SetDisplayOption(string option)
    {
      UncheckAllOptions();
      switch (option.ToLower())
      {
        case "small":
          SmallToolStripMenuItem.Checked = true;
          break;
        case "medium":
          MediumToolStripMenuItem.Checked = true;
          break;
        case "large":
          LargeToolStripMenuItem.Checked = true;
          break;
        default:
          SmallToolStripMenuItem.Checked = true;
          break;
      }
    }

    private void UncheckAllOptions()
    {
      SmallToolStripMenuItem.Checked = false;
      MediumToolStripMenuItem.Checked = false;
      LargeToolStripMenuItem.Checked = false;
    }

    private void FormMainFormClosing(object sender, FormClosingEventArgs e)
    {
      SaveWindowValue();
    }

    private void FrenchToolStripMenuItem_Click(object sender, EventArgs e)
    {
      _currentLanguage = Language.French.ToString();
      SetLanguage(Language.French.ToString());
      AdjustAllControls();
    }

    private void EnglishToolStripMenuItem_Click(object sender, EventArgs e)
    {
      _currentLanguage = Language.English.ToString();
      SetLanguage(Language.English.ToString());
      AdjustAllControls();
    }

    private void SetLanguage(string myLanguage)
    {
      switch (myLanguage)
      {
        case "English":
          frenchToolStripMenuItem.Checked = false;
          englishToolStripMenuItem.Checked = true;
          fileToolStripMenuItem.Text = _languageDicoEn["MenuFile"];
          newToolStripMenuItem.Text = _languageDicoEn["MenuFileNew"];
          openToolStripMenuItem.Text = _languageDicoEn["MenuFileOpen"];
          saveToolStripMenuItem.Text = _languageDicoEn["MenuFileSave"];
          saveasToolStripMenuItem.Text = _languageDicoEn["MenuFileSaveAs"];
          printPreviewToolStripMenuItem.Text = _languageDicoEn["MenuFilePrint"];
          printPreviewToolStripMenuItem.Text = _languageDicoEn["MenufilePageSetup"];
          quitToolStripMenuItem.Text = _languageDicoEn["MenufileQuit"];
          editToolStripMenuItem.Text = _languageDicoEn["MenuEdit"];
          cancelToolStripMenuItem.Text = _languageDicoEn["MenuEditCancel"];
          redoToolStripMenuItem.Text = _languageDicoEn["MenuEditRedo"];
          cutToolStripMenuItem.Text = _languageDicoEn["MenuEditCut"];
          copyToolStripMenuItem.Text = _languageDicoEn["MenuEditCopy"];
          pasteToolStripMenuItem.Text = _languageDicoEn["MenuEditPaste"];
          selectAllToolStripMenuItem.Text = _languageDicoEn["MenuEditSelectAll"];
          toolsToolStripMenuItem.Text = _languageDicoEn["MenuTools"];
          personalizeToolStripMenuItem.Text = _languageDicoEn["MenuToolsCustomize"];
          optionsToolStripMenuItem.Text = _languageDicoEn["MenuToolsOptions"];
          languagetoolStripMenuItem.Text = _languageDicoEn["MenuLanguage"];
          englishToolStripMenuItem.Text = _languageDicoEn["MenuLanguageEnglish"];
          frenchToolStripMenuItem.Text = _languageDicoEn["MenuLanguageFrench"];
          helpToolStripMenuItem.Text = _languageDicoEn["MenuHelp"];
          summaryToolStripMenuItem.Text = _languageDicoEn["MenuHelpSummary"];
          indexToolStripMenuItem.Text = _languageDicoEn["MenuHelpIndex"];
          searchToolStripMenuItem.Text = _languageDicoEn["MenuHelpSearch"];
          aboutToolStripMenuItem.Text = _languageDicoEn["MenuHelpAbout"];
          DisplayToolStripMenuItem.Text = _languageDicoEn["Display"];
          SmallToolStripMenuItem.Text = _languageDicoEn["Small"];
          MediumToolStripMenuItem.Text = _languageDicoEn["Medium"];
          LargeToolStripMenuItem.Text = _languageDicoEn["Large"];

          labelSelectContinent.Text = _languageDicoEn["Select"] + Punctuation.OneSpace + _languageDicoEn["a continent"];
          labelSelectCountry.Text = _languageDicoEn["Select"] + Punctuation.OneSpace + _languageDicoEn["a country"];
          labelSelectState.Text = _languageDicoEn["Select"] + Punctuation.OneSpace + _languageDicoEn["a state"];
          labelSelectCounty.Text = _languageDicoEn["Select"] + Punctuation.OneSpace + _languageDicoEn["a county"];
          labelSelectCity.Text = _languageDicoEn["Select"] + Punctuation.OneSpace + _languageDicoEn["a city"];
          labelEnterContinent.Text = _languageDicoEn["Enter"] + Punctuation.OneSpace + _languageDicoEn["a continent"];
          labelEnterCountry.Text = _languageDicoEn["Enter"] + Punctuation.OneSpace + _languageDicoEn["a country"];
          labelEnterState.Text = _languageDicoEn["Enter"] + Punctuation.OneSpace + _languageDicoEn["a state"];
          labelEnterCounty.Text = _languageDicoEn["Enter"] + Punctuation.OneSpace + _languageDicoEn["a county"];
          labelEnterCity.Text = _languageDicoEn["Enter"] + Punctuation.OneSpace + _languageDicoEn["a city"];
          AdjustAllControls();
          AlignAllControls();

          _currentLanguage = "English";
          break;
        case "French":
          frenchToolStripMenuItem.Checked = true;
          englishToolStripMenuItem.Checked = false;
          fileToolStripMenuItem.Text = _languageDicoFr["MenuFile"];
          newToolStripMenuItem.Text = _languageDicoFr["MenuFileNew"];
          openToolStripMenuItem.Text = _languageDicoFr["MenuFileOpen"];
          saveToolStripMenuItem.Text = _languageDicoFr["MenuFileSave"];
          saveasToolStripMenuItem.Text = _languageDicoFr["MenuFileSaveAs"];
          printPreviewToolStripMenuItem.Text = _languageDicoFr["MenuFilePrint"];
          printPreviewToolStripMenuItem.Text = _languageDicoFr["MenufilePageSetup"];
          quitToolStripMenuItem.Text = _languageDicoFr["MenufileQuit"];
          editToolStripMenuItem.Text = _languageDicoFr["MenuEdit"];
          cancelToolStripMenuItem.Text = _languageDicoFr["MenuEditCancel"];
          redoToolStripMenuItem.Text = _languageDicoFr["MenuEditRedo"];
          cutToolStripMenuItem.Text = _languageDicoFr["MenuEditCut"];
          copyToolStripMenuItem.Text = _languageDicoFr["MenuEditCopy"];
          pasteToolStripMenuItem.Text = _languageDicoFr["MenuEditPaste"];
          selectAllToolStripMenuItem.Text = _languageDicoFr["MenuEditSelectAll"];
          toolsToolStripMenuItem.Text = _languageDicoFr["MenuTools"];
          personalizeToolStripMenuItem.Text = _languageDicoFr["MenuToolsCustomize"];
          optionsToolStripMenuItem.Text = _languageDicoFr["MenuToolsOptions"];
          languagetoolStripMenuItem.Text = _languageDicoFr["MenuLanguage"];
          englishToolStripMenuItem.Text = _languageDicoFr["MenuLanguageEnglish"];
          frenchToolStripMenuItem.Text = _languageDicoFr["MenuLanguageFrench"];
          helpToolStripMenuItem.Text = _languageDicoFr["MenuHelp"];
          summaryToolStripMenuItem.Text = _languageDicoFr["MenuHelpSummary"];
          indexToolStripMenuItem.Text = _languageDicoFr["MenuHelpIndex"];
          searchToolStripMenuItem.Text = _languageDicoFr["MenuHelpSearch"];
          aboutToolStripMenuItem.Text = _languageDicoFr["MenuHelpAbout"];
          DisplayToolStripMenuItem.Text = _languageDicoFr["Display"];
          SmallToolStripMenuItem.Text = _languageDicoFr["Small"];
          MediumToolStripMenuItem.Text = _languageDicoFr["Medium"];
          LargeToolStripMenuItem.Text = _languageDicoFr["Large"];

          labelSelectContinent.Text = _languageDicoFr["Select"] + Punctuation.OneSpace +_languageDicoFr["a continent"];
          labelSelectCountry.Text = _languageDicoFr["Select"] + Punctuation.OneSpace + _languageDicoFr["a country"];
          labelSelectState.Text = _languageDicoFr["Select"] + Punctuation.OneSpace + _languageDicoFr["a state"];
          labelSelectCounty.Text = _languageDicoFr["Select"] + Punctuation.OneSpace + _languageDicoFr["a county"];
          labelSelectCity.Text = _languageDicoFr["Select"] + Punctuation.OneSpace + _languageDicoFr["a city"];
          labelEnterContinent.Text = _languageDicoFr["Enter"] + Punctuation.OneSpace + _languageDicoFr["a continent"];
          labelEnterCountry.Text = _languageDicoFr["Enter"] + Punctuation.OneSpace + _languageDicoFr["a country"];
          labelEnterState.Text = _languageDicoFr["Enter"] + Punctuation.OneSpace + _languageDicoFr["a state"];
          labelEnterCounty.Text = _languageDicoFr["Enter"] + Punctuation.OneSpace + _languageDicoFr["a county"];
          labelEnterCity.Text = _languageDicoFr["Enter"] + Punctuation.OneSpace + _languageDicoFr["a city"];
          AdjustAllControls();
          AlignAllControls();
          _currentLanguage = "French";
          break;
        default:
          SetLanguage("English");
          break;
      }
    }

    private void CutToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Control focusedControl = FindFocusedControl(new List<Control> { }); // add your controls in the List
      var tb = focusedControl as TextBox;
      if (tb != null)
      {
        CutToClipboard(tb);
      }
    }

    private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Control focusedControl = FindFocusedControl(new List<Control> { }); // add your controls in the List
      var tb = focusedControl as TextBox;
      if (tb != null)
      {
        CopyToClipboard(tb);
      }
    }

    private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Control focusedControl = FindFocusedControl(new List<Control> { }); // add your controls in the List
      var tb = focusedControl as TextBox;
      if (tb != null)
      {
        PasteFromClipboard(tb);
      }
    }

    private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Control focusedControl = FindFocusedControl(new List<Control> { }); // add your controls in the List
      TextBox control = focusedControl as TextBox;
      if (control != null) control.SelectAll();
    }

    private void CutToClipboard(TextBoxBase tb, string errorMessage = "nothing")
    {
      if (tb != ActiveControl) return;
      if (tb.Text == string.Empty)
      {
        DisplayMessage(Translate("ThereIs") + Punctuation.OneSpace +
          Translate(errorMessage) + Punctuation.OneSpace +
          Translate("ToCut") + Punctuation.OneSpace, Translate(errorMessage),
          MessageBoxButtons.OK);
        return;
      }

      if (tb.SelectedText == string.Empty)
      {
        DisplayMessage(Translate("NoTextHasBeenSelected"),
          Translate(errorMessage), MessageBoxButtons.OK);
        return;
      }

      Clipboard.SetText(tb.SelectedText);
      tb.SelectedText = string.Empty;
    }

    private void CopyToClipboard(TextBoxBase tb, string message = "nothing")
    {
      if (tb != ActiveControl) return;
      if (tb.Text == string.Empty)
      {
        DisplayMessage(Translate("ThereIsNothingToCopy") + Punctuation.OneSpace,
          Translate(message), MessageBoxButtons.OK);
        return;
      }

      if (tb.SelectedText == string.Empty)
      {
        DisplayMessage(Translate("NoTextHasBeenSelected"),
          Translate(message), MessageBoxButtons.OK);
        return;
      }

      Clipboard.SetText(tb.SelectedText);
    }

    private void PasteFromClipboard(TextBoxBase tb)
    {
      if (tb != ActiveControl) return;
      var selectionIndex = tb.SelectionStart;
      tb.SelectedText = Clipboard.GetText();
      tb.SelectionStart = selectionIndex + Clipboard.GetText().Length;
    }

    private void DisplayMessage(string message, string title, MessageBoxButtons buttons)
    {
      MessageBox.Show(this, message, title, buttons);
    }

    private string Translate(string index)
    {
      string result = string.Empty;
      switch (_currentLanguage.ToLower())
      {
        case "english":
          result = _languageDicoEn.ContainsKey(index) ? _languageDicoEn[index] :
           "the term: \"" + index + "\" has not been translated yet.\nPlease tell the developer to translate this term";
          break;
        case "french":
          result = _languageDicoFr.ContainsKey(index) ? _languageDicoFr[index] :
            "the term: \"" + index + "\" has not been translated yet.\nPlease tell the developer to translate this term";
          break;
      }

      return result;
    }

    private static Control FindFocusedControl(Control container)
    {
      foreach (Control childControl in container.Controls.Cast<Control>().Where(childControl => childControl.Focused))
      {
        return childControl;
      }

      return (from Control childControl in container.Controls
              select FindFocusedControl(childControl)).FirstOrDefault(maybeFocusedControl => maybeFocusedControl != null);
    }

    private static Control FindFocusedControl(List<Control> container)
    {
      return container.FirstOrDefault(control => control.Focused);
    }

    private static Control FindFocusedControl(params Control[] container)
    {
      return container.FirstOrDefault(control => control.Focused);
    }

    private static Control FindFocusedControl(IEnumerable<Control> container)
    {
      return container.FirstOrDefault(control => control.Focused);
    }

    private static string PeekDirectory()
    {
      string result = string.Empty;
      FolderBrowserDialog fbd = new FolderBrowserDialog();
      if (fbd.ShowDialog() == DialogResult.OK)
      {
        result = fbd.SelectedPath;
      }

      return result;
    }

    private string PeekFile()
    {
      string result = string.Empty;
      OpenFileDialog fd = new OpenFileDialog();
      if (fd.ShowDialog() == DialogResult.OK)
      {
        result = fd.SafeFileName;
      }

      return result;
    }

    private void SmallToolStripMenuItem_Click(object sender, EventArgs e)
    {
      UncheckAllOptions();
      SmallToolStripMenuItem.Checked = true;
      AdjustAllControls();
    }

    private void MediumToolStripMenuItem_Click(object sender, EventArgs e)
    {
      UncheckAllOptions();
      MediumToolStripMenuItem.Checked = true;
      AdjustAllControls();
    }

    private void LargeToolStripMenuItem_Click(object sender, EventArgs e)
    {
      UncheckAllOptions();
      LargeToolStripMenuItem.Checked = true;
      AdjustAllControls();
    }

    private static void AdjustControls(params Control[] listOfControls)
    {
      if (listOfControls.Length == 0)
      {
        return;
      }

      int position = listOfControls[0].Width + 33; // 33 is the initial padding
      bool isFirstControl = true;
      foreach (Control control in listOfControls)
      {
        if (isFirstControl)
        {
          isFirstControl = false;
        }
        else
        {
          control.Left = position + 10;
          position += control.Width;
        }
      }
    }

    private static void AdjustComboWithTextBoxes(params Control[] listOfControls)
    {
      if (listOfControls.Length == 0)
      {
        return;
      }

      const int offset = 15;
      const int fontWidth = 9;
      int position = 33;
      for (int i = 1; i < listOfControls.Length; i = i + 2) // we skip textboxes and labels
      {
        ComboBox box = listOfControls[i] as ComboBox;
        if (box != null)
        {
          if (i == 1)
          {
            position += Maximum(LongestItemInCb((ComboBox)listOfControls[i]) * fontWidth + offset, 
              listOfControls[0].Width);
            listOfControls[1].Width = Maximum(LongestItemInCb((ComboBox)listOfControls[1]) * fontWidth,
              listOfControls[0].Width);
            continue;
          }

          if (box.Items.Count != 0)
          {
            listOfControls[i].Width = Maximum(LongestItemInCb((ComboBox) listOfControls[i]) * fontWidth,
              listOfControls[i - 1].Width);
            listOfControls[i].Left = Maximum(position + offset, listOfControls[i - 1].Width);
            listOfControls[i - 1].Left = Maximum(position + offset, listOfControls[i - 1].Width);
            position += Maximum(LongestItemInCb((ComboBox)listOfControls[i]) * fontWidth + offset,
              listOfControls[i - 1].Width+ offset) ;
          }
          else
          {
            listOfControls[i].Left = Maximum(position + offset, listOfControls[i - 1].Width);
            listOfControls[i - 1].Left = Maximum(position + offset, listOfControls[i - 1].Width);
            position += listOfControls[i].Width + offset;
          }
        }
      }
    }

    private static int Maximum(int value1, int value2)
    {
      return value1 > value2 ? value1 : value2;
    }

    private void AdjustAllControls()
    {
      AdjustComboWithTextBoxes(labelSelectContinent, comboBoxSelectContinent,
        labelSelectCountry, comboBoxSelectCountry,
        labelSelectState, comboBoxSelectState,
        labelSelectCounty, comboBoxSelectCounty,
        labelSelectCity, comboBoxSelectCity);
      AlignAllControls();
    }

    private static void AlignControls(Control masterControl, params Control[] listOfSlaveControls)
    {
      foreach (Control control in listOfSlaveControls)
      {
        control.Left = masterControl.Left;
        control.Width = masterControl.Width;
      }
    }

    private void AlignAllControls()
    {
      AlignControls(comboBoxSelectContinent, labelEnterContinent, textBoxEnterContinent, labelSelectContinent);
      AlignControls(comboBoxSelectCountry, labelEnterCountry, textBoxEnterCountry, labelSelectCountry);
      AlignControls(comboBoxSelectState, labelEnterState, textBoxEnterState, labelSelectState);
      AlignControls(comboBoxSelectCounty, labelEnterCounty, textBoxEnterCounty, labelSelectCounty);
      AlignControls(comboBoxSelectCity, labelEnterCity, textBoxEnterCity, labelSelectCity);
    }

    private void OptionsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      FormOptions frmOptions = new FormOptions(_configurationOptions);

      if (frmOptions.ShowDialog() == DialogResult.OK)
      {
        _configurationOptions = frmOptions.ConfigurationOptions2;
      }
    }

    private static void SetButtonEnabled(Button button, params TextBox[] textBoxes)
    {
      bool result = true;
      foreach (TextBox box in textBoxes.Where(box => box.Text.Length == 0))
      {
        result = false;
      }

      button.Enabled = result;
    }

    private static void SetButtonEnabled(Button button, params ListView[] listViews)
    {
      bool result = true;
      foreach (ListView view in listViews.Where(view => view.Items.Count == 0))
      {
        result = false;
      }

      button.Enabled = result;
    }

    private void TextBoxName_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        // do something
      }
    }

    private static void ClearComboBoxes(params ComboBox[] listOfComboBoxes)
    {
      foreach (ComboBox box in listOfComboBoxes)
      {
        box.Items.Clear();
        box.Text = string.Empty;
      }
    }

    private void ComboBoxSelectContinent_SelectedIndexChanged(object sender, EventArgs e)
    {
      //Load country combobox according to the continent chosen
      ClearComboBoxes(comboBoxSelectCountry, comboBoxSelectState, comboBoxSelectCounty, comboBoxSelectCity);
      switch (comboBoxSelectContinent.SelectedItem.ToString())
      {
        case "Europe":
          LoadComboBox(comboBoxSelectCountry, "Resources\\Countries-Europe.xml", "country");
          break;
        case "North America":
          LoadComboBox(comboBoxSelectCountry, "Resources\\countries-NorthAmerica.xml", "country");
          break;
        case "South America":
          LoadComboBox(comboBoxSelectCountry, "Resources\\countries-SouthAmerica.xml", "country");
          break;
        case "Asia":
          LoadComboBox(comboBoxSelectCountry, "Resources\\countries-asia.xml", "country");
          break;
        case "Africa":
          LoadComboBox(comboBoxSelectCountry, "Resources\\countries-africa.xml", "country");
          break;
        case "Oceania":
          LoadComboBox(comboBoxSelectCountry, "Resources\\Countries-Oceania.xml", "country");
          break;
      }

      SetComboText(comboBoxSelectCountry, "a country");
      AdjustAllControls();
    }

    private void SetComboText(ComboBox box,string area)
    {
      box.Text = Translate("Select") + Punctuation.OneSpace + Translate(area);
    }

    private void ComboBoxSelectCountry_SelectedIndexChanged(object sender, EventArgs e)
    {
      ClearComboBoxes(comboBoxSelectState, comboBoxSelectCounty, comboBoxSelectCity);
      switch (comboBoxSelectCountry.SelectedItem.ToString())
      {
        case "France":
          LoadComboBox(comboBoxSelectCity, "Resources\\Cities-France.xml", "city");
          SetComboText(comboBoxSelectCity, "a city");
          break;
        case "United States of America":
          LoadComboBox(comboBoxSelectState, "Resources\\States-USA.xml", "state");
          SetComboText(comboBoxSelectState, "a state");
          break;
      }

      AdjustAllControls();
    }

    private void ComboBoxSelectState_SelectedIndexChanged(object sender, EventArgs e)
    {
      ClearComboBoxes(comboBoxSelectCounty, comboBoxSelectCity);
      switch (comboBoxSelectState.SelectedItem.ToString())
      {
        case "Florida":
          LoadComboBox(comboBoxSelectCounty, "Resources\\Counties-Florida.xml", "county");
          SetComboText(comboBoxSelectCounty, "a county");
          break;


      }

      AdjustAllControls();
    }

    private static int LongestItemInCb(ComboBox cb)
    {
      int result = 0;
      if (cb.Items.Count == 0)
      {
        return 0;
      }

      result = cb.Items[0].ToString().Length;
      // return (from object item in cb.Items select item.ToString().Length).Concat(new[] {result}).Max();
      foreach (var item in cb.Items)
      {
        if (item.ToString().Length > result)
        {
          result = item.ToString().Length;
        }
      }

      return result;
    }

    private void ComboBoxSelectCounty_SelectedIndexChanged(object sender, EventArgs e)
    {
      // get them from https://en.wikipedia.org/wiki/Category:Cities_in_Florida_by_county

      ClearComboBoxes(comboBoxSelectCity);
      switch (comboBoxSelectCounty.SelectedItem.ToString())
      {
        case "Pinellas county":
          LoadComboBox(comboBoxSelectCity, "Resources\\Cities-Pinellas.xml", "city");
          break;
        case "Miami-Dade county":
          LoadComboBox(comboBoxSelectCity, "Resources\\Cities-Miami-Dade.xml", "city");
          break;
        case "Alachua county":
          LoadComboBox(comboBoxSelectCity, "Resources\\Cities-Alachua.xml", "city");
          break;
        case "Baker county":
          LoadComboBox(comboBoxSelectCity, "Resources\\Cities-Baker-county.xml", "city");
          break;
        case "Bay county":
          LoadComboBox(comboBoxSelectCity, "Resources\\Cities-Bay-county.xml", "city");
          break;
        case "Bradford county":
          LoadComboBox(comboBoxSelectCity, "Resources\\Cities-Bradford-county.xml", "city");
          break;
        case "Brevard county":
          LoadComboBox(comboBoxSelectCity, "Resources\\Cities-Brevard-county.xml", "city");
          break;
      }

      SetComboText(comboBoxSelectCity, "a city");
      AdjustAllControls();
    }

    private void ComboBoxSelectCity_SelectedIndexChanged(object sender, EventArgs e)
    {
      AdjustAllControls();
    }
  }
}
