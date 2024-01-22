using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

namespace Projet_Console_Serialisation_Data
{
    [Serializable]
    public class Dossier
    {
        public string? Name { get; set; }
        public string? CreationDate { get; set; }
        public string? ModificationDate { get; set; }
        public List<Dossier> SubDossiers { get; set; }
        public List<Contacte> Contactes { get; set; }

        [XmlIgnore]
        public Dossier? Parent { get; set; }

        public Dossier()
        {
            SubDossiers = new List<Dossier>();
            Contactes = new List<Contacte>();
        }

        public Dossier(string nom, Dossier parent)
        {
            Name = nom;
            CreationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ModificationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            SubDossiers = new List<Dossier>();
            Contactes = new List<Contacte>();
            Parent = parent;
        }

        public void setCreationDate(string creation)
        {
            CreationDate = creation;
        }

        public void setModificationDate(string modif)
        {
            ModificationDate = modif;
        }

        public void afficher(int indentationLevel)
        {
            string indentation = new string(' ', indentationLevel * 3);
            Console.Write(indentation + "Dossier: " + Name + "\n");

            if (Contactes.Count != 0)
            {
                foreach (Contacte contact in Contactes)
                {
                    Console.Write(indentation + "|");
                    contact.toString(indentationLevel);
                }
            }
            if (SubDossiers.Count != 0)
            {
                foreach (Dossier dossier in SubDossiers)
                {
                    Console.Write(indentation + "|");
                    dossier.afficher(indentationLevel + 1);
                }
            }
        }

        public void afficheGlobal()
        {
            Dossier d = this;
            while (d.Parent != null)
            {
                d = d.Parent;
            }
            d.afficher(0);
        }

        public void addNewFolder(Dossier d)
        {
            if (d.Name == "..")
            {
                Console.WriteLine("ERREUR: nom de dossier interdit.\n");
                return;
            }

            bool error = false;
            foreach (Dossier d2 in this.SubDossiers)
            {
                if (d2.Name == d.Name)
                {
                    error = true;
                    Console.WriteLine("ERREUR: Il existe déjà un dossier avec le même nom.\n");
                    break;
                }
            }
            if (!error)
            {
                this.SubDossiers.Add(d);
            }
        }
        public void addNewContact(Contacte c)
        {
            bool error = false;
            foreach (Contacte c1 in this.Contactes)
            {
                if (c1.Courriel == c.Courriel)
                {
                    error = true;
                    Console.WriteLine("ERREUR: Il existe déjà un contacte avec la même adresse email.\n");
                    break;
                }
            }
            if (!error)
            {
                this.Contactes.Add(c);
            }
        }
    }
}
