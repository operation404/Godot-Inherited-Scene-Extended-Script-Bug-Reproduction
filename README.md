Godot Engine v4.2.1.stable.mono.official.b09f793f5

## Problem Description
A base scene (called `BaseEnemy` in this example) is created with a Node2D root and a child Node which has an exported field that is set to the scene root node through the Inspector. The base scene then has a c# script attached. An inherited scene (called `GreenEnemy`) is then created from the base scene and the base scene's script is extended for this inherited scene. The child Node of the extended scene does not have any changes made to its properties through the script or Inspector, its field still points to the scene root.

When the inherited scene is instantiated, any attempts by the child Node to use the reference to the parent root Node fail as it is not correctly set to the inherited scene root (`GreenEnemy`) but instead to a non-existent or immediately disposed base scene (`BaseEnemy`). As a result, the following errors are thrown:
```
E 0:00:01:0083   GodotObject.base.cs:73 @ nint Godot.GodotObject.GetPtr(Godot.GodotObject): System.ObjectDisposedException: Cannot access a disposed object.
Object name: 'BaseEnemy'.
  <C# Error>     System.ObjectDisposedException
  <C# Source>    /root/godot/modules/mono/glue/GodotSharp/GodotSharp/Core/GodotObject.base.cs:73 @ nint Godot.GodotObject.GetPtr(Godot.GodotObject)
  <Stack Trace>  GodotObject.base.cs:73 @ nint Godot.GodotObject.GetPtr(Godot.GodotObject)
                 Node2D.cs:335 @ void Godot.Node2D.Translate(Godot.Vector2)
                 MoveComponent.cs:13 @ void MoveComponent._Process(double)
                 Node.cs:2111 @ bool Godot.Node.InvokeGodotClassMethod(Godot.NativeInterop.godot_string_name&, Godot.NativeInterop.NativeVariantPtrArgs, Godot.NativeInterop.godot_variant&)
                 MoveComponent_ScriptMethods.generated.cs:38 @ bool MoveComponent.InvokeGodotClassMethod(Godot.NativeInterop.godot_string_name&, Godot.NativeInterop.NativeVariantPtrArgs, Godot.NativeInterop.godot_variant&)
                 CSharpInstanceBridge.cs:24 @ Godot.NativeInterop.godot_bool Godot.Bridge.CSharpInstanceBridge.Call(nint, Godot.NativeInterop.godot_string_name*, Godot.NativeInterop.godot_variant**, int, Godot.NativeInterop.godot_variant_call_error*, Godot.NativeInterop.godot_variant*)
```
## Reproduction
I have created a new Godot project and within it created 4 scenes. 

A `BaseEnemy` scene composed of a Node2D root, a sprite just to have a visual reference, and a MoveComponent. MoveComponent is a c# extending Node which exports two fields, a reference to a Node2D and a Vector2. MoveComponent has only one purpose: in its _Process method, it calls Translate on the Node2D reference it possesses. The `BaseEnemy` also has an attached script which simply prints to the output console upon completion of the _Ready call.

A `RedEnemy` which is an inherited scene of `BaseEnemy` that tints the sprite red and changes the MoveComponent's Vector2 field to cause the instance to slowly move across the screen.

A `GreenEnemy` which is an inherited scene of `BaseEnemy` that tints the sprite green and extends the base scene's script. The extended script calls the base script's _Ready method and then prints to the output console on completion of its own _Ready method.

A `World` scene in which an instance of both `RedEnemy` and `GreenEnemy` are placed and the scene is then run.

So long as the `BaseEnemy`'s MoveComponent has its Node2D reference set to the scene root through the Inspector and neither the `BaseEnemy` or `GreenEnemy` scripts set the MoveComponent's reference programmatically, the `GreenEnemy` instance's MoveComponent's reference is broken and refers to a non-existent object.

## Observations and Workarounds
If the base scene `BaseEnemy`'s MoveComponent does _not_ have its Node2D reference set in the Inspector, and the inherited scene `GreenEnemy` then has its MoveComponent set to the root node of its respective scene, the `GreenEnemy` inherited scene can be instantiated without any problems. Its child MoveComponent node has a correct reference to the root `GreenEnemy` Node2D and the instance moves across the scene if the MoveComponent has a non-zero vector set.

However this workaround is not ideal, as there are cases where the base scene may be intended to be itself instantiated and not just extended from. In this example `BaseEnemy` can be instantiated, but if its MoveComponent does not have its reference set to the root Node2D, errors are thrown.

The other workaround I have found is to programmatically set the MoveComponent's Node2D reference in _Ready through the attached scene script. This works if done in either the extended scene script or the base scene script so long as the extended script calls its parent _Ready method. Since this overwrites whatever reference may be set by the Inspector, its best to leave the field unset in the Inspector.
