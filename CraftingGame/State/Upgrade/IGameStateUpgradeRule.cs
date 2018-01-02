namespace CraftingGame.State.Upgrade
{
    internal interface IGameStateUpgradeRule
    {
        int FromVersion { get; }
        int ToVersion { get; }

        void Upgrade(GameState gameState);
    }
}