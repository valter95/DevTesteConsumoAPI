using ConsumindoAPIChallenge.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsumindoAPIChallenge
{
    public partial class FrmPrincipal : Form
    {
        string URI = "";
        int codigoUsuário;
        public FrmPrincipal()
        {
            InitializeComponent();
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            codigoUsuário = Convert.ToInt32(txtId.Text);
            GetUsuariosById(codigoUsuário);
        }

        private async void AddUsuário()
        {
            URI = txtURI.Text;
            Usuario user = new Usuario();
            user.Nome = txtNome.Text;
            user.DataNascimento = Convert.ToDateTime(txtData.Text);

            using (var client = new HttpClient())
            {
                var serializedUsuário = JsonConvert.SerializeObject(user);
                var content = new StringContent(serializedUsuário, Encoding.UTF8, "application/json");
                var result = await client.PostAsync(URI, content);
            }
            GetAllUsuários(URI);
        }

        private async void GetAllUsuários(string uri)
        {
            URI = uri;
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(URI))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var ProdutoJsonString = await response.Content.ReadAsStringAsync();
                        dgvDados.DataSource = JsonConvert.DeserializeObject<Usuario[]>(ProdutoJsonString).ToList();
                    }
                    else
                    {
                        MessageBox.Show("Não foi possível obter o usuário : " + response.StatusCode);
                    }
                }
            }
        }

        private async void GetUsuariosById(int codProduto)
        {
            using (var client = new HttpClient())
            {
                BindingSource bsDados = new BindingSource();
                URI = txtURI.Text + "/" + codProduto.ToString();
                HttpResponseMessage response = await client.GetAsync(URI);
                if (response.IsSuccessStatusCode)
                {
                    var UsuarioJsonString = await response.Content.ReadAsStringAsync();
                    bsDados.DataSource = JsonConvert.DeserializeObject<Usuario>(UsuarioJsonString);
                    dgvDados.DataSource = bsDados;
                }
                else
                {
                    MessageBox.Show("Falha ao obter o usuário : " + response.StatusCode);
                }
            }
        }

        private async void UpdateUsuarios(int codUsuario)
        {
            URI = txtURI.Text;
            Usuario user = new Usuario();
            user.IdUsuario = codUsuario;
            user.Nome = txtNome.Text;
            user.DataNascimento = Convert.ToDateTime(txtData.Text); // atualizando o preço do produto

            using (var client = new HttpClient())
            {
                HttpResponseMessage responseMessage = await client.PutAsJsonAsync(URI + "/" + user.IdUsuario, user);
                if (responseMessage.IsSuccessStatusCode)
                {
                    MessageBox.Show("Usuário atualizado");
                }
                else
                {
                    MessageBox.Show("Falha ao atualizar o usuário : " + responseMessage.StatusCode);
                }
            }
            GetAllUsuários(URI);
        }

        private async void DeleteUsuarios(int codProduto)
        {
            URI = txtURI.Text;
            int ProdutoID = codProduto;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URI);
                HttpResponseMessage responseMessage = await
                client.DeleteAsync(String.Format("{0}/{1}", URI, ProdutoID));
                if (responseMessage.IsSuccessStatusCode)
                {
                    MessageBox.Show("Usuário excluído com sucesso");
                }
                else
                {
                    MessageBox.Show("Falha ao excluir o Usuário  : " + responseMessage.StatusCode);
                }
            }
            GetAllUsuários(URI);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void FrmPrincipal_Load(object sender, EventArgs e)
        {

            string URI = ConfigurationManager.AppSettings["URI"];
            txtURI.Text = URI;
            txtData.Text = DateTime.Now.ToString();
            GetAllUsuários(URI);


            if (cboAcao.SelectedIndex == -1)
            {
                cboAcao.SelectedIndex = 0;
                lblUser.Visible = false;
                lblNome.Visible = false;
                LblData.Visible = false;
                txtId.Visible = false;
                txtNome.Visible = false;
                txtData.Visible = false;
                btnCadastrar.Visible = false;
                btnAtualizar.Visible = false;
                btnConsultar.Visible = false;
                btnExcluir.Visible = false;
            }

        }

        private void btnCadastrar_Click(object sender, EventArgs e)
        {
            AddUsuário();
        }

        private void btnAtualizar_Click(object sender, EventArgs e)
        {
            codigoUsuário = Convert.ToInt32(txtId.Text);
            UpdateUsuarios(codigoUsuário);
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            codigoUsuário = Convert.ToInt32(txtId.Text);
            DeleteUsuarios(codigoUsuário);
        }

        private void cboAcao_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cboAcao.SelectedIndex == 1)
            {
                lblUser.Visible = false;
                lblNome.Visible = true;
                LblData.Visible = true;
                txtNome.Visible = true;
                txtData.Visible = true;
                btnCadastrar.Visible = true;

            }
            else if (cboAcao.SelectedIndex == 2)
            {
                lblUser.Visible = true;
                lblNome.Visible = true;
                LblData.Visible = true;
                txtId.Visible = true;
                txtNome.Visible = true;
                txtData.Visible = true;
                btnAtualizar.Visible = true;
                btnCadastrar.Visible = false;

            }
            else if (cboAcao.SelectedIndex == 3)
            {
                lblUser.Visible = true;
                lblNome.Visible = false;
                LblData.Visible = false;
                txtId.Visible = true;
                txtNome.Visible = false;
                txtData.Visible = false;
                btnAtualizar.Visible = false;
                btnCadastrar.Visible = false;
                btnConsultar.Visible = true;
            }
            else if (cboAcao.SelectedIndex == 4)
            {
                lblUser.Visible = true;
                lblNome.Visible = false;
                LblData.Visible = false;
                txtId.Visible = true;
                txtNome.Visible = false;
                txtData.Visible = false;
                btnAtualizar.Visible = false;
                btnCadastrar.Visible = false;
                btnConsultar.Visible = false;
                btnExcluir.Visible = true;
            }
        }
    }
}
