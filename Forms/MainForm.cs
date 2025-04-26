using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using BioLis_30i.DTOs;
using BioLis_30i.Services;

namespace BioLis_30i.Forms
{
    public partial class MainForm : Form
    {
        private TcpListener _listener;
        private TcpClient _client;
        private bool _isListening = false;
        private Thread _listenerThread;
        private BioLis30iParser _parser;
        private GenericResultMappingService _genericParser;
        private delegate void UpdateUIDelegate(string message);
        private delegate void UpdateResultsDelegate(List<GenericResult> results);
        private System.Windows.Forms.Panel panelBottom;

        public MainForm()
        {
            InitializeComponent();
            _parser = new BioLis30iParser();
            _genericParser = new GenericResultMappingService();
            
            // Configure DataGridView columns for OLU message results
            dgvResults.Columns.Clear();
            dgvResults.Columns.Add("MessageId", "Message ID");
            dgvResults.Columns.Add("TestCode", "Test Code");
            dgvResults.Columns.Add("TestName", "Test Name");
            dgvResults.Columns.Add("Result", "Result");
            dgvResults.Columns.Add("Units", "Units");
            dgvResults.Columns.Add("ReferenceRange", "Reference Range");
            dgvResults.Columns.Add("ObservationDateTime", "Observation Date/Time");
            dgvResults.Columns.Add("ResultStatus", "Status");
            
            // Configure column properties
            foreach (DataGridViewColumn column in dgvResults.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                column.ReadOnly = true;
            }
            
            // Set default port and IP
            txtPort.Text = "50001";
            txtIP.Text = "127.0.0.1";
        }

        private void InitializeComponent()
        {
            this.btnStartStop = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.txtConsole = new System.Windows.Forms.TextBox();
            this.dgvResults = new System.Windows.Forms.DataGridView();
            this.lblStatus = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.lblIP = new System.Windows.Forms.Label();
            this.txtIP = new System.Windows.Forms.TextBox();
            
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Height = 60;
            this.panelTop.Padding = new System.Windows.Forms.Padding(10);
            
            // 
            // btnStartStop
            // 
            this.btnStartStop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnStartStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartStop.ForeColor = System.Drawing.Color.White;
            this.btnStartStop.Location = new System.Drawing.Point(10, 15);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(120, 30);
            this.btnStartStop.TabIndex = 0;
            this.btnStartStop.Text = "Start Listening";
            this.btnStartStop.UseVisualStyleBackColor = false;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            
            // 
            // btnClear
            // 
            this.btnClear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.ForeColor = System.Drawing.Color.White;
            this.btnClear.Location = new System.Drawing.Point(136, 15);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(80, 30);
            this.btnClear.TabIndex = 9;
            this.btnClear.Text = "Clear All";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            
            // 
            // txtConsole
            // 
            this.txtConsole.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.txtConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtConsole.ForeColor = System.Drawing.Color.White;
            this.txtConsole.Multiline = true;
            this.txtConsole.Name = "txtConsole";
            this.txtConsole.ReadOnly = true;
            this.txtConsole.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtConsole.TabIndex = 1;
            this.txtConsole.Font = new System.Drawing.Font("Consolas", 9F);
            
            // 
            // dgvResults
            // 
            this.dgvResults.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.dgvResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResults.Name = "dgvResults";
            this.dgvResults.TabIndex = 2;
            this.dgvResults.EnableHeadersVisualStyles = false;
            this.dgvResults.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.dgvResults.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.dgvResults.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.dgvResults.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.dgvResults.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.White;
            this.lblStatus.Location = new System.Drawing.Point(12, 428);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(100, 15);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Status: Not Listening";
            
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 60);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitContainer1.Size = new System.Drawing.Size(800, 390);
            this.splitContainer1.SplitterDistance = 200;
            this.splitContainer1.TabIndex = 4;
            this.splitContainer1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtConsole);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 200);
            this.panel1.TabIndex = 0;
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dgvResults);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(800, 186);
            this.panel2.TabIndex = 0;
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.ForeColor = System.Drawing.Color.White;
            this.lblPort.Location = new System.Drawing.Point(450, 20);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(35, 15);
            this.lblPort.TabIndex = 5;
            this.lblPort.Text = "Port:";
            
            // 
            // txtPort
            // 
            this.txtPort.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.txtPort.ForeColor = System.Drawing.Color.White;
            this.txtPort.Location = new System.Drawing.Point(491, 17);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(60, 23);
            this.txtPort.TabIndex = 6;
            this.txtPort.Text = "50001";
            
            // 
            // lblIP
            // 
            this.lblIP.AutoSize = true;
            this.lblIP.ForeColor = System.Drawing.Color.White;
            this.lblIP.Location = new System.Drawing.Point(570, 20);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(20, 15);
            this.lblIP.TabIndex = 7;
            this.lblIP.Text = "IP:";
            
            // 
            // txtIP
            // 
            this.txtIP.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.txtIP.ForeColor = System.Drawing.Color.White;
            this.txtIP.Location = new System.Drawing.Point(596, 17);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(120, 23);
            this.txtIP.TabIndex = 8;
            this.txtIP.Text = "127.0.0.1";
            
            // 
            // panelBottom
            // 
            this.panelBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Height = 30;
            this.panelBottom.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.White;
            this.lblStatus.Location = new System.Drawing.Point(10, 7);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(100, 15);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Status: Not Listening";
            
            // Add lblStatus to panelBottom
            this.panelBottom.Controls.Add(this.lblStatus);
            
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.txtIP);
            this.Controls.Add(this.lblIP);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.btnStartStop);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelBottom);
            this.Name = "MainForm";
            this.Text = "BioLis-30i HL7 Listener";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            
            // Set up split container panels
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            this.splitContainer1.Panel2.Controls.Add(this.panel2);
            
            // Add btnClear to panelTop
            this.panelTop.Controls.Add(this.btnClear);
            
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.TextBox txtConsole;
        private System.Windows.Forms.DataGridView dgvResults;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label lblIP;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Panel panelTop;

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (!_isListening)
            {
                StartListening();
            }
            else
            {
                StopListening();
            }
        }

        private void StartListening()
        {
            try
            {
                int port = int.Parse(txtPort.Text);
                string ipAddress = txtIP.Text;
                
                _listener = new TcpListener(IPAddress.Parse(ipAddress), port);
                _listener.Start();

                _isListening = true;
                btnStartStop.Text = "Stop Listening";
                lblStatus.Text = $"Status: Listening on {ipAddress}:{port}";
                
                LogToConsole($"Started listening on {ipAddress}:{port}");
                
                _listenerThread = new Thread(ListenForMessages);
                _listenerThread.IsBackground = true;
                _listenerThread.Start();
            }
            catch (Exception ex)
            {
                LogToConsole($"Error starting listener: {ex.Message}");
                MessageBox.Show($"Error starting listener: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StopListening()
        {
            try
            {
                _isListening = false;
                _listener?.Stop();
                btnStartStop.Text = "Start Listening";
                lblStatus.Text = "Status: Not Listening";
                LogToConsole("Stopped listening");
            }
            catch (Exception ex)
            {
                LogToConsole($"Error stopping listener: {ex.Message}");
                MessageBox.Show($"Error stopping listener: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ListenForMessages()
        {
            while (_isListening)
            {
                try
                {
                    _client = _listener.AcceptTcpClient();
                    LogToConsole($"Connection received from {_client.Client.RemoteEndPoint}");
                    
                    // Handle the client in the same thread to maintain the connection
                    HandleClient(_client);
                }
                catch (SocketException)
                {
                    // This is expected when stopping the listener
                    if (_isListening)
                    {
                        LogToConsole("Listener stopped unexpectedly");
                    }
                }
                catch (Exception ex)
                {
                    LogToConsole($"Error accepting connection: {ex.Message}");
                }
            }
        }

        private void HandleClient(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[4096];
                StringBuilder messageBuilder = new StringBuilder();
                
                while (_isListening && client.Connected)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        // Connection closed by client
                        break;
                    }
                    
                    string chunk = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    messageBuilder.Append(chunk);
                    
                    // Check for message terminator
                    if (chunk.Contains("\u001C"))
                    {
                        string message = messageBuilder.ToString();
                        LogToConsole($"Received message: {message}");

                        // Parse the message
                        var parsedMessage = _parser.ParseMessage(message);
                        var oluMessage = _parser.MapToOLUDTO(parsedMessage);
                        var genericResults = _genericParser.MapToGenericResult(oluMessage);
                        
                        // Update UI with results
                        UpdateResults(genericResults);
                        
                        // Send acknowledgment
                        string ack = _parser.CreateAcknowledgment(parsedMessage.MessageId);
                        byte[] ackBytes = Encoding.UTF8.GetBytes(ack);
                        stream.Write(ackBytes, 0, ackBytes.Length);
                        
                        LogToConsole($"Sent acknowledgment: {ack}");
                        
                        // Clear the message builder for the next message
                        messageBuilder.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                LogToConsole($"Error handling client: {ex.Message}");
            }
            finally
            {
                if (client != null)
                {
                    client.Close();
                    LogToConsole("Client connection closed");
                }
            }
        }

        private void LogToConsole(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new UpdateUIDelegate(LogToConsole), message);
                return;
            }
            
            txtConsole.AppendText($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}");
            txtConsole.ScrollToCaret();
        }

        private void UpdateResults(List<GenericResult> results)
        {
            if (InvokeRequired)
            {
                Invoke(new UpdateResultsDelegate(UpdateResults), results);
                return;
            }
            
            foreach (var result in results)
            {
                dgvResults.Rows.Add(
                    result.MessageId,
                    result.TestCode,
                    result.TestName,
                    result.Result,
                    result.Units,
                    result.ReferenceRange,
                    result.ObservationDateTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                    result.ResultStatus
                );
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopListening();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtConsole.Clear();
            dgvResults.Rows.Clear();
            LogToConsole("Console and results cleared");
        }
    }
} 
 