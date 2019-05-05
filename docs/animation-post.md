# Animation Irritation

Goal: Add 1 new character to our Isometric Game in Unity
Steps:

1. Create sprite strip (make sure all cells are the same width and height)
2. Import `Texture2D` into Unity
3. Set `Sprite Mode` to Multiple
4. Click `Sprite Editor` button (don't forget to save the changes to Sprite Mode)
5. Click the `Slice` option in the top menu
6. In the `Type` dropdown, select `Grid by Cell Size`
7. Enter `16` for width (Pixel size X) and `32` for height (Pixel size Y)
8. Click the `Slice` button
9. Repeat steps 1-8 for each direction your character is facing for both `Walk` animations and `Stand` animations (total of 8 sprite strips)
10. Take a deep breath, because you are not even half way done
11. Right click your Animation folder in your explorer and create a new `Animator Controller`
12. Click on your `AnimatorController` and open up the `Animator` tab in the Unity Editor
13. Right click in your Animation folder again and create a new `Animation`
14. Make sure to name your `Animation` object based on the sprite sheet it is going to be associated with (DownLeftWalk, DownLeftStand, UpRightWalk, UpRightStand, etc...)
15. Click and drag the newly created `Animation` into the `Animator` tab window that you have open
16. Repeat step 13-15 for each of your sprite sheets
17. Now create an empty `GameObject` in your scene (or you can use a prefab)
18. Add the `SpriteRenderer` and `Animator` components
19. Drag the `AnimatorController` from step 11 into the `Animator.Controller` slot
20. Open up the `Animation` tab (different from `Animator` tab) while you have the `GameObject` selected (that part is important)
21. Now you *should* have a dropdown in the `Animation` tab that has your list of `Animation`'s, if not, go back to step 20 and try again
22. Select one of the `Animations` from the drop down
23. Navigate to the folder you have your actual sprite sheet asset in
24. Click the little arrow to open up your sprite sheet and show all of the individual sprite frames in the explorer
25. Shift-Click to select all of the frames out of the sprite sheet
26. Drag the frames from the explorer and drop them into the `Animation` tab
27. You should see the frames listed one-by-one in the `Animation` tab
28. Click the play button in the `Animation` tab to view the animation (don't forget to navigate back to the `Scene` view) and you can now adjust the speed of the animation by changing the `Samples` number in the `Animation` tab
29. Repeat steps 22-28 for all of your `Animations` for this character
30. (Optional) Our animations looped, so you need to go into each `Animation` object and select `Loop Time` and `Loop Pose`

No biggy.  Now just do this entire process 13 times (1 for the player character, and 12 more times for each NPC).

In total, I spent just shy of 3 hours working on this task for our jam (not including the time to actually make the sprite sheet, that part was done by our artist, or the part where I had to actually LEARN how to make all this stuff work in the first place).  This list, I feel, really highlights the downside to using Unity for our animation framework.  There is just such an incredible amount of overhead to get just a single animation into the game.  And honestly, since we only do this process once every few months, one of us has to relearn it all from scratch each time.  We've probably all, during one jam or another, sat there reading through Unity docs and watching youtube tutorials trying to figure out how to configure everything just right so that Unity will allow us to play our 2D animations.

I'm sure there is a really great use-case for such a complex animation system somewhere in 3D land, but for all of us who are using Unity in just 2 dimensions, we could really use a better process for this. So, after this jam was over, I decided to investigate how it could be done.

AssetPostprocessor to the rescue!

In Unity, you can set up an Asset processing pipeline where scripts are run when specific types of Assets are imported into the project. After much fiddling, I've found a half-way-decent approach to remove MOST of the busy work.

Check out my gist to get the full file. https://gist.github.com/Kenoshen/494a83028e8c6509087172c21fefcef4

Instead of doing all that manually, this script will run every time you import a new sprite sheet into the project.  It slices the texture (given some texture naming conventions), creates the AnimatorController and each Animation, adds the frames of the sprite sheet into the Animation, and adds the Animation to the AnimatorController.  All together reducing your work down to dragging the appropriate AnimatorController into the correct GameObject Animator component.

If you were like our team and were constantly battling with Unity's Animation system, I hope this script helps you in your future work.

Good luck and happy Jamming!

PS. Check out our game: "2nd Bank"



