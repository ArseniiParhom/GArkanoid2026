using Godot;

namespace Arkanoid.Persistence;

public interface ISaveable
{
    Godot.Collections.Dictionary<string, Variant> SaveData();

    void LoadData(
        Godot.Collections.Dictionary<string, Variant> data
    );
}
