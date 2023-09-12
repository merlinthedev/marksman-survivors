using System.Collections.Generic;

namespace BuffsDebuffs.Stacks {
    public interface IStackableLivingEntity : IEntity {
        bool IsFragile { get; }

        List<Stack> Stacks { get; }

        void AddStacks(int stacks, Stack.StackType stackType);
        void RemoveStacks(int stacks, Stack.StackType stackType);
        void RemoveStack(Stack stack);
        void CheckStacksForExpiration();
    }
}