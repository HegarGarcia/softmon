﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Runtime.Serialization;
using System.IO;

namespace Softmon
{
    public partial class Main : Form
    {
        private List<Pokemon> Pokedex = new List<Pokemon>();
        private Trainer Player = null;

        private string pokedexFilePath = $@"{Environment.CurrentDirectory}\\Pokedex.xml";
        private string trainerFilePath = $@"{Environment.CurrentDirectory}\\Trainer.xml";

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            Initialize();
            savingTimer.Enabled = true;
            
        }
        
        private void savingTimer_Tick(object sender, EventArgs e)
        {
            //Create Serializer and Settins
            DataContractSerializer trainerSerializer = new DataContractSerializer(typeof(Trainer));
            XmlWriterSettings settings = new XmlWriterSettings { Indent = true };

            //Recreate file
            XmlWriter trainerFile = XmlWriter.Create(trainerFilePath, settings);

            //Put data in file and close file
            trainerSerializer.WriteObject(trainerFile, Player);
            trainerFile.Close();
        }

        private void Initialize()
        {
            //Create Serializer
            DataContractSerializer pokedexSerializer = new DataContractSerializer(typeof(List<Pokemon>));
            DataContractSerializer trainerSerializer = new DataContractSerializer(typeof(Trainer));

            try //Read file
            {
                LoadPokedex(pokedexSerializer);
                LoadTrainer(trainerSerializer);
            }
            catch //Create and fill File
            {
                XmlWriterSettings settings = new XmlWriterSettings { Indent = true };
                bool pokedexExists = File.Exists(pokedexFilePath);
                bool trainerExists = File.Exists(trainerFilePath);

                if (!pokedexExists) //Create Pokedex File and Set data to file
                    CreatePokedex(pokedexSerializer, settings);

                if (!trainerExists) //Create Trainer File and Set data to file
                    CreateTrainer(trainerSerializer, settings);

                this.Close();
                Application.Restart();
            }
        }

        public void LoadPokedex(DataContractSerializer serializer)
        {
            //Get Pokedex File
            FileStream pokedexFile = new FileStream(pokedexFilePath, FileMode.Open);

            //Get data from file
            Pokedex = (List<Pokemon>)serializer.ReadObject(pokedexFile);

            //Close file
            pokedexFile.Close();
        }

        public void LoadTrainer(DataContractSerializer serializer)
        {
            //Get Pokedex File
            FileStream trainerFile = new FileStream(trainerFilePath, FileMode.Open);

            //Get data from file
            Player = (Trainer)serializer.ReadObject(trainerFile);

            //Close file
            trainerFile.Close();
        }

        private void CreatePokedex(DataContractSerializer serializer, XmlWriterSettings settings)
        {
            //Adding starting pokemons...
            Pokemon bulbasaur = new PokemonGrass() { Name = "Bulbasaur", Health = 45,
                Attack = 49, Defense = 49, SpAttack = 65, Id = 1, };

            Pokemon charmander = new PokemonFire() { Name = "Charmander", Health = 39,
                Attack = 52, Defense = 43, SpAttack = 60, Id = 2 };

            Pokemon squirtle = new PokemonWater() { Name = "Squirtle", Health = 44,
                Attack = 48, Defense = 65, SpAttack = 50, Id = 3 };

            Pokemon pidgey = new PokemonFlying() { Name = "Pidgey", Health = 40,
                Attack = 45, Defense = 40, SpAttack = 35, Id = 4 };

            Pokemon rattata = new PokemonNormal() { Name = "Rattata", Health = 30,
                Attack = 56, Defense = 35, SpAttack = 25, Id = 5 };

            Pokemon spearow = new PokemonFlying() { Name = "Spearow", Health = 40,
                Attack = 60, Defense = 30, SpAttack = 31, Id = 6 };

            Pokedex.Add(bulbasaur);
            Pokedex.Add(charmander);
            Pokedex.Add(squirtle);
            Pokedex.Add(pidgey);
            Pokedex.Add(rattata);
            Pokedex.Add(spearow);


            XmlWriter pokedexFile = XmlWriter.Create(pokedexFilePath, settings);
            serializer.WriteObject(pokedexFile, Pokedex);
            pokedexFile.Close();
        }

        private void CreateTrainer(DataContractSerializer serializer, XmlWriterSettings settings)
        {
            var form = new Trainer_Registration();
            form.ShowDialog();

            Player = (form.player == null ? new Trainer("Ash", "Pueblo Paleta") : form.player);

            Player.ChosePokemon(Pokedex);

            XmlWriter trainerFile = XmlWriter.Create(trainerFilePath, settings);
            serializer.WriteObject(trainerFile, Player);
            trainerFile.Close();
        }

    }
}
