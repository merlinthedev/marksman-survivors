using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStackableLivingEntity {
    bool IsFragile { get; }

    List<Stack> Stacks { get; }

    void AddStacks(int stacks, Stack.StackType stackType);
    void RemoveStacks(int stacks, Stack.StackType stackType);
    void RemoveStack(Stack stack);
    void CheckStacksForExpiration();
}