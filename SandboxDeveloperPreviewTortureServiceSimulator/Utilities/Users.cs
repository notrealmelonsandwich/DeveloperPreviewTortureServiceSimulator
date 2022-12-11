using System.Collections.Generic;

namespace SandboxDeveloperPreviewTortureServiceSimulator.Utilities
{
    public static class Users
    {
        private const string UserPhotoPath = "Assets/Media/UserPhotos/";

        /// <summary>
        /// A list of users with profile pictures.
        /// </summary>
        public static readonly Dictionary<string, string> Predefined = new Dictionary<string, string>
        {
           { "garry", UserPhotoPath + "garry.png" },
           { "Rabscuttle", UserPhotoPath + "gaben.png" },
           { "melonsandwich", UserPhotoPath + "melonsandwich.png" },
           { "Patrick Bateman", UserPhotoPath + "patrick.png" },
        };

        /// <summary>
        /// A list of nicknames which are randomly generated along with user photos
        /// </summary>
        public static readonly List<string> Nicknames = new List<string>
        {
            "CoolGuy127",
            "PussyDestroyer1337",
            "PenisMusician666",
            "John Doe",
            "Jane Doe",
            "notch",
            "jeb_",
            "Dinnerbone",
            "xXX_D0N4LD_TRUMP_XXx",
            "Torture Service Victim",
            "grayball",
            "NikeTheHuman",
            "GordonPeeman",
            "i love hot bebra",
            "venomquietcqc",
            "notrealmelonsandwich",
            "there's no way you win the raffle",
            "dogfood",
            "torture me",
            "​xXXi_wud_nvrstøp_ÜXXx",
            "10000 gecs out 17th march",
            "mememe",
            "djmelonsandwich",
            "dylanbrady",
            "laurales",
            "your odds are below 0.01%",
            "no way you win it",
            "deal with the fact you don't win it",
            "stop trying there's no way you win it",
            "garryoldman",
            "Terry",
            "STEAM_0:1:82476071",
            "I Love Torturing Myself",
            "Ryan Gosling",
            "Bebra",
            "grayball is real",
            "SCP-173",
            "O5 Advisor",
            "Walter White",
            "Jesse Pinkman",
            "Benedict Cumberbatch",
            "Horseshit lover",
            "Tim Cock",
            "Battlefield Overwatch",
            "#SBOX_FOR_COMRADES",
            "I jerk off to Alyx",
            "Anti-sex Supporter",
            "Jerma985",
            "Ken Rosenberg"
        };

        public static readonly List<string> Photos = new List<string>
        {
            UserPhotoPath + "garry.png",
            UserPhotoPath + "gaben.png",
            UserPhotoPath + "melonsandwich.png",
            UserPhotoPath + "patrick.png",
            UserPhotoPath + "markiplier.jpg",
            UserPhotoPath + "walter.jpg",
            UserPhotoPath + "crystalcastles.jpg",
            UserPhotoPath + "alice.jpg",
            UserPhotoPath + "yourdeath.jpg",
            UserPhotoPath + "dylan.jpg",
            UserPhotoPath + "laura.jpg",
            UserPhotoPath + "sbox.jpg",
        };
    }
}
