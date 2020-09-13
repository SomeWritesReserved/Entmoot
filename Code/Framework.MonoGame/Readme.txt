------------------------
 Framework (MonoGame)
------------------------

This is a "framework" for Entmoot, which is a collection of utilities and "game-level" concepts built on top of Entmoot.Engine.
The engine proper is platform and game agnostic, so while it defines the entity/component/system engine, it doesn't actually define
any entities, components, or systems. This particular framework is built on top of MonoGame (specifically MonoGame.Framework.DesktopGL)
in order to use its rendering and math types.