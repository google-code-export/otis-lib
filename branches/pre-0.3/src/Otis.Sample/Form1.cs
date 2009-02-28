using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using Otis.Sample.Domain;
using Otis.Sample.Presentation;

namespace Otis.Sample
{
	public partial class Form1 : Form
	{
		private Configuration m_cfg = new Configuration();
		private BindingList<ArtistInfo> m_userData;

		public Form1()
		{
			InitializeComponent();

			grdRecords.AutoGenerateColumns = false;
			// configure assembler using mapping in mapping.otis.xml
			m_cfg.AddAssemblyResources(Assembly.GetExecutingAssembly(), "otis.xml");

			// retrieve list of users from somewhere
			IMusicService svc = new MusicService();
			Artist[] artists = svc.GetAllArtists();

			// get the assembler for User->UserInfo transformation
			IAssembler<ArtistInfo, Artist> asm = m_cfg.GetAssembler<ArtistInfo, Artist>();

			// transform array of User instances to a list of UserInfo instances
			// and initialize BindingList with it
			m_userData = new BindingList<ArtistInfo>(asm.ToList(artists));

			// setup data source for combo box
			cboUsers.DataSource = m_userData;
			cboUsers.DisplayMember = "Description";
			cboUsers.ValueMember = "Id";
		}

		private void cboUsers_SelectedIndexChanged(object sender, EventArgs e)
		{
			ArtistInfo userInfo = (ArtistInfo) cboUsers.SelectedItem;
			grdRecords.DataSource = userInfo.Records;
		}

	}
}