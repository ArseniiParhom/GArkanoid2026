# Scene Transition System Design

## 1. Overview

The purpose of this system is to add animated transitions between game states, such as fading the screen to black before loading a new scene and fading back in afterward.

The system will work together with the existing Finite State Machine (FSM) managed by `GameManager`.

## 2. Architecture

The transition system will consist of three main components:

- **GameManager:** Manages game states (MainMenu, Level, GameOver, Options) and requests transitions.
- **SceneTransitionController:** Controls transition animations and coordinates scene changes.
- **FadeOverlay:** A full-screen CanvasLayer containing a ColorRect and an AnimationPlayer for fade-in and fade-out animations.

The SceneTransitionController could be implemented as a child of the GameManager Autoload, allowing it to remain available between scene changes.

## 3. Transition Flow

For a normal scene transition, such as MainMenu → Level:

1. GameManager requests a transition to the Level state.
2. SceneTransitionController starts a fade-out animation.
3. The screen gradually becomes black.
4. Once the animation finishes, GameManager loads the Level scene and updates the current state.
5. SceneTransitionController starts the fade-in animation.
6. The black overlay gradually disappears, revealing the new level.

The system should ignore additional transition requests while an animation is already running.

## 4. Integration with the FSM

The FSM remains responsible for determining which state should be active. The transition system only controls how the visual change happens.

For example, when the player loses the last life, GameManager requests the GameOver state. Instead of immediately changing the scene, the transition controller first plays the fade-out animation.

The Options state requires different handling because it is loaded additively. Opening Options should keep the previous scene loaded underneath it. A short fade or transparency animation could be used when opening and closing the Options overlay.

Closing Options restores the previous state without reloading the level.

## 5. Error Handling

If a scene cannot be loaded, the system should report an error and avoid leaving the screen permanently black.

The transition controller should also prevent multiple animations from running simultaneously.

## 6. Implementation Status

This document describes a proposed design only. Animated transitions are not implemented as part of Exercise 6.

The existing FSM and additive Options loading are already implemented.
