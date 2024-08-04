# Match3
Simple and scalable vanilla Unity implementation of tile-matching game.

## Content
The repository contains abstractions for tile-matching game and implementation of Match-3 mechanics based on them.
In this project, you can see the following mechanics:
1. **Gameboard generation** - Creating the initial game board with tiles.
2. **Swapping and matching the tiles** - Allowing tiles to be swapped and matched according to Match-3 rules.
3. **Filling the board** - Automatically filling the board after matches are made.

**Note**: This project does not implement any score logic or advanced game features like power-ups or levels.

![](https://github.com/BarsikKato/Match3/blob/develop/example.gif)
## Scripts
The ***"Scripts"*** folder consists of 4 subfolder:
1. **Abstractions** - Contains basic abstractions of the game.
2. **Core** - Contains concrete implementation of the game.
3. **DependencyResolving** - A simple self-written dependency resolver.
4. **Extensions** - Extension methods for .NET and Unity objects.

## Known issues
1. Limited animations in this project.
2. Abstractions still depend on the vanilla Unity Engine, but they can be easily decoupled by allowing ***'IGameItem'*** to use generic types instead of ***'IEnumerator'***, and by using simple ***(x, y)*** coordinates instead of Vector2 in the ***'IGameBoard'*** interface.
