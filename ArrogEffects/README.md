# Arrog's Post-Processing Effects Showcase [CODE NOT PROVIDED]

## Gameplay Without Effect
![ArrogNoEffect](/Images/ArrogNoEffect.gif)

## Gameplay With Effect
![ArrogEffect](/Images/ArrogEffect.gif)

## HOW DOES IT WORK?
1. This effect is achieved by combining the stock Unity Post-Processing effects and a custom post processing effect called Black and White flipbook.

2. This custom post processing effect analyzes the completed rendered image and applies a different flipbook texture to both the white and black parts of the screen. The animated paper effect is achieved by overlaying each frame of the animated texture into the rendered image and changing the frame being displayed via a flipbook animation logic inside the post processing shader. 

3. If there are objects that the art director doesn't want to get affected by the post processing effect, they should be marked as such by adding them to a temporal render texture that will get used later.

4. After applying the flipbook texture, the custom effect replaces the pixels corresponding to the objects on screen that should not get affected by the effect with the original pixels grabbed by the original render texture.

5. This produces an effect where only certain parts of the screen have such texture without having to animate the texture manually in each object or hand painting the texture frame by frame.

6. Stock Unity effects like Chromatic Aberration and Bloom get applied on top of the resulting image of the Black and White flipbook post processing effect.
