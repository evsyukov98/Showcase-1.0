using System.Collections.Generic;

namespace Services.SaveServices
{
    public interface ISaveProvider
    {
        Dictionary<int, IState> LoadSave();
        void SaveSaves(Dictionary<int, IState> saves);
        void RemoveSaves();
    }
}
