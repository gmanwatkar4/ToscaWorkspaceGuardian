using ToscaWorkspaceGuardian.Core.Business;

namespace ToscaWorkspaceGuardian.Core.Compare;

public class WorkspaceSnapshotComparer
{
    public CompareResult Compare(
        WorkspaceSnapshot oldSnapshot,
        WorkspaceSnapshot newSnapshot)
    {
        var result = new CompareResult();

        var oldObjects = oldSnapshot.Objects
            .ToDictionary(x => x.NodePath);

        var newObjects = newSnapshot.Objects
            .ToDictionary(x => x.NodePath);

        //------------------------------------
        // Added
        //------------------------------------

        foreach (var node in newObjects.Keys)
        {
            if (!oldObjects.ContainsKey(node))
            {
                result.AddedObjects.Add(node);
            }
        }

        //------------------------------------
        // Removed
        //------------------------------------

        foreach (var node in oldObjects.Keys)
        {
            if (!newObjects.ContainsKey(node))
            {
                result.RemovedObjects.Add(node);
            }
        }

        //------------------------------------
        // Property Changes
        //------------------------------------

        foreach (var node in oldObjects.Keys)
        {
            if (!newObjects.ContainsKey(node))
                continue;

            var oldObj = oldObjects[node];
            var newObj = newObjects[node];

            foreach (var property in oldObj.Properties)
            {
                if (!newObj.Properties.TryGetValue(
                    property.Key,
                    out var newValue))
                    continue;

                if (property.Value == newValue)
                    continue;

                result.PropertyChanges.Add(
                    new PropertyChange
                    {
                        NodePath = node,
                        Property = property.Key,
                        OldValue = property.Value,
                        NewValue = newValue
                    });
            }
        }

        return result;
    }
}