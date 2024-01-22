using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Projet_Console_Serialisation_Data
{
    [Serializable]
    public class Contacte
    {
        public string? Nom { get; set; }
        public string? Prenom { get; set; }

        public string? Courriel { get; set; }
        public string? Society { get; set; }
        public string? Link { get; set; }
        public string? CreationDate { get; set; }
        public string? ModificationDate { get; set; }

        [XmlIgnore]
        public Dossier? Parent {  get; set; }

        public Contacte()
        {
        }

        public Contacte(string prenom, string nom, string societe, string courriel, string lien, Dossier parent)
        {
            Nom = nom;
            Prenom = prenom;
            Courriel = courriel;
            Society = societe;
            Link = lien;
            Parent = parent;
            CreationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ModificationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public void setCreationDate(string creation)
        {
            CreationDate = creation;
        }

        public void setModificationDate(string modif)
        {
            ModificationDate = modif;
        }

        public void toString(int indentationLevel)
        {
            string indentation = new string(' ', (indentationLevel+1) * 3);
            Console.Write(indentation + Nom + " " + Prenom + " " + Society + " " + Courriel + " Link:" + Link +"\n");
        }
    }
}
