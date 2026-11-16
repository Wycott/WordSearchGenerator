# Word Search Generator

<a href="https://docs.microsoft.com/en-us/dotnet/csharp/"><img src="https://raw.githubusercontent.com/Wycott/RepositoryResources/main/Graphics/language-csharp.svg" title="Language C#" alt="Language C#"></a>
<a href="https://github.com/Wycott/RepositoryResources/blob/main/REPOTYPE.md"><img src="https://raw.githubusercontent.com/Wycott/RepositoryResources/main/Graphics/repo%20type-Game-yellow.svg" title="Game" alt="Game"></a>
<img src="https://img.shields.io/badge/.NET_Core-11-red">

A winforms app to automate generation of word search games

## Usage

Words are defined in `WordSearchGen.Core/WordLists.cs`; `WordLists.Current` picks the list the app uses. By convention there are 10 words per game but this isn't a hard and fast limit. Words must contain only letters and be no longer than the grid (12 squares).

Choose a skill level before generating:

| Level | Words run |
| --- | --- |
| Beginner | Across (left to right) or down |
| Intermediate | Across or down, forwards or backwards |
| Advanced | Any direction, including diagonals |

The app remembers the level you had selected when you closed it. It is saved in your own `user.config` (under `%LocalAppData%\WordSearchGen`); the starting default for new users is the `SkillLevel` setting in `WordSearchGen/App.config`.

When the application starts, pressing `GENERATE` will have a go at filling the grid with the words the player needs to find. If you don't find the pattern that interesting, you can press `GENERATE` as many times as you like.

When you're happy with the grid, you can press `FILL` to add letters to the remaining empty squares on the grid. Fill letters follow English letter frequencies (lots of E, T and A, very few Q, X and Z), so the hidden words blend in.

Stuck on a word? Click it in the list below the grid and it flashes red, both in the list and in the grid, for three seconds. This works both before and after `FILL`. How long it flashes for (`HighlightSeconds`) and how fast (`FlashMilliseconds`) are set in `WordSearchGen/App.config`; to change them without rebuilding, edit `WordSearchGen.dll.config` next to the exe.

You can then use the Snipping Tool or similar to grab the screen and print the puzzle out.

If the words can't all be fitted in, the app says so instead of retrying forever.

## Solution Structure

| Project | Purpose |
| --- | --- |
| `WordSearchGen` | The WinForms app. It only draws the grid and handles the buttons. |
| `WordSearchGen.Core` | The puzzle logic: grid, word placement, generator and word lists. It has no UI dependencies. |
| `WordSearchGen.Tests` | xUnit tests for `WordSearchGen.Core`. Run them with `dotnet test`. |

## Further Development

The initial application was thrown together in a weekend to entertain my children back in 2018, but there is considerable scope for development. There are a lot of [issues](https://github.com/Wycott/WordSearchGenerator/issues) ranging from minor to not-so-minor that developers or hobbyists can cut their teeth on.

Want to suggest a new feature, raise a bug or get involved? Click [here](https://github.com/Wycott/WordSearchGenerator/discussions).

---

*Created: 28 October 2022*  
*Updated: 31 October 2025*
