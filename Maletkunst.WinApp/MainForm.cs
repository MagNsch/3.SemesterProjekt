using Maletkunst.DAL.Interfaces;
using Maletkunst.DAL.Models;
using Maletkunst.DAL.RestClient;
using System.Data.SqlClient;
using System.Net;

namespace MaletKunst.WinApp;

public partial class MainForm : Form
{
	IPaintingsDao _client;
	public MainForm(IPaintingsDao client)
	{
		_client = client;
		InitializeComponent();
	}

	private void MainForm_Load(object sender, EventArgs e) => LoadData();

	

	private async void LoadData()
	{
		await GetAllPaintings();
		SetButtonsDeleteAndUpdateDisabled();
	}

	private void SetButtonsDeleteAndUpdateDisabled()
	{
		buttonDelete.Enabled = false;
		buttonUpdate.Enabled = false;
	}

	private void SetButtonsDeleteAndUpdateEnabled()
	{
		buttonDelete.Enabled = true;
		buttonUpdate.Enabled = true;
	}



	private async Task GetAllPaintings()
	{
		var paintings = await _client.GetAllPaintingsAsync();
		foreach (var painting in paintings)
		{
			listBoxPaintings.Items.Add(painting);
		}
	}

	private void listBoxPaintings_SelectedIndexChanged(object sender, EventArgs e) => GetSelectedPaitingFromList();

	private void GetSelectedPaitingFromList()
	{
		if (listBoxPaintings.SelectedIndex != -1)
		{
			SetButtonsDeleteAndUpdateEnabled();

			Painting painting = (Painting)listBoxPaintings.SelectedItem;

			textBoxId.Text = painting.Id.ToString();
			textBoxTitle.Text = painting.Title;
			textBoxPrice.Text = painting.Price.ToString();
			textBoxArtist.Text = painting.Artist;
			comboBoxCategory.Text = painting.Category;
			textBoxDescription.Text = painting.Description;
			if (painting.Stock == 0) { radioButtonUnavailable.Checked = true; }
			if (painting.Stock == 1) { radioButtonAvailable.Checked = true; }
			LoadImage(painting);
		}
	}

	private void LoadImage(Painting painting)
	{
		string imagePath = $"https://www.maletkunst.dk/thumbnails/{painting.Id}.jpg";
		try
		{
			using (var webClient = new WebClient())
			{
				var imageBytes = webClient.DownloadData(imagePath);
				using (var pictureFromMemoryStream = new MemoryStream(imageBytes))
				{
					pictureBox1.Image = Image.FromStream(pictureFromMemoryStream);
				}
			}
		}
		catch (WebException)
		{
			pictureBox1.Image = null;
		}
	}

	private void buttonClose_Click(object sender, EventArgs e)
	{
		Clear();
	}

	private void Clear()
	{
		textBoxId.Text = "";
		textBoxTitle.Text = "";
		textBoxPrice.Text = "";
		textBoxArtist.Text = "";
		comboBoxCategory.Text = "";
		textBoxDescription.Text = "";
		pictureBox1.Image = null;
		listBoxPaintings.SelectedItem = null;
		SetButtonsDeleteAndUpdateDisabled();
	}

	private void buttonCreate_Click(object sender, EventArgs e)
	{
		CreatePainting();
	}

	private void CreatePainting()
	{
		int stock = 0;
		if (radioButtonAvailable.Checked == true) { stock = 1; } else { stock = 0; }

		Painting painting = new()
		{
			Title = textBoxTitle.Text,
			Price = decimal.Parse(textBoxPrice.Text),
			Stock = stock,
			Artist = textBoxArtist.Text,
			Category = comboBoxCategory.Text,
			Description = textBoxDescription.Text
			// Picture....
		};
		int createdId = _client.CreatePainting(painting);
		if (createdId == 0)
		{
			MessageBox.Show("Oprettelse af maleri mislykkedes", "Fejl", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		else
		{
			MessageBox.Show($"Maleri med id {createdId} er oprettet", "Succes", MessageBoxButtons.OK, MessageBoxIcon.None);
			listBoxPaintings.Items.Clear();
			LoadData();
			Clear();
		}
	}

	private void buttonDelete_Click(object sender, EventArgs e) => DeletePainting();

	private void DeletePainting()
	{
		int paintingToDeleteId = int.Parse(textBoxId.Text);
		if (_client.DeletePaintingById(paintingToDeleteId))
		{
			MessageBox.Show($"Maleri med id {paintingToDeleteId} er slettet", "Succes", MessageBoxButtons.OK, MessageBoxIcon.None);
			listBoxPaintings.Items.Clear();
			LoadData();
			Clear();
		}
		else
		{
			MessageBox.Show($"Maleri med id {paintingToDeleteId} kunne ikke slettes, da det allerede tilhører en ordre", "Fejl", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}




	private void buttonUpdate_Click(object sender, EventArgs e) => UpdatePainting();

	private void UpdatePainting()
	{
		int stock = 0;
		if (radioButtonAvailable.Checked == true) { stock = 1; } else { stock = 0; }

		Painting painting = new()
		{
			Id = int.Parse(textBoxId.Text),
			Title = textBoxTitle.Text,
			Price = decimal.Parse(textBoxPrice.Text),
			Stock = stock,
			Artist = textBoxArtist.Text,
			Category = comboBoxCategory.Text,
			Description = textBoxDescription.Text
			// Picture....
		};
		if (!_client.UpdatePainting(painting))
		{
			MessageBox.Show("Opdatering af maleri mislykkedes", "Fejl", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		else
		{
			MessageBox.Show($"Maleri med id {painting.Id} er opdateret", "Succes", MessageBoxButtons.OK, MessageBoxIcon.None);
			listBoxPaintings.Items.Clear();
			LoadData();
			Clear();
		}
	}
}