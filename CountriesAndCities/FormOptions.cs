using System;
using System.Windows.Forms;

namespace CountriesAndCities
{
  internal abstract partial class FormOptions : Form
  {
    internal FormOptions(ConfigurationOptions configurationOptions)
    {
      if (configurationOptions == null)
      {
        //throw new ArgumentNullException(nameof(configurationOptions));
        ConfigurationOptions2 = new ConfigurationOptions();
      }
      else
      {
        ConfigurationOptions2 = configurationOptions;
      }

      InitializeComponent();
      checkBoxOption1.Checked = ConfigurationOptions2.Option1Name;
      checkBoxOption2.Checked = ConfigurationOptions2.Option2Name;
    }

    public ConfigurationOptions ConfigurationOptions2 { get; }

    private void buttonOptionsOK_Click(object sender, EventArgs e)
    {
      ConfigurationOptions2.Option1Name = checkBoxOption1.Checked;
      ConfigurationOptions2.Option2Name = checkBoxOption2.Checked;
      Close();
    }

    private void buttonOptionsCancel_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void FormOptions_Load(object sender, EventArgs e)
    {
      // take care of language
      //buttonOptionsCancel.Text = _languageDicoEn["Cancel"];
    }
  }
}