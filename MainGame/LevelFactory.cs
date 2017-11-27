using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Contracts;
using Prerelease.Main.Physics;
using VectorMath;
using Object = Prerelease.Main.Physics.Object;

namespace Prerelease.Main
{
    public class LevelFactory
    {
        private readonly ActionQueue actionQueue;

        public LevelFactory(ActionQueue actionQueue)
        {
            this.actionQueue = actionQueue;
        }

        public LevelState Load(string levelName)
        {
            var levelEnemies = new List<EnemyObject>();
            var levelCrates = new List<MovableObject>();
            var levelStaticObjects = new List<StaticObject>();
            var levelCollectableObjects = new List<StaticObject>();
            var levelDoors = new List<Door>();
            var levelText = ReadLevelBlocks(levelName);

            var lines = levelText.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            var blocks = new BlockGrid(30, 30, (uint)lines.Length, (uint)lines[0].Length);

            Vector2 spawnPoint = new Vector2();

            for (uint row = 0; row < lines.Length; row++)
            {
                if(lines[row].Length > blocks.Columns)
                    throw new InvalidDataException($"Row number {row} has an invalid column count {lines[row].Length}, it must not exceed {blocks.Columns}.");

                for (uint col = 0; col < lines[row].Length; col++)
                {
                    var cellValue = lines[row][(int) col];
                    switch (cellValue)
                    {
                        case 's':
                        case 'S':
                            spawnPoint = new Vector2(30*col, 30*row);
                            break;
                        case 'x':
                        case 'X':
                            blocks.Insert(row, col);
                            break;
                        case 'h':
                        case 'H':
                            var hiddenObject = CreateHiddenObject(new Vector2(30 * col, 30 * row), new Vector2(30, 30));
                            levelStaticObjects.Add(hiddenObject);
                            break;
                        case 'f':
                        case 'F':
                            var collectableObject = CreateFlowerObject(new Vector2(30 * col, 30 * row), new Vector2(30, 30));
                            levelCollectableObjects.Add(collectableObject);
                            break;
                        case 'c':
                        case 'C':
                            var crate = CreateCrate(new Vector2(30*col, 30*row), new Vector2(30, 30));
                            levelCrates.Add(crate);
                            break;
                        case 'e':
                        case 'E':
                            var enemy = CreateEnemy(new Vector2(30 * col, 30 * row), new Vector2(30, 30));
                            levelEnemies.Add(enemy);
                            break;
                        case '1':
                        case '2':
                        case '3':
                            // Level door
                            int levelNumber = (int)char.GetNumericValue(cellValue);
                            var door = CreateDoorToLevel(new Vector2(30 * col, 30 * row), new Vector2(30, 30), levelNumber);
                            levelDoors.Add(door);
                            break;
                    }
                }
            }

            var level = new LevelState(levelName, spawnPoint);
            level.SetBlocks(blocks);
            level.AddEnemies(levelEnemies);
            level.AddCrates(levelCrates);
            level.AddDoors(levelDoors);
            level.AddStaticObjects(levelStaticObjects);
            level.AddCollectableObjects(levelCollectableObjects);
            return level;
        }

        private StaticObject CreateFlowerObject(IReadonlyVector position, IReadonlyVector size)
        {
            var flowerObject = new StaticObject(actionQueue, position, size);
            flowerObject.SpriteBinding = new ObjectBinding<ISprite>("flower");
            flowerObject.Collect += CollectFlower;
            return flowerObject;
        }

        private void CollectFlower(object sender, ICollectingObject target)
        {
            target.Inventory.Add("flower");
        }

        private StaticObject CreateHiddenObject(IReadonlyVector position, IReadonlyVector size)
        {
            var hiddenObject = new StaticObject(actionQueue, position, size);
            hiddenObject.SpriteBinding = new ObjectBinding<ISprite>("empty");
            return hiddenObject;
        }

        private MovableObject CreateCrate(IReadonlyVector position, IReadonlyVector size)
        {
            var crate = new MovableObject(actionQueue, position, size);
            crate.SpriteBinding = new ObjectBinding<ISprite>("crate");
            return crate;
        }

        private Door CreateDoorToLevel(IReadonlyVector position, IReadonlyVector size, int levelNumber)
        {
            var door = new Door(actionQueue, position, size);
            door.SpriteBinding = new ObjectBinding<ISprite>("door");
            door.Destination = new Destination()
            {
                Type = DestinationType.Level,
                Identifier = $"Level{levelNumber}"
            };
            return door;
        }

        private EnemyObject CreateEnemy(IReadonlyVector position, IReadonlyVector size)
        {
            var enemy = new EnemyObject(actionQueue, position, size);
            enemy.SpriteBinding = new ObjectBinding<ISprite>("skeleton");
            return enemy;
        }

        private string ReadLevelBlocks(string levelName)
        {
            var assembly = GetType().GetTypeInfo().Assembly;
            var resourceName = $"Prerelease.Main.Maps.{levelName}.blocks.txt";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}