using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Simulation;
using GeneticAlgo;
using System.Threading;

namespace My_Genetic_Algorithm_GUI
{
    public partial class Form1 : Form
    {
        List<KeyValuePair<int, double>> loadingElements = new List<KeyValuePair<int, double>>();
        List<KeyValuePair<int, double>> weighingElements = new List<KeyValuePair<int, double>>();
        List<KeyValuePair<int, double>> travelingElements = new List<KeyValuePair<int, double>>();
        Simu.Sim SimulationResults;
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// this button is made to rn the GA
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                chart1.Series["Series1"].Points.Clear();
                progressBar1.Visible = true;
                progressBar1.Minimum = 1;
                int population = int.Parse(txtbxPopulationNo.Text);
                float numCoal = float.Parse(txtbxmaterialAlgo.Text);
                float numTruckLoad = float.Parse(txtbxlodrprtrukAlgo.Text);
                float costTruckPD = float.Parse(txtbxcostprtrukAlgo.Text);
                float costLoaderPD = float.Parse(txtbxCostPrLoderAlgo.Text);
                float costScalerPD = float.Parse(txtbxCostPrScalerAlgo.Text);
                float projectDuration = float.Parse(txtbxProjectDurationAlgo.Text);
                float costDelayPD = float.Parse(txtbxCostOfDelayAlgo.Text);
                int numTruck = int.Parse(txtbxTrukNoMxAlgo.Text);
                int numScaler = int.Parse(txtbxScalerNoMxAlgo.Text);
                int numLoader = int.Parse(txtbxLosderNoMxAlgo.Text);
                int lastGeneration = int.Parse(txtbxGenerationNo.Text);
                progressBar1.Maximum = lastGeneration;

                GeneticAlgo.program.GeneticAlgoResults r = new GeneticAlgo.program.GeneticAlgoResults();
                GeneticAlgo(population, numCoal, numTruckLoad, costTruckPD, costLoaderPD, costScalerPD, projectDuration, costDelayPD, numTruck, numLoader, numScaler, lastGeneration, ref r);
              
                lblTruckNoAlgo.Text = r.BestGene.Genes[0].ToString();
                lblLoaderNoAlgo.Text = r.BestGene.Genes[1].ToString();
                lblScalerNoAlgo.Text = r.BestGene.Genes[2].ToString();
                lblUtiliLoaderAlgo.Text = Math.Round(r.BestGene.utilLoader, 3).ToString();
                lblUtiliScalerAlgo.Text = Math.Round(r.BestGene.utilScaler, 3).ToString();
                lblUtiliTruckAlgo.Text = Math.Round(r.BestGene.utilTruck, 3).ToString();
                lblDaysDelayedAlgo.Text = r.BestGene.DaysofDelay.ToString();
                lblCostodDelayAlgo.Text = r.BestGene.CostofDelay.ToString();
                lblTotalDaysAlgo.Text = r.BestGene.TotalDays.ToString();
                lblTotalCostAlgo.Text = r.BestGene.TotalCost.ToString();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        void GeneticAlgo(int size, float numCoal, float numTruckLoad, float costTruckPD, float costLoaderPD, float costScalerPD, float projectDuration, float costDelayPD, int numTruck, int numLoader, int numScaler, int lastGeneration, ref program.GeneticAlgoResults res)
        {
            GeneticAlgo.program.GeneticAlgorithm Population;
            int x = 0;
            //float numCoal = 10000, numTruckLoad = 20, costTruckPD = 1000, costLoaderPD = 2000, costScalerPD = 3000, projectDuration = 120, costDelayPD = 10000;
            Population = new GeneticAlgo.program.GeneticAlgorithm(size, numTruck, numLoader, numScaler, FitnessFunction, numCoal);
            GeneticAlgo.program.GeneticAlgoResults Results = new GeneticAlgo.program.GeneticAlgoResults();
            while (true)
            {
                x++;
                progressBar1.Value = x;
                chart1.Series["Series1"].Points.AddXY(x, Population.BestofEachGeneration.Fitness);
                chart1.Update();
                if (x == lastGeneration)
                {
                    break;
                }
                Population.NewGeneration();
            }

            Results.BestGeneration = Population.BestGeneGeneration;
            Results.BestGene = Population.BestGene;

            Simu.Sim FitnessFunction(float numTrucks, float numLoaders, float numScalers)
            {
                Simu.Sim simResults = Simu.Simulate(numCoal, numTrucks, numTruckLoad, costTruckPD, numLoaders, costLoaderPD, numScalers, costScalerPD, projectDuration, costDelayPD, loadingElements, weighingElements, travelingElements);
                return simResults;
            }
            res = Results;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            picbxalgo.Enabled = true;
            dataGridView1.Columns[0].Name = "Time";
            dataGridView2.Columns[0].Name = "Time";
            dataGridView3.Columns[0].Name = "Time";
            dataGridView1.Columns[1].Name ="Propability";
            dataGridView2.Columns[1].Name = "Propability";
            dataGridView3.Columns[1].Name = "Propability";
         }

        private void button6_Click(object sender, EventArgs e)
        {
            loadingElements.Clear();
            weighingElements.Clear();
            travelingElements.Clear();
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                loadingElements.Add(new KeyValuePair<int, double>(int.Parse(dataGridView1.Rows[i].Cells[0].Value.ToString()), double.Parse(dataGridView1.Rows[i].Cells[1].Value.ToString())));
            }
            for (int i = 0; i < dataGridView2.Rows.Count - 1; i++)
            {
                weighingElements.Add(new KeyValuePair<int, double>(int.Parse(dataGridView2.Rows[i].Cells[0].Value.ToString()), double.Parse(dataGridView2.Rows[i].Cells[1].Value.ToString())));
            }
            for (int i = 0; i < dataGridView3.Rows.Count - 1; i++)
            {
                travelingElements.Add(new KeyValuePair<int, double>(int.Parse(dataGridView3.Rows[i].Cells[0].Value.ToString()), double.Parse(dataGridView3.Rows[i].Cells[1].Value.ToString())));
            }
        }

        private void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;
            grid.Rows[e.RowIndex].ErrorText = "";   
            if (e.ColumnIndex == 0)
            {
                int newInteger;
                if (grid.Rows[e.RowIndex].IsNewRow) { return; }
                if (!int.TryParse(e.FormattedValue.ToString(), out newInteger) || newInteger <= 0)
                {
                    e.Cancel = true;
                    grid.Rows[e.RowIndex].ErrorText = "the value must be a non-negative integer";
                }
                if (grid.Rows[e.RowIndex].Cells[1].Value == null)
                {
                    grid.AllowUserToAddRows = false;
                }
                else
                {
                    grid.AllowUserToAddRows = true;
                }
            }
            if (e.ColumnIndex == 1)
            {
                double newDouble;
                double sum = 0;
                if (grid.Rows[e.RowIndex].IsNewRow) { return; }
                if (!double.TryParse(Convert.ToString(e.FormattedValue), out newDouble) || newDouble <= 0)
                {
                    e.Cancel = true;
                    grid.Rows[e.RowIndex].ErrorText = "the value must be a non zero positive double";
                }
                else
                {
                    for (int i = 0; i < grid.Rows.Count - 1; i++)
                    {
                        if (grid.Rows[i].Cells[1].Value == null)
                        {
                            continue;
                        }
                        if (i == e.RowIndex)
                        {
                            sum += Convert.ToDouble(e.FormattedValue);

                        }
                        else
                        {
                            sum += double.Parse(grid.Rows[i].Cells[1].Value.ToString());
                        }
                    }
                    if (sum > 1)
                    {
                        e.Cancel = true;
                        grid.Rows[e.RowIndex].ErrorText = "the total probability must be less than 1";
                    }
                }
                if (grid.Rows[e.RowIndex].Cells[0].Value == null)
                {
                    grid.AllowUserToAddRows = false;
                }
                else
                {
                    grid.AllowUserToAddRows = true;
                }
            }
        }
        /// <summary>
        /// this is simulation button 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                chart2.Series["Actual Cost"].Points.Clear();
                chart2.Series["Delay Cost"].Points.Clear();
                float numcoal = float.Parse(txtbxmaterialsim.Text);
                float numtruck = float.Parse(txtbxTrukNO.Text);
                float numtruckload = float.Parse(txtbxlodrprtruk.Text);
                float costtruckPD = float.Parse(txtbxcostprtruk.Text);
                float numloader = float.Parse(txtbxLoaderNo.Text);
                float costloaderPD = float.Parse(txtbxCostPrLoder.Text);
                float ScalerNo = float.Parse(txtbxScalerNo.Text);
                float costscalerPD = float.Parse(txtbxCostPrScaler.Text);
                float projectDuration = float.Parse(txtbxProjectDuration.Text);
                float CostofDelay = float.Parse(txtbxCostOfDelay.Text);
                SimulationResults = Simu.Simulate(numcoal, numtruck, numtruckload, costtruckPD, numloader, costloaderPD, ScalerNo, costscalerPD, projectDuration, CostofDelay, loadingElements, weighingElements, travelingElements);
                lblLoaderCost.Text = SimulationResults.totalCostLoader.ToString();
                lblScalerCost.Text = SimulationResults.totalCostScaler.ToString();
                lblTruckCost.Text = SimulationResults.totalCostTruck.ToString();
                lblDelayCost.Text = SimulationResults.costofDelay.ToString();
                lblprojectDuration.Text = SimulationResults.totaldays.ToString();
                lbltotalcost.Text = SimulationResults.totalCost.ToString();
                lbldaysofdelayed.Text = SimulationResults.DelayDuration.ToString();
                lblUtilScaler.Text = Math.Round(SimulationResults.utilScaler,3).ToString();
                UtilLoader.Text = Math.Round(SimulationResults.utilLoader, 3).ToString();
                UtilTrucks.Text = Math.Round(SimulationResults.utilTruck,3).ToString();

                chart2.Series["Actual Cost"].Points.AddXY(0,SimulationResults.totalCost);
                chart2.Series["Delay Cost"].Points.AddXY(2,SimulationResults.costofDelay);
                chart2.Series["Actual Cost"].BorderWidth = 3;
                chart2.Series["Delay Cost"].BorderWidth = 3;
                chart2.ChartAreas[0].AxisX.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.False;
                MessageBox.Show("Simulation is Done ","Message Information",MessageBoxButtons.OK,MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// this button is made to run GA
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            chart1.Series["Series1"].Points.Clear();
            progressBar1.Visible = true;
            progressBar1.Minimum = 1;
            progressBar1.Maximum = 100; 

            GeneticAlgo.program.GeneticAlgoResults r = new GeneticAlgo.program.GeneticAlgoResults();
            GeneticAlgo(20, 10000, 20, 1000, 2000, 3000, 120, 10000, 6, 2, 2, 100, ref r);
            try
            {
                lblTruckNoAlgo.Text = r.BestGene.Genes[0].ToString();
                lblLoaderNoAlgo.Text = r.BestGene.Genes[1].ToString();
                lblScalerNoAlgo.Text = r.BestGene.Genes[2].ToString();
                lblUtiliLoaderAlgo.Text = Math.Round(r.BestGene.utilLoader, 3).ToString();
                lblUtiliScalerAlgo.Text = Math.Round(r.BestGene.utilScaler, 3).ToString();
                lblUtiliTruckAlgo.Text = Math.Round(r.BestGene.utilTruck, 3).ToString();
                lblDaysDelayedAlgo.Text = r.BestGene.DaysofDelay.ToString();
                lblCostodDelayAlgo.Text = r.BestGene.CostofDelay.ToString();
                lblTotalDaysAlgo.Text = r.BestGene.TotalDays.ToString();
                lblTotalCostAlgo.Text = r.BestGene.TotalCost.ToString();
                MessageBox.Show("GA is Done ", "Message Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
               MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// this is the clear button for the simulation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtbxmaterialsim.Clear();
            txtbxTrukNO.Clear();
            txtbxlodrprtruk.Clear();
            txtbxcostprtruk.Clear();
            txtbxLoaderNo.Clear();
            txtbxCostPrLoder.Clear();
            txtbxScalerNo.Clear();
            txtbxCostPrScaler.Clear();
            txtbxProjectDuration.Clear(); ;
            txtbxCostOfDelay.Clear();
        }

        /// <summary>
        /// this is the clear button for the data grids
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearTables_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            dataGridView3.Rows.Clear();
        }
        /// <summary>
        /// this is the clear button for the GA
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            txtbxCostOfDelayAlgo.Clear();
            txtbxCostPrLoderAlgo.Clear();
            txtbxCostPrScalerAlgo.Clear();
            txtbxcostprtrukAlgo.Clear();
            txtbxScalerNoMxAlgo.Clear();
            txtbxTrukNoMxAlgo.Clear();
            txtbxPopulationNo.Clear();
            txtbxGenerationNo.Clear();
            txtbxMutationRate.Clear();
            txtbxmaterialAlgo.Clear();
            txtbxlodrprtrukAlgo.Clear();
            txtbxLosderNoMxAlgo.Clear();
            txtbxProjectDurationAlgo.Clear();
        }
    }
}
