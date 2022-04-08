# Arrog's Post-Processing Effects Showcase [CODE NOT PROVIDED]

## Gameplay Without Effect
![ArrogNoEffect](/Images/ArrogNoEffect.gif)

## Gameplay With Effect
![ArrogEffect](/Images/ArrogEffect.gif)

## HOW DOES IT WORK?
1. **The swap texture** : You will need to create a palette swap texture . The measures of the texture havve to be 2 pixels height by n pixels lenght (n being the number of colors you want to swap) . The botom row must contain the original colors , and the top row must contain the new colors that will replace the original colors positioned directly underneath them.

2. **Setting up the swap texture in Unity** : The texture should be importe with the following settings :
    - Read/Write Enabled : True.
    - Filter Mode : Point (No Filter).

3. **Generating the Palette Swap Asset** : To generate the palette swap asset , right click the swap texture and select the option Create/Color Palette. This assets is fully editable once generated , so you can make as many tweaks as you want without having to generate another Palette Swap Texture.

4. **Applying the Color Palette to the GameObject** : The GameObject should have the following components in order for the shader to work.
    - Sprite Renderer : The sprite renderer should use the following material >> **PaletteSwapMaterial**
    - Palette Swapper : This is the component that sends the palette swap data to the shader in order to replace the colors on the gameObject.
  In order to assign a palette swap , go to the Palette Swapper component in the inspector of the gameObject you want to swap colors and in the *Current Palette* property select the palette swap asset you want to be applied on the gameobject.
  To have a preview of the palette swap in Editor Mode , select the *Apply Palette* button.

5. **Changing the Swap Tolerance Threshold** : The swap threshold by default will have a value of 0 . This means that  , by defalut , no colors will be swaped . In order to let the shader know that you want to swap the colors , the value of the threshold has to be different than 0 . You can change this value using a slider inside the material proerties in the inspector section . The greater the threshold value , the easier similar colors to the ones indicated in the original color row of the palette swap texture will we swapped.
