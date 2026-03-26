namespace Entities
{
    /// <summary>
    /// Caractéristiques physiques d'un vélo (taille, poids, couleur, etc.).
    /// Classe de base héritée par <see cref="Bicycle"/>.
    /// </summary>
    public class BicycleCharacteristics
    {
        /// <summary>Taille du cadre en centimètres.</summary>
        public float Size { get; set; }

        /// <summary>Poids du vélo en kilogrammes.</summary>
        public float Weight { get; set; }

        /// <summary>Couleur du vélo.</summary>
        public string? Color { get; set; }

        /// <summary>Diamètre des roues en pouces.</summary>
        public float WheelSize { get; set; }

        /// <summary>Indique si le vélo est électrique (VAE).</summary>
        public bool Electric { get; set; }

        /// <summary>État du vélo (neuf, occasion, etc.).</summary>
        public string? State { get; set; }

        /// <summary>Marque du fabricant.</summary>
        public string? Brand { get; set; }

        /// <summary>Niveau de confort (ex. : sportif, touring, etc.).</summary>
        public string? Confort { get; set; }
    }
}
