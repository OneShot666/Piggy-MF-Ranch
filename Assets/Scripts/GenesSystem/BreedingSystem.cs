using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Système complet de reproduction des cochons.
/// Gère la génétique, les couleurs spéciales, les stats et les raretés.
/// </summary>
public class BreedingSystem : MonoBehaviour
{
    [Header("Configuration des Probabilités")]
    [SerializeField, Range(0f, 1f)] private float mutationChance = 0.03f;
    
    [Header("Recettes de Couleurs Spéciales")]
    [Tooltip("Définit les croisements qui donnent des couleurs uniques (ex: Rose + Marron = Beige)")]
    [SerializeField] private List<SpecialColorRecipeData> specialRecipes = new List<SpecialColorRecipeData>();
    
    [Header("Affinités des couleurs avec des pouvoirs spéciaux")]
    [SerializeField] private List<ColorPowerAffinityData> colorPowerAffinities = new List<ColorPowerAffinityData>();
    
    #region Structures et Dictionnaires
    
    /// <summary>
    /// Poids de chaque rareté pour le calcul de l'apparition des cochons.
    /// </summary>
    private static readonly Dictionary<PigRarity, float> rarityWeights = new Dictionary<PigRarity, float>
    {
        { PigRarity.Common, 10f },
        { PigRarity.Uncommon, 8f },
        { PigRarity.Rare, 6f },
        { PigRarity.Legendary, 4f },
        { PigRarity.UltraRare, 2f }
    };
    
    /// <summary>
    /// Poids des pouvoirs spéciaux pour le tirage aléatoire.
    /// </summary>
    private readonly (PigSpecialPower power, float weight)[] specialPowerWeights = new[]
    {
        (PigSpecialPower.None, 10f),
        (PigSpecialPower.Sprint, 8f),
        (PigSpecialPower.FatigueResist, 5f),
        (PigSpecialPower.MatingChance, 3f),
        (PigSpecialPower.XPBoost, 4f),
        (PigSpecialPower.GeneralBoost, 2f)
    };

    /// <summary>
    /// Poids des pouvoirs uniques pour le tirage aléatoire.
    /// </summary>
    private readonly (PigUniquePower power, float weight)[] uniquePowerWeights = new[]
    {
        (PigUniquePower.None, 10f),
        (PigUniquePower.Sprint, 8f),
        (PigUniquePower.SlipperyMud, 6f),
        (PigUniquePower.Confusion, 7f)
    };
    #endregion
    
    #region Public Methods
    /// <summary>
    /// Tente de faire se reproduire deux cochons et retourne le descendant.
    /// </summary>
    /// <param name="parent1">Premier parent.</param>
    /// <param name="parent2">Second parent.</param>
    /// <returns>Un nouveau cochon descendant, ou null si la reproduction n'est pas possible.</returns>
    public Pig Breed(Pig parent1, Pig parent2) {
        if (!CanBreed(parent1, parent2)) return null;
        
        bool isMutation = parent1.Color == PigColor.Rainbow || 
                          parent2.Color == PigColor.Rainbow || 
                          Random.value < mutationChance;
        
        PigColor offspringColor = GetOffspringColor(parent1, parent2, isMutation);
        
        PigRarity offspringRarity = GetOffspringRarity(parent1, parent2);
        
        float speed = CalculateStat(parent1.Speed, parent2.Speed, offspringRarity, isMutation);
        float endurance = CalculateStat(parent1.Endurance, parent2.Endurance, offspringRarity, isMutation);
        
        PigSpecialPower specialPower = GetOffspringSpecialPower(parent1.SpecialPower, parent2.SpecialPower, offspringColor, isMutation);
        PigUniquePower uniquePower = GetOffspringUniquePower(parent1.UniquePower, parent2.UniquePower, isMutation);
        
        int generation = Mathf.Max(parent1.Generation, parent2.Generation) + 1;

        return new Pig(offspringColor, offspringRarity, speed, endurance, specialPower, uniquePower, generation);
    }

    /// <summary>
    /// Vérifie si deux cochons peuvent se reproduire.
    /// </summary>
    /// <param name="parent1">Premier parent.</param>
    /// <param name="parent2">Second parent.</param>
    /// <returns>True si la reproduction est possible.</returns>
    public bool CanBreed(Pig parent1, Pig parent2) {
        return parent1.WellBeing != null && parent2.WellBeing != null &&
               // parent1.WellBeing.Satiety > 50f && parent2.WellBeing.Satiety > 50f &&
               parent1.WellBeing.Happiness >= 50f && parent2.WellBeing.Happiness >= 50f;

    }
    #endregion
    
    #region Private Methods
    
    #region Tests
    [ContextMenu("Test Lignée Analytique")]
    public void TestLigneeAnalytique()
    {
        Debug.Log("<color=orange><b>=== DÉBUT DU TEST DE GÉNÉALOGIE ANALYTIQUE ===</b></color>");

        // Initialisation des parents de départ (Génération 0)
        Pig parentA = new Pig(PigColor.Pink, PigRarity.Common, 10f, 10f, PigSpecialPower.None, PigUniquePower.None, 0);
        Pig parentB = new Pig(PigColor.Brown, PigRarity.Common, 10f, 10f, PigSpecialPower.None, PigUniquePower.None, 0);
        
        // On force les conditions optimales pour voir le potentiel max
        parentA.WellBeing.Cleanliness = 100f; parentA.WellBeing.Happiness = 100f;
        parentB.WellBeing.Cleanliness = 100f; parentB.WellBeing.Happiness = 100f;

        for (int gen = 1; gen <= 100; gen++)
        {
            Debug.Log($"<color=white><b>--- GÉNÉRATION {gen} ---</b></color>");
            Debug.Log($"Parents: {parentA.Rarity} (Vit:{parentA.Speed:F1}) x {parentB.Rarity} (Vit:{parentB.Speed:F1})");

            // --- CALCUL DES PROBABILITÉS ---
            PigRarity[] rarities = (PigRarity[])System.Enum.GetValues(typeof(PigRarity));
            float[] weights = new float[rarities.Length];
            int maxIndex = Mathf.Min(Mathf.Max((int)parentA.Rarity, (int)parentB.Rarity) + 1, rarities.Length - 1);
            float cleanlinessFactor = (parentA.WellBeing.Cleanliness + parentB.WellBeing.Cleanliness) / 200f;

            float totalWeight = 0;
            string probReport = "Probabilités calculées : ";

            for (int i = 0; i <= maxIndex; i++) {
                float dist = Mathf.Abs(rarityWeights[rarities[i]] - (rarityWeights[parentA.Rarity] + rarityWeights[parentB.Rarity]) / 2f);
                weights[i] = 1f / (1f + dist);
                if (i > Mathf.Max((int)parentA.Rarity, (int)parentB.Rarity)) weights[i] *= cleanlinessFactor;
                totalWeight += weights[i];
            }

            for (int i = 0; i < rarities.Length; i++) {
                if (i <= maxIndex) {
                    float prc = (weights[i] / totalWeight) * 100f;
                    probReport += $"| {rarities[i]}: {prc:F0}% ";
                } else {
                    probReport += $"| <color=red>{rarities[i]}: BLOQUÉ</color> ";
                }
            }
            Debug.Log(probReport);

            // --- REPRODUCTION RÉELLE ---
            Pig bebe = Breed(parentA, parentB);

            // AFFICHAGE DU RÉSULTAT
            string mutationText = (bebe.Color != parentA.Color && bebe.Color != parentB.Color) ? " <color=magenta>[NOUVELLE COULEUR !]</color>" : "";
            Debug.Log($"<b>RÉSULTAT BÉBÉ :</b> {bebe.Color} | {bebe.Rarity} | Vit: {bebe.Speed:F2} | Endurance: {bebe.Endurance:F2} |  {bebe.SpecialPower} | {bebe.UniquePower} {mutationText}");

            // SÉLECTION POUR LA PROCHAINE GÉNÉRATION
            // On remplace toujours le parent le "moins bon" (Priorité Rareté puis Vitesse)
            if ((int)parentA.Rarity < (int)parentB.Rarity || (parentA.Rarity == parentB.Rarity && parentA.Speed < parentB.Speed))
                parentA = bebe;
            else
                parentB = bebe;
        }

        Debug.Log("<color=orange><b>=== FIN DU TEST DE GÉNÉALOGIE ===</b></color>");
    }
    
    [ContextMenu("Test Affinité Pouvoirs")]
    public void TestAffinitePouvoirs()
    {
        Debug.Log("<color=cyan><b>=== LABORATOIRE GÉNÉTIQUE : TEST D'AFFINITÉ ===</b></color>");
    
        // On choisit une couleur à tester (ex: Noir pour le Sprint)
        PigColor couleurTest = PigColor.Brown; 
        int nombreTests = 1000;
    
        // Dictionnaire pour compter les résultats
        Dictionary<PigSpecialPower, int> resultats = new Dictionary<PigSpecialPower, int>();
        foreach (PigSpecialPower p in System.Enum.GetValues(typeof(PigSpecialPower))) 
            resultats[p] = 0;

        // Simulation de parents "vides" (None) pour voir si l'affinité crée l'éveil
        Pig p1 = new Pig(PigColor.Pink, PigRarity.Common, 10f, 10f, PigSpecialPower.None, PigUniquePower.None, 0);
        Pig p2 = new Pig(PigColor.Pink, PigRarity.Common, 10f, 10f, PigSpecialPower.None, PigUniquePower.None, 0);
        p1.WellBeing.Cleanliness = 100f; p2.WellBeing.Cleanliness = 100f;

        for (int i = 0; i < nombreTests; i++)
        {
            // On force la mutation pour voir les tirages de poids
            PigSpecialPower powerObtenu = GetOffspringSpecialPower(p1.SpecialPower, p2.SpecialPower,couleurTest, true);
            resultats[powerObtenu]++;
        }

        // Affichage des statistiques
        Debug.Log($"<color=yellow>Résultats pour {nombreTests} naissances de couleur {couleurTest} (Mutation Forcée) :</color>");
        foreach (var entry in resultats)
        {
            float pourcentage = (entry.Value / (float)nombreTests) * 100f;
            string highlight = entry.Value > (nombreTests / 5) ? "<color=green><b>(BOOSTÉ)</b></color>" : "";
            Debug.Log($"- {entry.Key}: {entry.Value} ({pourcentage:F1}%) {highlight}");
        }
    }
    
    [ContextMenu("Test Génétique Détaillé")]
    public void TestGenetiqueDetaille()
    {
        Debug.Log("<color=cyan><b>=== TEST GÉNÉTIQUE DÉTAILLÉ (10 000 Naissances) ===</b></color>");
        
        // Parents de test : Deux Roses (Couleurs standards)
        Pig p1 = new Pig(PigColor.Pink, PigRarity.Common, 10, 10, PigSpecialPower.None, PigUniquePower.None, 0);
        Pig p2 = new Pig(PigColor.Pink, PigRarity.Common, 10, 10, PigSpecialPower.None, PigUniquePower.None, 0);

        // Dictionnaires pour compter
        Dictionary<PigColor, int> totalParCouleur = new Dictionary<PigColor, int>();
        Dictionary<PigColor, int> mutationsParCouleur = new Dictionary<PigColor, int>();
        
        foreach (PigColor c in System.Enum.GetValues(typeof(PigColor))) {
            totalParCouleur[c] = 0;
            mutationsParCouleur[c] = 0;
        }

        int totalTests = 10000;
        int totalMutationsActives = 0;

        for (int i = 0; i < totalTests; i++)
        {
            // 1. On calcule si une mutation DOIT arriver (3% de chance selon ton code)
            // Note : On utilise la même logique que dans ta méthode Breed()
            bool isMutation = Random.value < mutationChance; 
            if (isMutation) totalMutationsActives++;

            // 2. On obtient la couleur
            PigColor couleurObtenue = GetOffspringColor(p1, p2, isMutation);
            
            totalParCouleur[couleurObtenue]++;
            if (isMutation) mutationsParCouleur[couleurObtenue]++;
        }

        // --- AFFICHAGE DES RÉSULTATS ---
        Debug.Log($"<color=yellow>Rapport de Naissances (Mutation Chance: {mutationChance*100}%)</color>");
        Debug.Log($"Total de mutations déclenchées : {totalMutationsActives} / {totalTests}");

        foreach (PigColor c in System.Enum.GetValues(typeof(PigColor)))
        {
            if (totalParCouleur[c] == 0) continue; // On n'affiche pas les couleurs à 0%

            float prcTotal = (totalParCouleur[c] / (float)totalTests) * 100f;
            int nbMutations = mutationsParCouleur[c];
            float prcMutationDansCetteCouleur = totalParCouleur[c] > 0 ? (nbMutations / (float)totalParCouleur[c]) * 100f : 0;

            Debug.Log($"- <b>{c}:</b> {prcTotal:F1}% du total " +
                      $"<color=orange>[Mutés: {nbMutations} ({prcMutationDansCetteCouleur:F1}% de cette couleur)]</color>");
        }

        // VÉRIFICATION DES RÈGLES
        bool beigeFound = totalParCouleur[PigColor.Beige] > 0;
        bool goldenFound = totalParCouleur[PigColor.Golden] > 0;
        
        if (!beigeFound && !goldenFound)
            Debug.Log("<color=green><b>SUCCÈS :</b> Aucune couleur de recette (Beige/Golden) n'est apparue par erreur.</color>");
        else
            Debug.Log("<color=red><b>ERREUR :</b> Des couleurs de recettes sont apparues sans les bons parents !</color>");
    }
    #endregion Tests

    /// <summary>
    /// Calcule la couleur du descendant en tenant compte des mutations et combinaisons spéciales.
    /// </summary>
    private PigColor GetOffspringColor(Pig parent1, Pig parent2, bool isMutation)
    {
        PigColor[] allColors = (PigColor[])System.Enum.GetValues(typeof(PigColor));
        float[] weights = new float[allColors.Length];
        
        // --- RÉGLAGES DE PROBABILITÉS ---
        float standardWeight = 20f; // Pourcentage totale de couleurs standards
        float parentBonus = 40f;   // Influence des parents
        float recipeBonus = 50f;   // Priorité absolue aux recettes
        // --------------------------------
        
        for (int i = 0; i < allColors.Length; i++)
        {
            PigColor currentColor = allColors[i];
        
            // CHANCE DE BASE (Couleurs standards uniquement)
            if (IsStandardColor(currentColor)) {
                weights[i] = standardWeight; 
            } else {
                weights[i] = 0f; 
            }

            // BONUS PARENTS (S'applique même si c'est une couleur spéciale)
            if (currentColor == parent1.Color) weights[i] += parentBonus;
            if (currentColor == parent2.Color) weights[i] += parentBonus;

            // BONUS RECETTES
            foreach (var recipe in specialRecipes)
            {
                if (((parent1.Color == recipe.parentColorA && parent2.Color == recipe.parentColorB) ||
                     (parent1.Color == recipe.parentColorB && parent2.Color == recipe.parentColorA)) 
                    && currentColor == recipe.result)
                {
                    weights[i] += recipeBonus; 
                }
            }
        }

        // 4. MUTATION (Ouverture de toutes les couleurs)
        if (isMutation)
        {
            for (int i = 0; i < allColors.Length; i++)
            {
                PigColor currentColor = allColors[i];

                // On donne une chance supplémentaire aux couleurs de base (pour les "Champions")
                if (IsStandardColor(currentColor))
                {
                    weights[i] += 5f;
                }
                // On donne une chance au Rainbow (l'exception)
                else if (currentColor == PigColor.Rainbow)
                {
                    weights[i] += 5f;
                }
            }
        }

        return WeightedRandom(allColors, weights);
    }
    
    // Fonction helper pour définir ce qui est "commun"
    private bool IsStandardColor(PigColor color) {
        return color == PigColor.Pink || color == PigColor.Brown || 
               color == PigColor.Black || color == PigColor.White;
    }
    
    /// <summary>
    /// Calcule la rareté du descendant selon les parents et la propreté.
    /// Le descendant ne peut pas dépasser 1 rang au-dessus de la rareté la plus élevée des parents.
    /// </summary>
    private PigRarity GetOffspringRarity(Pig parent1, Pig parent2) {
        PigRarity[] rarities = (PigRarity[])System.Enum.GetValues(typeof(PigRarity));
        float[] weights = new float[rarities.Length];
    
        int bestParentRarity = Mathf.Max((int)parent1.Rarity, (int)parent2.Rarity);
        // Le bébé peut être au maximum +1 par rapport au meilleur parent
        int maxPossibleRarity = Mathf.Min(bestParentRarity + 1, rarities.Length - 1);
    
        float cleanlinessFactor = (parent1.WellBeing.Cleanliness + parent2.WellBeing.Cleanliness) / 200f;
    
        for (int i = 0; i < rarities.Length; i++) {
            if (i > maxPossibleRarity) {
                weights[i] = 0f; // INTERDIT de sauter les rangs
            }
            else if (i == maxPossibleRarity && i > bestParentRarity) {
                // Chance de monter de rang : très liée à la propreté (ex: 10% de base * cleanliness)
                weights[i] = 1f * cleanlinessFactor; 
            }
            else if (i == bestParentRarity) {
                weights[i] = 5f; // Très probable de rester au même niveau
            }
            else {
                weights[i] = 2f; // Possible de redescendre (régression)
            }
        }
    
        return WeightedRandom(rarities, weights);
    }
    
    /// <summary>
    /// Calcule la valeur d'une statistique du descendant selon la rareté.
    /// </summary>
    private float CalculateStat(float stat1, float stat2, PigRarity rarity, bool mutation) 
    {
        float average = (stat1 + stat2) / 2f;
    
        // DÉFINITION DE LA ZONE D'INFLUENCE
        // Le bébé peut varier naturellement de +/- 15% autour de la moyenne des parents
        // Cela permet à un bébé de parents lents de faire un "saut" de performance.
        float fluctuationRange = average * 0.15f; 
        float randomBase = Random.Range(average - (fluctuationRange * 0.5f), average + fluctuationRange);

        // PALIERS ET LIMITES (Inchangés pour garder l'équilibre)
        float baseMax = rarity switch {
            PigRarity.Common => 30f, PigRarity.Uncommon => 60f, PigRarity.Rare => 100f,
            PigRarity.Legendary => 180f, PigRarity.UltraRare => 300f, _ => 30f
        };
        float absoluteLimit = rarity switch {
            PigRarity.Common => 45f, PigRarity.Uncommon => 80f, PigRarity.Rare => 140f,
            PigRarity.Legendary => 240f, PigRarity.UltraRare => 400f, _ => baseMax * 1.5f
        };

        // CALCUL DU POTENTIEL
        // On ajoute un bonus fixe selon la rareté pour que le rang compte quand même
        float rarityBonus = (int)rarity * 1.0f; 
        float theoreticalStat = randomBase + rarityBonus;
        if (mutation) theoreticalStat += Random.Range(2f, 5f); // Mutation plus généreuse

        // APPLICATION DU FREINAGE (Soft Cap dynamique)
        if (theoreticalStat > baseMax)
        {
            float excess = theoreticalStat - baseMax;
            float accessibleRange = absoluteLimit - baseMax;
            // Formule de freinage qui s'adoucit si les parents sont déjà très forts
            float damping = 1f + (excess / accessibleRange);
            float finalBonus = excess / damping;
        
            return baseMax + finalBonus;
        }

        // Sécurité : Un UltraRare ne devrait jamais être en dessous d'un minimum décent
        float minimumSecurity = (int)rarity * 5f; 
        return Mathf.Max(theoreticalStat, minimumSecurity);
    }
    
    /// <summary>
    /// Détermine le pouvoir spécial du descendant selon les parents et la mutation.
    /// </summary>
    private PigSpecialPower GetOffspringSpecialPower(PigSpecialPower specialPower1, PigSpecialPower specialPower2, PigColor babyColor,  bool isMutation)
    {
        // Préparer les poids dynamiques (on copie les poids de base)
        var dynamicWeights = new (PigSpecialPower power, float weight)[specialPowerWeights.Length];
        for (int i = 0; i < specialPowerWeights.Length; i++)
        {
            dynamicWeights[i] = specialPowerWeights[i];
        
            // Appliquer le boost du ScriptableObject si la couleur correspond
            foreach (var affinity in colorPowerAffinities)
            {
                if (affinity.color == babyColor && affinity.favoredPower == dynamicWeights[i].power)
                {
                    dynamicWeights[i].weight *= affinity.powerBoostMultiplier;
                    break; 
                }
            }
        }

        // Logique de mutation : tirage complet avec les nouveaux poids
        if (isMutation) return GetRandomPower(dynamicWeights);

        // Logique d'hérédité : si les parents n'ont rien, on a une petite chance "d'éveil"
        if (specialPower1 == PigSpecialPower.None && specialPower2 == PigSpecialPower.None)
        {
            // 5% de chance de voir un pouvoir apparaître selon les affinités de couleur
            return Random.value < 0.05f ? GetRandomPower(dynamicWeights) : PigSpecialPower.None;
        }

        // Transmission classique si un parent a un pouvoir
        if (specialPower1 == specialPower2 && specialPower1 != PigSpecialPower.None) 
            return Random.value < 0.7f ? specialPower1 : GetRandomPower(dynamicWeights);

        return Random.value > 0.5f ? specialPower1 : specialPower2;
    }

    /// <summary>
    /// Détermine le pouvoir unique du descendant selon les parents et la mutation.
    /// </summary>
    private PigUniquePower GetOffspringUniquePower(PigUniquePower uniquePower1, PigUniquePower uniquePower2, bool isMutation)
    {
        if (isMutation) return GetRandomPower(uniquePowerWeights);
        if (uniquePower1 == uniquePower2 && uniquePower1 != PigUniquePower.None) return Random.value < 0.7f ? uniquePower1 : GetRandomPower(uniquePowerWeights);
        return Random.value > 0.5f ? uniquePower1 : uniquePower2;
    }
    
    #endregion
    
    #region Helpers
    
    /// <summary>
    /// Sélectionne un élément aléatoire parmi une liste avec des poids spécifiques.
    /// </summary>
    private T WeightedRandom<T>(T[] items, float[] weights)
    {
        float total = 0;
        foreach (float w in weights) total += w;
        float r = Random.Range(0, total);
        float current = 0;
        for (int i = 0; i < items.Length; i++) {
            current += weights[i];
            if (r <= current) return items[i];
        }
        return items[0];
    }
    
    /// <summary>
    /// Sélectionne un élément aléatoire parmi une liste pondérée.
    /// </summary>
    private T GetRandomPower<T>((T power, float weight)[] items)
    {
        float total = 0;
        foreach (var item in items) total += item.weight;
        float r = Random.Range(0, total);
        float current = 0;
        foreach (var item in items) {
            current += item.weight;
            if (r <= current) return item.power;
        }
        return items[0].power;
    }
    
    #endregion
}
