# Pin-Ski
## MadHacks November 11 2023 - UW Madison
[Pin-Ski](https://lebrra.itch.io/pin-ski) is a combo of pinball and ski ball in a quick and easy virtual arcade game. 

Playable in-browser or as a downloadable apk for Android devices (both available on [itch.io](https://lebrra.itch.io/pin-ski)).

Use the **Left** and **Right arrow keys** to flip the flippers and throw the ball into the holes for points. 

Each game is unique, procedurally generating a new board every game! 

## Breakdown
This game was created from scratch in 24 hours using Unity3D, C#, and Krita.
Each round is unique; there are always 5 tracks, but the holes and distribution of points is randomized for every game. A total of 5000 points are disbursed among the tracks - 2000 between one and two, and 3000 among three, four, and five, favoring five. Each track chooses between 2-5 holes, favoring 3, and disburses its given point value among the holes.
Every time the ball enters a track, a random horizontal force is added so the player does not immediately lose (it is still possible, but randomly decided).
High scores are saved locally using an Unity feature caled PlayerPrefs.

I created all of the art using Krita. All of the code is my own and completed during the hackathon.
I did integrate some packages to assist:
* TextMeshPro (Unity Package)
* [BeauRoutines](https://github.com/BeauPrime/BeauRoutine)

## Remaining ToDos
There were several stretch goals I didn't quite accomplish, here's a list of everything I wanted to get to:
- [ ] Adding SFX
- [ ] Cleaner backgrounds; maybe some texture to make it more interesting
- [ ] Adding saved names to highscores
  * I wanted to mimic a pinball machine and have the flipper controls the char input
- [ ] Implementing online highscores using Firebase
- [ ] Fixing flipper directional force on ball when hitting weird side angles
- [ ] Hosting WebGL build on my own domain instead of itch.io
- [x] Android builds functionable and available as an apk download
- [ ] Android builds published to the GooglePlay Store
