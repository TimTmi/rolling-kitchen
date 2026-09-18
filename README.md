## Setup

1. Clone repository.

2. Open project with **Unity 6.6**.

3. Import following third-party assets into `Assets/ThirdParty/`:

   * **CubexCube - Free City Pack I**
   * **Pandazole - Kitchen Food low poly pack**
   * **Free Pack - Stick Man**
   * **Toony Kitchen & Ingredients Model FREE**
   * **Low-Poly Park**

### Third-Party Assets

These assets are not included in repository. Obtain them from their original sources and import them through Unity before opening `TruckScene`.

| Asset                                  | Creator / Publisher | Source                                                                                                               |
| -------------------------------------- | ------------------- | -------------------------------------------------------------------------------------------------------------------- |
| CubexCube - Free City Pack I           | Cube x Cube           | [Unity Asset Store](https://assetstore.unity.com/packages/3d/environments/urban/cubexcube-free-city-pack-i-199815)   |
| Pandazole - Kitchen Food low poly pack | Pandazole           | [Unity Asset Store](https://assetstore.unity.com/packages/3d/props/food/pandazole-kitchen-food-low-poly-pack-204525) |
| Free Pack - Stick Man                  | PolyOne Studio                   | [Unity Asset Store](https://assetstore.unity.com/packages/3d/characters/free-pack-stick-man-389802)                  |
| Toony Kitchen & Ingredients Model FREE | Sigun Studio        | [Unity Asset Store](https://assetstore.unity.com/packages/3d/props/toony-kitchen-ingredients-model-free-301805)      |
| Low-Poly Park | Thunderent | [Unity Asset Store](https://assetstore.unity.com/packages/3d/environments/urban/low-poly-park-61922) |

### Ingredient Images

The UI ingredient icons in `Assets/Features/Ingredients/Art/` are generated from the Toony Kitchen ingredient prefabs and are not included in the repository. After importing the third-party assets (see Setup), regenerate them:

1. Open any scene in the Unity Editor.
2. Run **Tools → Generate Ingredient Images**.

This renders every prefab in `Assets/ThirdParty/Unity/Toony Kitchen Ingredients Free/Prefabs/Ingredients/` to a transparent 512×512 PNG (perspective camera, Unity-preview-style 30°/30° angle). Output goes to `Assets/Features/Ingredients/Art/`. Camera angle and resolution can be changed via the constants at the top of `Assets/Features/Ingredients/Editor/IngredientImageGenerator.cs`.

### Miscellaneous Images

The UI icons in `Assets/Features/Container/Art/` are generated from the Toony Kitchen miscellaneous prefabs (crates, boxes, cups, …) and are not included in the repository. After importing the third-party assets (see Setup), regenerate them:

1. Open any scene in the Unity Editor.
2. Run **Tools → Generate Miscellaneous Images**.

This renders every prefab in `Assets/ThirdParty/Unity/Toony Kitchen Ingredients Free/Prefabs/Miscellaneous/` to a transparent 512×512 PNG. Output goes to `Assets/Features/Container/Art/`. Camera angle and resolution can be changed via the constants at the top of `Assets/Features/Container/Editor/MiscellaneousImageGenerator.cs`.

### Pickable Data References

The pickable data assets in `Assets/Features/Pickables/Data/` (one folder per ingredient) reference prefabs from the third-party assets. Those prefabs are not included in the repository, so the references are broken after cloning. After importing the third-party assets (see Setup), open each data asset in `Assets/Features/Pickables/Data/*/` and re-assign its **Prefab** (and **Icon**, where broken) reference.
