# Paddle

## Collision Object Node

The Paddle uses `CharacterBody2D` as its root node.

`CharacterBody2D` is suitable because the Paddle is directly controlled by the player and its movement should be predictable. The Paddle needs to move horizontally while still taking part in collisions with the ball.

`StaticBody2D` would not be suitable because the Paddle needs to move.

`RigidBody2D` would make the Paddle controlled by the physics simulation, while the Paddle should instead be moved directly by the game logic.

`Area2D` is mainly intended for detecting overlaps rather than normal physical collision response.

## Possible Paddle Problems

### Screen boundaries

The Paddle should not be able to move outside the visible game area. Its position should be limited according to the screen boundaries and the width of the Paddle.

### Movement

Paddle movement should remain consistent regardless of the frame rate. The player should also be able to stop and change direction quickly.

### Collision shape

The collision shape should match the visual size of the Paddle. This becomes especially important if the Paddle changes size.

### Power-ups

Possible Paddle-related power-ups include:

- increasing the Paddle width;
- decreasing the Paddle width;
- increasing or decreasing movement speed;
- making the Paddle temporarily sticky so that the ball attaches to it.

Temporary power-ups need a way to restore the Paddle to its normal state after the effect ends.

Multiple active power-ups may also conflict with each other. For example, two effects could try to change the Paddle width at the same time.