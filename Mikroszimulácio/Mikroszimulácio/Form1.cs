using Mikroszimulácio.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mikroszimulácio
{
    public partial class Form1 : Form
    {
        Random rng = new Random(1234);

        List<Person> Population = null;
        List<BirthProbability> BirthProbabilities = null;
        List<DeathProbability> DeathProbabilities = null;

        public Form1()
        {
            InitializeComponent();


            BirthProbabilities = GetBirthProbabilities(@"C:\Temp\születés.csv");
            DeathProbabilities = GetDeathProbabilities(@"C:\Temp\halál.csv");
            
        }

        private void StartSimulation(int endyear, string csvPath)
        {
            Population = GetPopulation(csvPath); //@ a backslash miatt

            // Végigmegyünk a vizsgált éveken
            for (int year = 2005; year <= endyear; year++)
            {
                // Végigmegyünk az összes személyen
                for (int i = 0; i < Population.Count; i++)
                {
                    SimStep(year, Population[i]);
                }

                int nbrOfMales = (from x in Population
                                  where x.Gender == Gender.Male && x.IsAlive
                                  select x).Count();
                int nbrOfFemales = (from x in Population
                                    where x.Gender == Gender.Female && x.IsAlive
                                    select x).Count();
                richTextBox1.Text +=
                    string.Format("Év:{0}\n\tFiúk:{1}\n\tLányok:{2}\n", year, nbrOfMales, nbrOfFemales);
            }
        }

        public List<Person> GetPopulation (string csvPath)
        {
            List<Person> population = new List<Person>();

            using (StreamReader sr = new StreamReader(csvPath, Encoding.Default))
            {
                sr.ReadLine();

                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');
                    population.Add(new Person()
                    {
                        BirthYear = int.Parse(line[0]),
                        Gender = (Gender)Enum.Parse(typeof(Gender), line[1]),
                        NbrOfChildren = int.Parse(line[2])
                    });
                }
            }

            return population;
        }

        public List<BirthProbability> GetBirthProbabilities(string csvPath)
        {
            List<BirthProbability> population = new List<BirthProbability>();

            using (StreamReader sr = new StreamReader(csvPath, Encoding.Default))
            {
                sr.ReadLine();

                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');
                    population.Add(new BirthProbability()
                    {
                        Age = int.Parse(line[0]),
                        NbrOfChildren = int.Parse(line[1]),
                        P = double.Parse(line[2])
                    });
                }
            }

            return population;
        }

        public List<DeathProbability> GetDeathProbabilities(string csvPath)
        {
            List<DeathProbability> population = new List<DeathProbability>();

            using (StreamReader sr = new StreamReader(csvPath, Encoding.Default))
            {
                sr.ReadLine();

                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');
                    population.Add(new DeathProbability()
                    {
                        Age = int.Parse(line[0]),
                        Gender = (Gender)Enum.Parse(typeof(Gender), line[1]),
                        P = double.Parse(line[2])
                    });
                }
            }

            return population;
        }

        private void SimStep(int year, Person person)
        {
            //Ha halott akkor kihagyjuk, ugrunk a ciklus következő lépésére
            if (!person.IsAlive) return;

            // Letároljuk az életkort, hogy ne kelljen mindenhol újraszámolni
            byte age = (byte)(year - person.BirthYear);

            // Halál kezelése
            // Halálozási valószínűség kikeresése
            double pDeath = (from x in DeathProbabilities
                             where x.Gender == person.Gender && x.Age == age
                             select x.P).FirstOrDefault();
            // Meghal a személy?
            if (rng.NextDouble() <= pDeath)
                person.IsAlive = false;

            //Születés kezelése - csak az élő nők szülnek
            if (person.IsAlive && person.Gender == Gender.Female)
            {
                //Szülési valószínűség kikeresése
                double pBirth = (from x in BirthProbabilities
                                 where x.Age == age
                                 select x.P).FirstOrDefault();
                //Születik gyermek?
                if (rng.NextDouble() <= pBirth)
                {
                    Person újszülött = new Person();
                    újszülött.BirthYear = year;
                    újszülött.NbrOfChildren = 0;
                    újszülött.Gender = (Gender)(rng.Next(1, 3));
                    Population.Add(újszülött);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StartSimulation((int)numericUpDown1.Value, textBox1.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.FileName = textBox1.Text;
            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            textBox1.Text = ofd.FileName;
        }
    }
}
