// See https://aka.ms/new-console-template for more information
using Projet_Console_Serialisation_Data;
using System.IO;

static void Serialiser(Dossier root, SerializationType type, string path)
{
    ISerializer serializer = SerializerFactory.CreateSerializer(type);
    serializer.Serialize(root, path); 
}

static Dossier Deserialiser(SerializationType type, string path)
{
    ISerializer serializer = SerializerFactory.CreateSerializer(type);
    return (Dossier)serializer.Deserialize(path); 
}

void updateParent(Dossier parent)
{
    foreach (Contacte contacte in parent.Contactes)
    {
        contacte.Parent = parent;
    }

    foreach (Dossier enfant in parent.SubDossiers)
    {
        enfant.Parent = parent;
        updateParent(enfant);
    }
}

void afficheCommande()
{
    Console.WriteLine("Liste des commandes:\n"
    + " help : affiche la liste des commandes\n"
    + " quit : ferme le programme\n"
    + " addFolder <<nom dossier>>: ajoute un dossier dans le répertoire courant\n"
    + " addContact <<prenom nom societe courriel lien>> : ajoute un contacte dans le répertoire courant\n"
    + " cd <<nom dossier>> : se déplacer dans un dossier enfant\n"
    + " cd .. : se déplacer dans le dossier parent\n"
    + " tree : affiche l'ensemble des dossiers à partir du répertoire courant\n"
    + " treeAll : affiche l'ensemble des dossiers\n"
    + " enregistrer : enregistre toutes les information dans un fichier\n"
    + " lire : Récupère toutes les informations dans un fichier\n"
    + " serialiser : Serialise les données dans un fichier XML\n"
    + " deserialiser : Deserialise les données depuis un fichier XML\n"
    + " sercryp : serialiser les données avec cryptage\n"
    + " desercryp : deserialiser les données avec cryptage\n\n");
}

static Dossier LireDossier(StreamReader reader)
{
    Dossier root = new("Root", null);
    Dossier cour = root;
    int index = -1;
    int[] pile = new int[50];
    string ligne;
    bool dos = false;

    while (!reader.EndOfStream)
    {
        ligne = reader.ReadLine();

        if (int.TryParse(ligne, out int nombre))
        {
            Console.WriteLine($"La ligne est un nombre : {nombre}");
            dos = true;
            index++;
            pile[index] = nombre;
            if (nombre == 0)
            {
                while (pile[index] == 0 && index > 0)
                {
                    index--;
                    cour = cour.Parent;
                }
            }
        }
        else
        {
            if (!dos)
            {
                Console.WriteLine($"La ligne est une chaîne de caractères : {ligne}");
                string[] elements = ligne.Split(',');
                Contacte contact = new Contacte(elements[0], elements[1], elements[2], elements[3], elements[4], cour);
                contact.setCreationDate(elements[5]);
                contact.setModificationDate(elements[6]);
                cour.Contactes.Add(contact);
            }
            else
            {
                Console.WriteLine($"La ligne est une chaîne de caractères : {ligne}");
                string[] elements = ligne.Split(',');
                Dossier doss = new Dossier(elements[0], cour);
                doss.setCreationDate(elements[1]);
                doss.setModificationDate(elements[2]);
                cour.SubDossiers.Add(doss);
                dos = false;
                pile[index] -= 1;
                cour = cour.SubDossiers[cour.SubDossiers.Count() - 1];
            }
        }
    }
    return root;
}

static void SauvegarderDansFichier(Dossier root, StreamWriter writer)
{
    foreach (Contacte contact in root.Contactes)
    {
        writer.WriteLine($"{contact.Prenom},{contact.Nom},{contact.Society},{contact.Courriel},{contact.Link},{contact.CreationDate},{contact.ModificationDate}");
    }

    writer.WriteLine($"{root.SubDossiers.Count}");
    foreach (Dossier doss in root.SubDossiers)
    {
        writer.WriteLine($"{doss.Name},{doss.CreationDate},{doss.ModificationDate}");
        SauvegarderDansFichier(doss, writer);
    }
}

Dossier root = new Dossier("Root", null);
Dossier root2 = root;
Console.WriteLine("Création du dossier: " + root.Name + " Création: " + root.CreationDate + ", Modification:" + root.ModificationDate);
bool Continu = true;
string[] Commande;
bool success = false;
afficheCommande();
do
{
    Console.Write(">");
    Commande = Console.ReadLine()?.ToLower().Split(' ');
    if (Commande != null && Commande.Length == 6 && (Commande[0] == "addContact" || Commande[0] == "addcontact"))
    {
        Contacte c = new Contacte(Commande[1], Commande[2], Commande[3], Commande[4], Commande[5], root);
        root.addNewContact(c);
    }
    else if (Commande != null && Commande.Length == 2 && (Commande[0] == "addFolder" || Commande[0] == "addfolder"))
    {
        Dossier d = new Dossier(Commande[1], root);
        root.addNewFolder(d);
    }
    else if (Commande != null && Commande.Length == 2 && Commande[0] == "cd" && Commande[1] != "..")
    {
        foreach (Dossier d in root.SubDossiers)
        {
            if (d.Name == Commande[1])
            {
                success = true;
                root = d;
                break;
            }
        }
        if (!success)
        {
            Console.WriteLine("Erreur, il n'existe pas de dossier à ce nom.\n");
        }
        success = false;
    }
    else if (Commande != null && Commande.Length == 1 && Commande[0] == "enregistrer")
    {
        string cheminFichier = "structure.txt";

        if (File.Exists(cheminFichier))
        {
            File.Delete(cheminFichier);
        }

        File.Create(cheminFichier).Close();

        using (StreamWriter writer = new StreamWriter(cheminFichier, false))
        {
            SauvegarderDansFichier(root2, writer);

            Console.WriteLine($"La structure a été enregistrée dans {cheminFichier}.");
            writer.Close();
        }
    }
    else if (Commande != null && Commande.Length == 1 && Commande[0] == "lire")
    {
        string cheminFichier = "structure.txt";
        using (StreamReader reader = new StreamReader(cheminFichier))
        {
            root = LireDossier(reader);
            root2 = root;
            reader.Close();
        }
    }
    else if (Commande != null && Commande.Length == 1 && Commande[0] == "serialiser")
    {
        Serialiser(root2, SerializationType.Xml, "structure.xml");
        Console.WriteLine("La structure a été sérialisée avec succès.");
    }
    else if (Commande != null && Commande.Length == 1 && Commande[0] == "deserialiser")
    {
        root = Deserialiser(SerializationType.Xml, "structure.xml");
        root2 = root;
        updateParent(root);
        Console.WriteLine("La structure a été désérialisée avec succès.");
    }
    else if (Commande != null && Commande.Length == 1 && Commande[0] == "sercryp")
    {
        Console.Write("Entrez la clé de chiffrement : ");
        string encryptionKey = Console.ReadLine();
        Serialiser(root2, SerializationType.Xml, "temp.xml");

        try
        {
            CryptoManager.EncryptFile("temp.xml", "donnee_cryp.xml", encryptionKey);
            Console.WriteLine("La structure a été sérialisée et chiffrée avec succès.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erreur lors du chiffrement : {e.Message}");
        }
        finally
        {
            if (File.Exists("temp.xml"))
            {
                File.Delete("temp.xml");
            }
        }
    }
    else if (Commande != null && Commande.Length == 1 && Commande[0] == "desercryp")
    {
        Console.Write("Entrez la clé de déchiffrement : ");
        string decryptionKey = Console.ReadLine();
        try
        {
            if (!File.Exists("temp.xml"))
            {
                using (File.Create("temp.xml")) { }
            }
            else
            {
                File.WriteAllText("temp.xml", string.Empty);
            }
            CryptoManager.DecryptFile("donnee_cryp.xml", "temp.xml", decryptionKey);

            root = Deserialiser(SerializationType.Xml, "temp.xml");
            root2 = root;
            updateParent(root);
            Console.WriteLine("La structure a été désérialisée avec succès.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erreur lors de la désérialisation : {e.Message}");
        }
        finally
        {
            if (File.Exists("temp.xml"))
            {
                File.Delete("temp.xml");
            }
        }
    }
    else
    {
        switch (Commande[0].ToLower())
        {
            case "help":
                afficheCommande();
                break;
            case "quit":
                Continu = false;
                break;
            case "cd":
                if (root.Parent != null)
                    root = root.Parent;
                break;
            case "tree":
                root.afficher(0);
                break;
            case "treeall":
                root.afficheGlobal();
                break;
            default:
                Console.WriteLine("Commande non reconnue. Tapez 'help' pour voir la liste des commandes.\n");
                break;
        }
    }

} while (Continu);
